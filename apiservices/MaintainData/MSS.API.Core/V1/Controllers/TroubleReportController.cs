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

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TroubleReportController : ControllerBase
    {
        private readonly ITroubleReportService _troubleReportService;
        public TroubleReportController(ITroubleReportService troubleReportService)
        {
            _troubleReportService = troubleReportService;
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _troubleReportService.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult ListPage(TroubleReportParm parm)
        {
            var resp = _troubleReportService.ListPage(parm);
            return Ok(resp.Result);
        }
    }
}