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
    public class DeviceMaintainRegController : ControllerBase
    {
        private readonly IDeviceMaintainRegService _devicemaintainService;
        private readonly int _userId;
        public DeviceMaintainRegController(IDeviceMaintainRegService expertService)
        {
            _devicemaintainService = expertService;
            _userId = 1;
        }

        // 添加
        [HttpPost("Save")]
        public async Task<ActionResult<ApiResult>> Save(tb_devicemaintain_reg setting)
        {
            setting.CreatedBy = _userId;
            setting.CreatedTime = DateTime.Now;
            setting.UpdatedBy = _userId;
            setting.UpdatedTime = setting.CreatedTime;
            setting.Is_deleted = 0;
            var ret = await _devicemaintainService.Add(setting);
            return ret;
        }

        // 更新
        [HttpPut("Update")]
        public async Task<ActionResult<ApiResult>> Update(tb_devicemaintain_reg setting)
        {
            setting.UpdatedBy = _userId;
            setting.UpdatedTime = DateTime.Now;
            var ret = await _devicemaintainService.Update(setting);
            return ret;
        }
       
        // 查询
        [HttpGet("GetListByPage")]
        public async Task<ActionResult<ApiResult>> GetListByPage([FromQuery]DeviceMainRegQuery query)
        {
            string where = "  1=1 ";
            if (!string.IsNullOrEmpty(query.DeviceType))
            {
                where += " and device_type_id =" + query.DeviceType.Trim();
            }
            if (!string.IsNullOrEmpty(query.DeviceId))
            {
                where += " and device_id =" + query.DeviceId.Trim();
            }
            if (!string.IsNullOrEmpty(query.TeamGroup))
            {
                where += " and  team_group_id=" + query.TeamGroup.Trim();
            }
            if (!string.IsNullOrEmpty(query.Director))
            {
                where += " and  director_id=" + query.Director.Trim();
            }
            if (!string.IsNullOrEmpty(query.maintain_date))
            {
                where += " and  maintain_date= '" + query.maintain_date.Trim() + "'";
            }
            var ret = await _devicemaintainService.GetListByPage(where, query.sort, query.order, query.page, query.rows);
            return ret;
        }

        // 查询
        [HttpGet("GetDeviceMaintainRegById/{id}")]
        public async Task<ActionResult<ApiResult>> GetDeviceMaintainRegById(int id)
        {
            var ret = await _devicemaintainService.GetModel(id);
            return ret;
        }

        // 查询
        [HttpGet("GetDirectorList")]
        public async Task<ActionResult<ApiResult>> GetDirectorList()
        {
            List<DirectorInfo> list = new List<DirectorInfo>()
            {
                new DirectorInfo{id=1,directorName="章仨"},
                new DirectorInfo{id=2,directorName="李素"},
                new DirectorInfo{id=3,directorName="王二"}  };
            ApiResult apiResult = new ApiResult();

            apiResult.code = Code.Success;
            apiResult.data = list;
            return apiResult;
        }

        // 查询
        [HttpGet("GetTeamGroupList")]
        public async Task<ActionResult<ApiResult>> GetTeamGroupList()
        {
            List<TeamGroupInfo> list = new List<TeamGroupInfo>()
            {
                new TeamGroupInfo{id=1,teamGroupName="维护班组"}, 
                new TeamGroupInfo{id=2,teamGroupName="计划班组"},
                new TeamGroupInfo{id=3,teamGroupName="信息班组"} };
            ApiResult apiResult = new ApiResult();

            apiResult.code = Code.Success;
            apiResult.data = list;
            return apiResult;
        }
        


        // 查询
        [HttpGet("GetEquipmentTypeList")]
        public async Task<ActionResult<ApiResult>> GetDeviceTypeList()
        {
           
            List<DeviceInfo> children = new List<DeviceInfo>()
            {
                new DeviceInfo{id=1,deviceName="设备A"},
                new DeviceInfo{id=2,deviceName="设备B"},
                new DeviceInfo{id=3,deviceName="设备C"},
                new DeviceInfo{id=4,deviceName="设备D"},
                new DeviceInfo{id=5,deviceName="设备E"} };
            List<DeviceType> list = new List<DeviceType>()
            {
                new DeviceType{id=1,deviceTypeName="设备001", children=children},
                new DeviceType{id=2,deviceTypeName="设备002"},
                new DeviceType{id=3,deviceTypeName="设备003"},
                new DeviceType{id=4,deviceTypeName="设备004"},
                new DeviceType{id=5,deviceTypeName="设备005"} };
            ApiResult apiResult = new ApiResult();

            apiResult.code = Code.Success;
            apiResult.data = list;
            return apiResult;
        }

        
        // 查询
        [HttpGet("GetDeviceListByTypeId/{id}")]
        public async Task<ActionResult<ApiResult>> GetDeviceListByTypeId(string id)
        {
          
            List<DeviceInfo> list = new List<DeviceInfo>()
            {
                new DeviceInfo{id=1,deviceName="设备A"},
                new DeviceInfo{id=2,deviceName="设备B"},
                new DeviceInfo{id=3,deviceName="设备C"},
                new DeviceInfo{id=4,deviceName="设备D"},
                new DeviceInfo{id=5,deviceName="设备E"} }; 
            ApiResult apiResult = new ApiResult();

            apiResult.code = Code.Success;
            apiResult.data = list;
            return apiResult;
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            tb_devicemaintain_reg setting = new tb_devicemaintain_reg();
            setting.UpdatedBy = _userId;
            setting.UpdatedTime = DateTime.Now;
            setting.ID = id;
            var ret = await _devicemaintainService.Delete(setting);

            return ret;
        }


        [HttpDelete("DeleteList/{ids}")]
        public async Task<ActionResult<ApiResult>> DeleteList(string ids)
        {
            ApiResult ret = new ApiResult();
            string[] strs = ids.Split(",");
            foreach (var v in strs)
            {
                tb_devicemaintain_reg setting = new tb_devicemaintain_reg();
                setting.UpdatedBy = _userId;
                setting.UpdatedTime = DateTime.Now;
                setting.ID = int.Parse(v);
                ret = await _devicemaintainService.Delete(setting);
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