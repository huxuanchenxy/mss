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

namespace MSS.API.Core.V1.Business
{
    public interface ITroubleReportService
    {
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> ListPage(TroubleReportParm parm);
    }
    public class TroubleReportService : ITroubleReportService
    {
        private readonly ITroubleReportRepo<TroubleReport> _TroubleReportRepo;
        private readonly IEqpHistoryRepo<EqpHistory> _eqpHistoryRepo;
        private readonly int _userID;

        public TroubleReportService(ITroubleReportRepo<TroubleReport> troubleReportRepo,
            IEqpHistoryRepo<EqpHistory> eqpHistoryRepo, IAuthHelper authhelper)
        {
            _TroubleReportRepo = troubleReportRepo;
            _eqpHistoryRepo = eqpHistoryRepo;
            _userID = authhelper.GetUserId();
        }


        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _TroubleReportRepo.GetByID(id);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> ListPage(TroubleReportParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<QueryItem> locations =await _eqpHistoryRepo.ListAllLocations();
                TroubleReportView view = await _TroubleReportRepo.ListPage(parm);
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

    }
}
