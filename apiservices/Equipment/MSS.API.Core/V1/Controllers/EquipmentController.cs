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
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _eqpService;
        public EquipmentController(IEquipmentService eqpService)
        {
            _eqpService = eqpService;

        }

        [HttpPost]
        public ActionResult Save(Equipment eqp)
        {
            var ret = _eqpService.Save(eqp);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(Equipment eqp)
        {
            var ret = _eqpService.Update(eqp);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _eqpService.Delete(ids);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] EqpQueryParm parm)
        {
            var resp = _eqpService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _eqpService.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpGet("Detail/{id}")]
        public ActionResult GetDetailByID(int id)
        {
            var resp = _eqpService.GetDetailByID(id);
            return Ok(resp.Result);
        }

        [HttpGet("ListByPosition/{location}/{locationBy}")]
        public ActionResult ListByPosition(int location, int locationBy)
        {
            var resp = _eqpService.ListByPosition(location, locationBy);
            return Ok(resp.Result);
        }

        [HttpGet("All")]
        public ActionResult GetAll()
        {
            var resp = _eqpService.GetAll();
            return Ok(resp.Result);
        }
    }
}