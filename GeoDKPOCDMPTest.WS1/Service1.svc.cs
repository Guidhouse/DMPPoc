using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using GeoDKPOCDMPTest.WS1.Repositories;
using GeoDKPOCDMPTest.WS1.Models;
using System.Security.Claims;
using System.Threading;
using GeoDKPOCDMPTest.Shared.Contracts;

namespace GeoDKPOCDMPTest.WS1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {

        public CompanyInfo GetCompanyByCvrNumber(int cvrNumber)
        {
            // var identity = GetClaimsIdentity();
            var rep = new DB1Repository();
            var cInfo = rep.GetCompanyInfo(cvrNumber);
            return cInfo;

        }
        public Datasets GetDatasets()
        {
            var datasets = new Datasets();
            var rep = new DB1Repository();
            datasets.PythagorasValues = rep.GetDataSets();
            
            return datasets;
        }

        public string SetDataset(int? valueA, int? valueB, int? valueC)
        {
            //isInRole etc.
            var rep = new DB1Repository();
            if(valueA < 0 || valueB < 0 || valueC < 0)
            {
                var _error = new ErrorLog()
                {
                    Timestamp = DateTime.UtcNow,
                    UserName = "unKnown",
                    Email = "unKnown",
                    Action = "Saving dataset",
                    ErrorMessage = "This physical world does not support negative distances"
                };
                rep.logError(_error);
                return _error.ErrorMessage;
            }


            if ((valueA == null && valueB == null) || (valueB == null && valueC == null) || (valueA == null && valueC == null))
            {
                var _error = new ErrorLog()
                {
                    Timestamp = DateTime.UtcNow,
                    UserName = "unKnown",
                    Email = "unKnown",
                    Action = "Saving dataset",
                    ErrorMessage = "More than one null in dataset"
                };
                rep.logError(_error);
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
                rep.logError(_error);
                return _error.ErrorMessage;
            }

            var pythagorasValue = new Models.PythagorasValue()
            {
                ValueA = valueA,
                ValueB = valueB,
                ValueC = valueC
            };
            if (rep.SavedataSet(pythagorasValue))
            {
                return "Ok"; 
            }
            return "Ukendt fejl: kontakt en relevant ansvarlig";
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

        public CalculatedDataset CalculateDataSet(int id)
        {
            var calcSet = ServiceRepository.CalculateDataSet(id);
            return calcSet;
        }
    }
}
