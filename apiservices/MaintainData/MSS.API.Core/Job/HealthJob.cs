using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.API.Dao.Implement;
using MSS.API.Model.Data;
using MSS.Common.Consul;
using Newtonsoft.Json;
using Quartz;
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
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        public HealthJob(IHealthConfigRepo<HealthConfig> healthConfigRepo,
            IServiceDiscoveryProvider consulServiceProvider,
            IHealthRepo<Health> healthRepo,
            IHealthHistoryRepo<HealthHistory> healthHistoryRepo)
        {
            //_logger = logger;
            _healthConfigRepo = healthConfigRepo;
            _consulServiceProvider = consulServiceProvider;
            _healthRepo = healthRepo;
            _healthHistoryRepo = healthHistoryRepo;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            //return Task.Run(() =>
            //{
            //    Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            //});
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

                        }
                        await Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " 健康度每日执行成功" + msg);
                        scope.Complete();
                    }
                }
                else await Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " 查询不到设备");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                await Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " 健康度每日执行失败");
            }
        }

    }

}
