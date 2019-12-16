using EventForm.bcfed9e1.Class4;
using FormAdapterService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace FormAdapterService.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "Api is running";
        }
        [HttpPost]
        public string APIToWorkflow(APIToWorkflowDTO dto)
        {
            Class4 class4 = new Class4();
            return class4.Convert(dto.input);
        }
        [HttpGet]
        public string APIToWorkflowSample()
        {
            string input= "<DataLoadingForms><ControlsXml><Xml>*securestring*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</Xml></ControlsXml></DataLoadingForms>";
            Class4 class4 = new Class4();
            return class4.Convert(input);
        }
        [HttpPost]
        public string SendSOAPRequest(UploadTicketDTO dto)
        {
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("sid", dto.sid);
                parameters.Add("repositoryHandle", dto.repositoryHandle);
                parameters.Add("objectHandle", dto.objectHandle);
                parameters.Add("description", dto.description);
                parameters.Add("fileName", dto.fileName);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                var xmlStr = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ser=\"http://www.ca.com/UnicenterServicePlus/ServiceDesk\">" + Environment.NewLine + "<soapenv:Header/>" + Environment.NewLine + "<soapenv:Body>" + Environment.NewLine + "<ser:{0}>" + Environment.NewLine + "{1}" + Environment.NewLine + "</ser:{0}>" + Environment.NewLine + "</soapenv:Body>" + Environment.NewLine + "</soapenv:Envelope>";
                string parms = string.Join(string.Empty, parameters.Select(kv => String.Format("<{0}>{1}</{0}>", kv.Key, kv.Value)).ToArray());
                var s = String.Format(xmlStr, dto.action, parms);
                soapEnvelopeXml.LoadXml(s);
                // Create the web request
                string boundary = "=" + DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(dto.url);
                webRequest.ContentType = string.Format("multipart/related; type=\"text/xml\"; start=\"<rootpart@soapui.org> \"; boundary=\"{0}\"", boundary);
                webRequest.Headers.Add("Accept-Encoding", "gzip,deflate");
                webRequest.Headers.Add("SOAPAction", "");
                webRequest.Headers.Add("MIME-Version", "1.0");
                webRequest.Accept = "application/xml";
                webRequest.Method = "POST";
                // Insert SOAP envelope
                using (Stream stream = webRequest.GetRequestStream())
                {
                    string topBoundry = "--" + boundary + Environment.NewLine + "Content-Type: text/xml; charset=UTF-8" + Environment.NewLine + "Content-Transfer-Encoding: 8bit" + Environment.NewLine + "Content-ID: <rootpart@soapui.org>" + Environment.NewLine + Environment.NewLine;
                    byte[] topBoundryBytes = Encoding.UTF8.GetBytes(topBoundry);
                    stream.Write(topBoundryBytes, 0, topBoundryBytes.Length);
                    soapEnvelopeXml.Save(stream);
                    //_dbcontext.LogInformation(soapEnvelopeXml.OuterXml);

                    var filename = parameters["fileName"];
                    string fileHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine + "Content-Type: text/plain; charset=us-ascii; name={0}" + Environment.NewLine + "Content-Transfer-Encoding: 7bit" + Environment.NewLine + "Content-ID: <{0}>" + Environment.NewLine + "Content-Disposition: attachment; name=\"{0}\"; filename=\"{0}\"" + Environment.NewLine;
                    fileHeaderTemplate = string.Format(fileHeaderTemplate, filename);
                    byte[] fileHeaderBytes = Encoding.UTF8.GetBytes(fileHeaderTemplate + Environment.NewLine);
                    stream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                    //_dbcontext.LogInformation(Encoding.UTF8.GetString(fileHeaderBytes, 0, fileHeaderBytes.Length));
                    stream.Write(dto.fileData, 0, dto.fileData.Length);
                    //_dbcontext.LogInformation(Encoding.UTF8.GetString(fileData, 0, fileData.Length));
                    byte[] fileHeaderBytes2 = Encoding.UTF8.GetBytes(Environment.NewLine + "--" + boundary + "--" + Environment.NewLine);
                    stream.Write(fileHeaderBytes2, 0, fileHeaderBytes2.Length);
                    //_dbcontext.LogInformation(Encoding.UTF8.GetString(fileHeaderBytes2, 0, fileHeaderBytes2.Length));

                }

                // Send request and retrieve result
                string result = null;
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        result = rd.ReadToEnd();
                    }
                }
                XDocument xdoc = XDocument.Parse(result);
                var ret = xdoc.DescendantNodes().Last().ToString();
                return "TRUE";
            }
            catch (Exception e)
            {
                return "Exception: " + e.Message;
            }

        }

        [HttpPost]
        public string SendSOAPRequestSample(UploadTicketDTO dto)
        {
            return "TRUE: "+Json(dto);
        }
    }
}