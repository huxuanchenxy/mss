﻿using Dapper;
using MSS.API.Common;
using MSS.Platform.Workflow.WebApi.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Coded By admin 2019/9/27 11:18:53
namespace MSS.Platform.Workflow.WebApi.Data
{
    public interface IConstructionPlanMonthDetailRepo<T>
    {
        Task<ConstructionPlanMonthDetails> ListPage(ConstructionPlanMonthDetailParm parm);
        Task<ConstructionPlanMonthDetail> GetByID(int id);
        Task<int> Update(ConstructionPlanMonthDetail obj);
    }

    public class ConstructionPlanMonthDetailRepo : BaseRepo, IConstructionPlanMonthDetailRepo<ConstructionPlanMonthDetail>
    {
        public ConstructionPlanMonthDetailRepo(DapperOptions options) : base(options)
        {
        }

        public async Task<ConstructionPlanMonthDetails> ListPage(ConstructionPlanMonthDetailParm parm)
        {
            return await WithConnection(async c =>
            {
                ConstructionPlanMonthDetails ret = new ConstructionPlanMonthDetails();
                string sql = "SELECT c.*,et.type_name,ot.name as team_name,d.name,dt.name as pm_type_name " +
                " FROM construction_plan_month_detail c " +
                " left join equipment_type et on et.id=c.eqp_type " +
                " left join org_tree ot on ot.id=c.team " +
                " left join dictionary_tree d on d.id=c.work_type " +
                " left join dictionary_tree dt on dt.id=c.pm_type ";
                string sqlwhere=" where c.query=" + parm.Query+" and c.month=" + parm.Month;
                if (parm.Team!=null)
                {
                    sqlwhere += " and c.team=" + parm.Team;
                }
                if (parm.Location!=null && parm.LocationBy!=null)
                {
                    sqlwhere += " and c.location=" + parm.Location + " and c.locatin_by=" + parm.LocationBy;
                }
                if (!string.IsNullOrWhiteSpace(parm.PlanDate))
                {
                    string[] tmpDate = parm.PlanDate.Split('.');
                    string lastDay = MSS.Platform.Workflow.WebApi.Model.Common.GetLastDay(Convert.ToInt32(tmpDate[1]), Convert.ToInt32(tmpDate[0]));
                    string myDate = tmpDate[0] + "." + tmpDate[1] + ".01-" + tmpDate[0] + "." + tmpDate[1] + "." + lastDay;
                    sqlwhere += " and (c.plan_date='" + parm.PlanDate + "' or c.plan_date='" + myDate+"') ";
                }
                sql = sql + sqlwhere + " order by plan_date limit " + (parm.page - 1) * parm.rows + "," + parm.rows;
                var tmp = await c.QueryAsync<ConstructionPlanMonthDetail>(sql);
                if (tmp.Count()>0)
                {
                    sql= "select count(*) FROM construction_plan_month_detail c " + sqlwhere;
                    ret.total = await c.QueryFirstOrDefaultAsync<int>(sql);
                    ret.rows = tmp.ToList();
                }
                else
                {
                    ret.rows = new List<ConstructionPlanMonthDetail>();
                    ret.total = 0 ;
                }
                return ret;
            });
        }

        public async Task<ConstructionPlanMonthDetail> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                ConstructionPlanMonthDetails ret = new ConstructionPlanMonthDetails();
                string sql = "SELECT *,et.type_name,ot.name as team_name,d.name,dt.name as pm_type_name " +
                " FROM construction_plan_month_detail c " +
                " left join equipment_type et on et.id=c.eqp_type " +
                " left join org_tree ot on ot.id=c.team " +
                " left join dictionary_tree d on d.id=c.work_type " +
                " left join dictionary_tree dt on dt.id=c.pm_type where c.id=@id";
                return await c.QueryFirstOrDefaultAsync<ConstructionPlanMonthDetail>(sql,new { id});
            });
        }
        public async Task<int> Update(ConstructionPlanMonthDetail obj)
        {
            return await WithConnection(async c =>
            {
                string sql = " update construction_plan_month_detail " +
                        " set work_type =@WorkType,pm_type=@PMType,update_time=@UpdateTime," +
                        " update_by=@UpdateBy where id=@id ";
                int ret = await c.QueryFirstOrDefaultAsync<int>(sql, obj);
                return ret;
            });
        }

    }
}



