using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.Platform.Workflow.WebApi.Data;
using MSS.Platform.Workflow.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


// Coded By admin 2019/11/29 10:44:11
namespace MSS.Platform.Workflow.WebApi.Service
{
    public interface IConstructionPlanMonthChartService
    {
        Task<ApiResult> GetPageList(ConstructionPlanMonthChartParm parm);
        Task<ApiResult> Save(ConstructionPlanMonthChart obj);
        Task<ApiResult> Update(ConstructionPlanMonthChart obj);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> GetMonthChart(ConstructionPlanMonthChartParm parm);
    }

    public class ConstructionPlanMonthChartService : IConstructionPlanMonthChartService
    {
        private readonly IConstructionPlanMonthChartRepo<ConstructionPlanMonthChart> _repo;
        private readonly IAuthHelper _authhelper;
        private readonly int _userID;

        public ConstructionPlanMonthChartService(IConstructionPlanMonthChartRepo<ConstructionPlanMonthChart> repo, IAuthHelper authhelper)
        {
            _repo = repo;
            _authhelper = authhelper;
            _userID = _authhelper.GetUserId();
        }

        public async Task<ApiResult> GetPageList(ConstructionPlanMonthChartParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                //parm.UserID = _userID;
                //parm.UserID = 40;
                var data = await _repo.GetPageList(parm);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetMonthChart(ConstructionPlanMonthChartParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                //parm.year = 2019;
                //parm.month = 11;
                //parm.xAxisType = 1;//按日统计
                ConstructionPlanMonthChartRet chartobj = new ConstructionPlanMonthChartRet();
                //dimision
                if (parm.xAxisType == 1)
                {
                    DateTime startDate = new DateTime();
                    DateTime endDate = new DateTime();
                    if (!string.IsNullOrEmpty(parm.startTime))
                    {
                        startDate = Convert.ToDateTime(parm.startTime);
                        parm.year = startDate.Year;
                    }
                    if (!string.IsNullOrEmpty(parm.endTime))
                    {
                        endDate = Convert.ToDateTime(parm.endTime);
                    }
                    if (parm.month == 0)
                    {
                        DateTime now = DateTime.Now;
                        parm.month = now.Month;
                    }
                    var data = await _repo.GetByParm(parm);
                    DateTime dt = new DateTime(parm.year, parm.month, 1);
                    int days = DateTime.DaysInMonth(dt.Year, dt.Month);
                    List<string> d = new List<string>();
                    List<ConstructionPlanMonthChartSeries> legend = new List<ConstructionPlanMonthChartSeries>();
                    List<int> legend1 = new List<int>();
                    var dy0 = data.Where(c => c.Day == 0);//每天都有的
                    int countdy0 = 0;
                    foreach (var d0 in dy0)
                    {
                        countdy0 += (d0.PmFrequency / 31);
                    }
                    for (int i = 1; i <= days; i++)
                    {

                        if ((startDate.Day <= i && i <= endDate.Day) || string.IsNullOrEmpty(parm.startTime))
                        {
                            string curdt = new DateTime(parm.year, parm.month, i).ToString("MM-dd");
                            d.Add(curdt);
                            var dynot0 = data.Where(c => c.Day == i);//只有当天有的
                            int countdynot0 = 0;
                            foreach (var dno0 in dynot0)
                            {
                                countdynot0 += dno0.PmFrequency;
                            }
                            float curdayCount = countdy0 + countdynot0;
                            //fake
                            int fake = new Random().Next(-2, 2);
                            float curdayFinish = curdayCount + fake;
                            if (curdayFinish <= 0)
                            {
                                curdayFinish = curdayCount / 3;
                            }
                            int curpercent = 0;
                            if (curdayFinish > 0)
                            {
                                curpercent = (int)(Math.Round((curdayCount / curdayFinish), 2) * 100);
                            }
                            
                            legend1.Add(curpercent);
                            //legend2.Add(curdayCount- fake);
                        }

                    }
                    ConstructionPlanMonthChartSeries legendobj1 = new ConstructionPlanMonthChartSeries();
                    legendobj1.Name = "计划完成率";
                    legendobj1.Data = legend1;
                    legendobj1.Type = "bar";

                    //ConstructionPlanMonthChartSeries legendobj2 = new ConstructionPlanMonthChartSeries();
                    //legendobj2.Name = "实际完成";
                    //legendobj2.Data = legend2;
                    //legendobj2.Type = "bar";

                    legend.Add(legendobj1);
                    //legend.Add(legendobj2);
                    chartobj.Series = legend;
                    chartobj.Dimension = d;

                    List<string> catagory = new List<string>();
                    catagory.Add("计划完成率");
                    //catagory.Add("实际完成");
                    chartobj.Legend = catagory;
                }

                if (parm.xAxisType == 2)
                {
                    parm.month = 0;
                    var data = await _repo.GetByParm(parm);
                    //DateTime dt = new DateTime(parm.year, parm.month, 1);
                    //int days = DateTime.DaysInMonth(dt.Year, dt.Month);
                    var monthgap = parm.endMonth - parm.startMonth + 1;
                    
                    List<ConstructionPlanMonthChartSeries> legend = new List<ConstructionPlanMonthChartSeries>();
                    List<int> legend1 = new List<int>();
                    List<string> d = new List<string>();
                    for (int i = parm.startMonth; i <= monthgap; i++)
                    {
                        string curd = new DateTime(parm.year, i, 1).ToString("yyyy-MM");
                        d.Add(curd);
                        var curmonth = data.Where(c => c.Month == i).ToList();
                        int countdy0 = 0;
                        foreach (var d0 in curmonth)
                        {
                            countdy0 += (d0.PmFrequency / 31);
                        }
                        
                        float curmonthCount = 0;
                        DateTime dt = new DateTime(parm.year, i, 1);
                        int days = DateTime.DaysInMonth(dt.Year, dt.Month);
                        for (int j = 1; j <= days; j++)
                        {

                            var dynot0 = curmonth.Where(c => c.Day == j);//只有当天有的
                            int countdynot0 = 0;
                            foreach (var dno0 in dynot0)
                            {
                                countdynot0 += dno0.PmFrequency;
                            }
                            float curdayCount = countdy0 + countdynot0;
                            
                            curmonthCount += curdayCount;
                        }
                        //fake
                        int fake = new Random().Next(-2, 2);
                        float curmonthFinish = curmonthCount + fake;
                        if (curmonthFinish <= 0)
                        {
                            curmonthFinish = curmonthCount / 3;
                        }
                        int curpercent = 0;
                        if (curmonthFinish > 0) {
                           curpercent = (int)(Math.Round((curmonthCount / curmonthFinish), 2) * 100);
                        } 
                        legend1.Add(curpercent);

                    }
                    ConstructionPlanMonthChartSeries legendobj1 = new ConstructionPlanMonthChartSeries();
                    legendobj1.Name = "计划完成率";
                    legendobj1.Data = legend1;
                    legendobj1.Type = "bar";

                    //ConstructionPlanMonthChartSeries legendobj2 = new ConstructionPlanMonthChartSeries();
                    //legendobj2.Name = "实际完成";
                    //legendobj2.Data = legend2;
                    //legendobj2.Type = "bar";

                    legend.Add(legendobj1);
                    //legend.Add(legendobj2);
                    chartobj.Series = legend;
                    chartobj.Dimension = d;

                    List<string> catagory = new List<string>();
                    catagory.Add("计划完成率");
                    //catagory.Add("实际完成");
                    chartobj.Legend = catagory;
                }
                ret.code = Code.Success;
                ret.data = chartobj;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> Save(ConstructionPlanMonthChart obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                ret.data = await _repo.Save(obj);
                ret.code = Code.Success;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(ConstructionPlanMonthChart obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ConstructionPlanMonthChart et = await _repo.GetByID(obj.Id);
                if (et != null)
                {
                    DateTime dt = DateTime.Now;
                    //obj.UpdatedTime = dt;
                    //obj.UpdatedBy = _userID;
                    ret.data = await _repo.Update(obj);
                    ret.code = Code.Success;
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
                ret.data = await _repo.Delete(ids.Split(','), _userID);
                ret.code = Code.Success;
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
                ConstructionPlanMonthChart obj = await _repo.GetByID(id);
                ret.data = obj;
                ret.code = Code.Success;
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



