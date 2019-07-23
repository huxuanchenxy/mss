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
using System.IO;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IUploadFileService _uploadService;
        public UploadController(IUploadFileService uploadService)
        {
            _uploadService = uploadService;

        }
        [HttpPost]
        public ActionResult Save([FromForm]MyData myData,List<IFormFile> file)
        {
            var ret = _uploadService.Save(myData.type, myData.systemResource, file);
            return Ok(ret.Result);
            //return Ok("");
        }
        public class MyData
        {
            public int type { get; set; }
            public int systemResource { get; set; }
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var ret = _uploadService.Delete(id);
            return Ok(ret.Result);
        }

        [HttpGet("{ids}")]
        public ActionResult ListByIDs(string ids)
        {
            var ret = _uploadService.ListByIDs(ids);
            return Ok(ret.Result);
        }
    }
}