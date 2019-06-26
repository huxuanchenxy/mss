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
    public class WarnSettingController : ControllerBase
    {
        private readonly IWarnningSettingService  _warnService;
        public WarnSettingController(IWarnningSettingService warnService)
        {
            _warnService = warnService;

        }

        // 添加
        [HttpPost]
        public async Task<ActionResult<ApiResult>> SaveWarnningSetting(EarlyWarnningSetting setting)
        {
            setting.CreatedBy = 1;
            setting.CreatedTime = DateTime.Now;
            setting.IsDel = false;
            var ret = await _warnService.SaveWarnningSetting(setting);
            return ret;
        }

    }
}