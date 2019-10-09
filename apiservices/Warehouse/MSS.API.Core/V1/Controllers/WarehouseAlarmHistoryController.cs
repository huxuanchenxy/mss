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
    public class WarehouseAlarmHistoryController : ControllerBase
    {
        private readonly IWarehouseAlarmHistoryService _warehouseAlarmHistoryService;
        public WarehouseAlarmHistoryController(IWarehouseAlarmHistoryService warehouseAlarmHistoryService)
        {
            _warehouseAlarmHistoryService = warehouseAlarmHistoryService;

        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] WarehouseAlarmHistoryQueryParm parm)
        {
            var resp = _warehouseAlarmHistoryService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

    }
}