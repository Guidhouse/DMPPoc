using System.Runtime.Serialization;

namespace GeoDKPOCDMPTest.Shared.Contract
{
    /// <summary>
    /// Datakontrakt for servicens svar til kalderen
    /// </summary>
    [DataContract(Namespace = "urn:dmp:brugerstyring:samples:1.0.0")]
    public class HelloWorldResult
    {
        /// <summary>
        /// En tekststreng der returneres
        /// </summary>
        [DataMember]
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Antallet af claims servicen ser
        /// </summary>
        [DataMember]
        public int ClaimsCount
        {
            get;
            set;
        }
    }
}