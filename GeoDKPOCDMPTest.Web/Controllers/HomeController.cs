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

            model.Msg = getValuesFromWs1(64942212);
            if (pClaim.IsInRole("proverolleB")) { model.Msg = "Du er B'er!"; }
            if (pClaim.IsInRole("proverolleA")) { model.Msg = "Du er A'er!"; }
            return View(model);
        }


        private static ClaimsPrincipal GetClaimsIdentity()
        {
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            if (claimsPrincipal == null)
                throw new ApplicationException("ClaimsPrincipal must not be null");

            if (claimsPrincipal.Identities == null || claimsPrincipal.Identities.Count() < 1)
                throw new ApplicationException("ClaimsPrincipal must contain identities");

            return claimsPrincipal;
        }

        private string getValuesFromWs1(int value)
        {


            WS1.Service1Client WS1 = new Web.WS1.Service1Client();
            try
            {
                var cinfo = WS1.GetCompanyByCvrNumber(value);
                return "Du er fra " + cinfo.Name;
            }
            catch
            {
                return "CVR-nummer kan ikke valideres";
            }

        }

        private static WS2007FederationHttpBinding CreateServiceBindingDirectLogin()
        {
            // Lav binding til STS'en
            var stsBinding = new WS2007HttpBinding(SecurityMode.Message, false);
            stsBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            stsBinding.Security.Message.EstablishSecurityContext = false;
            stsBinding.Security.Message.NegotiateServiceCredential = false;

            // Lav binding til servicen
            var serviceBinding = new WS2007FederationHttpBinding(WSFederationHttpSecurityMode.Message, false);
            serviceBinding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            serviceBinding.Security.Message.IssuedKeyType = SecurityKeyType.SymmetricKey;
            serviceBinding.Security.Message.IssuedTokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1";
            serviceBinding.Security.Message.NegotiateServiceCredential = false;
            serviceBinding.Security.Message.IssuerAddress = System.Configuration.ConfigurationManager.AppSettings["StsIdentifyCertificateEndpointUserName_Uri"].; //Constants.StsAddressUserName;
            serviceBinding.Security.Message.IssuerBinding = stsBinding;
            serviceBinding.Security.Message.IssuerMetadataAddress = new EndpointAddress("https://log-in.test.miljoeportal.dk/runtime/services/trust/mex");

            return serviceBinding;
        }



    }
}