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
        private readonly int _userId;
        public WarnSettingController(IWarnningSettingService warnService)
        {
            _warnService = warnService;
            _userId = 1;
        }

        // 添加
        [HttpPost]
        public async Task<ActionResult<ApiResult>> SaveWarnningSetting(EarlyWarnningSetting setting)
        {
            setting.CreatedBy = _userId;
            setting.CreatedTime = DateTime.Now;
            setting.IsDel = false;
            var ret = await _warnService.SaveWarnningSetting(setting);
            return ret;
        }

        // 更新
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult>> UpdateWarnningSetting(int id, EarlyWarnningSetting setting)
        {
            setting.UpdatedBy = _userId;
            setting.UpdatedTime = DateTime.Now;
            var ret = await _warnService.UpdateWarnningSetting(setting);
            return ret;
        }

        // 查询
        [HttpGet()]
        public async Task<ActionResult<ApiResult>> GetWarnningSettingByPage(WarnSettingQueryParm query)
        {
            var ret = await _warnService.ListWarnningSettingByPage(query.page, query.rows, query.sort, query.order);
            return ret;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            EarlyWarnningSetting setting = new EarlyWarnningSetting();
            setting.UpdatedBy = _userId;
            setting.UpdatedTime = DateTime.Now;
            setting.ID = id;
            var ret = await _warnService.DeleteWarnningSetting(setting);

            return ret;
        }

    }
}