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

using GeoDKPOCDMPTest.Web.Models;
using GeoDKPOCDMPTest.Shared;
using System.IdentityModel.Services;
using System.Xml;
using System.IO;

namespace GeoDKPOCDMPTest.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var pClaim = GetClaimsIdentity();
            var model = new PythagoraModel();           
            Thread.CurrentPrincipal = null;
            var uid = pClaim.Claims.Single(c => string.Equals(c.Type, "urn:oid/0.9.2342.19200300.100.1.1")).Value;
            ViewBag.Name = uid;
        
            //model.Msg = getValuesFromWs1(64942212);//No security and tied to 101kmd cvr.

            var identity = pClaim.Identity as ClaimsIdentity;
            var token = identity.BootstrapContext as BootstrapContext;
            if (token == null)
            {
                throw new ApplicationException("Cannot get boostrap context from current identity.");
            }
            try
            {
                var actasToken = GetTokenForActasWithCertificate(token);
                model.Msg = model.Msg + " We have a token.";
            }catch(Exception ex)
            {
                model.Msg = "Vi er kede af det, men denne fejl opstod på DMP, da vi ville hente en ActAs-token: " + ex.Message;
            }

            if (pClaim.IsInRole("proverolleB")) { model.Msg = model.Msg + " Du er B'er!"; }
            if (pClaim.IsInRole("proverolleA")) { model.Msg = model.Msg + " Du er A'er!"; }
            return View(model);
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
               Constants.StsPocCertificate,//Constants.StsPocCertificate, 
               Constants.DotNetServiceAddress,
               Constants.GetPocClientCertificateTest(),//Constants.GetPocClientCertificateTest(),
               EnsureBootstrapSecurityToken(token));

            return securityToken;
        }

        private static SecurityToken GetTokenForActas(BootstrapContext token)
        {
            var newToken = WsTrustClient.RequestSecurityTokenWithUserName(
                Constants.StsAddressUserName,
                Constants.StsCertificate,
                Constants.DotNetServiceAddress, //JavaServiceAddress
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