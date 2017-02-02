using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading;
using System.IdentityModel.Services;
using System.Xml;
using System.IO;
using System.Text;

using GeoDKPOCDMPTest.Web.Models;
using GeoDKPOCDMPTest.Shared;
using System.Security.Cryptography.X509Certificates;
using GeoDKPOCDMPTest.Shared.Contracts;
using GeoDKPOCDMPTest.Shared.Contract;

namespace GeoDKPOCDMPTest.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var pClaim = GetClaimsIdentity();
            var model = new PythagoraModel();

            var uid = pClaim.Claims.Single(c => string.Equals(c.Type, "identify/urn:oid:0.9.2342.19200300.100.1.1")).Value;
            var userCvr = int.Parse(pClaim.Claims.Single(c => string.Equals(c.Type, "identify/dk:gov:saml:attribute:CvrNumberIdentifier")).Value);

            ViewBag.Name = uid;
            //var sb = new StringBuilder();//Get a list of claims..
            //foreach (var c in pClaim.Claims)//To list types and values of claims.
            //{
            //    sb.AppendLine($"type: {c.Type} value: {c.Value}{Environment.NewLine}");
            //}
            //model.Msg = $"{sb.ToString()} {Environment.NewLine}";

            model.Msg = $"{model.Msg} Velkommen {uid}. {ServiceClient.getValuesFromWs1(userCvr)}. {Environment.NewLine}";//No security and tied to 101kmd cvr.

            var role = pClaim.IsInRole("proverolleA") ? "dataredaktør" : pClaim.IsInRole("proverolleB") ? "længdeberegner" : "uden rettigheder";
            model.Msg = model.Msg + $"Du er {role}.  {Environment.NewLine}";

            var identity = pClaim.Identity as ClaimsIdentity;
            var token = identity.BootstrapContext as BootstrapContext;
            if (token == null)
            {
                throw new ApplicationException("Cannot get boostrap context from current identity.");
            }
            try
            {
                var actasToken = GetTokenForActas(token);
                model.Msg = model.Msg + " We have a uid/pw token.";
            }
            catch (Exception ex)
            {
                model.Msg = model.Msg + $"Uid/pw DMP ActAs-token:  {ex.Message}{Environment.NewLine}";
            }
            try
            {
                var actasToken = GetTokenForActasWithCertificate(token);
                model.Msg = model.Msg + " We have a cert token.";
            }
            catch (Exception ex)
            {
                model.Msg = model.Msg + $"Cert DMP ActAs-token: {ex.Message}{Environment.NewLine}";
            }

            var ls = getValuesFromWs1WT(token);
            model.Msg = model.Msg + ls;

            model.Datasets = ServiceClient.getDataSets();
            return View(model);
        }
        public PartialViewResult DataSetPicker()
        {
            var model = new List<WebDataset>();
            model = ServiceClient.getDataSets();
            return PartialView(model);
        }
        
        public JsonResult sendData(int? inputA, int? inputB, int? inputC)
        {
            var pClaim = GetClaimsIdentity();
            var uid = pClaim.Claims.Single(c => string.Equals(c.Type, "identify/urn:oid:0.9.2342.19200300.100.1.1")).Value;
            var responseMessage = ServiceClient.sendDataSetToService(inputA, inputB, inputC);

            return (responseMessage == "Ok") ?
                Json($"Ok {uid}, indsendte data var a = {inputA}, b = {inputB}, c= {inputC}", JsonRequestBehavior.AllowGet) :
                Json($"Desværre {uid}. {inputA}, {inputB}, {inputC} Er ikke valide pga. følgende fejl: {responseMessage}", JsonRequestBehavior.AllowGet);
        }

        public JsonResult CalculateDataset(int id)
        {
            var result = ServiceClient.GetCalculetedDataset(id);
         //   var json = $"ID: {result.Id}, Message: \"{result.Message}\", A:{result.valueA}, B:{result.valueB}, C:{result.valueC}";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public static string getValuesFromWs1WT(BootstrapContext token)
        {
            var uid = "101kmd";
            var pw = "g_3R_Lo45_XjC";
            var serviceToken = WsTrustClient.RequestSecurityTokenWithUserName(
                Constants.StsAddressUserName,
                Constants.StsCertificate,
                Constants.DotNetServiceAddress,//PocServiceAddress, //JavaServiceAddress
                Constants.DmpUserName,
                Constants.DmpPassword,
                EnsureBootstrapSecurityToken(token));

            //var serviceToken = GetToken("330kmd", "Yd4_6G_e_7s");

            // Lav binding til STS'en
            var stsBinding = WsTrustClient.GetStsBinding();

            // Lav binding til servicen
            var serviceBinding = WsTrustClient.GetServiceBinding(stsBinding, Constants.StsAddressUserName, true);

            // Lav channel til servicen
            var channelFactory = new ChannelFactory<IServiceContract>(serviceBinding, Constants.PocServiceAddress);

            if (channelFactory.Credentials == null)
                throw new ApplicationException("ChannelFactory must have credentials");

            // Deaktiver service cert validering
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.None;
            channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            // Påhæft token til kald til service
            var channel = channelFactory.CreateChannelWithIssuedToken(serviceToken);
            try
            {
                var query = new HelloWorldQuery
                {
                    Text = "Jesper Hvid"
                };
                var msg = channel.HelloWorld(query);
                return msg.Text;

            }catch(Exception ex)
            {
                var msg = ex.Message;
                return msg;
            }


        }






        private static ClaimsPrincipal GetClaimsIdentity()//Get the user from the login portal.
        {
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            if (claimsPrincipal == null)
                throw new ApplicationException("ClaimsPrincipal must not be null");

            if (claimsPrincipal.Identities == null || claimsPrincipal.Identities.Count() < 1)
                throw new ApplicationException("ClaimsPrincipal must contain identities");

            return claimsPrincipal;
        }

        private static SecurityToken GetTokenForActasWithCertificate(BootstrapContext token)
        {
            var securityToken = WsTrustClient.RequestSecurityTokenWithX509(
               Constants.StsAddressCertificate,
               Constants.StsPocCertificate,// KMDProveopgave  Encryption certifikat
               Constants.PocServiceAddress,// DotNetServiceAddress,
               Constants.GetPocClientCertificateTest(),//,GeoDK...,
               EnsureBootstrapSecurityToken(token));

            return securityToken;
        }

        private static SecurityToken GetTokenForActas(BootstrapContext token)
        {
            var newToken = WsTrustClient.RequestSecurityTokenWithUserName(
                Constants.StsAddressUserName,
                Constants.StsCertificate,
                Constants.DotNetServiceAddress,//PocServiceAddress, //JavaServiceAddress
                Constants.DmpUserName,
                Constants.DmpPassword,
                EnsureBootstrapSecurityToken(token));
            return newToken;
        }

        private static SecurityToken EnsureBootstrapSecurityToken(BootstrapContext bootstrapContext)
        {
            if (bootstrapContext.SecurityToken != null)
                return bootstrapContext.SecurityToken;
            if (string.IsNullOrWhiteSpace(bootstrapContext.Token))
                return null;
            var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
            return handlers.ReadToken(new XmlTextReader(new StringReader(bootstrapContext.Token)));
        }
    }
}