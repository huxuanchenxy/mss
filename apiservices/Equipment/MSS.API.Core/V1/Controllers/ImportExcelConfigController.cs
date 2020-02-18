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
    public class ImportExcelConfigController : ControllerBase
    {
        private readonly IImportExcelConfigService _service;
        public ImportExcelConfigController(IImportExcelConfigService service)
        {
            _service = service;
        }

        [HttpPost("GetExcelField")]
        [RequestSizeLimit(52428800)]
        public ActionResult GetExcelField(IFormFile file)
        {
            ImportExcelHelper importExcelHelper = new ImportExcelHelper();
            string errMsg = "";
            ApiResult ret = new ApiResult();
            ret.data = importExcelHelper.GetExcelField(file, ref errMsg);
            ret.msg = errMsg;
            return Ok(ret);
        }
        [HttpGet("ListClass")]
        public ActionResult ListClass()
        {
            var ret = _service.ListClass();
            return Ok(ret.Result);
        }

        [HttpGet("ListPropertyByClass/{id}")]
        public ActionResult ListPropertyByClass(int id)
        {
            ApiResult ret = new ApiResult();
            ImportExcelClass tmp = _service.GetClassByID(id).Result.data as ImportExcelClass;
            ImportExcelHelper importExcelHelper = new ImportExcelHelper(tmp.FullName, tmp.AssemblyName);
            ret.data = importExcelHelper.classes;
            return Ok(ret);
        }

        [HttpPost]
        public ActionResult Save(ImportExcelConfig obj)
        {
            var ret = _service.Save(obj);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(ImportExcelConfig obj)
        {
            var ret = _service.Update(obj);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _service.Delete(ids);
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
        public ActionResult GetPageByParm([FromQuery] ImportExcelConfigParm parm)
        {
            var resp = _service.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("ListLogByParm")]
        public ActionResult ListLogByParm([FromQuery] ImportExcelLogParm parm)
        {
            var resp = _service.ListLogByParm(parm);
            return Ok(resp.Result);
        }
    }
}