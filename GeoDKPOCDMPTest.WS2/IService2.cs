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
        [OperationContract]
        CalculatedDataset GetCalculatedDataSet(int value);
    }

    [DataContract]
    public class CalculatedDataset
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        float valueA { get; set; }
        [DataMember]
        float valueB { get; set; }
        [DataMember]
        float valueC { get; set; }
        [DataMember]
        string Message { get; set; }
    }
}
