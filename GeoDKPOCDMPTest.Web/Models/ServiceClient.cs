using GeoDKPOCDMPTest.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

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

        public static List<Dataset> getDataSets()
        {
            WS1.Service1Client WS1 = new WS1.Service1Client();
            try
            {
                var dSets = new List<Dataset>();

                var rawSets =  WS1.GetDatasets();
                foreach(var ds in rawSets.PythagorasValues)
                {
                    dSets.Add(new Dataset()
                    {
                        Id = ds.Id,
                        ValueA = (float) ds.ValueA.GetValueOrDefault(),
                        ValueB = (float) ds.ValueB.GetValueOrDefault(),
                        ValueC = (float) ds.ValueC.GetValueOrDefault()
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
            WS1.Service1Client WS1 = new WS1.Service1Client();
            try
            {
                var succes = WS1.SetDataset(A, B, C);
                return succes;
            }
            catch (Exception ex)
            {
                throw new Exception("WS1 stejler: " + ex.Message);
            }

        }


        // Lav binding til STS'en
        //
        // Lav binding til servicen
        //  var serviceBinding = WsTrustClient.GetServiceBinding(stsBinding, Constants.StsAddressUserName, false);

        // Deaktiver secure conversation, da Java servicen ikke bruger det
        //  var serviceBindingWithoutSecureConversation = WsTrustClient.CreateWsFederationBindingWithoutSecureConversation(serviceBinding);

        // Lav channel til servicen
        //var channelFactory =
        //    new ChannelFactory<IServiceChannel>(
        //        serviceBindingWithoutSecureConversation,
        //        Constants.JavaServiceAddress);

        //if (channelFactory.Credentials == null)
        //    throw new ApplicationException("ChannelFactory must have credentials");

        //// Sæt encryption certifikat til java servicen
        //channelFactory.Credentials.ServiceCertificate.ScopedCertificates.Add(Constants.JavaServiceAddress.Uri, Constants.ServiceCertificate);

        // Lav channel med token'et fra STS'en
        //var channel = channelFactory.CreateChannelWithIssuedToken(securityToken);
        //try
        //{
        //    // Lav query
        //    //var query = new DotNetSamples.Common.DAI.
        //    //                {
        //    //                   Body = new Common.Tomcat.helloRequestBody("Jesper")
        //    //                };

        //    // Kald service
        //    return channel.HelloWorld(new HelloWorldRequest());
        //}
        //finally
        //{
        //    // Luk channel
        //    WsTrustClient.CloseChannel(channel);
        //}
        //            return null;
    }
}