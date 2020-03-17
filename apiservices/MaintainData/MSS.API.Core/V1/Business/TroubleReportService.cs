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
using MSS.API.Common.Utility;
using Microsoft.Extensions.Caching.Distributed;
using MSS.Common.Consul;
using static MSS.API.Common.MyDictionary;
using MSS.API.Dao.Implement;

namespace MSS.API.Core.V1.Business
{
    public interface ITroubleReportService
    {
        Task<ApiResult> ListAll();
        Task<ApiResult> ListByStatus(int status);
        Task<ApiResult> GetNoByStatus(AttandenceStatus attandenceStatus);
        Task<ApiResult> ListHistoryByTrouble(int id);
        Task<ApiResult> ListEqpByTrouble(int id,int topOrg, TroubleView troubleView);
        Task<ApiResult> AssignEqp(List<TroubleEqp> troubleEqp);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> ListPage(TroubleReportParm parm);
        Task<ApiResult> Operation(string ids, TroubleOperation operation, string content);
        Task<ApiResult> Save(TroubleReport troubleReport);
        Task<ApiResult> Update(TroubleReport troubleReport);
        Task<ApiResult> SaveDeal(TroubleDeal troubleDeal);
        Task<ApiResult> GetDealByID(int id, int orgTop);
    }
    public class TroubleReportService : ITroubleReportService
    {
        private readonly ITroubleReportRepo<TroubleReport> _troubleReportRepo;
        private readonly IEqpHistoryRepo<EqpHistory> _eqpHistoryRepo;
        private readonly int _userID;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly IHealthHistoryRepo<HealthHistory> _healthHistoryRepo;
        private readonly IHealthRepo<Health> _healthRepo;
        private readonly IHealthConfigRepo<HealthConfig> _healthConfigRepo;

        public TroubleReportService(ITroubleReportRepo<TroubleReport> troubleReportRepo,
            IEqpHistoryRepo<EqpHistory> eqpHistoryRepo, IAuthHelper authhelper, 
            IServiceDiscoveryProvider consulServiceProvider,
            IHealthHistoryRepo<HealthHistory> healthHistoryRepo,
            IHealthRepo<Health> healthRepo,
            IHealthConfigRepo<HealthConfig> healthConfigRepo)
        {
            _troubleReportRepo = troubleReportRepo;
            _eqpHistoryRepo = eqpHistoryRepo;
            _userID = authhelper.GetUserId();
            _consulServiceProvider = consulServiceProvider;
            _healthHistoryRepo = healthHistoryRepo;
            _healthRepo = healthRepo;
            _healthConfigRepo = healthConfigRepo;
        }
        #region 调度员/值班员接口 未接报、未处理、未修复、未完结、七十二小时外未修复的数量统计
        public async Task<ApiResult> GetNoByStatus(AttandenceStatus attandenceStatus)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _troubleReportRepo.GetNoByStatus(attandenceStatus);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
        #endregion

        public async Task<ApiResult> ListAll()
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _troubleReportRepo.ListAll();
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
        public async Task<ApiResult> ListByStatus(int status)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _troubleReportRepo.ListByStatus(status);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> ListHistoryByTrouble(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _troubleReportRepo.ListHistoryByTrouble(id);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
        public async Task<ApiResult> ListEqpByTrouble(int id,int topOrg,TroubleView troubleView)
        {
            ApiResult ret = new ApiResult();
            try
            {
                int orgNode=0;
                if (troubleView==TroubleView.MyProcessing)
                {
                    orgNode=await _troubleReportRepo.GetOrgNodeByUser(_userID);
                }
                ret.data = await _troubleReportRepo.ListEqpByTrouble(id,topOrg, orgNode);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
        public async Task<ApiResult> AssignEqp(List<TroubleEqp> troubleEqp)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                TroubleHistory thTmp = new TroubleHistory();
                thTmp.Trouble = troubleEqp[0].Trouble;
                thTmp.OrgTop = troubleEqp[0].Org;
                thTmp.Operation = TroubleOperation.Assign;
                thTmp.CreatedBy = _userID;
                thTmp.CreatedTime = dt;

                foreach (var item in troubleEqp)
                {
                    item.AssignedBy = _userID;
                    item.AssignedTime = dt;
                    thTmp.Content += "设备-"+item.EqpName + " 调度分配给 " + item.OrgNodeName+";";
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    await _troubleReportRepo.SaveHistory(thTmp);
                    await _troubleReportRepo.UpdateStatus(new string[] { thTmp.Trouble.ToString() }, 
                        _userID, TroubleStatus.Processing, TroubleOperation.Assign);
                    ret.data = await _troubleReportRepo.UpdateTroubleEqp(troubleEqp);
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
        /// <summary>
        /// 角色规定了我的接修、我的审核的权限
        /// 登录用户只要拥有此权限，且属于接修单位就可以执行接修和审核的操作
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public async Task<ApiResult> ListPage(TroubleReportParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<QueryItem> locations =await _eqpHistoryRepo.ListAllLocations();
                parm.RepairCompany = await getTopOrgByUser();
                if (parm.MenuView == TroubleView.MyProcessing)
                {
                    parm.OrgNode = await _troubleReportRepo.GetOrgNodeByUser(_userID);
                }
                TroubleReportView view = await _troubleReportRepo.ListPage(parm);
                List<TroubleReport>  trs= view.rows;
                foreach (var item in trs)
                {
                    item.StartLocationName = locations.Where(a => a.LocationBy == item.StartLocationBy && a.ID == item.StartLocation)
                        .FirstOrDefault().Name;
                    if (item.EndLocation!=null && item.EndLocationBy!=null)
                    {
                        item.EndLocationName = locations
                            .Where(a => a.LocationBy == item.EndLocationBy && a.ID == item.EndLocation)
                            .FirstOrDefault().Name;
                    }
                }
                ret.data = view;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> Operation(string ids,TroubleOperation operation,string content)
        {
            ApiResult ret = new ApiResult();
            List<EqpHistory> eqps = new List<EqpHistory>();
            DateTime dt = DateTime.Now;
            int topOrg= await getTopOrgByUser();
            TroubleStatus status=TroubleStatus.NewTrouble;
            List<TroubleHistory> ths = new List<TroubleHistory>();
            TroubleHistory thTmp = new TroubleHistory();
            if (operation != TroubleOperation.CancelTrouble)
            {
                thTmp.Trouble = Convert.ToInt32(ids);
                thTmp.OrgTop = topOrg;
                thTmp.Operation = operation;
                thTmp.Content = content=="null"?"":content;
                thTmp.CreatedBy = _userID;
                thTmp.CreatedTime = dt;
            }
            else
            {
                foreach (var item in ids.Split(','))
                {
                    TroubleHistory th = new TroubleHistory();
                    th.Trouble = Convert.ToInt32(item);
                    th.OrgTop = topOrg;
                    th.Operation = TroubleOperation.CancelTrouble;
                    th.CreatedBy = _userID;
                    th.CreatedTime = dt;
                    ths.Add(th);
                }
                status = TroubleStatus.Canceled;
            }
            switch (operation)
            {
                case TroubleOperation.PrePass:
                    //审核不通过其实不用修改状态，由于程序中默认修改为新增状态，所以这里要改回来
                case TroubleOperation.Unpass:
                    status = TroubleStatus.PendingApproval;
                    break;
                case TroubleOperation.UnPrePass:
                    status = TroubleStatus.Processing;
                    break;
                case TroubleOperation.Delayed:
                    status = TroubleStatus.Delayed;
                    //int tmp=await isRepaired(Convert.ToInt32(ids));
                    //if (tmp>1) status = TroubleStatus.Repaired;
                    break;
                case TroubleOperation.Pass:
                    status = TroubleStatus.Finished;
                    //记录设备历史
                    List<TroubleEqp> tes = await _troubleReportRepo.ListEqpByTrouble(thTmp.Trouble);
                    TroubleReport tr = await _troubleReportRepo.GetByID(thTmp.Trouble);
                    foreach (var item in tes)
                    {
                        EqpHistory eqp = new EqpHistory();
                        eqp.CreatedBy = tr.ReportedBy;//报修人
                        eqp.CreatedTime = tr.HappeningTime;//故障发生时间
                        eqp.EqpID = item.Eqp;
                        eqp.ShowName = tr.Code;
                        eqp.Type = (int)EqpHistoryType.TroublePM;
                        eqp.WorkingOrder = tr.ID;
                        eqps.Add(eqp);
                    }
                    break;
                case TroubleOperation.RepairReject:
                case TroubleOperation.AssignReject:
                    status = TroubleStatus.NewTrouble;
                    break;
            }
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (operation == TroubleOperation.CancelTrouble)
                    {
                        List<TroubleEqp> troubleEqps = await _troubleReportRepo.ListEqpIDByTroubles(ids.Split(','));
                        List<int> eqpIDs = troubleEqps.Select(a => a.Eqp).Distinct().ToList();
                        List<Equipment> eqpInfos = await _healthConfigRepo.GetEqpTypeByEqps(eqpIDs);
                        List<int> eqpTypes = eqpInfos.Select(a => a.Type).Distinct().ToList();
                        List<HealthConfig> configs = await _healthConfigRepo.GetValByCon((int)HealthType.Trouble, eqpTypes);
                        List<Health> health = await _healthRepo.ListEqp(eqpIDs);
                        Health h = new Health();
                        h.CreatedBy = _userID;
                        h.CreatedTime = dt;
                        h.UpdatedBy = _userID;
                        h.UpdatedTime = dt;
                        h.Type = (int)HealthType.Trouble;
                        HealthHistory hh = new HealthHistory();
                        hh.CreatedBy = _userID;
                        hh.CreatedTime = dt;
                        hh.Type = (int)HealthType.Trouble;
                        //考虑到多个故障包含同一个设备，这种情况就需要多减几次
                        foreach (TroubleEqp te in troubleEqps)
                        {
                            h.Eqp = te.Eqp;
                            hh.Eqp = h.Eqp;
                            h.CorrelationID = te.Trouble;
                            h.TroubleLevel = te.TroubleLevel;
                            hh.CorrelationID = te.Trouble;
                            hh.TroubleLevel = te.TroubleLevel;
                            bool? isInsert = GetHealthVal(false, eqpInfos, configs,ref health, ref h);
                            hh.Val = h.Val;
                            hh.EqpType = h.EqpType;
                            if (isInsert == false)
                            {
                                await _healthRepo.Update(h);
                                await _healthHistoryRepo.Save(hh);
                            }
                        }
                        await _troubleReportRepo.SaveHistory(ths);
                    }
                    else
                    {
                        await _troubleReportRepo.SaveHistory(thTmp);
                        if (operation == TroubleOperation.Pass)
                        {
                            await _eqpHistoryRepo.SaveEqpHistory(eqps);
                        }
                    }
                    if (operation==TroubleOperation.Unpass || operation==TroubleOperation.Pass)
                    {
                        await _troubleReportRepo.UpdateDealResult(thTmp.Trouble, _userID, status, operation, content);
                    }
                    ret.data = await _troubleReportRepo.UpdateStatus(ids.Split(','), _userID, status, operation);
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

        public async Task<ApiResult> Save(TroubleReport troubleReport)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                troubleReport.UpdatedTime = dt;
                troubleReport.CreatedTime = dt;
                troubleReport.UpdatedBy = _userID;
                troubleReport.CreatedBy = _userID;
                troubleReport.Status = (int)TroubleStatus.NewTrouble;
                troubleReport.LastOperation = (int)TroubleOperation.NewTrouble;
                string lineCode = await _troubleReportRepo.GetLineCodeByID(troubleReport.Line);
                string orgCode = await _troubleReportRepo.GetNodeCodeByID(troubleReport.ReportedCompany);
                string lastNum = await _troubleReportRepo.GetLastNumByDate(troubleReport.ReportedTime);
                string reportDate = troubleReport.ReportedTime.ToString("yyyyMMdd");
                string tmp;
                if (!string.IsNullOrWhiteSpace(lastNum))
                {
                    tmp = reportDate + (Convert.ToInt32(lastNum.Substring(lastNum.Length-3)) + 1).ToString("D3");
                }
                else
                {
                    tmp = reportDate + "001";
                }
                troubleReport.Code = orgCode + lineCode + tmp;
                List<TroubleEqp> troubleEqp = JsonConvert.DeserializeObject<List<TroubleEqp>>(troubleReport.Eqps);
                List<int> eqpIDs = troubleEqp.Select(a=>a.Eqp).ToList();
                List<Equipment> eqps =await _healthConfigRepo.GetEqpTypeByEqps(eqpIDs);
                List<int> eqpTypes = eqps.Select(a => a.Type).Distinct().ToList();
                List<HealthConfig> configs = await _healthConfigRepo.GetValByCon((int)HealthType.Trouble, eqpTypes);
                List<Health> health = await _healthRepo.ListEqp(eqpIDs);
                using (TransactionScope ts=new TransactionScope())
                {
                    TroubleReport tr= await _troubleReportRepo.Save(troubleReport);
                    foreach (var item in troubleEqp)
                    {
                        item.Trouble = tr.ID;
                        await _troubleReportRepo.SaveTroubleEqp(item);
                    }
                    if (!string.IsNullOrWhiteSpace(troubleReport.UploadFiles))
                    {
                        List<object> objs = new List<object>();
                        JArray jobj = JsonConvert.DeserializeObject<JArray>(troubleReport.UploadFiles);
                        foreach (var obj in jobj)
                        {
                            foreach (var item in obj["ids"].ToString().Split(','))
                            {
                                UploadFileRelation upload = new UploadFileRelation();
                                upload.Entity = tr.ID;
                                upload.File = Convert.ToInt32(item);
                                upload.Type = Convert.ToInt32(obj["type"]);
                                upload.SystemResource = (int)SystemResource.TroubleReport;
                                await _troubleReportRepo.SaveUploadFile(upload);
                            }
                        }
                    }
                    TroubleHistory history = new TroubleHistory();
                    history.Trouble = tr.ID;
                    history.OrgTop = 0;
                    history.Operation = TroubleOperation.NewTrouble;
                    history.Content = troubleReport.Code;
                    history.CreatedBy = _userID;
                    history.CreatedTime = dt;
                    await _troubleReportRepo.SaveHistory(history);
                    Health h = new Health();
                    h.CorrelationID = tr.ID;
                    h.CreatedBy = _userID;
                    h.CreatedTime = dt;
                    h.UpdatedBy = _userID;
                    h.UpdatedTime = dt;
                    h.Type = (int)HealthType.Trouble;
                    h.TroubleLevel = tr.Level;
                    HealthHistory hh = new HealthHistory();
                    hh.CorrelationID = tr.ID;
                    hh.CreatedBy = _userID;
                    hh.CreatedTime = dt;
                    hh.Type=(int)HealthType.Trouble;
                    hh.TroubleLevel = tr.Level;
                    foreach (var item in eqpIDs)
                    {
                        h.Eqp = item;
                        hh.Eqp = h.Eqp;
                        bool? isInsert = GetHealthVal(true,eqps,configs,ref health, ref h);
                        hh.Val = h.Val;
                        hh.EqpType = h.EqpType;
                        if (isInsert==true)
                        {
                            await _healthRepo.Save(h);
                            await _healthHistoryRepo.Save(hh);
                        }
                        else if (isInsert == false)
                        {
                            await _healthRepo.Update(h);
                            await _healthHistoryRepo.Save(hh);
                        }
                    }
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 计算当前健康度
        /// </summary>
        /// <param name="isNew">故障报警/取消故障报警</param>
        /// <param name="eqpTypes">所有故障设备的设备类型列表</param>
        /// <param name="configs">设备类型和故障对应的健康度配置列表</param>
        /// <param name="health">当前设备的健康度</param>
        /// <param name="h">所要更新的设备的健康度</param>
        /// <returns></returns>
        private bool? GetHealthVal(bool isNew,List<Equipment> eqpTypes,
            List<HealthConfig> configs,ref List<Health> health, ref Health h)
        {
            int? troubleLevel = h.TroubleLevel;
            int eqp = h.Eqp;
            if (h.TroubleLevel == null) return null;
            int eqpType = eqpTypes.Where(a => a.ID == eqp).Select(a => a.Type).FirstOrDefault();
            double? val = configs.Where(a => a.EqpType == eqpType && a.TroubleLevel== troubleLevel).Select(a => a.Val).FirstOrDefault();
            if (val == null) return null;
            var hasEqp = health.Where(a => a.Eqp == eqp);
            double beforeVal = HEATHFULLVAL;
            double diff = configs.Where(a => a.EqpType == eqpType && a.TroubleLevel == troubleLevel)
                .Select(a => a.Val).FirstOrDefault();
            h.EqpType = eqpType;
            if (hasEqp.Count()>0)
            {
                var tmp = hasEqp.FirstOrDefault();
                h.ID = tmp.ID;
                beforeVal = tmp.Val;
                if (isNew)
                {
                    h.Val = beforeVal - diff;
                }
                else
                {
                    h.Val = beforeVal + diff;
                    if (h.Val > HEATHFULLVAL) h.Val = HEATHFULLVAL;
                }
                hasEqp.FirstOrDefault().Val = h.Val;
                return false;
            }
            else
            {
                h.Val = beforeVal - diff;
                return true;
            }
        }
        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                TroubleReport tr = await _troubleReportRepo.GetByID(id);
                List<QueryItem> locations = await _eqpHistoryRepo.ListAllLocations();
                List<TroubleEqp> tes = await _troubleReportRepo.ListEqpByTrouble(id);
                tr.RepairCompany= JsonConvert.SerializeObject(GetShowEqps(tes));
                tr.StartLocationName = locations.Where(a => a.LocationBy == tr.StartLocationBy && a.ID == tr.StartLocation)
                        .FirstOrDefault().Name;
                string ids = id.ToString();
                List<UploadFilesEntity> jarr = await UploadFileHelper.GetUploadFile(ids, 
                    UploadShowType.List,_consulServiceProvider,MyDictionary.SystemResource.TroubleReport);
                if (jarr != null && jarr.Count() > 0)
                {
                    tr.UploadFiles = jarr.FirstOrDefault().UploadFiles;
                }
                tr.TroubleDeals = await _troubleReportRepo.ListDealByTrouble(id);
                ret.data = tr;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        private List<RepairCompany> GetShowEqps(List<TroubleEqp> tes)
        {
            List<RepairCompany> ret = new List<RepairCompany>();
            IEnumerable<IGrouping<int, TroubleEqp>> groupAction = tes.GroupBy(a => a.Org);
            foreach (IGrouping<int, TroubleEqp> group in groupAction)
            {
                RepairCompany rc = new RepairCompany();
                rc.id = group.Key;
                rc.companyName = group.FirstOrDefault().OrgName;
                rc.eqpList = new List<Eqp>();
                foreach (TroubleEqp item in group)
                {
                    Eqp e = new Eqp();
                    e.id = item.Eqp;
                    e.eqp = item.Eqp;
                    e.name = item.EqpName;
                    e.org = item.Org;
                    e.topOrg = item.Org;
                    rc.eqpList.Add(e);
                }
                ret.Add(rc);
            }
            return ret;
        }

        public async Task<ApiResult> Update(TroubleReport troubleReport)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                troubleReport.UpdatedTime = dt;
                troubleReport.UpdatedBy = _userID;
                troubleReport.LastOperation = (int)TroubleOperation.UpdateTrouble;
                TroubleReport oldtr = await _troubleReportRepo.GetByID(troubleReport.ID);
                string oldDate = oldtr.ReportedTime.ToString("yyyyMMdd");
                string nowDate = troubleReport.ReportedTime.ToString("yyyyMMdd");
                troubleReport.Code = oldtr.Code;
                string lineCode, orgCode;
                TroubleHistory th = new TroubleHistory();
                th.Trouble = troubleReport.ID;
                th.Operation = TroubleOperation.UpdateTrouble;
                th.CreatedBy = _userID;
                th.CreatedTime = dt;
                string myOld = JsonConvert.SerializeObject(oldtr);
                string myNew= JsonConvert.SerializeObject(troubleReport);
                th.Content = myOld + "-" + myNew;
                //if (oldtr.Line != troubleReport.Line)
                //{
                //    lineCode = await _troubleReportRepo.GetLineCodeByID(troubleReport.Line);
                //    th.Content += "线路:"+oldtr.LineName+"-"+ troubleReport.LineName + ",";
                //}
                //if (oldtr.ReportedCompanyPath != troubleReport.ReportedCompanyPath)
                //{
                //    orgCode = await _troubleReportRepo.GetNodeCodeByID(troubleReport.ReportedCompany);
                //    th.Content += "报告单位:" + oldtr.ReportedCompany + "-" + troubleReport.ReportedCompany + ",";
                //}
                if (oldtr.Line != troubleReport.Line || oldtr.ReportedCompanyPath != troubleReport.ReportedCompanyPath ||
                    oldDate != nowDate)
                {
                    //根据主键查询，效率高，所以没有再次判断
                    lineCode = await _troubleReportRepo.GetLineCodeByID(troubleReport.Line);
                    orgCode = await _troubleReportRepo.GetNodeCodeByID(troubleReport.ReportedCompany);
                    string tmp= oldtr.Code.Substring(oldtr.Code.Length-11);
                    //这个查询效率低，所以再判断下
                    if (oldDate != nowDate)
                    {
                        string lastNum = await _troubleReportRepo.GetLastNumByDate(troubleReport.ReportedTime);
                        if (!string.IsNullOrWhiteSpace(lastNum))
                        {
                            tmp = nowDate + (Convert.ToInt32(lastNum) + 1).ToString("D3");
                        }
                        else
                        {
                            tmp = nowDate + "001";
                        }
                    }
                    troubleReport.Code = orgCode + lineCode + tmp;
                }
                //健康度
                //先判断设备和故障等级有没有修改
                List<TroubleEqp> troubleEqp = JsonConvert.DeserializeObject<List<TroubleEqp>>(troubleReport.Eqps);
                List<int> eqpIDs = troubleEqp.Select(a => a.Eqp).OrderBy(a=>a).ToList();
                List<TroubleEqp> oldTroubleEqp = await _troubleReportRepo.ListEqpIDByTroubles(new string[] { troubleReport.ID.ToString() });
                List<int> oldEqpIDs = oldTroubleEqp.Select(a => a.Eqp).OrderBy(a => a).ToList();
                if (!oldEqpIDs.SequenceEqual(eqpIDs) || oldtr.Level!= troubleReport.Level)
                {
                    List<int> eqpIDAll = eqpIDs.Union(oldEqpIDs).ToList();
                    List<Equipment> eqpInfos = await _healthConfigRepo.GetEqpTypeByEqps(eqpIDAll);
                    List<int> eqpTypes = eqpInfos.Select(a => a.Type).Distinct().ToList();
                    List<HealthConfig> configs = await _healthConfigRepo.GetValByCon((int)HealthType.Trouble, eqpTypes);
                    List<Health> health = await _healthRepo.ListEqp(eqpIDAll);
                    using (TransactionScope ts=new TransactionScope())
                    {
                        //按取消逻辑恢复修改前的设备健康度
                        Health h = new Health();
                        h.CreatedBy = _userID;
                        h.CreatedTime = dt;
                        h.UpdatedBy = _userID;
                        h.UpdatedTime = dt;
                        h.Type = (int)HealthType.Trouble;
                        HealthHistory hh = new HealthHistory();
                        hh.CreatedBy = _userID;
                        hh.CreatedTime = dt;
                        hh.Type = (int)HealthType.Trouble;
                        foreach (int eqpID in oldEqpIDs)
                        {
                            h.Eqp = eqpID;
                            hh.Eqp = eqpID;
                            var tmp = oldTroubleEqp.Where(a => a.Eqp == eqpID).FirstOrDefault();
                            h.CorrelationID = tmp.Trouble;
                            h.TroubleLevel = tmp.TroubleLevel;
                            hh.CorrelationID = tmp.Trouble;
                            hh.TroubleLevel = tmp.TroubleLevel;
                            bool? isInsert = GetHealthVal(false, eqpInfos, configs, ref health, ref h);
                            hh.Val = h.Val;
                            hh.EqpType = h.EqpType;
                            if (isInsert == false)
                            {
                                await _healthRepo.Update(h);
                                await _healthHistoryRepo.Save(hh);
                            }
                        }
                        //按添加逻辑更新设备健康度
                        Health h1 = new Health();
                        h1.CreatedBy = _userID;
                        h1.CreatedTime = dt;
                        h1.UpdatedBy = _userID;
                        h1.UpdatedTime = dt;
                        h1.Type = (int)HealthType.Trouble;
                        HealthHistory hh1 = new HealthHistory();
                        hh1.CreatedBy = _userID;
                        hh1.CreatedTime = dt;
                        hh1.Type = (int)HealthType.Trouble;
                        foreach (int eqpID in eqpIDs)
                        {
                            h1.Eqp = eqpID;
                            hh1.Eqp = eqpID;
                            var tmp = troubleEqp.Where(a => a.Eqp == eqpID).FirstOrDefault();
                            h1.CorrelationID = tmp.Trouble;
                            h1.TroubleLevel = troubleReport.Level;
                            hh1.CorrelationID = tmp.Trouble;
                            hh1.TroubleLevel = troubleReport.Level;
                            bool? isInsert = GetHealthVal(true, eqpInfos, configs, ref health, ref h1);
                            hh1.Val = h1.Val;
                            hh1.EqpType = h1.EqpType;
                            if (isInsert == false)
                            {
                                await _healthRepo.Update(h1);
                                await _healthHistoryRepo.Save(hh1);
                            }
                            else if (isInsert == true)
                            {
                                await _healthRepo.Save(h1);
                                await _healthHistoryRepo.Save(hh1);
                            }
                        }
                        ts.Complete();
                    }
                    
                }
                //应该和健康度合在一个事务里
                ret.data = await _troubleReportRepo.Update(troubleReport, th);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        class Eqp
        {
            public int id { get; set; }
            public int eqp { get; set; }
            public string name { get; set; }
            public int org { get; set; }
            public int topOrg { get; set; }
        }

        class RepairCompany
        {
            public int id { get; set; }
            public string companyName { get; set; }
            public List<Eqp> eqpList { get; set; }
        }

        private async Task<int> getTopOrgByUser()
        {
            var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
            IHttpClientHelper<ApiResult> h = new HttpClientHelper<ApiResult>();
            ApiResult r = await h.GetSingleItemRequest(_services + "/api/v1/user/" + _userID);
            JObject jobj = JsonConvert.DeserializeObject<JObject>(r.data.ToString());
            if ((bool)jobj["is_super"])
            {
                return -1;
            }
            else
            {
                _services = await _consulServiceProvider.GetServiceAsync("OrgService");
                IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                ApiResult result = await httpHelper.GetSingleItemRequest(_services + "/api/v1/org/topnode/" + _userID);//orguser/ListNodeByUser
                if (result.data != null)
                {
                    JObject obj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    return Convert.ToInt32(obj["id"]);
                }
            }
            return 0;
        }

        #region deal
        public async Task<ApiResult> SaveDeal(TroubleDeal troubleDeal)
        {
            ApiResult ret = new ApiResult();
            TroubleDeal td = new TroubleDeal();
            try
            {
                DateTime dt = DateTime.Now;
                troubleDeal.CreatedTime = dt;
                troubleDeal.CreatedBy = _userID;
                troubleDeal.UpdateTime = dt;
                troubleDeal.UpdateBy = _userID;
                int id = troubleDeal.Trouble;
                using (TransactionScope scope = new TransactionScope())
                {
                    if (troubleDeal.ID > 0)
                    {
                        await _troubleReportRepo.UpdateDeal(troubleDeal);
                    }
                    else
                    {
                        td = await _troubleReportRepo.SaveDeal(troubleDeal);
                    }
                    //目前只考虑一个公司，以后多个公司也只是改对应的状态，显示也显示多个状态，没有主状态的概念
                    //if (await isRepaired(id)>0)
                    {
                        await _troubleReportRepo.UpdateStatus(new string[] { id.ToString() }, _userID,
                                    TroubleStatus.Repaired, TroubleOperation.Dealed);
                    }
                    TroubleHistory th = new TroubleHistory();
                    th.Trouble = id;
                    th.OrgTop = troubleDeal.OrgTop;
                    th.Operation = TroubleOperation.Dealed;
                    th.Content = troubleDeal.Code;
                    th.CreatedBy = _userID;
                    th.CreatedTime = dt;
                    await _troubleReportRepo.SaveHistory(th);
                    scope.Complete();
                }
                ret.data = td;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>0:其他单位有还未处理的；1：只有自家单位需要处理；2：其他单位都处理了</returns>
        private async Task<int> isRepaired(int id)
        {
            List<int> orgTops = await _troubleReportRepo.ListOrgTopByTrouble(id);
            if (orgTops.Count == 1)
            {
                return 1;
            }
            else if (orgTops.Count > 1)
            {
                orgTops.Remove(id);
                List<TroubleHistory> ths = await _troubleReportRepo.ListOperationByTroubles(orgTops);
                if (ths.Count > 0)
                {
                    bool isFinished = true;
                    foreach (var item in ths)
                    {
                        TroubleOperation to = item.Operation;
                        if (to != TroubleOperation.Dealed && to != TroubleOperation.Delayed)
                        {
                            isFinished = false;
                            break;
                        }
                    }
                    if (isFinished)
                    {
                        return 2;
                    }
                }
            }
            return 0;
        }
        public async Task<ApiResult> GetDealByID(int id,int orgTop)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _troubleReportRepo.ListDealByTrouble(id,orgTop);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }
        #endregion
    }
}
