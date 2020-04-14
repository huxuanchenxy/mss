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
using MSS.API.Common;
using Newtonsoft.Json;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HealthChartSubsystemController : ControllerBase
    {
        private readonly IHealthChartSubsystemService _service;
        public HealthChartSubsystemController(IHealthChartSubsystemService service)
        {
            _service = service;
        }

        [HttpGet("GetChart")]
        public ActionResult GetChart([FromQuery] HealthChartSubsystemParm parm)
        {
            var resp = _service.GetChart(parm);
            return Ok(resp.Result);
        }
    }
}