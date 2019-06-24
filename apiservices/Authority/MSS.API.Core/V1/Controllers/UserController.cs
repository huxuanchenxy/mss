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
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;
        public UserController(IUserService UserService)

        {
            //_logger = logger;
            //_mediator = mediator;
            //_cache = cache;
            _UserService = UserService;

        }

        [HttpPost]
        public ActionResult GetPageByParm([FromBody] UserQueryParm parm)
        {
            var ret = _UserService.GetPageByParm(parm).Result;
            if (ret.code == (int)ErrType.OK)
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
            var resp = _UserService.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpPost]
        public ActionResult Add(User user)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            user.created_by = userID;
            user.updated_by = userID;
            var resp = _UserService.Add(user);
            return Ok(resp.Result);
        }

        [HttpPost]
        public ActionResult Update([FromBody] User user)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            user.updated_by = userID;
            var resp = _UserService.Update(user);
            return Ok(resp.Result);
        }

        [HttpPost]
        public ActionResult Delete(string ids)
        {
            var resp = _UserService.Delete(ids);
            return Ok(resp.Result);
        }

        [HttpPost]
        public ActionResult GetAll()
        {
            var resp = _UserService.GetAll();
            return Ok(resp.Result);
        }

        [HttpPost]
        public ActionResult ChangePwd(ChangePwdParm parm)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            var resp = _UserService.ChangePwd(userID, parm.oldPwd, parm.newPwd);
            return Ok(resp.Result);
        }


        [HttpPost]
        public ActionResult ResetPwd(string ids)
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            var resp = _UserService.ResetPwd(ids, userID);
            return Ok(resp.Result);
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            var resp = _UserService.CheckUserLogin(user).Result;
            if (resp.code == (int)ErrType.OK)
            {
                User u = (User)resp.relatedData;
                HttpContext.Session.SetInt32("UserID", u.id);
                HttpContext.Session.SetString("UserName", u.user_name);
            }
            return Ok(resp);
        }

        [HttpPost]
        public ActionResult GetMenu()
        {
            int userID = (int)HttpContext.Session.GetInt32("UserID");
            var resp = _UserService.GetByID(userID);
            User u = (User)resp.Result.data;
            MSSResult<MenuTree> ret = new MSSResult<MenuTree>();
            if (u.is_super)
            {
                ret = _UserService.GetMenu().Result;
            }
            else
            {
                ret = _UserService.GetMenu(userID).Result;
            }
            return Ok(ret);
        }

    }
}