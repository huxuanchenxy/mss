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
    public class EqpHistoryController : ControllerBase
    {
        private readonly IEqpHistoryService _eqpHistoryService;
        private readonly int _userId;
        public EqpHistoryController(IEqpHistoryService eqpHistoryService)
        {
            _eqpHistoryService = eqpHistoryService;
            _userId = 1;
        }

        [HttpGet("ListByEqp/{id}")]
        public ActionResult ListByEqp(int id)
        {
            var resp = _eqpHistoryService.ListByEqp(id);
            return Ok(resp.Result);
        }
        [HttpGet("ListByType/{types}")]
        public ActionResult ListByType(string types)
        {
            var resp = _eqpHistoryService.ListByType(types);
            return Ok(resp.Result);
        }
    }
}