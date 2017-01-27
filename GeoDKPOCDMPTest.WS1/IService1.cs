using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net.Security;

namespace GeoDKPOCDMPTest.WS1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [FaultContract(typeof(FaultException))]
        CompanyInfo GetCompanyByCvrNumber(int cvrNummer);

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        DataSet GetDatasets();

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        bool SetDataset(int? valueA, int? valueB, int? valueC);
    }

    [DataContract]
    public class CompanyInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int CvrNumber { get; set; }
    }
    [DataContract]
    public class DataSet
    {
        [DataMember]
        public List<Repositories.PythagorasValue> PythagorasValues { get; set; }
    }
}
