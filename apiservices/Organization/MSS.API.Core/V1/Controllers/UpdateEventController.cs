using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.API.Core.EventServer;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UpdateEventController : ControllerBase
    {
        private readonly GlobalDataManager  _globalDataManager;
        public UpdateEventController(GlobalDataManager globalDataManager)
        {
            _globalDataManager = globalDataManager;
        }

        [HttpGet("warnsetting")]
        public ActionResult<ApiResult> updateWarnSetting()
        {
            ApiResult ret = new ApiResult { code = Code.Success };
            _globalDataManager.postUpdateEvent(Common.UpdateEventType.InitWarnSetting);
            return ret;
        }

        [HttpGet("eqp")]
        public ActionResult<ApiResult> updateEquipmentList()
        {
            ApiResult ret = new ApiResult { code = Code.Success };
            _globalDataManager.postUpdateEvent(Common.UpdateEventType.InitEquipment);
            return ret;
        }

        // 更新顶级节点下所有用户
        [HttpGet("toporg")]
        public ActionResult<ApiResult> updateTopOrg()
        {
            ApiResult ret = new ApiResult { code = Code.Success };
            _globalDataManager.postUpdateEvent(Common.UpdateEventType.InitTopOrg);
            return ret;
        }

    }
}