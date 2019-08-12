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
using MSS.API.Common.Utility;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WarnSettingController : ControllerBase
    {
        private readonly IWarnningSettingService  _warnService;
        private readonly int _userId;
        private readonly IAuthHelper _authHelper;
        public WarnSettingController(IWarnningSettingService warnService, IAuthHelper authHelper)
        {
            _warnService = warnService;
            _authHelper = authHelper;
            _userId = _authHelper.GetUserId();
        }

        // 添加
        [HttpPost]
        public async Task<ActionResult<ApiResult>> SaveWarnningSetting(EarlyWarnningSetting setting)
        {
            setting.CreatedBy = _userId;
            setting.CreatedTime = DateTime.Now;
            setting.UpdatedBy = _userId;
            setting.UpdatedTime = DateTime.Now;
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
        public async Task<ActionResult<ApiResult>> GetWarnningSettingByPage([FromQuery]WarnSettingQueryParm query)
        {
            int? eqpTypeID = null;
            if (!string.IsNullOrEmpty(query.EquipmentTypeID)) {
                eqpTypeID = Convert.ToInt32(query.EquipmentTypeID);
            }
            var ret = await _warnService.ListWarnningSettingByPage(query.page, query.rows, query.sort, query.order,
                eqpTypeID, query.ParamID);
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult>> GetWarnningSettingByID(int id)
        {
            var ret = await _warnService.GetWarnningSettingByID(id);
            return ret;
        }

        [HttpDelete("{ids}")]
        public async Task<ActionResult<ApiResult>> Delete(string ids)
        {
            List<EarlyWarnningSetting > list = new List<EarlyWarnningSetting>();
            string[] idsArry = ids.Split(',');
            foreach (string item in idsArry)
            {
                int id = Convert.ToInt32(item);
                EarlyWarnningSetting setting = new EarlyWarnningSetting();
                setting.UpdatedBy = _userId;
                setting.UpdatedTime = DateTime.Now;
                setting.ID = id;
                list.Add(setting);
            }
            if (list.Count > 0)
            {
                var ret = await _warnService.DeleteListWarnningSetting(list);
                return ret;
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("extype")]
        public async Task<ActionResult<ApiResult>> GetWarnningSettingExType()
        {
            var ret = await _warnService.ListWarnningSettingExType();
            return ret;
        }

        [HttpGet("props/{eqpTypeID}")]
        public async Task<ActionResult<ApiResult>> GetEqpPropByEqpType(int eqpTypeID)
        {
            var ret = await _warnService.ListEqpPropByEqpTypeID(eqpTypeID);
            return ret;
        }
    }
}