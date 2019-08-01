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

        [HttpGet("config")]
        public ActionResult<ApiResult> updateWarnSetting()
        {
            ApiResult ret = new ApiResult { code = Code.Success };
            _globalDataManager.postUpdateEvent(3);
            return ret;
        }

    }
}