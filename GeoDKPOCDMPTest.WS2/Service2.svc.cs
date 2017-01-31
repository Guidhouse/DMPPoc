using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using GeoDKPOCDMPTest.WS2.Repositories;

namespace GeoDKPOCDMPTest.WS2
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service2 : IService2
    {

        //public CalculatedDataset GetCalculatedDataSet(PythagorasDataset input)
        public CalculatedDataset GetCalculatedDataSet(int id, int? valueA, int? valueB, int? valueC)

        {
            CalculatedDataset calcSet;
            var rep = new DB2Repository();
            calcSet = rep.GetCalculateddataset(id);
            if (calcSet != null)
            {
                return calcSet;
            }
            else {
              return CalculateAndSaveNewDataset(id, valueA, valueB, valueC);
            };
        }

        private CalculatedDataset CalculateAndSaveNewDataset(int id, int? valueA, int? valueB, int? valueC)
        {
            var rep = new DB2Repository();
            var calcSet = new CalculatedDataset()
            {
                DB1id = id,
                ValueA = (valueA != null)? (double) valueA : Math.Sqrt(Math.Pow((int)valueC, 2) - Math.Pow((int)valueB, 2)),
                ValueB = (valueB != null)? (double) valueB : Math.Sqrt(Math.Pow((int)valueC, 2) - Math.Pow((int)valueA, 2)),
                ValueC = (valueC != null)? (double) valueC : Math.Sqrt(Math.Pow((int)valueA, 2) + Math.Pow((int)valueB, 2)),
                Message = "New dataset stored."
            };
            rep.SaveCalculatedDataset(calcSet);
            return calcSet;
        }

    }
}
