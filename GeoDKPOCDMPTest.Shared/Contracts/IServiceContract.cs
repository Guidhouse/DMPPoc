using System.ServiceModel;

namespace GeoDKPOCDMPTest.Shared.Contract
{
    /// <summary>
    /// Servicens kontrakt
    /// </summary>
    [ServiceContract(Namespace = "urn:dmp:brugerstyring:samples:1.0.0")]
    public interface IServiceContract
    {
        /// <summary>
        /// Hello world operation, der blot echo'er brugerens
        /// query
        /// </summary>
        [OperationContract]
        HelloWorldResult HelloWorld(HelloWorldQuery query);

        /// <summary>
        /// Operation, der checker at bootstrap token'et er tilgængeligt
        /// på service-siden ift. brugen af act as
        /// </summary>
        [OperationContract]
        void AssertBootstrapTokenIsAvailable();

    }
}
