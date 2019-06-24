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
    public class ActionController : ControllerBase
    {
        private readonly IActionService _ActionService;
        public ActionController(IActionService ActionService)

        {
            //_logger = logger;
            //_mediator = mediator;
            //_cache = cache;
            _ActionService = ActionService;

        }
        [HttpPost]
        public ActionResult GetPageByParm([FromBody] ActionQueryParm parm)
        {
            var ret = _ActionService.GetPageByParm(parm).Result;
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
            var resp = _ActionService.GetByID(id);
            return Ok(resp.Result);
        }
        [HttpPost]
        public ActionResult Add([FromBody] ActionInfo action)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            action.created_by = userID;
            action.updated_by = userID;
            var resp = _ActionService.Add(action);
            return Ok(resp.Result);
        }
        [HttpPost]
        public ActionResult Update([FromBody] ActionInfo action)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            action.updated_by = userID;
            var resp = _ActionService.Update(action);
            return Ok(resp.Result);
        }
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            var resp = _ActionService.Delete(ids);
            return Ok(resp.Result);
        }
        [HttpPost]
        public ActionResult GetAll()
        {
            var resp = _ActionService.GetAll();
            return Ok(resp.Result);
        }
        [HttpPost]
        public ActionResult GetActionTree()
        {
            var resp = _ActionService.GetActionTree();
            return Ok(resp.Result);
        }
    }
}