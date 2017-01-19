using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeoDKPOCDMPTest.Web.Controllers
{
    public class AccountController : Controller
    {
        [Authorize]
        public ActionResult SignIn()
        {
            // Redirects to default page after login
            return RedirectToAction("Index", "Home");
        }

        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            // Alternativ logud kun lokalt
            //FederatedAuthentication.SessionAuthenticationModule.SignOut();
            //FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();

            WSFederationAuthenticationModule authModule = FederatedAuthentication.WSFederationAuthenticationModule;
            WSFederationAuthenticationModule.FederatedSignOut(new Uri(authModule.Issuer), new Uri(callbackUrl));
        }

        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }



}