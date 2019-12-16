using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quantis.WorkFlow.Services;
using Quantis.WorkFlow.Services.API;
using Quantis.WorkFlow.Services.DTOs.Information;
using Quantis.WorkFlow.Services.Framework;
using System.Collections.Generic;

namespace Quantis.Workflow.Complete.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class GlobalFilterController : ControllerBase
    {
        private IGlobalFilterService _globalfilterService;

        public GlobalFilterController(IGlobalFilterService globalfilterService)
        {
            _globalfilterService = globalfilterService;
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetOrganizationHierarcy")]
        public List<HierarchicalNameCodeDTO> GetOrganizationHierarcy(int globalFilterId)
        {
            var usr = HttpContext.User as AuthUser;
            return _globalfilterService.GetOrganizationHierarcy(globalFilterId, usr.UserId);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetContractParties")]
        public List<KeyValuePair<int, string>> GetContractParties(int globalFilterId)
        {
            var usr = HttpContext.User as AuthUser;
            return _globalfilterService.GetContractParties(globalFilterId, usr.UserId);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetContracts")]
        public List<KeyValuePair<int, string>> GetContracts(int globalFilterId, int contractpartyId)
        {
            var usr = HttpContext.User as AuthUser;
            return _globalfilterService.GetContracts(globalFilterId, usr.UserId, contractpartyId);
        }

        [Authorize(WorkFlowPermissions.BASIC_LOGIN)]
        [HttpGet("GetKPIs")]
        public List<KeyValuePair<int, string>> GetKPIs(int globalFilterId, int contractId)
        {
            var usr = HttpContext.User as AuthUser;
            return _globalfilterService.GetKPIs(globalFilterId, usr.UserId, contractId);
        }
    }
}