using Microsoft.Extensions.Configuration;
using Quantis.WorkFlow.APIBase.Framework;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.BSI;
using Quantis.WorkFlow.Services.DTOs.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Quantis.WorkFlow.APIBase.API
{
    public class BSIService : IBSIService
    {
        private BSIAuth.OblicoreAuthSoapClient _authService = null;
        private BSIReports.ReportsSoapClient _reportService = null;
        private readonly WorkFlowPostgreSqlContext _dbcontext;
        private readonly IConfiguration _configuration;

        public BSIService(IConfiguration configuration, WorkFlowPostgreSqlContext dbcontext)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
            if (_authService == null)
            {
                _authService = new BSIAuth.OblicoreAuthSoapClient(BSIAuth.OblicoreAuthSoapClient.EndpointConfiguration.OblicoreAuthSoap, _configuration["BSIAuthWebServices"]);
            }
            if (_reportService == null)
            {
                _reportService = new BSIReports.ReportsSoapClient(BSIReports.ReportsSoapClient.EndpointConfiguration.ReportsSoap, _configuration["BSIReportsWebServices"]);
            }
        }

        public List<BSIReportLVDTO> GetMyNormalReports(string userName)
        {
            var session = Login(userName);
            var myreports = _reportService.GetMyReportsAsync(session).Result;
            if (myreports.Nodes[1].Element("Result") == null)
            {
                return new List<BSIReportLVDTO>();
            }
            var list = myreports.Nodes[1].Element("Result").Elements();
            Logout(session);
            var reports = parseReports(list);
            return reports.Where(o => o.ReportType == "NORMAL" || o.ReportType == "GROUP").ToList();
        }

        public List<BSIUserFolderDTO> GetAllUserReports()
        {
            var results = new List<BSIUserFolderDTO>();
            var users = _dbcontext.TUsers.ToList();
            foreach (var user in users)
            {
                try
                {
                    var username = user.user_name;
                    var userContext = Login(username);
                    var xmlResult = _reportService.GetFolderListAsync(userContext).Result;
                    if (xmlResult.Nodes.Any())
                    {
                        var elems = xmlResult.Nodes[1].Element("Result").Elements();
                        foreach (var elem in elems)
                        {
                            var result = new BSIUserFolderDTO();
                            result.UserName = username;
                            result.ReportID = int.Parse(elem.Element("ITEM_ID").Value);
                            result.ReportName = elem.Element("ITEM_NAME").Value;
                            result.UserId = int.Parse(elem.Element("USER_ID").Value);
                            result.IsMyFolder = elem.Element("IS_MY_FOLDER").Value;
                            results.Add(result);
                        }
                    }
                    Logout(userContext);
                }
                catch (Exception e)
                {

                }                
            }
            return results;

        }

        public List<BSIReportLVDTO> GetAllNormalReports(string userName)
        {
            var session = Login(userName);
            var myreports = _reportService.GetAllReportsAsync(session).Result;
            if (myreports.Nodes[1].Element("Result") == null)
            {
                return new List<BSIReportLVDTO>();
            }
            var list = myreports.Nodes[1].Element("Result").Elements();
            Logout(session);
            var reports = parseReports(list);
            return reports.Where(o => o.ReportType == "NORMAL" || o.ReportType == "GROUP").ToList();
        }

        public BSIReportMainDTO GetReportDetail(string userName, int reportId)
        {
            var results = new BSIReportMainDTO();
            results.Reports = new List<BSIReportDetailDTO>();
            var session = Login(userName);
            var report = _reportService.GetReportDataAsync(session, reportId, 0, 100, 100).Result;
            results.ResultType = report.Attribute("TYPE").Value;                                
            Logout(session);
            if (results.ResultType == "GROUP")
            {
                results.Name = report.Elements().FirstOrDefault().Value;
            }
            else
            {
                results.Name = report.Elements().FirstOrDefault().Element("NAME").Value;
            }
            var baseElements = report.Elements().FirstOrDefault().Elements("ITEM");
            if (results.ResultType == "GROUP")
            {
                baseElements = report.Elements("ITEM").SelectMany(o=>o.Elements("ITEM"));
            }
            foreach (var baseElement in baseElements)
            {
                var result = new BSIReportDetailDTO();
                var reportInfo = baseElement.Element("REPORT_INFO");
                result.Name = baseElement.Element("NAME")?.Value;
                result.XLabel = reportInfo.Element("ByX")?.Value;
                result.YLabel = reportInfo.Element("ByY")?.Value;
                result.ReportType = reportInfo.Element("Report_Type")?.Value;
                result.ReportTitle = reportInfo.Element("Report_Title")?.Value;
                result.FromDate = reportInfo.Element("DateFromOrg")?.Value;
                result.ToDate = reportInfo.Element("DateToOrg")?.Value;

                var reportInfoTitle = reportInfo.Element("TITLE");
                result.ContractParty = reportInfoTitle.Element("Customer")?.Value;
                result.Contract = reportInfoTitle.Element("SLA")?.Value;
                result.Rule = reportInfoTitle.Element("Rule")?.Value;
                result.Application = reportInfoTitle.Element("Application")?.Value;
                result.ServiceDomain = reportInfoTitle.Element("ServiceDomain")?.Value;
                result.DomainCategory = reportInfoTitle.Element("DomainCategory")?.Value;
                result.Incomplete = reportInfoTitle.Element("Incomplete")?.Value;
                result.MetricType = reportInfoTitle.Element("MetricType")?.Value;
                result.DataGranularity = reportInfoTitle.Element("DataGranularity")?.Value;

                result.DefAgg = reportInfo.Element("DefAgg")?.Value;
                result.LocaleId = int.Parse(reportInfo.Element("LocaleId")?.Value);
                result.Units = reportInfo.Element("Units")?.Value;
                result.GridUnits = reportInfo.Element("GridUnits")?.Value;
                result.Messages = new List<string>();
                var reportInfoMessages = reportInfo.Element("MESSAGES").Elements();
                foreach (var m in reportInfoMessages)
                {
                    result.Messages.Add(m?.Value);
                }

                var reportInfoCal = reportInfo.Element("CALC_STATUS");
                result.CalculationStatusText = reportInfoCal.Element("TEXT")?.Value;
                result.CalculationStatusBookletText = reportInfoCal.Element("BOOKLET_TEXT")?.Value;
                result.CalculationStatusLastDate = reportInfoCal.Element("LAST_CALC_DATE")?.Value;
                result.Data = new List<Services.DTOs.Widgets.XYZDTO>();
                result.GlobalRuleId= int.Parse(reportInfo.Element("FILTER").Element("RULE").Value);
                var reportGrid = reportInfo.Element("GRID").Elements();
                foreach (var series in reportGrid)
                {
                    var elems = series.Elements();
                    string label = "";
                    foreach (var data in elems)
                    {
                        if (data.Name == "TITLE")
                        {
                            label = data.Value;
                        }
                        else
                        {
                            var dto = new XYZDTO();
                            dto.ZValue = label;
                            dto.XValue = data.Element("LABLE")?.Value;
                            dto.YValue = (data.Element("VALUE")?.Value == "") ? null : (double?)double.Parse(data.Element("VALUE").Value);
                            result.Data.Add(dto);
                        }
                    }
                }
                results.Reports.Add(result);
            }

            return results;
        }

        private List<BSIReportLVDTO> parseReports(IEnumerable<XElement> elements)
        {
            var results = new List<BSIReportLVDTO>();
            foreach (var element in elements)
            {
                var result = new BSIReportLVDTO();
                result.ItemId = int.Parse(element.Element("ITEM_ID").Value);
                result.ItemName = element.Element("ITEM_NAME").Value;
                result.UserId = int.Parse(element.Element("USER_ID").Value);
                result.FolderId = int.Parse(element.Element("FOLDER_ID").Value);
                result.FolderName = element.Element("FOLDER_NAME").Value;
                result.IsParameterized = (element.Element("IS_PARAMETERIZED").Value == "1");
                result.IsExecutable = (element.Element("IS_EXECUTABLE").Value == "1");
                result.ReportType = element.Element("REPORT_TYPE").Value;
                result.ModifiedDate = DateTime.Parse(element.Element("MODIFY_DATE").Value);

                results.Add(result);
            }
            return results;
        }

        private string Login(string userName)
        {
            var sessionId = _authService.CreateSessionContextAsync(userName, "Poste Italiane").Result;
            return sessionId;
        }

        private void Logout(string sessionID)
        {
            _authService.ClearSessionContextAsync(sessionID);
        }

        ~BSIService()
        {
            int x = 1;
        }
    }
}