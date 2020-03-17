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
    public class HealthConfigController : ControllerBase
    {
        private readonly IHealthConfigService _healthConfigService;
        public HealthConfigController(IHealthConfigService healthConfigService)
        {
            _healthConfigService = healthConfigService;

        }

        [HttpPost]
        public ActionResult Save([FromBody]List<HealthConfig> hConfigs)
        {
            var ret = _healthConfigService.UpdateList(hConfigs);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(HealthConfig hConfig)
        {
            var ret = _healthConfigService.Update(hConfig);
            return Ok(ret.Result);
        }

        [HttpDelete("{compsiteID}")]
        public ActionResult Delete(string compsiteID)
        {
            string[] tmp = compsiteID.Split(',');
            int? eqpType = null;
            if (tmp.Count() > 1) eqpType = Convert.ToInt32(tmp[1]);
            var resp = _healthConfigService.Delete(
                Convert.ToInt32(tmp[0]), eqpType);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] HealthConfigQueryParm parm)
        {
            var resp = _healthConfigService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("GetByType/{type}")]
        public ActionResult GetByType(int type)
        {
            var resp = _healthConfigService.GetByType(type);
            return Ok(resp.Result);
        }

        [HttpGet("{compsiteID}")]
        public ActionResult GetByTroubleLevel(string compsiteID)
        {
            string[] tmp = compsiteID.Split(',');
            var resp = _healthConfigService.GetByTroubleLevel(
                Convert.ToInt32(tmp[0]), Convert.ToInt32(tmp[1]));
            return Ok(resp.Result);
        }
    }
}