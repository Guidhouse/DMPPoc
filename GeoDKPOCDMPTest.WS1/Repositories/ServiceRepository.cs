using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeoDKPOCDMPTest.WS1.Models;
using GeoDKPOCDMPTest.Shared.Contracts;
namespace GeoDKPOCDMPTest.WS1.Repositories
{
    public static class ServiceRepository
    {
        public static CalculatedDataset CalculateDataSet(int id)
        {
            var db1 = new DB1Repository();
            var rawData = db1.GetDataSet(id);

            if (rawData != null)
            {       
                //var calculatedDataset = new WS2.CalculatedDataset();
                var serviceClient = new WS2.Service2Client();
                var calculatedDataset = serviceClient.GetCalculatedDataSet(rawData.Id, rawData.ValueA, rawData.ValueB, rawData.ValueC);
                var returnSet = new CalculatedDataset()
                {
                    Id = calculatedDataset.Id,
                    ValueA = calculatedDataset.ValueA,
                    ValueB = calculatedDataset.ValueB,
                    ValueC = calculatedDataset.ValueC,
                    Message = calculatedDataset.Message
                };

                return returnSet;
               

            }
            else
            {
                throw new Exception("No row with that Id");
            }
         
            
        }
    }
}