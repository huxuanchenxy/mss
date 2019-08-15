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
using MSS.API.Core.Common;

using MSS.API.Common;
using MSS.API.Common.Utility;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrgUserController : ControllerBase
    {
        private readonly IOrgService _orgService;
        private readonly int _userId;
        private readonly IAuthHelper _authHelper;
        public OrgUserController(IOrgService orgService, IAuthHelper authHelper)
        {
            _orgService = orgService;
            _authHelper = authHelper;
            _userId = _authHelper.GetUserId();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult>> Get()
        {
            var ret = await _orgService.GetOrgUserByUserID(_userId);
            return ret;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult>> Get(int id)
        {
            var ret = await _orgService.GetOrgUserByNodeID(id);
            return ret;
        }
        [HttpGet("selected")]
        public async Task<ActionResult<ApiResult>> GetSelectedUsers()
        {
            var ret = await _orgService.ListAllOrgUsers();
            return ret;
        }
        [HttpDelete("{ids}")]
        public async Task<ActionResult<ApiResult>> Delete(string ids)
        {
            OrgUserView nodeView = new OrgUserView();
            nodeView.UpdatedBy = _userId;
            nodeView.UpdatedTime = DateTime.Now;
            nodeView.UserIDs = new List<int>();

            string[] idsArry = ids.Split(',');
            foreach (string item in idsArry)
            {
                int id = Convert.ToInt32(item);
                nodeView.UserIDs.Add(id);
            }
            if (nodeView.UserIDs.Count > 0)
            {
                var ret = await _orgService.DeleteOrgNodeUsers(nodeView);
                return ret;
            }
            else
            {
                return NotFound();
            }
            
        }

        [HttpGet("usersintoporg")]
        public async Task<ActionResult<ApiResult>> GetTopNodeWithUsers()
        {
            var ret = await _orgService.ListTopNodeWithUsers();
            return ret;
        }
    }
}