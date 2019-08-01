using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MSS.API.Model.DTO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Common;
using MSS.API.Core.Common;
using MSS.Common.Consul;
using MSS.API.Common.Utility;

namespace MSS.API.Core.V1.Business
{
    public class DeviceMaintainRegService : IDeviceMaintainRegService
    { //private readonly ILogger<UserService> _logger;
        private readonly Itb_devicemaintain_regRepo<tb_devicemaintain_reg> _deviceMaintainRegRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;

        public DeviceMaintainRegService(Itb_devicemaintain_regRepo<tb_devicemaintain_reg> expertRepo, IServiceDiscoveryProvider consulServiceProvider)
        {
            //_logger = logger;
            _deviceMaintainRegRepo = expertRepo;
            _consulServiceProvider = consulServiceProvider;
        }
        public async Task<ApiResult> Add(tb_devicemaintain_reg model)
        {
            ApiResult result = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool isExist = await _deviceMaintainRegRepo.Exists(model.ID.ToString());
                    if (!isExist)
                    {
                        var ret = await _deviceMaintainRegRepo.Add(model);
                        //if (ret<1)
                        //{
                        //    result.code = Code.Failure;
                        //    result.data = null;
                        //}
                        result.code = Code.Success;
                        result.data = true;
                    }
                    else
                    {
                        result.code = Code.DataIsnotExist;
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                result.code = Code.Failure;
                result.msg = ex.Message;
            }
            return result;
        }

        public async Task<ApiResult> Update(tb_devicemaintain_reg model)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool isExist = await _deviceMaintainRegRepo.Exists(model.ID.ToString());
                    if (!isExist)
                    {
                        var data = await _deviceMaintainRegRepo.Update(model);
                        ret.code = Code.Success;
                        ret.data = true;
                    }
                    else
                    {
                        ret.code = Code.DataIsnotExist;
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.data = ex.Message;
            }
            return ret;
        }


        public async Task<ApiResult> Delete(tb_devicemaintain_reg model)
        {
            ApiResult ret = new ApiResult();
            try
            {
                await _deviceMaintainRegRepo.Delete(model);
                ret.code = Code.Success;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> DeleteList(string ids)
        {

            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    string[] strs = ids.Split(",");
                    foreach (var v in strs)
                    {
                        bool isExist = await _deviceMaintainRegRepo.Delete(new tb_devicemaintain_reg { ID = int.Parse(v), UpdatedTime = DateTime.Now, UpdatedBy = 101 });
                    }
                    ret.code = Code.Success;
                    ret.data = true;
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public Task<ApiResult> Exists(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<ApiResult> GetListByPage(string strWhere, string orderby, string sort, int page, int size)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int count = await _deviceMaintainRegRepo.GetRecordCount(strWhere);
                    var list = await _deviceMaintainRegRepo.GetListByPage(strWhere, sort, orderby, page, size);
                    if (list != null && list.Count > 0)
                    {
                        foreach (var v in list)
                        {
                            v.maintain_date = v.maintain_date.Substring(0, 10); 
                            v.director_name = GetderoctName(v.director_id.ToString());
                        }
                    }
                    List<UploadFile> ufs = await _deviceMaintainRegRepo.ListByEntity(
                    list.Select(a => a.ID).ToArray(), MyDictionary.SystemResource.MaintainReg);
                    if (ufs != null && ufs.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            var tmp = ufs.Where(a => a.entity_id == item.ID);
                            if (tmp != null)
                            {
                                item.UploadFiles = JsonConvert.SerializeObject(UploadFileHelper.CascaderShow(tmp.ToList()));
                            }
                        }
                    }

                    ret.code = Code.Success;
                    ret.data = new
                    {
                        total = count,
                        list = list
                    };
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        private string GetDeviceTypeNameByID(string device_type)
        {
            string deviceTypeName = string.Empty;
            switch (device_type)
            {
                case "1":
                    deviceTypeName = "设备001";
                    break;
                case "2":
                    deviceTypeName = "设备002";
                    break;
                case "3":
                    deviceTypeName = "设备003";
                    break;
                case "4":
                    deviceTypeName = "设备004";
                    break;
                case "5":
                    deviceTypeName = "设备005";
                    break;
            }
            return deviceTypeName;
        }

        private string GetdeviceName(string deviceID)
        {
            string deviceName = string.Empty;
            switch (deviceID)
            {
                case "1":
                    deviceName = "设备A";
                    break;
                case "2":
                    deviceName = "设备B";
                    break;
                case "3":
                    deviceName = "设备C";
                    break;
                case "4":
                    deviceName = "设备D";
                    break;
                case "5":
                    deviceName = "设备E";
                    break;
            }
            return deviceName;
        }

        private string GettermGroupName(string ID)
        {
            string deviceName = string.Empty;
            switch (ID)
            {
                case "1":
                    deviceName = "维护班组";
                    break;
                case "2":
                    deviceName = "计划班组";
                    break;
                case "3":
                    deviceName = "信息班组";
                    break;
            }
            return deviceName;
        }

        private string GetderoctName(string ID)
        {
            string deviceName = string.Empty;
            switch (ID)
            {
                case "1":
                    deviceName = "章仨";
                    break;
                case "2":
                    deviceName = "李素";
                    break;
                case "3":
                    deviceName = "王二";
                    break;
            }
            return deviceName;
        }

        private string GetdeviceNameByID(string deviceID)
        {
            string deviceName = string.Empty;
            switch (deviceID)
            {
                case "1":
                    deviceName = "设备A";
                    break;
                case "2":
                    deviceName = "设备B";
                    break;
                case "3":
                    deviceName = "设备C";
                    break;
            }
            return deviceName;
        }
        public async Task<ApiResult> GetModel(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //var model = await _deviceMaintainRegRepo.GetModel(id);
                    //model.maintain_date = model.maintain_date.Substring(0, 10);
                    ////model.device_name = GetdeviceName(model.device_id.ToString());
                    ////model.device_type_name = GetDeviceTypeNameByID(model.device_type_id.ToString());
                    //model.director_name = GetderoctName(model.director_id.ToString());
                    ////model.team_group_name = GettermGroupName(model.team_group_id.ToString());
                    //ret.code = Code.Success;
                    //ret.data = model;
                    //scope.Complete();
                    var model = await _deviceMaintainRegRepo.GetModel(id);
                    model.maintain_date = model.maintain_date.Substring(0, 10);
                    model.director_name = GetderoctName(model.director_id.ToString());
                    List<UploadFile> ufs = await _deviceMaintainRegRepo.ListByEntity(new int[] { model.ID }, MyDictionary.SystemResource.MaintainReg);
                    if (ufs != null && ufs.Count() > 0)
                    {
                        var tmp = ufs.Where(a => a.entity_id == model.ID);
                        if (tmp != null)
                        {
                            model.UploadFiles = JsonConvert.SerializeObject(UploadFileHelper.ListShow(tmp.ToList()));
                        }
                    }
                    ret.code = Code.Success;
                    ret.data = model;
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetDeviceTypeList()
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var _services = await _consulServiceProvider.GetServiceAsync("EqpService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    ApiResult result = await httpHelper.GetSingleItemRequest(_services + "api/v1/EquipmentType/all");
                   // List<DeviceInfo> children = new List<DeviceInfo> { };
                   // OrgTree topNode = await _findTopNode(id);

                    ret.code = Code.Success;
                    ret.data = result.data;
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }
    }
}

