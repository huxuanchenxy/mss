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
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> ListPage(TroubleReportParm parm);
        Task<ApiResult> UpdateStatus(string ids, TroubleStatus status);
        Task<ApiResult> Save(TroubleReport troubleReport);
        Task<ApiResult> Update(TroubleReport troubleReport);
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


        //public async Task<ApiResult> GetByID(int id)
        //{
        //    ApiResult ret = new ApiResult();
        //    try
        //    {
        //        ret.data = await _troubleReportRepo.GetByID(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        ret.code = Code.Failure;
        //        ret.msg = ex.Message;
        //    }
        //    return ret;
        //}

        public async Task<ApiResult> ListPage(TroubleReportParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<QueryItem> locations =await _eqpHistoryRepo.ListAllLocations();
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

        public async Task<ApiResult> UpdateStatus(string ids,TroubleStatus status)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _troubleReportRepo.UpdateStatus(ids.Split(),_userID,status);
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
                string lineCode = await _troubleReportRepo.GetLineCodeByID(troubleReport.Line);
                string orgCode = await _troubleReportRepo.GetNodeCodeByID(troubleReport.ReportedCompany);
                string lastNum = await _troubleReportRepo.GetLastNumByDate(troubleReport.ReportedTime);
                string reportDate = troubleReport.ReportedTime.ToString("yyyyMMdd");
                string tmp;
                if (!string.IsNullOrWhiteSpace(lastNum))
                {
                    tmp = reportDate + (Convert.ToInt32(lastNum) + 1).ToString("D3");
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
                TroubleReport oldtr = await _troubleReportRepo.GetByID(troubleReport.ID);
                string oldDate = oldtr.ReportedTime.ToString("yyyyMMdd");
                string nowDate = troubleReport.ReportedTime.ToString("yyyyMMdd");
                troubleReport.Code = oldtr.Code;
                if (oldtr.Line != troubleReport.Line || oldtr.ReportedCompanyPath != troubleReport.ReportedCompanyPath ||
                    oldDate != nowDate)
                {
                    //根据主键查询，效率高，所以没有再次判断
                    string lineCode = await _troubleReportRepo.GetLineCodeByID(troubleReport.Line);
                    string orgCode = await _troubleReportRepo.GetNodeCodeByID(troubleReport.ReportedCompany);
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
                ret.data = await _troubleReportRepo.Update(troubleReport);
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
    }
}
