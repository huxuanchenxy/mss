﻿using MSS.API.Dao.Interface;
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

namespace MSS.API.Core.V1.Business
{
    public class ExpertDataService : IExpertDataService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly Itb_expert_dataRepo<tb_expert_data> _expertRepo;

        public ExpertDataService(Itb_expert_dataRepo<tb_expert_data> expertRepo)
        {
            //_logger = logger;
            _expertRepo = expertRepo;
        }


        public async Task<ApiResult> Add(tb_expert_data model)
        {
            ApiResult result = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool isExist = await _expertRepo.Exists(model.keyword);
                    if (!isExist)
                    {
                        var ret = await _expertRepo.Add(model);
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

        public async Task<ApiResult> Update(tb_expert_data model)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool isExist = await _expertRepo.Exists(model.title);
                    if (!isExist)
                    {
                        var data = await _expertRepo.Update(model);
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


        public async Task<ApiResult> Delete(tb_expert_data model)
        {
            ApiResult ret = new ApiResult();
            try
            {
                await _expertRepo.Delete(model);
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
                        bool isExist = await _expertRepo.Delete(new tb_expert_data { ID = int.Parse(v),UpdatedTime=DateTime.Now, UpdatedBy=101 });
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

       public Task<ApiResult>  Exists(int id)
        {
            throw new NotImplementedException();
        }

        //Task<ApiResult> IExpertDataService.GetList(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //Task<ApiResult> IExpertDataService.GetList(string strWhere)
        //{
        //    throw new NotImplementedException();
        //}

       public async Task<ApiResult>  GetListByPage(string strWhere, string orderby, string sort, int page, int size)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int count = await _expertRepo.GetRecordCount(strWhere);
                    var list = await _expertRepo.GetListByPage(strWhere , sort, orderby, page, size);
                    if (list != null && list.Count > 0)
                    {
                        foreach (var v in list)
                        {
                          v.deptname= GetDeptNameByID(v.deptid.ToString());
                          v.deviceTypeName = GetDeviceTypeNameByID(v.device_type.ToString());
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

        private string GetDeptNameByID(string deptId)
        {
            string deptName = string.Empty;
            switch (deptId)
            {
                case "1":
                    deptName = "维护一部";
                    break;
                case "2":
                    deptName = "维护二部";
                    break;
                case "3":
                    deptName = "维护三部";
                    break;
                case "4":
                    deptName = "计划部";
                    break;
                case "5":
                    deptName = "信息部";
                    break;
            }
            return deptName;
        }

        //Task<ApiResult> IExpertDataService.GetMaxId()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ApiResult>  GetModel(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var model = await _expertRepo.GetModel(id);                   
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

        //Task<ApiResult> IExpertDataService.GetRecordCount(string where)
        //{
        //    throw new NotImplementedException();
        //}
 
    }
}