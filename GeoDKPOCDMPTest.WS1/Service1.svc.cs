using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
           // var identity = GetClaimsIdentity();
            var cInfo = GetCompanyInfo(cvrNumber);
            return cInfo;

        }
        public DataSet GetDatasets()
        {

            var dataset = new DataSet();
            dataset.PythagorasValues = GetDataRows();
            return dataset;
        }

        public string SetDataset(int? valueA, int? valueB, int? valueC)
        {
            //isInRole etc.

            if((valueA == null && valueB == null) || (valueB == null && valueC == null) || (valueA == null && valueC == null))
            {
                var _error = new ErrorLog()
                {
                    Timestamp = DateTime.UtcNow,
                    UserName = "unKnown",
                    Email = "unKnown",
                    Action = "Saving dataset",
                    ErrorMessage = "More than one null in dataset"
                };
                logError(_error);
                return _error.ErrorMessage;
            }

            if (valueA != null && valueB != null && valueC != null)
            {
                var _error = new ErrorLog()
                {
                    Timestamp = DateTime.UtcNow,
                    UserName = "unKnown",
                    Email = "unKnown",
                    Action = "Saving dataset",
                    ErrorMessage = "No empty data to fill in dataset"
                };
                logError(_error);
                return _error.ErrorMessage;
            }

            var pythagorasValue = new PythagorasValue()
            {
                ValueA = valueA,
                ValueB = valueB,
                ValueC = valueC
            };
            if (SavedataSet(pythagorasValue))
            {
                return "Ok"; 
            }
            return "Ukendt fejl: kontakt en relevant ansvarlig";
        }

        public CalculatedDataSet CalculateDataSet(int Id)
        {
            return new CalculatedDataSet();
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
        private List<PythagorasValue> GetDataRows()
        {
            var pInfo = new List<PythagorasValue>();
            try
            {
                using (DB1Entities1 db = new DB1Entities1())
                {
                    pInfo = db.PythagorasValues.ToList();
                }
                return pInfo;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message, new FaultCode("02 Data-fault"));
            }
        }
        private bool SavedataSet(PythagorasValue pythagorasValue)
        {
            using (DB1Entities1 db = new DB1Entities1())
            {
                db.PythagorasValues.Add(pythagorasValue);
                db.SaveChanges();
            }
            return true;
        }

        private void logError(ErrorLog message)
        {
            try
            {
                using (DB1Entities1 db = new DB1Entities1())
                {
                    db.ErrorLogs.Add(message);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message, new FaultCode("02 Data-fault"));
            }
        }
    }
}
