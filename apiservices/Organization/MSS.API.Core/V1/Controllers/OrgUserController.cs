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

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrgUserController : ControllerBase
    {
        private readonly IOrgService _orgService;
        private readonly int _userId;
        public OrgUserController(IOrgService orgService)
        {
            _orgService = orgService;
            _userId = 1;
        }

        [HttpGet]
        public async Task<ActionResult<DataResult>> Get()
        {
            var ret = await _orgService.GetOrgUserByUserID(_userId);
            return ret;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DataResult>> Get(int id)
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
    }
}