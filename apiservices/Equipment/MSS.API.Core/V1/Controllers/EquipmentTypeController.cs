﻿using System;
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
    public class EquipmentTypeController : ControllerBase
    {
        private readonly IEquipmentTypeService _eqpTypeService;
        public EquipmentTypeController(IEquipmentTypeService eqpTypeService)
        {
            _eqpTypeService = eqpTypeService;

        }

        [HttpPost]
        public ActionResult Save(EquipmentType eqpType)
        {
            var ret = _eqpTypeService.Save(eqpType);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(EquipmentType eqpType)
        {
            var ret = _eqpTypeService.Update(eqpType);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _eqpTypeService.Delete(ids);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] EqpTypeQueryParm parm)
        {
            var resp = _eqpTypeService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _eqpTypeService.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpGet("All")]
        public ActionResult GetAll()
        {
            var resp = _eqpTypeService.GetAll();
            return Ok(resp.Result);
        }

        [HttpGet("mockdata")]
        public ActionResult MockData()
        {
            var resp = _eqpTypeService.MockData();
            return Ok(resp.Result);
        }
    }
}