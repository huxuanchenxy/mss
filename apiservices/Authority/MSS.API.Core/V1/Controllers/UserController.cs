using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.API.Core.Infrastructure;
using MSS.API.Core.V1.Business;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using Newtonsoft.Json;
using static MSS.API.Common.Const;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    //[EnableCors("AllowAll")]
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

        [HttpGet("QueryList")]
        public ActionResult GetPageByParm([FromQuery] UserQueryParm parm)
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

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _UserService.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpPost("Add")]
        public ActionResult Add(User user)
        {
            //int userID = (int)HttpContext.Session.GetInt32("UserID");
            int userID = 1;
            user.created_by = userID;
            user.updated_by = userID;
            var resp = _UserService.Add(user);
            return Ok(resp.Result);
        }

        [HttpPut("Update")]
        public ActionResult Update(User user)
        {
            //int userID = (int)HttpContext.Session.GetInt32("UserID");
            int userID = 1;
            user.updated_by = userID;
            var resp = _UserService.Update(user);
            return Ok(resp.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            int userID = 1;
            var resp = _UserService.Delete(ids,userID);
            return Ok(resp.Result);
        }

        [HttpGet("All")]
        public ActionResult GetAll()
        {
            var resp = _UserService.GetAll();
            return Ok(resp.Result);
        }

        [HttpPut("ChangePwd/{oldPwd}/{newPwd}")]
        public ActionResult ChangePwd(string oldPwd,string newPwd)
        {
            //int userID = (int)HttpContext.Session.GetInt32("UserID");
            int userID = 1;
            var resp = _UserService.ChangePwd(userID, oldPwd, newPwd);
            return Ok(resp.Result);
        }


        [HttpPut("ResetPwd/{ids}")]
        public ActionResult ResetPwd(string ids)
        {
            //int userID = (int)HttpContext.Session.GetInt32("UserID");
            int userID = 1;
            var resp = _UserService.ResetPwd(ids, userID);
            return Ok(resp.Result);
        }

        [HttpGet("Login/{acc}/{pwd}")]
        public ActionResult Login(string acc,string pwd)
        {
            var resp = _UserService.CheckUserLogin(acc,pwd).Result;
            if (resp.code == (int)ErrType.OK)
            {
                User u = (User)resp.relatedData;
                HttpContext.Session.SetInt32("UserID", u.id);
                HttpContext.Session.SetString("UserName", u.user_name);
            }
            return Ok(resp);
        }

        [HttpGet("GetMenu")]
        public string GetMenu()
        {
            //int userID = (int)HttpContext.Session.GetInt32("UserID");
            int userID = 1;
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
            StringBuilder strJson = new StringBuilder();
            strJson.Append("{");
            int i = 0;
            foreach (var item in ret.data)
            {
                i++;
                //strJson.Append("\"" + item.path + "\": " + JsonConvert.SerializeObject(item));
                strJson.Append("\"" + item.order + "\": " + JsonConvert.SerializeObject(item));
                if (i < ret.data.Count()) strJson.Append(",");
            }
            strJson.Append("}");
            return strJson.ToString();
        }

    }
}