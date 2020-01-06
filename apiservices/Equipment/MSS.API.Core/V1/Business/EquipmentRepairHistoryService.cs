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
using static MSS.API.Common.Const;
using static MSS.API.Common.FilePath;
using Microsoft.AspNetCore.Http;
using System.IO;
using MSS.API.Common.Utility;

namespace MSS.API.Core.V1.Business
{
    public interface IEquipmentRepairHistoryService
    {
        Task<ApiResult> Save(EquipmentRepairHistory equipmentRepairHistory);
        Task<ApiResult> Update(EquipmentRepairHistory equipmentRepairHistory);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetPageByParm(EquipmentRepairHistoryQueryParm parm);
        Task<ApiResult> GetByID(int id);
    }
    public class EquipmentRepairHistoryService : IEquipmentRepairHistoryService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IEquipmentRepairHistoryRepo<EquipmentRepairHistory> _equipmentRepairHistoryRepo;
        private readonly IUploadFileRepo<UploadFile> _uploadFileRepo;
        private readonly int userID;
        public EquipmentRepairHistoryService(IEquipmentRepairHistoryRepo<EquipmentRepairHistory> equipmentRepairHistoryRepo,
            IUploadFileRepo<UploadFile> uploadFileRepo,IAuthHelper auth)
        {
            //_logger = logger;
            _equipmentRepairHistoryRepo = equipmentRepairHistoryRepo;
            _uploadFileRepo = uploadFileRepo;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> Save(EquipmentRepairHistory equipmentRepairHistory)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                equipmentRepairHistory.UpdatedTime = dt;
                equipmentRepairHistory.CreatedTime = dt;
                equipmentRepairHistory.UpdatedBy = userID;
                equipmentRepairHistory.CreatedBy = userID;
                ret.data = await _equipmentRepairHistoryRepo.Save(equipmentRepairHistory);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(EquipmentRepairHistory equipmentRepairHistory)
        {
            ApiResult ret = new ApiResult();
            try
            {
                EquipmentRepairHistory et = await _equipmentRepairHistoryRepo.GetByID(equipmentRepairHistory.ID);
                if (et!=null)
                {
                    DateTime dt = DateTime.Now;
                    equipmentRepairHistory.UpdatedTime = dt;
                    equipmentRepairHistory.UpdatedBy = userID;
                    ret.data = await _equipmentRepairHistoryRepo.Update(equipmentRepairHistory);
                }
                else
                {
                    ret.code = Code.DataIsnotExist;
                    ret.msg = "所要修改的数据不存在";
                }
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Delete(string ids)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _equipmentRepairHistoryRepo.Delete(ids.Split(','));
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetPageByParm(EquipmentRepairHistoryQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                EquipmentRepairHistoryView erhv = await _equipmentRepairHistoryRepo.GetPageByParm(parm);
                if (erhv.total != 0)
                {
                    var list = await _uploadFileRepo.ListByEntity(erhv.rows.Select(a => a.ID).ToArray(), MyDictionary.SystemResource.EqpRepair);
                    if (list != null && list.Count>0)
                    {
                        foreach (var item in erhv.rows)
                        {
                            var tmp = list.Where(a => a.Entity == item.ID);
                            if (tmp.Count() > 0)
                            {
                                item.UploadFiles = JsonConvert.SerializeObject(UploadFileHelper.CascaderShow(tmp.ToList()));
                            }
                        }
                    }
                }
                ret.data = erhv;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                EquipmentRepairHistory erh = await _equipmentRepairHistoryRepo.GetByID(id);
                var list = await _uploadFileRepo.ListByEntity(new int[] { id }, MyDictionary.SystemResource.EqpRepair);
                if (list != null)
                {
                    erh.UploadFiles = JsonConvert.SerializeObject(UploadFileHelper.ListShow(list));
                }
                ret.data = erh;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }
    }
}
