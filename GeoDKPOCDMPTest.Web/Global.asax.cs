using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IdentityModel.Services;

namespace GeoDKPOCDMPTest.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Registrer event handler
            FederatedAuthentication.WSFederationAuthenticationModule.RedirectingToIdentityProvider += WSFederationAuthenticationModule_RedirectingToIdentityProvider;
            //Ugly insecure hack
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
    (se, cert, chain, sslerror) =>
    {
        return true;
    };
        }

        void WSFederationAuthenticationModule_RedirectingToIdentityProvider(object sender, RedirectingToIdentityProviderEventArgs e)
        {
            // Populer sign in request'et med whr-parameteren
            e.SignInRequestMessage.HomeRealm = Request.QueryString["whr"];
        }
    }
}
