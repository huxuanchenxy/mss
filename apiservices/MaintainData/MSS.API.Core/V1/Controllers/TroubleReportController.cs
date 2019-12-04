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
        public ActionResult ListPage([FromQuery] TroubleReportParm parm)
        {
            var resp = _troubleReportService.ListPage(parm);
            return Ok(resp.Result);
        }

        [HttpGet("ListHistoryByTrouble/{id}")]
        public ActionResult ListHistoryByTrouble(int id)
        {
            var resp = _troubleReportService.ListHistoryByTrouble(id);
            return Ok(resp.Result);
        }

        [HttpGet("ListEqpByTrouble/{id}/{topOrg}/{troubleView}")]
        public ActionResult ListEqpByTrouble(int id,int topOrg, TroubleView troubleView)
        {
            var resp = _troubleReportService.ListEqpByTrouble(id, topOrg, troubleView);
            return Ok(resp.Result);
        }
        [HttpPut("AssignEqp/{troubleEqps}")]
        public ActionResult AssignEqp(string troubleEqps)
        {
            List<TroubleEqp> eqps = JsonConvert.DeserializeObject<List<TroubleEqp>>(troubleEqps);
            var resp = _troubleReportService.AssignEqp(eqps);
            return Ok(resp.Result);
        }
        [HttpPut("operation/{ids}/{operation}/{content}")]
        public ActionResult Operation(string ids, TroubleOperation operation, string content)
        {
            var resp = _troubleReportService.Operation(ids, operation, content);
            return Ok(resp.Result);
        }

        [HttpPost]
        public ActionResult Save(TroubleReport troubleReport)
        {
            var ret = _troubleReportService.Save(troubleReport);
            return Ok(ret.Result);
        }

        [HttpPut()]
        public ActionResult Update(TroubleReport troubleReport)
        {
            var resp = _troubleReportService.Update(troubleReport);
            return Ok(resp.Result);
        }

        #region deal
        [HttpPost("SaveDeal")]
        public ActionResult SaveDeal(TroubleDeal troubleDeal)
        {
            var ret = _troubleReportService.SaveDeal(troubleDeal);
            return Ok(ret.Result);
        }
        [HttpGet("GetDealByID/{id}/{orgTop}")]
        public ActionResult GetDealByID(int id,int orgTop)
        {
            var resp = _troubleReportService.GetDealByID(id,orgTop);
            return Ok(resp.Result);
        }
        #endregion
    }
}