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
    public class EquipmentConfigController : ControllerBase
    {
        private readonly IEquipmentConfigService _service;
        //private readonly IAuthHelper _auth;
        private int _userId;
        public EquipmentConfigController(IEquipmentConfigService service/*, IAuthHelper auth*/)
        {
            _service = service;
            //_auth = auth;
            //_userId = _auth.GetUserId();
            _userId = 1;
        }

        [HttpPost]
        public ActionResult Save(EquipmentConfig obj)
        {
            obj.CreatedBy = _userId;
            var ret = _service.Save(obj);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(EquipmentConfig obj)
        {
            obj.UpdatedBy = _userId;
            var ret = _service.Update(obj);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _service.Delete(ids,_userId);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _service.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpGet("All")]
        public ActionResult GetAll()
        {
            var resp = _service.GetAll();
            return Ok(resp.Result);
        }


        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] EquipmentConfigParm parm)
        {
            var resp = _service.GetPageByParm(parm);
            return Ok(resp.Result);
        }
    }
}