using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSS.API.Core.V1.Business;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using MSS.API.Core.Common;
using MSS.API.Common;
using MSS.API.Common.Utility;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly int _userId;
        private readonly IAuthHelper _authHelper;
        public StatisticsController(IStatisticsService statisticsService, IAuthHelper authHelper)

        {
            _statisticsService = statisticsService;
            _authHelper = authHelper;
            _userId = _authHelper.GetUserId();
        }

        [HttpGet("all")]
        public async Task<ActionResult<ApiResult>> Get()
        {
            // var ret = await _orgService.GetAllOrg();
            return Ok();
        }
    }
}