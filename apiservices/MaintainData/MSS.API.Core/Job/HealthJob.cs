using MathNet.Numerics.Statistics;
using Microsoft.Extensions.Logging;
using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.API.Dao.Implement;
using MSS.API.Model.Data;
using MSS.Common.Consul;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static MSS.API.Common.MyDictionary;

namespace MSS.API.Core.Job
{
    public class HealthJob:IJob
    {
        private readonly IHealthConfigRepo<HealthConfig> _healthConfigRepo;
        private readonly IHealthRepo<Health> _healthRepo;
        private readonly IHealthHistoryRepo<HealthHistory> _healthHistoryRepo;
        private readonly IHealthChartSubsystemRepo<HealthChartSubsystem> _healthChartSubsystemRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public HealthJob(IHealthConfigRepo<HealthConfig> healthConfigRepo,
            IServiceDiscoveryProvider consulServiceProvider,
            IHealthRepo<Health> healthRepo,
            IHealthHistoryRepo<HealthHistory> healthHistoryRepo,
            IHealthChartSubsystemRepo<HealthChartSubsystem> healthChartSubsystemRepo,
            ILogger<HealthJob> logger)
        {
            //_logger = logger;
            _healthConfigRepo = healthConfigRepo;
            _consulServiceProvider = consulServiceProvider;
            _healthRepo = healthRepo;
            _healthHistoryRepo = healthHistoryRepo;
            _healthChartSubsystemRepo = healthChartSubsystemRepo;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            //await Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " 健康度每日执行开始...");
            //return Task.Run(() =>
            //{
            //    Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            //});
            _logger.LogWarning("健康度开始LogInformation...");
            string msg = "";
            DateTime dt = DateTime.Now;
            double healthVal = HEATHFULLVAL;
            try
            {
                var _services = await _consulServiceProvider.GetServiceAsync("EqpService");
                IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                ApiResult result = await httpHelper.GetSingleItemRequest(_services + "/api/v1/Equipment/All");
                List<Equipment> allEqp = new List<Equipment>();
                if (result.data != null)
                {
                    allEqp = JsonConvert.DeserializeObject<List<Equipment>>(result.data.ToString());
                    List<HealthConfig> configs = await _healthConfigRepo.GetValByCon((int)HealthType.Time);
                    List<Health> healths = await _healthRepo.ListAll();
                    double configVal = 1;
                    if (configs.Count > 0)
                    {
                        configVal = configs.FirstOrDefault().Val / 100;
                        msg = ",查询到参数为" + configVal;
                    }
                    else
                    {
                        msg = ",未查询到参数，默认为1";
                    }
                    using (TransactionScope scope = new TransactionScope())
                    {
                        List<HealthChartSubsystem> chart = new List<HealthChartSubsystem>();
                        foreach (Equipment e in allEqp)
                        {
                            Health h = new Health();
                            h.CreatedBy = SYSTEM;
                            h.CreatedTime = dt;
                            h.Eqp = e.ID;
                            h.UpdatedBy = SYSTEM;
                            h.UpdatedTime = dt;
                            h.Type = (int)HealthType.Time;
                            HealthHistory hh = new HealthHistory();
                            hh.CreatedBy = SYSTEM;
                            hh.CreatedTime = dt;
                            hh.Eqp = e.ID;
                            hh.Type = (int)HealthType.Time;
                            hh.CreatedYear = dt.Year;
                            hh.CreatedMonth = dt.Month;
                            hh.CreatedDay = dt.Day;
                            //当天健康度无变化，按天衰减，否则按秒衰减
                            double standardDay = HEATHFULLVAL / (double)e.Life / 365;
                            double standardSecond = standardDay / (24 * 60 * 60);
                            var health = healths.Where(a => a.Eqp == e.ID);
                            if (health.Count() > 0)
                            {
                                Health oldHealth = health.FirstOrDefault();
                                if (oldHealth.IsRepaired > 0)
                                {
                                    standardSecond = standardSecond * configVal;
                                }
                                TimeSpan ts = dt - oldHealth.UpdatedTime;
                                h.Val = oldHealth.Val - standardSecond * ts.TotalSeconds;
                                h.ID = oldHealth.ID;
                                hh.Val = h.Val;
                                await _healthRepo.Update(h);
                                await _healthHistoryRepo.Save(hh);
                            }
                            else
                            {
                                DateTime onLine = e.Online;
                                if (e.OnlineAgain != null && DateTime.Compare((DateTime)e.OnlineAgain, onLine) > 0)
                                {
                                    onLine = (DateTime)e.OnlineAgain;
                                }
                                if (DateTime.Compare(dt, onLine) > 0)
                                {
                                    TimeSpan ts = dt - onLine;
                                    h.Val = healthVal - standardDay * ts.Days;
                                    hh.Val = h.Val;
                                    await _healthRepo.Save(h);
                                    await _healthHistoryRepo.Save(hh);
                                }
                            }
                            HealthChartSubsystem curSub = chart.Where(c => c.SubSystemId == e.SubSystem).FirstOrDefault();
                            if (curSub != null)
                            {
                                curSub.ValArr.Add(hh.Val);
                            }
                            else
                            {
                                List<double> valArr = new List<double>();
                                valArr.Add(hh.Val);
                                chart.Add(new HealthChartSubsystem() { SubSystemId = e.SubSystem, ValArr = valArr, Year = dt.Year, Month = dt.Month, Day = dt.Day, CreatedBy = SYSTEM, CreatedTime = dt, UpdatedBy = SYSTEM, UpdatedTime = dt });
                            }
                        }
                        //计算子系统维度的平均数等
                        foreach (var cc in chart)
                        {
                            double curAvg = cc.ValArr.Average();
                            double curMax = cc.ValArr.Max();
                            double curMin = cc.ValArr.Min();
                            double curMiddle = MathHelper.Median(cc.ValArr.ToArray());
                            cc.ValAvg = curAvg;
                            cc.ValMax = curMax;
                            cc.ValMin = curMin;
                            cc.ValMiddle = curMiddle;
                            await _healthChartSubsystemRepo.Delete2(cc);
                            await _healthChartSubsystemRepo.Save(cc);
                        }
                        //await Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " 健康度每日执行成功" + msg);
                        _logger.LogWarning(" 健康度每日执行成功LogInformation" + msg);
                        scope.Complete();
                    }
                }
                else 
                { 
                    //await Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " 查询不到设备");
                    _logger.LogWarning("查询不到设备LogWarning");
                    //_logger.LogInformation(" 查询不到设备");
                }
            }
            catch (Exception ex)
            {
                //await Console.Out.WriteLineAsync(ex.Message);
                //await Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " 健康度每日执行失败");
                _logger.LogError(" 健康度每日执行失败LogError:");
                _logger.LogError(ex.Message);
            }
        }

    }

}
