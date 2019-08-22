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
    public class WarehouseAlarmController : ControllerBase
    {
        private readonly IWarehouseAlarmService _warehouseAlarmService;
        public WarehouseAlarmController(IWarehouseAlarmService warehouseAlarmService)
        {
            _warehouseAlarmService = warehouseAlarmService;

        }

        [HttpPost]
        public ActionResult Save(WarehouseAlarm warehouseAlarm)
        {
            var ret = _warehouseAlarmService.Save(warehouseAlarm);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(WarehouseAlarm warehouseAlarm)
        {
            var ret = _warehouseAlarmService.Update(warehouseAlarm);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _warehouseAlarmService.Delete(ids);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] WarehouseAlarmQueryParm parm)
        {
            var resp = _warehouseAlarmService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _warehouseAlarmService.GetByID(id);
            return Ok(resp.Result);
        }
    }
}