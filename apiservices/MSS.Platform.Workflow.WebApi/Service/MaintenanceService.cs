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

        Task<ApiResult> SaveMMoudleItemValue(MaintenanceModuleItemValueParm parm);

        Task<ApiResult> SaveMModule(MaintenanceModule maintenanceModule);
        Task<ApiResult> SaveMList(MaintenanceList maintenanceList);

        Task<ApiResult> ListPage(MaintenanceListParm parm);
        Task<ApiResult> ListMModule(int id, bool isInit);
    }

    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepo<MaintenanceItem> _repo;
        private readonly IConstructionPlanImportRepo<ConstructionPlanYear> _importRepo;
        private readonly IAuthHelper _authhelper;
        private readonly int _userID;

        public MaintenanceService(IMaintenanceRepo<MaintenanceItem> repo,
            IConstructionPlanImportRepo<ConstructionPlanYear> importRepo,IAuthHelper authhelper)
        {
            _repo = repo;
            _importRepo = importRepo;
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
        public async Task<ApiResult> SaveMMoudleItemValue(MaintenanceModuleItemValueParm parm)
        {
            ApiResult ret = new ApiResult();
            int retData = 0;
            int listID = parm.MaintenanceModuleItemValues[0].List;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    retData = await _repo.DelMMoudleItemValue(listID);
                    retData = await _repo.SaveMMoudleItemValue(parm.MaintenanceModuleItemValues);
                    int status = (int)PMStatus.Editing;
                    if (parm.IsFinished)
                    {
                        status = (int)PMStatus.Finished;
                    }
                    retData = await _repo.UpdateMList(status,_userID, listID);
                    scope.Complete();
                }
                ret.data = retData;
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
        public async Task<ApiResult> ListMModule(int id,bool isInit)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<MaintenanceModule> mms = new List<MaintenanceModule>();
                List<MaintenanceModuleItemAll> mmiAll = new List<MaintenanceModuleItemAll>();
                if (isInit)
                {
                    mmiAll = await _repo.ListItems(id);
                    mms=ListItems(mmiAll, id);
                }
                else
                {
                    mmiAll = await _repo.ListValues(id);
                    mms=ListValues(mmiAll, id);
                }
                
                ret.data =mms;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        private List<MaintenanceModule> ListItems(List<MaintenanceModuleItemAll> mmiAll, int listID)
        {
            List<MaintenanceModule> mms = new List<MaintenanceModule>();
            IEnumerable<IGrouping<int, MaintenanceModuleItemAll>> groupList = mmiAll.GroupBy(a => a.ID);
            foreach (IGrouping<int, MaintenanceModuleItemAll> group in groupList)
            {
                int count = group.FirstOrDefault().Count;
                for (int i = 0; i < count; i++)
                {
                    MaintenanceModule mm = new MaintenanceModule();
                    mm.ID = group.Key;
                    mm.Name = group.FirstOrDefault().ModuleName;
                    if (count > 1)
                    {
                        mm.ShowName = mm.Name + "(设备设施" + (i + 1).ToString() + ")";
                        mm.IsShowEqp = true;
                    }
                    else
                    {
                        mm.ShowName = mm.Name;
                    }
                    mm.Items = new List<MaintenanceModuleItemValue>();
                    foreach (var item in group)
                    {
                        MaintenanceModuleItemValue mmiv = new MaintenanceModuleItemValue();
                        mmiv.Item = item.Item;
                        mmiv.List = listID;
                        mmiv.Module = mm.ID;
                        mmiv.ItemName = item.ItemName;
                        mmiv.ItemType = item.ItemType;
                        mm.Items.Add(mmiv);
                    }
                    mms.Add(mm);
                }
            }
            return mms;
        }
        private List<MaintenanceModule> ListValues(List<MaintenanceModuleItemAll> mmiAll, int listID)
        {
            List<MaintenanceModule> mms = new List<MaintenanceModule>();
            IEnumerable<IGrouping<int, MaintenanceModuleItemAll>> groupList = mmiAll.GroupBy(a => a.ID);
            foreach (IGrouping<int, MaintenanceModuleItemAll> group in groupList)
            {
                IEnumerable<IGrouping<string, MaintenanceModuleItemAll>> groupByEqp = group.GroupBy(a => a.Eqp);
                foreach (IGrouping<string, MaintenanceModuleItemAll> items in groupByEqp)
                {
                    MaintenanceModule mm = new MaintenanceModule();
                    mm.ID = group.Key;
                    mm.Name = group.FirstOrDefault().ModuleName;
                    if (items.Key!=null)
                    {
                        mm.ShowName = mm.Name + "(设备设施" + items.Key + ")";
                        mm.IsShowEqp = true;
                        mm.Eqp = items.Key;
                    }
                    else
                    {
                        mm.ShowName = mm.Name;
                    }
                    mm.Items = new List<MaintenanceModuleItemValue>();
                    foreach (var item in items)
                    {
                        MaintenanceModuleItemValue mmiv = new MaintenanceModuleItemValue();
                        mmiv.Item = item.Item;
                        mmiv.List = listID;
                        mmiv.Module = mm.ID;
                        mmiv.ItemName = item.ItemName;
                        mmiv.ItemType = item.ItemType;
                        mmiv.ItemValue = item.ItemValue;
                        mm.Items.Add(mmiv);
                    }
                    mms.Add(mm);
                }
            }
            return mms;
        }
        #endregion

        #region MaintenanceList
        public async Task<ApiResult> SaveMList(MaintenanceList maintenanceList)
        {
            ApiResult ret = new ApiResult();
            int retData = 0;
            DateTime dt = DateTime.Now;
            maintenanceList.CreatedBy = _userID;
            maintenanceList.CreatedTime = dt;
            maintenanceList.Status = (int)PMStatus.Init;
            maintenanceList.UpdatedBy = _userID;
            maintenanceList.UpdatedTime = dt;
            try
            {
                List<MaintenancePlanDetail> mpds = new List<MaintenancePlanDetail>();
                using (TransactionScope scope = new TransactionScope())
                {
                    retData = await _repo.SaveMList(maintenanceList);
                    foreach (var item in maintenanceList.Details)
                    {
                        item.List = retData;
                        mpds.Add(item);
                    }
                    retData = await _repo.SaveMDetail(mpds);
                    scope.Complete();
                }
                ret.data = retData;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> ListPage(MaintenanceListParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                MaintenanceListView mlv = await _repo.ListPage(parm);
                List<QueryItem> locations=await _importRepo.ListAllLocations();
                foreach (var item in mlv.rows)
                {
                    item.LocationName = locations.Where(a => a.LocationBy == item.Locationby && a.ID == item.Location).FirstOrDefault().Name;
                }
                ret.data = mlv;
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



