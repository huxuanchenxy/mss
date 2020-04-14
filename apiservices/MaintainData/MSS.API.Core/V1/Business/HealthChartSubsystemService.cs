using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using MSS.API.Common;
using static MSS.API.Common.Const;
using MSS.API.Common.Utility;
using MSS.Common.Consul;
using static MSS.API.Common.MyDictionary;
using Newtonsoft.Json.Linq;
using MSS.API.Dao.Implement;
using System.Transactions;

namespace MSS.API.Core.V1.Business
{
    public interface IHealthChartSubsystemService
    {
        Task<ApiResult> GetChart(HealthChartSubsystemParm parm);
    }
    public class HealthChartSubsystemService : IHealthChartSubsystemService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IHealthChartSubsystemRepo<HealthChartSubsystem> _repo;

        public HealthChartSubsystemService(IHealthChartSubsystemRepo<HealthChartSubsystem> repo)
        {
            _repo = repo;
        }

        public async Task<ApiResult> GetChart(HealthChartSubsystemParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                HealthChartResult rt = new HealthChartResult();
                DateTime dt = DateTime.Now;
                if (string.IsNullOrEmpty(parm.startTime))
                {
                    parm.startTime = dt.AddMonths(-6).ToString("yyyy-MM-01");
                    parm.endTime = dt.ToString("yyyy-MM-01");
                }
                var _data = await _repo.GetChart(parm);
                if (_data != null)
                {
                    List<string> xAxisData = new List<string>();
                    for (int i = 6; i >= 1; i--)
                    {
                        DateTime curXaxis = dt.AddMonths(-i);
                        var checkyearmonth = _data.Where(c => c.year == curXaxis.Year && c.month == curXaxis.Month).FirstOrDefault();
                        if (checkyearmonth != null)//说明db里有这个月的数据
                        {
                            xAxisData.Add(curXaxis.ToString("yyyy-MM"));
                        }
                    }
                    rt.xAxisData = xAxisData;

                    List<HealthSeries> seriesData = new List<HealthSeries>();
                    var extidArr = _data.GroupBy(c => c.extid);
                    foreach (var ea in extidArr)
                    {
                        var curdata = _data.Where(c => c.extid == ea.Key);
                        HealthSeries curSerie = new HealthSeries();
                        if (curdata != null)
                        {
                            curSerie.name = curdata.First().name;
                            List<double> ddavg = new List<double>();
                            foreach (var c in curdata)
                            {
                                ddavg.Add(c.avg);
                            }
                            curSerie.dataAvg = ddavg;
                        }
                        seriesData.Add(curSerie);
                    }
                    rt.seriesData = seriesData;
                }
                ret.data = rt;
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
