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
    public class EmergencyPlanController : ControllerBase
    {
        private readonly IEmergencyPlanService _emergencyPlanService;
        public EmergencyPlanController(IEmergencyPlanService emergencyPlanService)
        {
            _emergencyPlanService = emergencyPlanService;

        }

        [HttpPost]
        public ActionResult Save(EmergencyPlan ePlan)
        {
            var ret = _emergencyPlanService.Save(ePlan);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(EmergencyPlan ePlan)
        {
            var ret = _emergencyPlanService.Update(ePlan);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _emergencyPlanService.Delete(ids);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] EPlanQueryParm parm)
        {
            var resp = _emergencyPlanService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _emergencyPlanService.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpGet("All")]
        public ActionResult GetAll()
        {
            var resp = _emergencyPlanService.GetAll();
            return Ok(resp.Result);
        }
    }
}