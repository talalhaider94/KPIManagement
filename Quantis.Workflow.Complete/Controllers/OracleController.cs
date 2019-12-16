using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quantis.WorkFlow.Services;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.API;
using Quantis.WorkFlow.Services.DTOs.OracleAPI;
using Quantis.WorkFlow.Services.Framework;
using System.Collections.Generic;

namespace Quantis.WorkFlow.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class OracleController : ControllerBase
    {
        private IOracleDataService _oracleAPI { get; set; }
        private IDataService _dataAPI { get; set; }

        public OracleController(IOracleDataService oracleAPI, IDataService dataAPI)
        {
            _oracleAPI = oracleAPI;
            _dataAPI = dataAPI;
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetCustomerById/{id}")]
        public List<OracleCustomerDTO> GetCustomerById(int id)
        {
            return _oracleAPI.GetCustomer(id, "");
        }
        [Authorize(WorkFlowPermissions.VIEW_BOOKLET)]
        [HttpGet("GetBookletsLV")]
        public List<BookletLVDTO> GetBookletsLV()
        {
            var usr = HttpContext.User as AuthUser;
            return _oracleAPI.GetBookletsLV(usr.UserId);
        }
        [Authorize(WorkFlowPermissions.VIEW_BOOKLET_FROM_TEMPLATE)]
        [HttpGet("GetBooklets")]
        public List<OracleBookletDTO> GetBooklets()
        {
            return _oracleAPI.GetBooklets();
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetCustomers")]
        public List<OracleCustomerDTO> GetCustomers(string name)
        {
            return _oracleAPI.GetCustomer(0, name);
        }

        /*[Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetFormById/{id}")]
        public List<OracleFormDTO> GetFormById(int id)
        {
            return _oracleAPI.GetForm(id, 0);
        }*/

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetLandingPageByUser")]
        public List<LandingPageDTO> GetLandingPageByUser(string period)
        {
            var usr = HttpContext.User as AuthUser;
            return _oracleAPI.GetLandingPageByUser(usr.UserId, period);
        }

        [HttpGet("GetBSIFreeFormReports")]
        public List<BSIFreeFormReportDTO> GetBSIFreeFormReports()
        {
            return _oracleAPI.GetBSIFreeFormReports();
        }

        /*[Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetFormsByUser")]
        public List<OrcaleFormWithAttachmentCountDTO> GetFormsByUser()
        {
            var usr=HttpContext.User as AuthUser;
            if (usr != null)
            {
                var dtos=_oracleAPI.GetForm(0, usr.UserId);
                var attachmentCount = _dataAPI.GetFormDetials(dtos.Select(o => o.form_id).ToList());

                return (from d in dtos
                    join a in attachmentCount on d.form_id equals a.form_id
                        select new OrcaleFormWithAttachmentCountDTO {
                        form_id = d.form_id,
                        AttachmentsCount= a.attachment_count,
                        create_date=d.create_date,
                        cutoff=d.cutoff,
                        form_description=d.form_description,
                        form_name=d.form_name,
                        form_owner_id=d.form_owner_id,
                        modify_date=d.modify_date,
                        reader_configuration=d.reader_configuration,
                        reader_id=d.reader_id,
                        user_group_id=d.user_group_id,
                        user_group_name=d.user_group_name,
                        latest_input_date=a.latest_modified_date
                    }).ToList();
            }
            return null;
        }
        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetForms")]
        public List<OracleFormDTO> GetForms(int id)
        {
            return _oracleAPI.GetForm(0, 0);
        }*/

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetGroupById/{id}")]
        public List<OracleGroupDTO> GetGroupById(int id)
        {
            return _oracleAPI.GetGroup(id, "");
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetGroups")]
        public List<OracleGroupDTO> GetGroups(string name)
        {
            return _oracleAPI.GetGroup(0, name);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetSlaById/{id}")]
        public List<OracleSlaDTO> GetSlaById(int id)
        {
            return _oracleAPI.GetSla(id, "");
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetSlas")]
        public List<OracleSlaDTO> GetSlas(string name)
        {
            return _oracleAPI.GetSla(0, name);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetRuleById/{id}")]
        public List<OracleRuleDTO> GetRuleById(int id)
        {
            return _oracleAPI.GetRule(id, "");
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetRules")]
        public List<OracleRuleDTO> GetRules(string name)
        {
            return _oracleAPI.GetRule(0, name);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetUserById/{id}")]
        public List<OracleUserDTO> GetUserById(int id)
        {
            return _oracleAPI.GetUser(id, "");
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetUsers")]
        public List<OracleUserDTO> GetUsers(string name)
        {
            return _oracleAPI.GetUser(0, name);
        }

        //[Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetPsl")]
        //public List<PslDTO> GetPsl(string period, string sla_name, string rule_name, string tracking_period)
        public List<PslDTO> GetPsl(string period, int global_rule_id, string tracking_period)
        {
            //return _oracleAPI.GetPsl(period, sla_name, rule_name, tracking_period);
            return _oracleAPI.GetPsl(period, global_rule_id, tracking_period);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetLandingPageLevel1")]
        public List<LandingPageLevel1DTO> GetLandingPageLevel1(int contractPartyId, string period)
        {
            var usr = HttpContext.User as AuthUser;
            return _oracleAPI.GetLandingPageLevel1(usr.UserId, contractPartyId, period);
        }
        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetLandingPageLevel1ByContract")]
        public LandingPageLevel1DTO GetLandingPageLevel1ByContract(int contractId, string period)
        {
            var usr = HttpContext.User as AuthUser;
            return _oracleAPI.GetLandingPageLevel1ByContract(usr.UserId, contractId, period);
        }
        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetLandingPageKPIDetails")]
        public List<LandingPageBaseDTO> GetLandingPageKPIDetails(int contractPartyId, string period)
        {
            var usr = HttpContext.User as AuthUser;
            return _oracleAPI.GetLandingPageKPIDetails(usr.UserId, contractPartyId, period);
        }
        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpPost("GetPersonalReport")]
        public List<ReportPersonalDTO> GetPersonalReport([FromBody]PersonalReportFilterDTO filter)
        {
            return _oracleAPI.GetPersonalReport(filter);
        }
    }
}