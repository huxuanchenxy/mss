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
            eqp.CreatedBy = 1;
            eqp.UpdatedBy = 1;
            eqp.IsDel = false;

            var ret = _eqpService.Save(eqp, file);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(Equipment eqp)
        {
            eqp.UpdatedBy = 1;
            var ret = _eqpService.Update(eqp);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            int userID = 1;
            var resp = _eqpService.Delete(ids, userID);
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

        [HttpGet("All")]
        public ActionResult GetAll()
        {
            var resp = _eqpService.GetAll();
            return Ok(resp.Result);
        }
    }
}