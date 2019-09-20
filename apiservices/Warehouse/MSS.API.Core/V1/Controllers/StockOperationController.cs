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
using Newtonsoft.Json;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StockOperationController : ControllerBase
    {
        private readonly IStockOperationService _stockOperationService;
        public StockOperationController(IStockOperationService stockOperationService)
        {
            _stockOperationService = stockOperationService;

        }

        #region 库存操作
        [HttpPost]
        public ActionResult Save(StockOperation stockOperation)
        {
            var ret = _stockOperationService.Save(stockOperation);
            return Ok(ret.Result);
        }

        [HttpGet("ListByReason/{reason}")]
        public ActionResult ListByReason(int reason)
        {
            var resp = _stockOperationService.ListByReason(reason);
            return Ok(resp.Result);
        }

        [HttpGet("ListByOperation/{operation}")]
        public ActionResult ListByOperation(int operation)
        {
            var resp = _stockOperationService.ListByOperation(operation);
            return Ok(resp.Result);
        }

        [HttpGet("ListByOperationForEdit/{operation}")]
        public ActionResult ListByOperationForEdit(int operation)
        {
            var resp = _stockOperationService.GetByID(operation);
            ApiResult ret = resp.Result;
            StockOperation so = (StockOperation)ret.data;
            ret.data = new
            {
                so.WarehouseName,
                so.Warehouse,
                detailList = JsonConvert.DeserializeObject<List<StockOperationDetail>>(so.DetailList)
            };
            return Ok(ret);
        }

        [HttpGet("GetStockDetailByID/{id}")]
        public ActionResult GetStockDetailByID(int id)
        {
            var resp = _stockOperationService.GetStockDetailByID(id);
            return Ok(resp.Result);
        }

        [HttpGet("GetSparePartsByWH/{id}")]
        public ActionResult ListByWH(int id)
        {
            var resp = _stockOperationService.ListByWH(id);
            return Ok(resp.Result);
        }

        [HttpGet("GetStockDetailAll")]
        public ActionResult ListStockDetail()
        {
            var resp = _stockOperationService.ListStockDetail();
            return Ok(resp.Result);
        }
        #endregion

        #region 库存操作查询
        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] StockOperationQueryParm parm)
        {
            var resp = _stockOperationService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _stockOperationService.GetByID(id);
            return Ok(resp.Result);
        }
        #endregion

        #region 库存查询
        [HttpGet("ListStockSum")]
        public ActionResult GetStockSumPageByParm([FromQuery] StockSumQueryParm parm)
        {
            var resp = _stockOperationService.GetStockSumPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("ListStockDetail/{spareParts}/{warehouse}")]
        public ActionResult ListStockDetailBySPs(int spareParts,int warehouse)
        {
            var resp = _stockOperationService.ListStockDetailBySPsAndWH(spareParts,warehouse);
            return Ok(resp.Result);
        }
        #endregion
    }
}