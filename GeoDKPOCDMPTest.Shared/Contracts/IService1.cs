
using System.Collections.Generic;

using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace GeoDKPOCDMPTest.Shared.Contracts
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
        Datasets GetDatasets();

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        string SetDataset(int? valueA, int? valueB, int? valueC);

        [OperationContract]
        [FaultContract(typeof(FaultException))]
        CalculatedDataset CalculateDataSet(int Id);
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
    public class Datasets
    {
        [DataMember]
        public List<Dataset> PythagorasValues { get; set; }
    }

    [DataContract]
    public class Dataset
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int? ValueA { get; set; }
        [DataMember]
        public int? ValueB { get; set; }
        [DataMember]
        public int? ValueC { get; set; }
    }

    [DataContract]
    public class CalculatedDataset
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public double ValueA { get; set; }
        [DataMember]
        public double ValueB { get; set; }
        [DataMember]
        public double ValueC { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
