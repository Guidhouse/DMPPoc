using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            model.Msg = "Du må intet!";
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



    }
}