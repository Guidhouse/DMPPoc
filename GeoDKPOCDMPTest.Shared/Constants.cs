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

        private static string PocServiceCertifikatName
        {
            get { return ConfigurationManager.AppSettings["PocServiceCertifikatNavn"]; }
        }


        private static string JavaServiceAddressUrl
        {
            get { return ConfigurationManager.AppSettings["JavaServiceURL"]; }
        }

        private static string JavaServiceCertifikatName
        {
            get { return ConfigurationManager.AppSettings["JavaServiceCertifikatNavn"]; }
        }

        public static X509Certificate2 ServiceCertificate
        {
            get { return GetX509Certificate(ConfigurationManager.AppSettings["JavaServiceCertificate"]); }
        }

        public static readonly EndpointAddress DotNetServiceAddress = new EndpointAddress(new Uri(DotNetServiceAddressUrl), EndpointIdentity.CreateDnsIdentity(DotNetServiceCertifikatName));
        public static readonly EndpointAddress PocServiceAddress = new EndpointAddress(new Uri(PocServiceAddressUrl), EndpointIdentity.CreateDnsIdentity(PocServiceCertifikatName));

        public static readonly EndpointAddress JavaServiceAddress = new EndpointAddress(new Uri(JavaServiceAddressUrl), EndpointIdentity.CreateDnsIdentity(JavaServiceCertifikatName));

        // Client certifikat til x509 authentication (TEST) vnTest PKI certifikat.
        public static X509Certificate2 GetClientCertificateTest()
        {
            return CertUtil.GetCertificate(
                StoreLocation.LocalMachine,
                StoreName.My,
                X509FindType.FindByThumbprint,
                "f8 a3 9b 92 74 dd e5 8b 79 bc 95 45 f5 57 a5 be 3e 1e 13 12");//vnTest
        }
        // Client certifikat til x509 authentication (TEST) KMDProveopgave PKI certifikat.
        public static X509Certificate2 GetPocClientCertificateTest()
        {
            return CertUtil.GetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
              //  "03 87 73 3f 0f f5 a2 10 6b 90 a3 f0 5f 9a 9d 3b 7b 55 6f a7");
           //"d9 18 ad 8c 49 57 9f 76 3d ab a1 3e 76 ba 20 ce 55 5a e0 3a");//GeoDKPOCDMPTest (funktionscertifikat)
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
