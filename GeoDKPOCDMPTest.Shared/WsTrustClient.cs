using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;

namespace GeoDKPOCDMPTest.Shared
{
    /// <summary>
    /// Utility klasse til at requeste security tokens fra AD FS 2.0-sts'en
    /// via x509 authentication og windows authentication
    /// </summary>
    public static class WsTrustClient
    {
        /// <summary>
        /// Requester et security token fra AD FS 2.0 sts'en via x509 authentication
        /// Token'et kan derpå vedhæftes et kald til en service. Der kan angives
        /// et optionelt ActAs security token. Hvis dette token angives
        /// vil kaldet til STS'en blive et ActAs kald
        /// </summary>
        public static SecurityToken RequestSecurityTokenWithX509(
            EndpointAddress stsAddress,
            X509Certificate2 stsEncryptionCert,
            EndpointAddress serviceAddress,
            X509Certificate2 clientCertificate,
            SecurityToken actAsSecurityToken = null)
        {
            // Certificate binding med ws-trust 1.3 og message security
            var stsBinding = new CertificateWSTrustBinding(SecurityMode.Message);
            // Lav channel factory
            var channelFactory = new WSTrustChannelFactory(
                stsBinding,
                stsAddress);

            if (channelFactory.Credentials == null)
                throw new ApplicationException("ChannelFactory credentials cannot be null");

            // Sæt sts'ens encryption cert
            channelFactory.Credentials.ServiceCertificate.ScopedCertificates.Add(
                stsAddress.Uri,
                stsEncryptionCert);

            // Sæt client cert som mapper til en bruger i adfs'en
            channelFactory.Credentials.ClientCertificate.Certificate = clientCertificate;
            channelFactory.TrustVersion = TrustVersion.WSTrust13;
            // Konfigurer Windows Identity Foundation
            // Lav channel og brug act as hvis der er et act as token med
            var channel = (WSTrustChannel)channelFactory.CreateChannel();
            try
            {
                // Lav RST med symmetric keys og saml 1.1 token
                var requestSecurityToken = new RequestSecurityToken(RequestTypes.Issue)
                {
                    AppliesTo = new EndpointReference(serviceAddress.ToString()),
                    TokenType = SecurityTokenTypes.Saml11TokenProfile11,
                    KeyType = KeyTypes.Symmetric
                };

                // Påhæft evt. ActAs token
                if (actAsSecurityToken != null)
                    requestSecurityToken.ActAs = new SecurityTokenElement(actAsSecurityToken);

                // Send request (RST) og modtag response (RSTR)
                var securityToken = channel.Issue(requestSecurityToken);

                return securityToken;
            }
            finally
            {
                CloseChannel(channel);
            }
        }
        /// <summary>
        /// Requester et security token fra AD FS 2.0 sts'en via 
        /// user name authentication 
        /// Token'et kan derpå vedhæftes et kald til en service.
        /// Der kan angives
        /// et optionelt ActAs security token. Hvis dette token angives
        /// vil kaldet til STS'en blive et ActAs kald
        /// </summary>
        public static SecurityToken RequestSecurityTokenWithUserName(
           EndpointAddress stsAddress,
           X509Certificate2 stsEncryptionCert,
           EndpointAddress serviceAddress,
           string dmpUserName,
           string dmpPassword,
           SecurityToken actAsSecurityToken = null)
        {
            // Certificate binding med ws-trust 1.3 og message security
            var stsBinding = new UserNameWSTrustBinding
            {
                SecurityMode = SecurityMode.Message,
                TrustVersion = TrustVersion.WSTrust13
            };

            // Lav channel factory
            var channelFactory = new WSTrustChannelFactory(
                stsBinding,
                stsAddress);

            if (channelFactory.Credentials == null)
                throw new ApplicationException("ChannelFactory credentials cannot be null");

            // Sæt sts'ens encryption cert
            channelFactory.Credentials.ServiceCertificate.ScopedCertificates.Add(
                stsAddress.Uri,
                stsEncryptionCert);

            // Sæt client credentials som mapper til en dmp bruger
            channelFactory.Credentials.UserName.UserName = dmpUserName;
            channelFactory.Credentials.UserName.Password = dmpPassword;
            channelFactory.TrustVersion = TrustVersion.WSTrust13;

            //ALA: I Added this bit to get the token from Identify:
            channelFactory.Credentials.UseIdentityConfiguration = true;
            //END

            // Konfigurer Windows Identity Foundation
            var channel = (WSTrustChannel)channelFactory.CreateChannel();
            try
            {
                // Lav RST med symmetric keys og saml 1.1 token
                var requestSecurityToken = new RequestSecurityToken(RequestTypes.Issue)
                {
                    AppliesTo = new EndpointReference(serviceAddress.ToString()),
                    TokenType = SecurityTokenTypes.Saml11TokenProfile11,
                    KeyType = KeyTypes.Symmetric
                };

                // Påhæft evt. ActAs token
                if (actAsSecurityToken != null)
                    requestSecurityToken.ActAs = new SecurityTokenElement(actAsSecurityToken);

                // Send request (RST) og modtag response (RSTR)
                var securityToken = channel.Issue(requestSecurityToken);

                return securityToken;
            }
            finally
            {
                CloseChannel(channel);
            }
        }

        public static WS2007HttpBinding GetStsBinding()
        {
            var stsBinding = new WS2007HttpBinding(SecurityMode.Message);
            stsBinding.Security.Message.EstablishSecurityContext = false;

            return stsBinding;
        }

        public static WS2007FederationHttpBinding GetServiceBinding(WS2007HttpBinding stsBinding, EndpointAddress usernameEndpoint, bool negotiateServiceCertificate)
        {
            var serviceBinding = new WS2007FederationHttpBinding(
                WSFederationHttpSecurityMode.Message,
                false);

            var messageSecurity = serviceBinding.Security.Message;

            // Saml 1.1 tokens
            messageSecurity.IssuedTokenType = SecurityTokenTypes.Saml11TokenProfile11;
            // Adresse til STS
            messageSecurity.IssuerAddress = usernameEndpoint;
            // Negotiation slået fra
            messageSecurity.NegotiateServiceCredential = negotiateServiceCertificate;
            // Symmetric keys
            messageSecurity.IssuedKeyType = SecurityKeyType.SymmetricKey;
            // Binding til STS
            messageSecurity.IssuerBinding = stsBinding;

            return serviceBinding;
        }

        public static CustomBinding CreateWsFederationBindingWithoutSecureConversation(WSFederationHttpBinding inputBinding)
        {
            // This CustomBinding starts out identical to the specified WSFederationHttpBinding.
            var outputBinding = new CustomBinding(inputBinding.CreateBindingElements());
            // Find the SecurityBindingElement for message security.
            var security = outputBinding.Elements.Find<SecurityBindingElement>();
            // If the security mode is message, then the secure session settings are the protection token parameters.
            SecureConversationSecurityTokenParameters secureConversation = null;
            if (WSFederationHttpSecurityMode.Message == inputBinding.Security.Mode)
            {
                var symmetricSecurity = security as SymmetricSecurityBindingElement;
                if (symmetricSecurity != null)
                    secureConversation = symmetricSecurity.ProtectionTokenParameters as SecureConversationSecurityTokenParameters;
            }
                // If the security mode is message, then the secure session settings are the endorsing token parameters.
            else if (WSFederationHttpSecurityMode.TransportWithMessageCredential == inputBinding.Security.Mode)
            {
                var transportSecurity = security as TransportSecurityBindingElement;
                if (transportSecurity != null)
                    secureConversation = transportSecurity.EndpointSupportingTokenParameters.Endorsing[0] as SecureConversationSecurityTokenParameters;
            }
            else
            {
                throw new NotSupportedException(String.Format("Unhandled security mode {0}.", inputBinding.Security.Mode));
            }
            // Replace the secure session SecurityBindingElement with the bootstrap SecurityBindingElement.
            int securityIndex = outputBinding.Elements.IndexOf(security);
            if (secureConversation != null)
                outputBinding.Elements[securityIndex] = secureConversation.BootstrapSecurityBindingElement;
            // Return modified binding.
            return outputBinding;
        }

        /// <summary>
        /// Requester et security token fra AD FS 2.0 sts'en via 
        /// user name authentication 
        /// Token'et kan derpå vedhæftes et kald til en service.
        /// Der kan angives
        /// et optionelt ActAs security token. Hvis dette token angives
        /// vil kaldet til STS'en blive et ActAs kald
        /// </summary>
        public static SecurityToken RequestSecurityTokenWithWindowsAuth(
            EndpointAddress stsAddress,
            EndpointAddress serviceAddress,
            string userName = null,
            string password = null)
        {
            // Windows binding og ws-trust 1.3
            var stsBinding = new WindowsWSTrustBinding
            {
                SecurityMode = SecurityMode.Message,
                TrustVersion = TrustVersion.WSTrust13
            };

            // Lav channel factory
            var channelFactory = new WSTrustChannelFactory(
                stsBinding,
                stsAddress);

            if (channelFactory.Credentials == null)
                throw new ApplicationException("ChannelFactory credentials cannot be null");

            if (!string.IsNullOrEmpty(userName))
            {
                // Sæt client credentials til en lokal bruger
                channelFactory.Credentials.Windows.ClientCredential.UserName = userName;
                channelFactory.Credentials.Windows.ClientCredential.Password = password;
                channelFactory.Credentials.UseIdentityConfiguration = true;
            }


            // Lav channel og brug act as hvis der er et act as token med
            var channel = (WSTrustChannel)channelFactory.CreateChannel();

            try
            {
                // Lav RST med symmetric keys og saml 1.1 token
                var requestSecurityToken = new RequestSecurityToken(RequestTypes.Issue)
                {
                    AppliesTo = new EndpointReference(serviceAddress.ToString()),
                    TokenType = SecurityTokenTypes.Saml11TokenProfile11,
                    KeyType = KeyTypes.Symmetric
                };

                // Send request (RST) og modtag response (RSTR)
                var securityToken = channel.Issue(requestSecurityToken);

                return securityToken;
            }
            finally
            {
                CloseChannel(channel);
            }
        }

        /// <summary>
        /// Requester et security token fra AD FS 2.0 sts'en via 
        /// Issued Token authentication
        /// Token'et kan derpå vedhæftes et kald til en service.
        /// Der kan angives et optionelt ActAs security token. 
        /// Hvis dette token angives vil kaldet til STS'en blive et ActAs kald
        /// </summary>
        public static SecurityToken RequestSecurityTokenWithIssuedTokenAuth(
            EndpointAddress stsAddress,
            EndpointAddress serviceAddress,
            EndpointAddress localStsAddress,
            SecurityToken localToken,
            X509Certificate2 stsEncryptionCert)
        {
            // What binding should I use here for the issuer binding?
            var stsBinding = CreateBindingForAdfs2IssuedTokenBinding(localStsAddress);

            // Lav channel factory
            var channelFactory = new WSTrustChannelFactory(
                stsBinding,
                stsAddress);

            if (channelFactory.Credentials != null)
            {
                channelFactory.Credentials.ServiceCertificate.ScopedCertificates.Add(
                    stsAddress.Uri, stsEncryptionCert);
            }

            // Lav channel og brug act as hvis der er et act as token med
            var channel = (WSTrustChannel)channelFactory.CreateChannelWithIssuedToken(localToken);
            try
            {
                // Lav RST med symmetric keys og saml 1.1 token
                var requestSecurityToken = new RequestSecurityToken(RequestTypes.Issue)
                {
                    AppliesTo = new EndpointReference(serviceAddress.ToString()),
                    TokenType = SecurityTokenTypes.Saml11TokenProfile11,
                    KeyType = KeyTypes.Symmetric
                };

                // Send request (RST) og modtag response (RSTR)
                var securityToken = channel.Issue(requestSecurityToken);

                return securityToken;
            }
            finally
            {
                CloseChannel(channel);
            }
        }

        private static CustomBinding CreateBindingForAdfs2IssuedTokenBinding(EndpointAddress localStsAddress)
        {
            // Opret security binding element
            var securityElement = SecurityBindingElement.CreateIssuedTokenForCertificateBindingElement(
                new IssuedSecurityTokenParameters
                {
                    KeySize = 256,
                    // 256 bit keysize
                    KeyType = SecurityKeyType.SymmetricKey,
                    // Symmetriske nøgler
                    TokenType = SecurityTokenTypes.Saml11TokenProfile11, // SAML 1.1 tokens
                    IssuerAddress = localStsAddress,
                    IssuerBinding = new WS2007HttpBinding(SecurityMode.Message)
                });
            securityElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256Sha256;
            securityElement.SecurityHeaderLayout = SecurityHeaderLayout.Strict;
            securityElement.IncludeTimestamp = true;
            securityElement.KeyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;
            securityElement.MessageProtectionOrder = MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature;
            securityElement.MessageSecurityVersion =
                MessageSecurityVersion.
                    WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
            securityElement.RequireSignatureConfirmation = true;

            var textmessageEncoding = new TextMessageEncodingBindingElement
            {
                WriteEncoding = Encoding.UTF8,
                MessageVersion = MessageVersion.Soap12WSAddressing10
            };

            var httpTransport = new HttpTransportBindingElement
            {
                AuthenticationScheme = AuthenticationSchemes.Anonymous,
                ProxyAuthenticationScheme = AuthenticationSchemes.Anonymous
            };

            return new CustomBinding(securityElement, textmessageEncoding, httpTransport);
        }

        public static void CloseChannel(object channel)
        {
            var communicationObject = (ICommunicationObject)channel;

            if (communicationObject.State == CommunicationState.Created ||
                communicationObject.State == CommunicationState.Opened)
            {
                communicationObject.Close();
            }
            else
            {
                communicationObject.Abort();
            }
        }
    }
}
