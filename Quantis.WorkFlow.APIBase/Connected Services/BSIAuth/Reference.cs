//------------------------------------------------------------------------------
// <generato automaticamente>
//     Questo codice è stato generato da uno strumento.
//     //
//     Le modifiche apportate a questo file possono causare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </generato automaticamente>
//------------------------------------------------------------------------------

namespace BSIAuth
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://www.oblicore.com", ConfigurationName = "BSIAuth.OblicoreAuthSoap")]
    public interface OblicoreAuthSoap
    {
        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/AuthenticateUser", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<string> AuthenticateUserAsync(string userName, string organizationName);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/CreateSessionContext", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<string> CreateSessionContextAsync(string userName, string organizationName);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/ClearSessionContext", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task ClearSessionContextAsync(string sessionContextID);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface OblicoreAuthSoapChannel : BSIAuth.OblicoreAuthSoap, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class OblicoreAuthSoapClient : System.ServiceModel.ClientBase<BSIAuth.OblicoreAuthSoap>, BSIAuth.OblicoreAuthSoap
    {
        /// <summary>
        /// Implementare questo metodo parziale per configurare l'endpoint servizio.
        /// </summary>
        /// <param name="serviceEndpoint">Endpoint da configurare</param>
        /// <param name="clientCredentials">Credenziali del client</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);

        public OblicoreAuthSoapClient(EndpointConfiguration endpointConfiguration) :
                base(OblicoreAuthSoapClient.GetBindingForEndpoint(endpointConfiguration), OblicoreAuthSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public OblicoreAuthSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) :
                base(OblicoreAuthSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public OblicoreAuthSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) :
                base(OblicoreAuthSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public OblicoreAuthSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public System.Threading.Tasks.Task<string> AuthenticateUserAsync(string userName, string organizationName)
        {
            return base.Channel.AuthenticateUserAsync(userName, organizationName);
        }

        public System.Threading.Tasks.Task<string> CreateSessionContextAsync(string userName, string organizationName)
        {
            return base.Channel.CreateSessionContextAsync(userName, organizationName);
        }

        public System.Threading.Tasks.Task ClearSessionContextAsync(string sessionContextID)
        {
            return base.Channel.ClearSessionContextAsync(sessionContextID);
        }

        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }

        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }

        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.OblicoreAuthSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.OblicoreAuthSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpTransportBindingElement httpBindingElement = new System.ServiceModel.Channels.HttpTransportBindingElement();
                httpBindingElement.AllowCookies = true;
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("L\'endpoint denominato \'{0}\' non è stato trovato.", endpointConfiguration));
        }

        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.OblicoreAuthSoap))
            {
                return new System.ServiceModel.EndpointAddress("http://93.58.120.77/WebServices/OblicoreAuth.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.OblicoreAuthSoap12))
            {
                return new System.ServiceModel.EndpointAddress("http://93.58.120.77/WebServices/OblicoreAuth.asmx");
            }
            throw new System.InvalidOperationException(string.Format("L\'endpoint denominato \'{0}\' non è stato trovato.", endpointConfiguration));
        }

        public enum EndpointConfiguration
        {
            OblicoreAuthSoap,

            OblicoreAuthSoap12,
        }
    }
}