using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace GeoDKPOCDMPTest.Shared
{
    /// <summary>
    /// Indeholder constants for samples-projektet.  Herunder adresser til Identify-sts'en, adresse til servicen og alle certifikater
    /// </summary>
    public class Constants
    {
        private static string StsAddressUserNameUrl
        {
            get { return ConfigurationManager.AppSettings["StsIdentifyCertificateEndpointUserName_Uri"]; }
        }

        private static string StsAddressUserNameIdentity
        {
            get { return ConfigurationManager.AppSettings["StsIdentifyCertificateEndpointuserName_Identity"]; }
        }

        private static string StsAddressCertificateUrl
        {
            get { return ConfigurationManager.AppSettings["StsIdentifyCertificateEndpointX509_Uri"]; }
        }

        private static string StsAddressCertificateIdentity
        {
            get { return ConfigurationManager.AppSettings["StsIdentifyCertificateEndpointX509_Identity"]; }
        }

        private static string StsAddressIssuedTokenUrl
        {
            get { return ConfigurationManager.AppSettings["StsIdentifyIssuedToken_Uri"]; }
        }

        private static string StsAddressIssuedTokenIdentity
        {
            get { return ConfigurationManager.AppSettings["StsIdentifyIssuedToken_Identity"]; }
        }

        public static X509Certificate2 StsCertificate
        {
            get { return GetX509Certificate(ConfigurationManager.AppSettings["StsIdentifyServiceCertificate"]); }
        }
        public static X509Certificate2 StsPocCertificate
        {
            get { return GetX509Certificate(ConfigurationManager.AppSettings["StsPocIdentifyServiceCertificate"]); }
        }

        private static X509Certificate2 GetX509Certificate(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return null;

            var bytes = Convert.FromBase64String(base64);

            var certificate = new X509Certificate2(bytes);

            return certificate;
        }

        public static readonly EndpointAddress StsAddressCertificate = new EndpointAddress(new Uri(StsAddressCertificateUrl), new DnsEndpointIdentity(StsAddressCertificateIdentity));
        public static readonly EndpointAddress StsAddressUserName = new EndpointAddress(new Uri(StsAddressUserNameUrl), new DnsEndpointIdentity(StsAddressUserNameIdentity));
        public static readonly EndpointAddress StsAddressIssuedToken = new EndpointAddress(new Uri(StsAddressIssuedTokenUrl), new DnsEndpointIdentity(StsAddressIssuedTokenIdentity));

        private static string DotNetServiceAddressUrl
        {
            get { return ConfigurationManager.AppSettings["DotNetServiceURL"]; }
        }

        private static string DotNetServiceCertifikatName
        {
            get { return ConfigurationManager.AppSettings["DotNetServiceCertifikatNavn"]; }
        }
        private static string PocServiceAddressUrl
        {
            get { return ConfigurationManager.AppSettings["PocServiceURL"]; }
        }
        private static string PocServiceEndpointUrl
        {
            get { return ConfigurationManager.AppSettings["PocServiceEndpointURL"]; }
        }

        private static string PocServiceCertifikatName
        {
            get { return ConfigurationManager.AppSettings["PocServiceCertifikatNavn"]; }
        }




        public static readonly EndpointAddress PocServiceEndpoint = new EndpointAddress(new Uri(PocServiceEndpointUrl), EndpointIdentity.CreateDnsIdentity(PocServiceCertifikatName));
        public static readonly EndpointAddress PocServiceAddress = new EndpointAddress(new Uri(PocServiceAddressUrl), EndpointIdentity.CreateDnsIdentity(PocServiceCertifikatName));
        

        // Client certifikat til x509 authentication (TEST) KMDProveopgave PKI certifikat.
        public static X509Certificate2 GetPocClientCertificate()
        {
            return CertUtil.GetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
            "ad 18 03 87 5c 2b 57 d3 8a bb bf b0 b5 32 f9 0c 7c 81 98 21");//KmdProveopgave
        }
        // Brugernavn til test dmp bruger
        public static string DmpUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["DmpUserName"];
            }
        }
        // Password til test dmp bruger (TESTMILJØ)
        public static string DmpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["DmpPassword"];
            }
        }
    }
}
