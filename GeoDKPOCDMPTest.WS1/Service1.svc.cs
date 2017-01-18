using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using System.Configuration;
using System.Data.SqlClient;
using GeoDKPOCDMPTest.WS1.Repositories;
using System.Security.Claims;
using System.Threading;

namespace GeoDKPOCDMPTest.WS1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {

        public CompanyInfo GetCompanyByCvrNumber(int cvrNumber)
        {
            var identity = GetClaimsIdentity();
            var cInfo = GetCompanyInfo(cvrNumber);
            return cInfo;

        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        private static ClaimsIdentity GetClaimsIdentity()
        {
            if (OperationContext.Current != null)
            {
                Thread.CurrentPrincipal = OperationContext.Current.ClaimsPrincipal;
            }

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            if (claimsPrincipal == null)
                throw new ApplicationException("ClaimsPrincipal must exist");

            if (claimsPrincipal.Identities.Count() < 1)
                throw new ApplicationException("IClaimsIdentityt must exist");

            var identity = claimsPrincipal.Identities.First();

            return identity;
        }

        private CompanyInfo GetCompanyInfo(int cvrNumber)
        {
            var cInfo = new CompanyInfo();
            try
            {
                using (DB1Entities1 db = new DB1Entities1())
                {
                    cInfo = db.CvrValues.Where(ci => ci.CvrNumber == cvrNumber).Select(ci => new CompanyInfo() { CvrNumber = ci.CvrNumber, Name = ci.OrganizationName }).First();
                }
                return cInfo;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message, new FaultCode("01 CVR-fault"));
            }
        }

    }
}
