//------------------------------------------------------------------------------
// <generato automaticamente>
//     Questo codice è stato generato da uno strumento.
//     //
//     Le modifiche apportate a questo file possono causare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </generato automaticamente>
//------------------------------------------------------------------------------

namespace BSIReports
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://www.oblicore.com", ConfigurationName = "BSIReports.ReportsSoap")]
    public interface ReportsSoap
    {
        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetFolderList", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetFolderListAsync(string sessionID);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetMyReports", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetMyReportsAsync(string sessionID);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetMyReportsAdvanced", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.GetMyReportsAdvanced1> GetMyReports1Async(BSIReports.GetMyReportsAdvanced request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetReportsByFolderID", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetReportsByFolderIDAsync(string sessionID, int folderID);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetReportsByFolderIDAdvanced", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.GetReportsByFolderIDAdvanced1> GetReportsByFolderID1Async(BSIReports.GetReportsByFolderIDAdvanced request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetReportsByFolderName", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetReportsByFolderNameAsync(string sessionID, string folderName);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetReportsByFolderNameAdvanced", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.GetReportsByFolderNameAdvanced1> GetReportsByFolderName1Async(BSIReports.GetReportsByFolderNameAdvanced request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetAllReports", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetAllReportsAsync(string sessionID);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetAllReportsAdvanced", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.GetAllReportsAdvanced1> GetAllReports1Async(BSIReports.GetAllReportsAdvanced request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetReportData", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<System.Xml.Linq.XElement> GetReportDataAsync(string sessionID, int favoriteID, int pictureRequest, int width, int height);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetReportDataAdvanced", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<BSIReports.GetReportDataAdvanced1> GetReportData1Async(BSIReports.GetReportDataAdvanced request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetReportDataAndExport", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<System.Xml.Linq.XElement> GetReportDataAndExportAsync(string sessionID, int favoriteID, string parametersXML, string exportXML, string criteriaXML);

        [System.ServiceModel.OperationContractAttribute(Action = "http://www.oblicore.com/GetReportParameterizedXML", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        System.Threading.Tasks.Task<System.Xml.Linq.XElement> GetReportParameterizedXMLAsync(string sessionID, int reportID);
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetMyReportsAdvanced", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetMyReportsAdvanced
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public string sessionID;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 1)]
        public string criteriaXML;

        public GetMyReportsAdvanced()
        {
        }

        public GetMyReportsAdvanced(string sessionID, string criteriaXML)
        {
            this.sessionID = sessionID;
            this.criteriaXML = criteriaXML;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetMyReportsAdvancedResponse", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetMyReportsAdvanced1
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public BSIReports.ArrayOfXElement GetMyReportsAdvancedResult;

        public GetMyReportsAdvanced1()
        {
        }

        public GetMyReportsAdvanced1(BSIReports.ArrayOfXElement GetMyReportsAdvancedResult)
        {
            this.GetMyReportsAdvancedResult = GetMyReportsAdvancedResult;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetReportsByFolderIDAdvanced", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetReportsByFolderIDAdvanced
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public string sessionID;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 1)]
        public int folderID;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 2)]
        public string criteriaXML;

        public GetReportsByFolderIDAdvanced()
        {
        }

        public GetReportsByFolderIDAdvanced(string sessionID, int folderID, string criteriaXML)
        {
            this.sessionID = sessionID;
            this.folderID = folderID;
            this.criteriaXML = criteriaXML;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetReportsByFolderIDAdvancedResponse", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetReportsByFolderIDAdvanced1
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public BSIReports.ArrayOfXElement GetReportsByFolderIDAdvancedResult;

        public GetReportsByFolderIDAdvanced1()
        {
        }

        public GetReportsByFolderIDAdvanced1(BSIReports.ArrayOfXElement GetReportsByFolderIDAdvancedResult)
        {
            this.GetReportsByFolderIDAdvancedResult = GetReportsByFolderIDAdvancedResult;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetReportsByFolderNameAdvanced", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetReportsByFolderNameAdvanced
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public string sessionID;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 1)]
        public string folderName;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 2)]
        public string criteriaXML;

        public GetReportsByFolderNameAdvanced()
        {
        }

        public GetReportsByFolderNameAdvanced(string sessionID, string folderName, string criteriaXML)
        {
            this.sessionID = sessionID;
            this.folderName = folderName;
            this.criteriaXML = criteriaXML;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetReportsByFolderNameAdvancedResponse", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetReportsByFolderNameAdvanced1
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public BSIReports.ArrayOfXElement GetReportsByFolderNameAdvancedResult;

        public GetReportsByFolderNameAdvanced1()
        {
        }

        public GetReportsByFolderNameAdvanced1(BSIReports.ArrayOfXElement GetReportsByFolderNameAdvancedResult)
        {
            this.GetReportsByFolderNameAdvancedResult = GetReportsByFolderNameAdvancedResult;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetAllReportsAdvanced", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetAllReportsAdvanced
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public string sessionID;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 1)]
        public string criteriaXML;

        public GetAllReportsAdvanced()
        {
        }

        public GetAllReportsAdvanced(string sessionID, string criteriaXML)
        {
            this.sessionID = sessionID;
            this.criteriaXML = criteriaXML;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetAllReportsAdvancedResponse", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetAllReportsAdvanced1
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public BSIReports.ArrayOfXElement GetAllReportsAdvancedResult;

        public GetAllReportsAdvanced1()
        {
        }

        public GetAllReportsAdvanced1(BSIReports.ArrayOfXElement GetAllReportsAdvancedResult)
        {
            this.GetAllReportsAdvancedResult = GetAllReportsAdvancedResult;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetReportDataAdvanced", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetReportDataAdvanced
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public string sessionID;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 1)]
        public int favoriteID;

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 2)]
        public string criteriaXML;

        public GetReportDataAdvanced()
        {
        }

        public GetReportDataAdvanced(string sessionID, int favoriteID, string criteriaXML)
        {
            this.sessionID = sessionID;
            this.favoriteID = favoriteID;
            this.criteriaXML = criteriaXML;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "GetReportDataAdvancedResponse", WrapperNamespace = "http://www.oblicore.com", IsWrapped = true)]
    public partial class GetReportDataAdvanced1
    {
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.oblicore.com", Order = 0)]
        public System.Xml.Linq.XElement GetReportDataAdvancedResult;

        public GetReportDataAdvanced1()
        {
        }

        public GetReportDataAdvanced1(System.Xml.Linq.XElement GetReportDataAdvancedResult)
        {
            this.GetReportDataAdvancedResult = GetReportDataAdvancedResult;
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface ReportsSoapChannel : BSIReports.ReportsSoap, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class ReportsSoapClient : System.ServiceModel.ClientBase<BSIReports.ReportsSoap>, BSIReports.ReportsSoap
    {
        /// <summary>
        /// Implementare questo metodo parziale per configurare l'endpoint servizio.
        /// </summary>
        /// <param name="serviceEndpoint">Endpoint da configurare</param>
        /// <param name="clientCredentials">Credenziali del client</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);

        public ReportsSoapClient(EndpointConfiguration endpointConfiguration) :
                base(ReportsSoapClient.GetBindingForEndpoint(endpointConfiguration), ReportsSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public ReportsSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) :
                base(ReportsSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public ReportsSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) :
                base(ReportsSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public ReportsSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetFolderListAsync(string sessionID)
        {
            return base.Channel.GetFolderListAsync(sessionID);
        }

        public System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetMyReportsAsync(string sessionID)
        {
            return base.Channel.GetMyReportsAsync(sessionID);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BSIReports.GetMyReportsAdvanced1> BSIReports.ReportsSoap.GetMyReports1Async(BSIReports.GetMyReportsAdvanced request)
        {
            return base.Channel.GetMyReports1Async(request);
        }

        public System.Threading.Tasks.Task<BSIReports.GetMyReportsAdvanced1> GetMyReports1Async(string sessionID, string criteriaXML)
        {
            BSIReports.GetMyReportsAdvanced inValue = new BSIReports.GetMyReportsAdvanced();
            inValue.sessionID = sessionID;
            inValue.criteriaXML = criteriaXML;
            return ((BSIReports.ReportsSoap)(this)).GetMyReports1Async(inValue);
        }

        public System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetReportsByFolderIDAsync(string sessionID, int folderID)
        {
            return base.Channel.GetReportsByFolderIDAsync(sessionID, folderID);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BSIReports.GetReportsByFolderIDAdvanced1> BSIReports.ReportsSoap.GetReportsByFolderID1Async(BSIReports.GetReportsByFolderIDAdvanced request)
        {
            return base.Channel.GetReportsByFolderID1Async(request);
        }

        public System.Threading.Tasks.Task<BSIReports.GetReportsByFolderIDAdvanced1> GetReportsByFolderID1Async(string sessionID, int folderID, string criteriaXML)
        {
            BSIReports.GetReportsByFolderIDAdvanced inValue = new BSIReports.GetReportsByFolderIDAdvanced();
            inValue.sessionID = sessionID;
            inValue.folderID = folderID;
            inValue.criteriaXML = criteriaXML;
            return ((BSIReports.ReportsSoap)(this)).GetReportsByFolderID1Async(inValue);
        }

        public System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetReportsByFolderNameAsync(string sessionID, string folderName)
        {
            return base.Channel.GetReportsByFolderNameAsync(sessionID, folderName);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BSIReports.GetReportsByFolderNameAdvanced1> BSIReports.ReportsSoap.GetReportsByFolderName1Async(BSIReports.GetReportsByFolderNameAdvanced request)
        {
            return base.Channel.GetReportsByFolderName1Async(request);
        }

        public System.Threading.Tasks.Task<BSIReports.GetReportsByFolderNameAdvanced1> GetReportsByFolderName1Async(string sessionID, string folderName, string criteriaXML)
        {
            BSIReports.GetReportsByFolderNameAdvanced inValue = new BSIReports.GetReportsByFolderNameAdvanced();
            inValue.sessionID = sessionID;
            inValue.folderName = folderName;
            inValue.criteriaXML = criteriaXML;
            return ((BSIReports.ReportsSoap)(this)).GetReportsByFolderName1Async(inValue);
        }

        public System.Threading.Tasks.Task<BSIReports.ArrayOfXElement> GetAllReportsAsync(string sessionID)
        {
            return base.Channel.GetAllReportsAsync(sessionID);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BSIReports.GetAllReportsAdvanced1> BSIReports.ReportsSoap.GetAllReports1Async(BSIReports.GetAllReportsAdvanced request)
        {
            return base.Channel.GetAllReports1Async(request);
        }

        public System.Threading.Tasks.Task<BSIReports.GetAllReportsAdvanced1> GetAllReports1Async(string sessionID, string criteriaXML)
        {
            BSIReports.GetAllReportsAdvanced inValue = new BSIReports.GetAllReportsAdvanced();
            inValue.sessionID = sessionID;
            inValue.criteriaXML = criteriaXML;
            return ((BSIReports.ReportsSoap)(this)).GetAllReports1Async(inValue);
        }

        public System.Threading.Tasks.Task<System.Xml.Linq.XElement> GetReportDataAsync(string sessionID, int favoriteID, int pictureRequest, int width, int height)
        {
            return base.Channel.GetReportDataAsync(sessionID, favoriteID, pictureRequest, width, height);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BSIReports.GetReportDataAdvanced1> BSIReports.ReportsSoap.GetReportData1Async(BSIReports.GetReportDataAdvanced request)
        {
            return base.Channel.GetReportData1Async(request);
        }

        public System.Threading.Tasks.Task<BSIReports.GetReportDataAdvanced1> GetReportData1Async(string sessionID, int favoriteID, string criteriaXML)
        {
            BSIReports.GetReportDataAdvanced inValue = new BSIReports.GetReportDataAdvanced();
            inValue.sessionID = sessionID;
            inValue.favoriteID = favoriteID;
            inValue.criteriaXML = criteriaXML;
            return ((BSIReports.ReportsSoap)(this)).GetReportData1Async(inValue);
        }

        public System.Threading.Tasks.Task<System.Xml.Linq.XElement> GetReportDataAndExportAsync(string sessionID, int favoriteID, string parametersXML, string exportXML, string criteriaXML)
        {
            return base.Channel.GetReportDataAndExportAsync(sessionID, favoriteID, parametersXML, exportXML, criteriaXML);
        }

        public System.Threading.Tasks.Task<System.Xml.Linq.XElement> GetReportParameterizedXMLAsync(string sessionID, int reportID)
        {
            return base.Channel.GetReportParameterizedXMLAsync(sessionID, reportID);
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
            if ((endpointConfiguration == EndpointConfiguration.ReportsSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.ReportsSoap12))
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
            if ((endpointConfiguration == EndpointConfiguration.ReportsSoap))
            {
                return new System.ServiceModel.EndpointAddress("http://93.58.120.77/WebServices/Reports.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.ReportsSoap12))
            {
                return new System.ServiceModel.EndpointAddress("http://93.58.120.77/WebServices/Reports.asmx");
            }
            throw new System.InvalidOperationException(string.Format("L\'endpoint denominato \'{0}\' non è stato trovato.", endpointConfiguration));
        }

        public enum EndpointConfiguration
        {
            ReportsSoap,

            ReportsSoap12,
        }
    }

    [System.Xml.Serialization.XmlSchemaProviderAttribute(null, IsAny = true)]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class ArrayOfXElement : object, System.Xml.Serialization.IXmlSerializable
    {
        private System.Collections.Generic.List<System.Xml.Linq.XElement> nodesList = new System.Collections.Generic.List<System.Xml.Linq.XElement>();

        public ArrayOfXElement()
        {
        }

        public virtual System.Collections.Generic.List<System.Xml.Linq.XElement> Nodes
        {
            get
            {
                return this.nodesList;
            }
        }

        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new System.NotImplementedException();
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            System.Collections.Generic.IEnumerator<System.Xml.Linq.XElement> e = nodesList.GetEnumerator();
            for (
            ; e.MoveNext();
            )
            {
                ((System.Xml.Serialization.IXmlSerializable)(e.Current)).WriteXml(writer);
            }
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            for (
            ; (reader.NodeType != System.Xml.XmlNodeType.EndElement);
            )
            {
                if ((reader.NodeType == System.Xml.XmlNodeType.Element))
                {
                    System.Xml.Linq.XElement elem = new System.Xml.Linq.XElement("default");
                    ((System.Xml.Serialization.IXmlSerializable)(elem)).ReadXml(reader);
                    Nodes.Add(elem);
                }
                else
                {
                    reader.Skip();
                }
            }
        }
    }
}