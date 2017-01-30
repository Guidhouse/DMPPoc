using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoDKPOCDMPTest.WS1.Repositories
{
    public static class ServiceRepository
    {
        public static string CalculateDataSet(int Id)
        {

            var calculatedDataset = "";// new CalculatedDataSet();
            using (var serviceClient = new WS2.Service2Client())
            {
                calculatedDataset = serviceClient.GetData(3);
            }

            return calculatedDataset;
        }
    }
}