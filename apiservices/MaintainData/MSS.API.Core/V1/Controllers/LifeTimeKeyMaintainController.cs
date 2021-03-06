﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.API.Core.V1.Business;
using MSS.API.Model.DTO;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LifeTimeKeyMaintainController : ControllerBase
    {
        private readonly ILifeTimeKeyMaintainService _lifeTimeKeyMaintainService;
        private readonly int _userId;
        public LifeTimeKeyMaintainController(ILifeTimeKeyMaintainService lifeTimeKeyMaintainService)
        {
            _lifeTimeKeyMaintainService = lifeTimeKeyMaintainService;
            _userId = 1;
        }

        // 查询
        [HttpGet("GetLifeTimeKeyListByPage")]
        public ActionResult<ApiResult> GetListByPage([FromQuery] LifeTimeKeyMaintainQurey query)
        {
            //string where = "  1=1 "; 
            //if (!string.IsNullOrEmpty(query.device_type))
            //{
            //    where += " and  a.eqp_type= '" + query.device_type.Trim() + "'";
            //}
            //if (!string.IsNullOrEmpty(query.device_id))
            //{
            //    where += " and  a.id= '" + query.device_id.Trim() + "'";
            //}
            //if (!string.IsNullOrEmpty(query.maintain_type))
            //{
            //    where += " and   IsDaXiuOrZhongXiu(a.large_repair, a.medium_repair, a.online_date, a.online_again, a.life)= '" + query.maintain_type.Trim() + "'";
            //}
            //var ret = await _lifeTimeKeyMaintainService.GetListByPage(where, query.sort, query.order, query.page, query.rows);
            var resp = _lifeTimeKeyMaintainService.GetListByPage(query);
            return Ok(resp.Result);
        }

        // 查询
        [HttpGet("GetDeviceListByTypeId/{device_type}")]
        public async Task<ActionResult<ApiResult>> GetdeviceList(int device_type)
        {
            string where = " and 1=1 ";
            if (device_type!=0)
            {
                where += " and  eqp_type= " + device_type;
            }
            var ret = await _lifeTimeKeyMaintainService.GetdeviceList(where);
            return ret;
        }

        // 查询
        [HttpGet("GetEqpByTypeAndLine/{type}")]
        public async Task<ActionResult<ApiResult>> ListCascaderByEqpType(int type)
        {
            var ret = await _lifeTimeKeyMaintainService.ListCascaderByEqpTypeAndLine(type,Const.LINE);
            return ret;
        }
    }
}