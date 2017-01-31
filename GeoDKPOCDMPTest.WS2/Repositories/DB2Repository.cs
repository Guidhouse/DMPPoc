using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeoDKPOCDMPTest.WS2.Models;
using System.ServiceModel;

namespace GeoDKPOCDMPTest.WS2.Repositories
{
    public class DB2Repository
    {
        public CalculatedDataset GetCalculateddataset(int id)
        {
            CalculatedDataset calcSet;
            using(DB2Entities db = new DB2Entities())
            {
                var tempSet = db.PythagorasValues.Where(p => p.DB1iD == id).FirstOrDefault();
                if(tempSet != null)
                {
                    calcSet = new CalculatedDataset()
                    {
                        Id = tempSet.Id,
                        DB1id = id,
                        Message = "Stored dataset fetched.",
                        ValueA = (float) tempSet.ValueA,
                        ValueB = (float) tempSet.ValueB,
                        ValueC = (float) tempSet.ValueC
                    };
                    return calcSet;
                }else
                {
                    return null;
                }
            }
        }
        public void SaveCalculatedDataset(CalculatedDataset dataset)
        {
            using(DB2Entities db = new DB2Entities())
            {
                try
                {
                    var pVal = new PythagorasValue()
                    {
                        DB1iD = dataset.DB1id,
                        ValueA = dataset.ValueA,
                        ValueB = dataset.ValueB,
                        ValueC = dataset.ValueC
                    };

                    db.PythagorasValues.Add(pVal);
                    db.SaveChanges();
                   // return true;
                }
                catch
                {
                    var _error = new ErrorLog()
                    {
                        Timestamp = DateTime.UtcNow,
                        UserName = "unKnown",
                        Email = "unKnown",
                        Action = "Saving dataset",
                        ErrorMessage = "Something went wrong with saving the dataset."
                    };
                    logError(_error);
                    //return false;
                }
            }
        }

        public void logError(ErrorLog message)
        {
            try
            {
                using (DB2Entities db = new DB2Entities())
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
