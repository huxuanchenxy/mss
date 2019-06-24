using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.API.Core.Infrastructure;
using MSS.API.Core.V1.Business;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using static MSS.API.Common.Const;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _RoleService;
        public RoleController(IRoleService RoleService)

        {
            //_logger = logger;
            //_mediator = mediator;
            //_cache = cache;
            _RoleService = RoleService;

        }
        [HttpGet("QueryList")]
        public ActionResult GetPageByParm([FromQuery] RoleQueryParm parm)
        {
            var ret = _RoleService.GetPageByParm(parm).Result;
            if (ret.code==(int)ErrType.OK)
            {
                var data = new { rows = ret.data, total = ret.relatedData };
                var resp = new { code = ret.code, data = data };
                return Ok(resp);
            }
            else
            {
                var resp = new { code = ret.code, msg = ret.msg };
                return Ok(resp);
            }
        }
        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _RoleService.GetByID(id);
            return Ok(resp.Result);
        }
        [HttpPost("Add")]
        public ActionResult Add(RoleStrActions roleStrActions)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            roleStrActions.created_by = userID;
            roleStrActions.updated_by = userID;
            var resp = _RoleService.Add(roleStrActions);
            return Ok(resp.Result);
        }
        [HttpPut("Update")]
        public ActionResult Update(RoleStrActions roleStrActions)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            roleStrActions.updated_by = userID;
            var resp = _RoleService.Update(roleStrActions);
            return Ok(resp.Result);
        }
        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _RoleService.Delete(ids);
            return Ok(resp.Result);
        }
        [HttpGet("All")]
        public ActionResult GetAll()
        {
            var resp = _RoleService.GetAll();
            return Ok(resp.Result);
        }
    }
}