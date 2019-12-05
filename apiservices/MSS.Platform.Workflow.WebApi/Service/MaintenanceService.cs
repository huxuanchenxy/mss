using Microsoft.AspNetCore.Http;
using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.Platform.Workflow.WebApi.Data;
using MSS.Platform.Workflow.WebApi.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;


// Coded By admin 2019/9/26 17:41:26
namespace MSS.Platform.Workflow.WebApi.Service
{
    public interface IMaintenanceService
    {
        Task<ApiResult> SaveMItem(MaintenanceItem maintenanceItem);

        Task<ApiResult> SaveMMoudleItem(List<MaintenanceModuleItem> maintenanceModuleItem);

        Task<ApiResult> SaveMMoudleItemValue(List<MaintenanceModuleItemValue> maintenanceModuleItemValues);

        Task<ApiResult> SaveMModule(MaintenanceModule maintenanceModule);
    }

    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepo<MaintenanceItem> _repo;
        private readonly IAuthHelper _authhelper;
        private readonly int _userID;

        public MaintenanceService(IMaintenanceRepo<MaintenanceItem> repo,IAuthHelper authhelper)
        {
            _repo = repo;
            _authhelper = authhelper;
            _userID = _authhelper.GetUserId();
        }
        #region MaintenanceItem
        public async Task<ApiResult> SaveMItem(MaintenanceItem maintenanceItem)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _repo.SaveMItem(maintenanceItem);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
        #endregion

        #region MaintenanceModuleItem
        public async Task<ApiResult> SaveMMoudleItem(List<MaintenanceModuleItem> maintenanceModuleItem)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _repo.SaveMMoudleItem(maintenanceModuleItem);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
        #endregion

        #region MaintenanceModuleItemValue
        public async Task<ApiResult> SaveMMoudleItemValue(List<MaintenanceModuleItemValue> maintenanceModuleItemValues)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _repo.SaveMMoudleItemValue(maintenanceModuleItemValues);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
        #endregion

        #region MaintenanceModule
        public async Task<ApiResult> SaveMModule(MaintenanceModule maintenanceModule)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _repo.SaveMModule(maintenanceModule);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
        #endregion

    }
}



