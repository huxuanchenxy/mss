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
    public class MetroLineController : ControllerBase
    {
        private readonly IMetroLineService _lineService;
        private readonly int _userId;
        private readonly IAuthHelper _authHelper;
        public MetroLineController(IMetroLineService lineService, IAuthHelper authHelper)
        {
            _lineService = lineService;
            _authHelper = authHelper;
            _userId = _authHelper.GetUserId();
        }

        // 添加
        [HttpPost]
        public async Task<ActionResult<ApiResult>> Save(MetroLine line)
        {
            line.CreatedBy = _userId;
            line.CreatedTime = DateTime.Now;
            line.UpdatedBy = _userId;
            line.UpdatedTime = DateTime.Now;
            line.IsDel = false;
            var ret = await _lineService.SaveLine(line);
            return ret;
        }

        // 更新
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult>> Update(int id, MetroLine line)
        {
            line.UpdatedBy = _userId;
            line.UpdatedTime = DateTime.Now;
            var ret = await _lineService.UpdateLine(line);
            return ret;
        }

        [HttpDelete("{ids}")]
        public async Task<ActionResult<ApiResult>> Delete(string ids)
        {
            List<MetroLine> list = new List<MetroLine>();
            string[] idsArry = ids.Split(',');
            DateTime now = DateTime.Now;
            foreach (string item in idsArry)
            {
                int id = Convert.ToInt32(item);
                MetroLine line = new MetroLine();
                line.UpdatedBy = _userId;
                line.UpdatedTime = now;
                line.ID = id;
                list.Add(line);
            }
            if (list.Count > 0)
            {
                var ret = await _lineService.DeleteLine(list);
                return ret;
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<ApiResult>> Get()
        {
            var ret = await _lineService.ListAllMetroLine();
            return ret;
        }

        // 查询
        [HttpGet()]
        public async Task<ActionResult<ApiResult>> GetLineByPage([FromQuery]MetroLineParam query)
        {
            var ret = await _lineService.ListMetroLineByPage(query);
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult>> GetLineByID(int id)
        {
            var ret = await _lineService.GetMetroLineByID(id);
            return ret;
        }

        [HttpGet("GetLineStation")]
        public async Task<ActionResult<ApiResult>> GetLineStation()
        {
            var ret = await _lineService.GetMetroStation();
            return ret;
        }

    }
}