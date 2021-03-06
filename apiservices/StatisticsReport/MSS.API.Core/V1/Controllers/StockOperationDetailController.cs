﻿using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.API.Core.V1.Business;
using MSS.API.Model.Data;
using System.Threading.Tasks;

// Coded By admin 2019/11/29 17:56:33
namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StockOperationDetailController : ControllerBase
    {
        private readonly IStockOperationDetailService _service;

        public StockOperationDetailController(IStockOperationDetailService service)
        {
            _service = service;
        }

        [HttpGet("GetStockOperationChart")]
        public async Task<ActionResult<ApiResult>> GetStockOperationChart([FromQuery] StockOperationDetailParm parm)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.GetStockOperationChart(parm);

            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "获取报表数据StockOperationDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpPut]
        public async Task<ActionResult<ApiResult>> Update(StockOperationDetail obj)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.Update(obj);
            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "更新数据StockOperationDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpDelete("{ids}")]
        public async Task<ActionResult<ApiResult>> Delete(string ids)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.Delete(ids);
            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "删除数据StockOperationDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult>> Save(StockOperationDetail obj)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.Save(obj);
            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "新增数据StockOperationDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult>> GetByID(int id)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.GetByID(id);

            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "获取单个数据StockOperationDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }
    }
}



