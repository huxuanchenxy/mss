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
    public class EquipmentRepairHistoryController : ControllerBase
    {
        private readonly IEquipmentRepairHistoryService _equipmentRepairHistoryService;
        public EquipmentRepairHistoryController(IEquipmentRepairHistoryService equipmentRepairHistoryService)
        {
            _equipmentRepairHistoryService = equipmentRepairHistoryService;

        }

        [HttpPost]
        public ActionResult Save(EquipmentRepairHistory equipmentRepairHistory)
        {
            var ret = _equipmentRepairHistoryService.Save(equipmentRepairHistory);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(EquipmentRepairHistory equipmentRepairHistory)
        {
            var ret = _equipmentRepairHistoryService.Update(equipmentRepairHistory);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _equipmentRepairHistoryService.Delete(ids);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] EquipmentRepairHistoryQueryParm parm)
        {
            var resp = _equipmentRepairHistoryService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _equipmentRepairHistoryService.GetByID(id);
            return Ok(resp.Result);
        }
    }
}