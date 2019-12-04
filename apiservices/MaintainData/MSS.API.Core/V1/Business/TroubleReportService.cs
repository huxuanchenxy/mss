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

namespace MSS.API.Core.V1.Business
{
    public interface ITroubleReportService
    {
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

        public TroubleReportService(ITroubleReportRepo<TroubleReport> troubleReportRepo,
            IEqpHistoryRepo<EqpHistory> eqpHistoryRepo, IAuthHelper authhelper, 
            IServiceDiscoveryProvider consulServiceProvider)
        {
            _troubleReportRepo = troubleReportRepo;
            _eqpHistoryRepo = eqpHistoryRepo;
            _userID = authhelper.GetUserId();
            _consulServiceProvider = consulServiceProvider;
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
                        await _troubleReportRepo.SaveHistory(ths);
                    }
                    else
                    {
                        await _troubleReportRepo.SaveHistory(thTmp);
                    }
                    ret.data = await _troubleReportRepo.UpdateStatus(ids.Split(), _userID, status, operation,content);
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
                ret.data = await _troubleReportRepo.Save(troubleReport);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
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
                ret.data = await _troubleReportRepo.Update(troubleReport,th);
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
