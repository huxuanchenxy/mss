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
    public class StockOperationController : ControllerBase
    {
        private readonly IStockOperationService _stockOperationService;
        public StockOperationController(IStockOperationService stockOperationService)
        {
            _stockOperationService = stockOperationService;

        }

        [HttpPost]
        public ActionResult Save(StockOperation stockOperation)
        {
            var ret = _stockOperationService.Save(stockOperation);
            return Ok(ret.Result);
        }

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
    }
}