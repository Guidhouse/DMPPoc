using GeoDKPOCDMPTest.Shared;
using GeoDKPOCDMPTest.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Web;
using System.Xml;
//using GeoDKPOCDMPTest.Shared.Contracts;

namespace GeoDKPOCDMPTest.Web.Models
{
    public static class ServiceClient
    {
        public static string getValuesFromWs1(int value)
        {
            WS1.Service1Client WS1 = new WS1.Service1Client();
            try
            {
                var cinfo = WS1.GetCompanyByCvrNumber(value);
                return "Du er fra " + cinfo.Name;
            }
            catch (Exception ex)
            {
                return "WS1 stejler: " + ex.Message;
            }
        }

        public static List<WebDataset> getDataSets()
        {
            WS1.Service1Client client = new WS1.Service1Client();
            try
            {
                var dSets = new List<WebDataset>();

                var rawSets =  client.GetDatasets();
                foreach(var ds in  rawSets.PythagorasValues)
                {
                    dSets.Add(new WebDataset()
                    {
                        Id = ds.Id,
                        ValueA = ds.ValueA.GetValueOrDefault(),
                        ValueB = ds.ValueB.GetValueOrDefault(),
                        ValueC = ds.ValueC.GetValueOrDefault()
                    });
                }
                return dSets;
            }
            catch (Exception ex)
            {
                throw new Exception( "WS1 stejler: " + ex.Message);
            }
        }

        public static string sendDataSetToService(int? A, int? B, int? C)
        {
            WS1.Service1Client client = new WS1.Service1Client();
            try
            {
                var succes = client.SetDataset(A, B, C);
                return succes;
            }
            catch (Exception ex)
            {
                throw new Exception("WS1 stejler: " + ex.Message);
            }
        }

        public static CalculatedDataset GetCalculetedDataset(int id)
        {
            WS1.Service1Client client = new WS1.Service1Client();
            try
            {
                var dataset = client.CalculateDataSet(id);
                var calcSet = new CalculatedDataset()
                {
                    Id = dataset.Id,
                    ValueA = dataset.ValueA,
                    ValueB = dataset.ValueB,
                    ValueC = dataset.ValueC,
                    Message = dataset.Message
                };
                return calcSet;
            }
            catch (Exception ex)
            {
                throw new Exception("WS1 stejler: " + ex.Message);
            }

        }

        public static string getCompanyInfoWithActas(BootstrapContext token, int userCvr)
        {
            var serviceToken = GetTokenForActasWithCertificate(token);
            // Lav binding til STS'en
            var stsBinding = WsTrustClient.GetStsBinding();

            // Lav binding til servicen
            var serviceBinding = WsTrustClient.GetServiceBinding(stsBinding, Constants.StsAddressUserName, true);

            // Lav channel til servicen
            var channelFactory = new ChannelFactory<IService1>(serviceBinding, Constants.PocServiceAddress);

            if (channelFactory.Credentials == null)
                throw new ApplicationException("ChannelFactory must have credentials");
            // Deaktiver service cert validering
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                X509CertificateValidationMode.None;
            channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            // Påhæft token til kald til service
            var channel = channelFactory.CreateChannelWithActAsToken(serviceToken);
            try
            {
                var msg = channel.GetCompanyByCvrNumber(userCvr);
                return msg.Name;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return msg;
            }
            finally
            {
                WsTrustClient.CloseChannel(channel);
            }
        }

        public static SecurityToken GetTokenForActasWithCertificate(BootstrapContext token)
        {
            var securityToken = WsTrustClient.RequestSecurityTokenWithX509(
               Constants.StsAddressCertificate,
               Constants.StsCertificate,
               Constants.PocServiceAddress,
               Constants.GetPocClientCertificate(),//KmdProveopgave,
               EnsureBootstrapSecurityToken(token));
            return securityToken;
        }

        private static SecurityToken EnsureBootstrapSecurityToken(BootstrapContext bootstrapContext)
        {
            if (bootstrapContext.SecurityToken != null)
                return bootstrapContext.SecurityToken;
            if (string.IsNullOrWhiteSpace(bootstrapContext.Token))
                return null;
            var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
            return handlers.ReadToken(new XmlTextReader(new StringReader(bootstrapContext.Token)));
        }
    }
}
