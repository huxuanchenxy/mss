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
    public class OrgController : ControllerBase
    {
        private readonly IOrgService _orgService;
        private readonly int _userId;
        public OrgController(IOrgService orgService)

        {
            //_logger = logger;
            //_mediator = mediator;
            //_cache = cache;
            _orgService = orgService;
            _userId = 1;

        }

        [HttpGet("all")]
        public async Task<ActionResult<DataResult>> Get()
        {
            var ret = await _orgService.GetAllOrg();
            return ret;
        }

        [HttpGet("curorg")]
        public async Task<ActionResult<DataResult>> GetByUserID()
        {
            int userId = _userId;
            var ret = await _orgService.GetOrgByUserID(userId);
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DataResult>> Get(int id)
        {
            var ret = await _orgService.GetOrgByIDs(new List<int>{id});
            return ret;
        }

        // 添加
        [HttpPost]
        public async Task<ActionResult<ApiResult>> AddOrgNode(OrgTree node)
        {
            node.CreatedBy = _userId;
            node.CreatedTime = DateTime.Now;
            node.IsDel = false;
            var ret = await _orgService.AddOrgNode(node);
            return ret;
        }

        // 更新
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult>> UpdateOrgNode(int id, OrgTree node, [FromHeader]string token)
        {
            node.UpdatedBy = _userId;
            node.UpdatedTime = DateTime.Now;
            var ret = await _orgService.UpdateOrgNode(node);
            // return CreatedAtRoute("GetById", new { id = id }, node);
            return ret;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DataResult>> Delete(int id)
        {
            OrgTree node = new OrgTree();
            node.UpdatedBy = _userId;
            node.UpdatedTime = DateTime.Now;
            node.ID = id;
            var ret = await _orgService.DeleteOrgNode(node);

            return ret;
        }

        // 组织节点绑定用户
        [HttpPost("user")]
        public async Task<ActionResult<DataResult>> BindOrgUser(OrgUserView nodeView)
        {
            nodeView.CreatedBy = _userId;
            nodeView.CreatedTime = DateTime.Now;
            var ret = await _orgService.BindOrgNodeUsers(nodeView);
            return ret;
        }
        [HttpGet("user/{id}")]
        public async Task<ActionResult<DataResult>> getOrgUser(int id)
        {
            var ret = await _orgService.GetOrgNodeUsers(id);
            return ret;
        }
        [HttpGet("user/all/{id}")]
        public async Task<ActionResult<DataResult>> getCanSelectedUser(int id)
        {
            var ret = await _orgService.GetCanSelectedUsers(id);
            return ret;
        }
        [HttpGet("nodetype")]
        public async Task<ActionResult<DataResult>> getNodeType()
        {
            var ret = await _orgService.GetNodeType();
            return ret;
        }
        [HttpGet("node/{id}")]
        public async Task<ActionResult<DataResult>> getOrgNode(int id)
        {
            var ret = await _orgService.GetOrgNode(id);
            return ret;
        }
    }
}