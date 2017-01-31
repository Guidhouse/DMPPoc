using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GeoDKPOCDMPTest.WS2
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService2
    {
        //[OperationContract]
        //CalculatedDataset GetCalculatedDataSet(PythagorasDataset value);

        [OperationContract]
        CalculatedDataset GetCalculatedDataSet(int id, int? ValueA, int? ValueB, int? ValueC);

    }
    [DataContract]
    public class PythagorasDataset
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
        public int DB1id { get; set; }
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
