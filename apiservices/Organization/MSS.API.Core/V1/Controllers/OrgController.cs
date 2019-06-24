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

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrgController : ControllerBase
    {
        private readonly IOrgService _OrgService;
        public OrgController(IOrgService orgService)

        {
            //_logger = logger;
            //_mediator = mediator;
            //_cache = cache;
            _OrgService = orgService;

        }

        [HttpGet("all")]
        public async Task<ActionResult<DataResult>> Get()
        {
            var ret = await _OrgService.GetAllOrg();
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DataResult>> Get(int id)
        {
            var ret = await _OrgService.GetOrgByIDs(new List<int>{id});
            return ret;
        }

        // 添加
        [HttpPost]
        public async Task<ActionResult<DataResult>> AddOrgNode(OrgTree node)
        {
            node.CreatedBy = 1;
            node.CreatedTime = DateTime.Now;
            node.IsDel = false;
            var ret = await _OrgService.AddOrgNode(node);
            return ret;
        }

        // 更新
        [HttpPut("{id}")]
        public async Task<ActionResult<DataResult>> UpdateOrgNode(int id, OrgTree node, [FromHeader]string token)
        {
            node.UpdatedBy = 1;
            node.UpdatedTime = DateTime.Now;
            var ret = await _OrgService.UpdateOrgNode(node);
            // return CreatedAtRoute("GetById", new { id = id }, node);
            return ret;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DataResult>> Delete(int id)
        {
            OrgTree node = new OrgTree();
            node.UpdatedBy = 1;
            node.UpdatedTime = DateTime.Now;
            node.ID = id;
            var ret = await _OrgService.DeleteOrgNode(node);

            return ret;
        }

        // 组织节点绑定用户
        [HttpPost("user")]
        public async Task<ActionResult<DataResult>> BindOrgUser(OrgUserView nodeView)
        {
            nodeView.CreatedBy = 1;
            nodeView.CreatedTime = DateTime.Now;
            var ret = await _OrgService.BindOrgNodeUsers(nodeView);
            return ret;
        }
        [HttpGet("user/{id}")]
        public async Task<ActionResult<DataResult>> getOrgUser(int id)
        {
            var ret = await _OrgService.GetOrgNodeUsers(id);
            return ret;
        }
        [HttpGet("user/all/{id}")]
        public async Task<ActionResult<DataResult>> getCanSelectedUser(int id)
        {
            var ret = await _OrgService.GetCanSelectedUsers(id);
            return ret;
        }
        [HttpGet("nodetype")]
        public async Task<ActionResult<DataResult>> getNodeType()
        {
            var ret = await _OrgService.GetNodeType();
            return ret;
        }
        [HttpGet("node/{id}")]
        public async Task<ActionResult<DataResult>> getOrgNode(int id)
        {
            var ret = await _OrgService.GetOrgNode(id);
            return ret;
        }
    }
}