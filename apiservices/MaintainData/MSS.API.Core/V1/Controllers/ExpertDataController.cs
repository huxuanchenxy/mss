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
using MSS.API.Common;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExpertDataController : ControllerBase
    {
        private readonly IExpertDataService _expertService;
        private readonly int _userId;
        public ExpertDataController(IExpertDataService expertService)
        {
            _expertService = expertService;
            _userId = 1;
        }

        // 添加
        [HttpPost("Save")]
        public async Task<ActionResult<ApiResult>> Save(tb_expert_data setting)
        {
            setting.CreatedBy = _userId;
            setting.CreatedTime = DateTime.Now;
            setting.UpdatedBy = _userId;
            setting.UpdatedTime = setting.CreatedTime;
            setting.Is_deleted = 0;
            var ret = await _expertService.Add(setting);
            return ret;
        }

        // 更新
        [HttpPut("Update")]
        public async Task<ActionResult<ApiResult>> Update(tb_expert_data setting)
        {
            setting.UpdatedBy = _userId;
            setting.UpdatedTime = DateTime.Now;
            var ret = await _expertService.Update(setting);
            return ret;
        }

        // 查询
        [HttpGet("GetListByPage")]
        public async Task<ActionResult<ApiResult>> GetListByPage([FromQuery]ExpertDataQurey query)
        {
            string where = "  1=1 ";
            if (!string.IsNullOrEmpty(query.keyword))
            {
                where += " and keyword like '%" + query.keyword.Trim() + "%'";
            }
            if (!string.IsNullOrEmpty(query.title))
            {
                where += " and  title like '%" + query.title.Trim() + "%'";
            }
            if (!string.IsNullOrEmpty(query.deptid))
            {
                where += " and  deptid= '" + query.deptid.Trim() + "'";
            }
            if (!string.IsNullOrEmpty(query.deviceType))
            {
                where += " and  device_type= '" + query.deviceType.Trim() + "'";
            }
            var ret = await _expertService.GetListByPage(where, query.sort, query.order, query.page, query.rows);
            return ret;
        } 

        // 查询
        [HttpGet("GetExpertDataById/{id}")]
        public async Task<ActionResult<ApiResult>> GetExpertDataById(int id)
        {
            var ret = await _expertService.GetModel(id);
            return ret;
        }

        // 查询
        [HttpGet("GetdeptList")]
        public async Task<ActionResult<ApiResult>> GetdeptList()
        {
            List<DeptInfo> list = new List<DeptInfo>()
            {
                new DeptInfo{id=1,deptName="维护一部"},
                new DeptInfo{id=2,deptName="维护二部"},
                new DeptInfo{id=3,deptName="维护三部"},
                new DeptInfo{id=4,deptName="计划部"},
                new DeptInfo{id=5,deptName="信息部"} };
            ApiResult apiResult = new ApiResult();

            apiResult.code = Code.Success;
            apiResult.data = list;
            return apiResult;
        }

        // 查询
        [HttpGet("GetDeviceTypeList")]
        public async Task<ActionResult<ApiResult>> GetDeviceTypeList()
        {
            List<DeviceType> list = new List<DeviceType>()
            {
                new DeviceType{id=1,deviceTypeName="设备001"},
                new DeviceType{id=2,deviceTypeName="设备002"},
                new DeviceType{id=3,deviceTypeName="设备003"},
                new DeviceType{id=4,deviceTypeName="设备004"},
                new DeviceType{id=5,deviceTypeName="设备005"} };
            ApiResult apiResult = new ApiResult();

            apiResult.code = Code.Success;
            apiResult.data = list;
            return apiResult;
        }




        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            tb_expert_data setting = new tb_expert_data();
            setting.UpdatedBy = _userId;
            setting.UpdatedTime = DateTime.Now;
            setting.ID = id;
            var ret = await _expertService.Delete(setting);

            return ret;
        }


        [HttpDelete("{ids}")]
        public async Task<ActionResult<ApiResult>> DeleteList(string ids)
        {
            ApiResult ret = new ApiResult();
            string[] strs = ids.Split(",");
            foreach (var v in strs)
            {
                tb_expert_data setting = new tb_expert_data();
                setting.UpdatedBy = _userId;
                setting.UpdatedTime = DateTime.Now;
                setting.ID = int.Parse(v);
                ret = await _expertService.Delete(setting);
                if (ret.code == Code.Failure)
                {
                    return ret;
                }
            }
            ret.code = Code.Success;
            ret.data = true;
            return ret;
        }
    }
}