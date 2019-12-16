using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quantis.WorkFlow.Services;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.BSI;
using Quantis.WorkFlow.Services.Framework;
using System.Collections.Generic;
using Quantis.WorkFlow.Services.DTOs.Widgets;

namespace Quantis.Workflow.Complete.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class BSIController : ControllerBase
    {
        private IBSIService _bsiAPI { get; set; }

        public BSIController(IBSIService bsiAPI)
        {
            _bsiAPI = bsiAPI;
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetMyNormalReports")]
        public List<BSIReportLVDTO> GetMyNormalReports()
        {
            var user = HttpContext.User as AuthUser;
            return _bsiAPI.GetMyNormalReports(user.UserName);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetAllNormalReports")]
        public List<BSIReportLVDTO> GetAllNormalReports()
        {
            var user = HttpContext.User as AuthUser;
            return _bsiAPI.GetAllNormalReports(user.UserName);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetReportDetail")]
        public BSIReportMainDTO GetReportDetail(int reportId)
        {
            var user = HttpContext.User as AuthUser;
            return _bsiAPI.GetReportDetail(user.UserName, reportId);
        }
        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetReportDetailTestAllZero")]
        public BSIReportMainDTO GetReportDetailTestAllZero(int reportId)
        {
            var user = HttpContext.User as AuthUser;
            var report= _bsiAPI.GetReportDetail(user.UserName, reportId);
            foreach (var r in report.Reports)
            {
                foreach (var d in r.Data)
                {
                    d.YValue = 0.0;
                    d.Description = "NE";
                }
            }
            return report;
        }
        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetReportDetailTestRandomZero")]
        public BSIReportMainDTO GetReportDetailTestRandomZero(int reportId)
        {
            var user = HttpContext.User as AuthUser;
            var report = _bsiAPI.GetReportDetail(user.UserName, reportId);
            foreach (var r in report.Reports)
            {
                foreach (var d in r.Data)
                {
                    Random rnd = new Random();
                    if (rnd.Next(1,11)<=5)
                    {
                        d.YValue = 0.0;
                        d.Description = "NE";
                    }
                    
                }
            }
            return report;
        }
        [HttpGet("GetAllUserReports")]
        public List<BSIUserFolderDTO> GetAllUserReports()
        {
            return _bsiAPI.GetAllUserReports();
        }
    }
}