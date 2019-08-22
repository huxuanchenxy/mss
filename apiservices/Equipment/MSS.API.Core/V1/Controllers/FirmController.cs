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
    public class FirmController : ControllerBase
    {
        private readonly IFirmService _firmService;
        public FirmController(IFirmService firmService)
        {
            _firmService = firmService;

        }

        [HttpPost]
        public ActionResult Save(Firm firm)
        {
            var ret = _firmService.Save(firm);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(Firm firm)
        {
            var ret = _firmService.Update(firm);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _firmService.Delete(ids);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] FirmQueryParm parm)
        {
            var resp = _firmService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _firmService.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpGet("ListByType/{type}")]
        public ActionResult ListByType(int? type)
        {
            var resp = _firmService.ListByType(type);
            return Ok(resp.Result);
        }

        [HttpGet("All")]
        public ActionResult ListAll()
        {
            var resp = _firmService.ListAll();
            return Ok(resp.Result);
        }
    }
}