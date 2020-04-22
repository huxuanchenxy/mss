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
using MSS.API.Dao.Implement;
using MSS.Common.Consul;
using static MSS.API.Common.MyDictionary;

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
        private readonly IEqpHistoryRepo<EqpHistory> _eqpHistoryRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly ITroubleReportRepo<TroubleReport> _troubleReportRepo;
        private readonly IHealthHistoryRepo<HealthHistory> _healthHistoryRepo;
        private readonly IHealthRepo<Health> _healthRepo;
        private readonly IHealthConfigRepo<HealthConfig> _healthConfigRepo;
        private readonly int userID;
        public EquipmentRepairHistoryService(IEquipmentRepairHistoryRepo<EquipmentRepairHistory> equipmentRepairHistoryRepo,
            IAuthHelper auth, IServiceDiscoveryProvider consulServiceProvider,
            ITroubleReportRepo<TroubleReport> troubleReportRepo,
            IHealthHistoryRepo<HealthHistory> healthHistoryRepo,
            IHealthRepo<Health> healthRepo,
            IHealthConfigRepo<HealthConfig> healthConfigRepo,
            IEqpHistoryRepo<EqpHistory> eqpHistoryRepo)
        {
            //_logger = logger;
            _equipmentRepairHistoryRepo = equipmentRepairHistoryRepo;
            _eqpHistoryRepo = eqpHistoryRepo;
            _troubleReportRepo = troubleReportRepo;
            _consulServiceProvider = consulServiceProvider;
            _healthHistoryRepo = healthHistoryRepo;
            _healthRepo = healthRepo;
            _healthConfigRepo = healthConfigRepo;
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
                //健康度
                bool isInsert = true;
                int eqpID = equipmentRepairHistory.Eqp;
                List<int> eqpIDs = new List<int>() { eqpID };
                //int eqpTypeID = (await _healthConfigRepo.GetEqpTypeByEqps(eqpIDs)).FirstOrDefault().Type;
                Health h = new Health();
                h.Eqp = equipmentRepairHistory.Eqp;
                h.CreatedBy = userID;
                h.CreatedTime = dt;
                h.UpdatedBy = userID;
                h.UpdatedTime = dt;
                h.IsRepaired = 1;
                HealthHistory hh = new HealthHistory();
                hh.Eqp = equipmentRepairHistory.Eqp;
                hh.CreatedBy = userID;
                hh.CreatedTime = dt;
                double currentVal,configVal;
                List<HealthConfig> tmp = new List<HealthConfig>(); 
                HealthType ht;
                if (equipmentRepairHistory.ReplaceType==(int)EqpHistoryType.AllChange)
                {
                    ht = HealthType.EqpReplace;
                    tmp = await _healthConfigRepo.GetValByCon((int)ht);
                    if (tmp.Count > 0)
                    {
                        h.Val = tmp.FirstOrDefault().Val;
                        hh.Val = h.Val;
                    }
                    else
                    {
                        h.Val = HEATHFULLVAL;
                        hh.Val = HEATHFULLVAL;
                    }
                }
                else
                {
                    if (equipmentRepairHistory.PMType== (int)EqpHistoryType.MediumPM)
                    {
                        ht = HealthType.MediumPM;
                    }
                    else //if (equipmentRepairHistory.PMType == (int)EqpHistoryType.MajorPM)
                    {
                        ht = HealthType.MajorPM;
                    }
                    tmp = await _healthConfigRepo.GetValByCon((int)ht);
                    if (tmp.Count>0)
                    {
                        configVal = tmp.FirstOrDefault().Val/100;
                        var tmp1 = await _healthRepo.ListEqp(eqpIDs);
                        if (tmp1.Count > 0)
                        {
                            Health tmph = tmp1.FirstOrDefault();
                            currentVal = tmph.Val;
                            h.ID = tmph.ID;
                            isInsert = false;
                        }
                        else currentVal = HEATHFULLVAL;
                        h.Val = (HEATHFULLVAL - currentVal) * configVal + currentVal;
                        if (h.Val > HEATHFULLVAL) h.Val = HEATHFULLVAL;
                        hh.Val = h.Val;
                    }
                }
                h.Type = (int)ht;
                hh.Type = (int)ht;
                //履历
                EqpHistory eh = new EqpHistory();
                eh.CreatedBy = userID;
                eh.CreatedTime = dt;
                eh.EqpID = h.Eqp;
                //eh.ShowName = "维修过程记录"+ eh.CreatedTime.ToString("yyyyMMdd");
                //更新下一次的大修中修时间
                DateTime? large = null;
                DateTime? medium=null;
                //从设备表中获取大中修频率
                Equipment frequency =await _equipmentRepairHistoryRepo.GetRepairFrequencyByEqpID(eqpID);
                string str;
                if (equipmentRepairHistory.PMType == (int)EqpHistoryType.MajorPM)
                {
                    str = "(大修)";
                    medium = equipmentRepairHistory.PMDate.AddDays(frequency.LargeRepair);
                }
                else
                {
                    str = "(中修)";
                    large = equipmentRepairHistory.PMDate.AddDays(frequency.MediumRepair);
                }
                eh.ShowName = "维修过程记录" + str;
                if (equipmentRepairHistory.ReplaceType == 0) eh.Type = equipmentRepairHistory.PMType;
                else eh.Type = equipmentRepairHistory.ReplaceType;
                using (TransactionScope ts = new TransactionScope())
                {
                    EquipmentRepairHistory erh = await _equipmentRepairHistoryRepo.Save(equipmentRepairHistory);
                    h.CorrelationID = erh.ID;
                    hh.CorrelationID = erh.ID;
                    eh.WorkingOrder = erh.ID;
                    if (!string.IsNullOrWhiteSpace(equipmentRepairHistory.UploadFiles))
                    {
                        List<object> objs = new List<object>();
                        JArray jobj = JsonConvert.DeserializeObject<JArray>(equipmentRepairHistory.UploadFiles);
                        foreach (var obj in jobj)
                        {
                            foreach (var item in obj["ids"].ToString().Split(','))
                            {
                                UploadFileRelation upload = new UploadFileRelation();
                                upload.Entity = erh.ID;
                                upload.File = Convert.ToInt32(item);
                                upload.Type = Convert.ToInt32(obj["type"]);
                                upload.SystemResource = (int)SystemResource.EqpRepair;
                                await _troubleReportRepo.SaveUploadFile(upload);
                            }
                        }
                    }
                    if (isInsert) await _healthRepo.Save(h);
                    else await _healthRepo.Update(h);
                    await _healthHistoryRepo.Save(hh);
                    await _eqpHistoryRepo.SaveEqpHistory(eh);
                    await _equipmentRepairHistoryRepo.UpdateEqpRepairDate(large, medium, eqpID);
                    ts.Complete();
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
                    //更新下一次的大修中修时间
                    DateTime? large = null;
                    DateTime? medium = null;
                    //从设备表中获取大中修频率
                    Equipment frequency = await _equipmentRepairHistoryRepo.GetRepairFrequencyByEqpID(equipmentRepairHistory.ID);
                    if (equipmentRepairHistory.PMType == (int)EqpHistoryType.MajorPM)
                    {
                        medium = equipmentRepairHistory.PMDate.AddDays(frequency.LargeRepair);
                    }
                    else
                    {
                        large = equipmentRepairHistory.PMDate.AddDays(frequency.MediumRepair);
                    }
                    ret.data = await _equipmentRepairHistoryRepo.Update(equipmentRepairHistory,medium,large);
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
                    string ids = String.Join(",", erhv.rows.Select(a => a.ID));
                    List<UploadFilesEntity> jarr = await UploadFileHelper.GetUploadFile(ids,
                    UploadShowType.Cascader, _consulServiceProvider, SystemResource.EqpRepair);
                    if (jarr != null && jarr.Count() > 0)
                    {
                        foreach (var item in erhv.rows)
                        {
                            var tmp = jarr.Where(a => a.Entity == item.ID);
                            if (tmp.Count() > 0)
                            {
                                item.UploadFiles = tmp.FirstOrDefault().UploadFiles;
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
                string ids = id.ToString();
                List<UploadFilesEntity> jarr = await UploadFileHelper.GetUploadFile(ids,
                    UploadShowType.List, _consulServiceProvider, SystemResource.EqpRepair);
                if (jarr != null && jarr.Count() > 0)
                {
                    erh.UploadFiles = jarr.FirstOrDefault().UploadFiles;
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
