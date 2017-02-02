using System.Runtime.Serialization;

namespace GeoDKPOCDMPTest.Shared.Contract
{
    /// <summary>
    /// Datakontrakt for query på web service'n
    /// </summary>
    [DataContract(Namespace = "urn:dmp:brugerstyring:samples:1.0.0")]
    public class HelloWorldQuery
    {
        /// <summary>
        /// En tekststreng der overføres til servicen
        /// </summary>
        [DataMember]
        public string Text
        {
            get;
            set;
        }
    }
}