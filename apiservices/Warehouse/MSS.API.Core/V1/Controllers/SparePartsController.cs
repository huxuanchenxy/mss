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
    public class SparePartsController : ControllerBase
    {
        private readonly ISparePartsService _sparePartsService;
        public SparePartsController(ISparePartsService sparePartsService)
        {
            _sparePartsService = sparePartsService;

        }

        [HttpPost]
        public ActionResult Save(SpareParts spareParts)
        {
            var ret = _sparePartsService.Save(spareParts);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(SpareParts spareParts)
        {
            var ret = _sparePartsService.Update(spareParts);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _sparePartsService.Delete(ids);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] SparePartsQueryParm parm)
        {
            var resp = _sparePartsService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _sparePartsService.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpGet("All")]
        public ActionResult GetAll()
        {
            var resp = _sparePartsService.GetAll();
            return Ok(resp.Result);
        }
    }
}