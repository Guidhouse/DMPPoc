using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using GeoDKPOCDMPTest.WS1.Models;
using GeoDKPOCDMPTest.Shared.Contracts;

namespace GeoDKPOCDMPTest.WS1.Repositories
{
    public class DB1Repository
    {
        public CompanyInfo GetCompanyInfo(int cvrNumber)
        {
            var cInfo = new CompanyInfo();
            try
            {
                using (DB1Entities1 db = new  DB1Entities1())
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

        public List<Dataset> GetDataSets()
        {
            var pInfo = new List<Dataset>();
            try
            {
                using (DB1Entities1 db = new DB1Entities1())
                {
                    var values = db.PythagorasValues.ToList();
                    foreach (var val in values)
                    {
                        pInfo.Add(new Dataset()
                        {
                            Id = val.Id,
                            ValueA = val.ValueA,
                            ValueB = val.ValueB,
                            ValueC = val.ValueC
                        });
                    }
                }
                return pInfo;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message, new FaultCode("02 Data-fault"));
            }
        }

        public Models.PythagorasValue GetDataSet(int id)
        {
            var pInfo = new Models.PythagorasValue();
            try
            {
                using (DB1Entities1 db = new DB1Entities1())
                {
                    pInfo = db.PythagorasValues.Where(p => p.Id == id).FirstOrDefault();
                }
                return pInfo;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message, new FaultCode("02 Data-fault"));
            }
        }

        public bool SavedataSet(Models.PythagorasValue pythagorasValue)
        {
            using (DB1Entities1 db = new DB1Entities1())
            {
                db.PythagorasValues.Add(pythagorasValue);
                db.SaveChanges();
            }
            return true;
        }

        public void logError(ErrorLog message)
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