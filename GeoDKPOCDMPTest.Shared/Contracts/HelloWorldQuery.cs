using System.Runtime.Serialization;

namespace GeoDKPOCDMPTest.Shared.Contract
{
    /// <summary>
    /// Datakontrakt for query p� web service'n
    /// </summary>
    [DataContract(Namespace = "urn:dmp:brugerstyring:samples:1.0.0")]
    public class HelloWorldQuery
    {
        /// <summary>
        /// En tekststreng der overf�res til servicen
        /// </summary>
        [DataMember]
        public string Text
        {
            get;
            set;
        }
    }
}