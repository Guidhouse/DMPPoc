﻿using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace GeoDKPOCDMPTest.Shared
{
    public class UserNameWSTrustBinding : WSTrustBinding
    {
        // Fields
        private HttpClientCredentialType _clientCredentialType;

        // Methods
        public UserNameWSTrustBinding()
            : this(SecurityMode.Message, HttpClientCredentialType.None)
        {
        }

        public UserNameWSTrustBinding(SecurityMode securityMode)
            : base(securityMode)
        {
            if (SecurityMode.Message == securityMode)
            {
                _clientCredentialType = HttpClientCredentialType.None;
            }
        }

        public UserNameWSTrustBinding(SecurityMode mode, HttpClientCredentialType clientCredentialType)
            : base(mode)
        {
            if (!IsHttpClientCredentialTypeDefined(clientCredentialType))
            {
                throw new ArgumentOutOfRangeException("clientCredentialType");
            }

            if (((SecurityMode.Transport == mode) && (HttpClientCredentialType.Digest != clientCredentialType)) &&
                (HttpClientCredentialType.Basic != clientCredentialType))
            {
                throw new InvalidOperationException("ID3225");
            }

            _clientCredentialType = clientCredentialType;
        }

        public HttpClientCredentialType ClientCredentialType
        {
            get { return _clientCredentialType; }
            set
            {
                if (!IsHttpClientCredentialTypeDefined(value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (((SecurityMode.Transport == SecurityMode) && (HttpClientCredentialType.Digest != value)) &&
                    (HttpClientCredentialType.Basic != value))
                {
                    throw new InvalidOperationException("ID3225");
                }
                _clientCredentialType = value;
            }
        }

        protected override void ApplyTransportSecurity(HttpTransportBindingElement transport)
        {
            transport.AuthenticationScheme = _clientCredentialType == HttpClientCredentialType.Basic ? AuthenticationSchemes.Basic : AuthenticationSchemes.Digest;
        }

        protected override SecurityBindingElement CreateSecurityBindingElement()
        {
            if (SecurityMode.Message == SecurityMode)
            {
                return SecurityBindingElement.CreateUserNameForCertificateBindingElement();
            }

            if (SecurityMode.TransportWithMessageCredential == SecurityMode)
            {
                return SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            }

            return null;
        }

        private static bool IsHttpClientCredentialTypeDefined(HttpClientCredentialType value)
        {
            if ((((value != HttpClientCredentialType.None) && (value != HttpClientCredentialType.Basic)) &&
                 ((value != HttpClientCredentialType.Digest) && (value != HttpClientCredentialType.Ntlm))) &&
                (value != HttpClientCredentialType.Windows))
            {
                return (value == HttpClientCredentialType.Certificate);
            }

            return true;
        }

        // Properties
    }
}