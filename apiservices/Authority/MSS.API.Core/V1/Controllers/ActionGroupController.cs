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
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class ActionGroupController : ControllerBase
    {
        private readonly IActionGroupService _ActionGroupService;
        public ActionGroupController(IActionGroupService ActionGroupService)

        {
            //_logger = logger;
            //_mediator = mediator;
            //_cache = cache;
            _ActionGroupService = ActionGroupService;

        }
        [HttpPost]
        public ActionResult GetPageByParm([FromBody] ActionGroupQueryParm parm)
        {
            var ret = _ActionGroupService.GetPageByParm(parm).Result;
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
        [HttpPost]
        public ActionResult GetByID(int id)
        {
            var resp = _ActionGroupService.GetByID(id);
            return Ok(resp.Result);
        }
        [HttpPost]
        public ActionResult Add([FromBody] ActionGroup actionGroup)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            actionGroup.created_by = userID;
            actionGroup.updated_by = userID;
            var resp = _ActionGroupService.Add(actionGroup);
            return Ok(resp.Result);
        }
        [HttpPost]
        public ActionResult Update([FromBody] ActionGroup actionGroup)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            actionGroup.updated_by = userID;
            var resp = _ActionGroupService.Update(actionGroup);
            return Ok(resp.Result);
        }
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            var resp = _ActionGroupService.Delete(ids);
            return Ok(resp.Result);
        }
        [HttpPost]
        public ActionResult GetAll()
        {
            var resp = _ActionGroupService.GetAll();
            return Ok(resp.Result);
        }
    }
}