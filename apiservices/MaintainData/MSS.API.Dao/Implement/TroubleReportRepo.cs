using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Dapper.FastCrud;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static MSS.API.Common.MyDictionary;
using MSS.API.Common;

namespace MSS.API.Dao.Implement
{
    public class TroubleReportRepo :BaseRepo, ITroubleReportRepo<TroubleReport>
    {
        public TroubleReportRepo(DapperOptions options) :base(options)
            {
            }
        #region trouble_report
        public async Task<TroubleReport> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = "select tr.*,dt1.name,tcb.AreaName,ot.name as rcname, " +
                    "u.user_name,u1.user_name as cname,dt.name as lname " +
                    "from trouble_report tr " +
                    "LEFT JOIN tb_config_bigarea tcb on tcb.id=tr.end_location " +
                    "LEFT JOIN org_tree ot on ot.ID=tr.reported_company " +
                    "LEFT JOIN dictionary_tree dt1 on dt1.ID=tr.`status` " +
                    "LEFT JOIN dictionary_tree dt on dt.ID=tr.level " +
                    "LEFT JOIN `user` u on u.id=tr.reported_by " +
                    "LEFT JOIN `user` u1 on u1.id=tr.created_by " +
                    //"LEFT JOIN `user` u2 on u2.id=tr.accepted_by " +
                    "WHERE tr.id = @id";
                var result = await c.QueryFirstOrDefaultAsync<TroubleReport>(sql, new { id = id });
                return result;
            });
        }
        public async Task<TroubleReportView> ListPage(TroubleReportParm parm)
        {
            return await WithConnection(async c =>
            {
                TroubleReportView ret = new TroubleReportView();
                StringBuilder sql = new StringBuilder();
                sql.Append("select tr.*,dt1.name,")
                .Append("u.user_name,u1.user_name as cname,dt2.name as lname,ml.line_name, ")
                .Append("ot.name as rcname,u.mobile,u2.user_name as uname,dt3.name as loname ")
                .Append("from trouble_report tr ")
                .Append("LEFT JOIN metro_line ml on ml.id=tr.line ")
                //.Append("LEFT JOIN equipment e on e.id=tr.eqp ")
                //.Append("LEFT JOIN dictionary_tree dt on dt.ID=tr.type ")
                .Append("LEFT JOIN dictionary_tree dt1 on dt1.ID=tr.status ")
                .Append("LEFT JOIN dictionary_tree dt2 on dt2.ID=tr.level ")
                .Append("LEFT JOIN dictionary_tree dt3 on dt3.ID=tr.last_operation ")
                .Append("LEFT JOIN `user` u on u.id=tr.reported_by ")
                .Append("LEFT JOIN `user` u1 on u1.id=tr.created_by ")
                .Append("LEFT JOIN `user` u2 on u2.id=tr.updated_by ")
                .Append("LEFT JOIN org_tree ot on ot.id=tr.reported_company where 1=1 ");
                //.Append("LEFT JOIN org_tree ot1 on ot1.id=tr.accepted_company");
                StringBuilder whereSql = new StringBuilder();
                //if (!string.IsNullOrWhiteSpace(parm.RepairDesc))
                //{
                //    whereSql.Append(" and tr.description like '%" + parm.RepairDesc+"%' ");
                //}
                if (!string.IsNullOrWhiteSpace(parm.TroubleReportDesc))
                {
                    whereSql.Append(" and tr.description like '%" + parm.TroubleReportDesc + "%' ");
                }
                if (parm.TroubleStatus!=null)
                {
                    whereSql.Append(" and tr.status = " + parm.TroubleStatus);
                }
                else if (parm.MenuView==TroubleView.MyRepair)
                {
                    whereSql.Append(" and tr.status = " + (int)TroubleStatus.NewTrouble)
                        //.Append(" or tr.status = " + (int)TroubleStatus.Delayed+") ")
                        .Append(" and tr.last_operation != " + (int)TroubleOperation.RepairReject);
                }
                else if (parm.MenuView == TroubleView.MyProcessing)
                {
                    whereSql.Append(" and (tr.status = " + (int)TroubleStatus.Processing )
                    .Append(" or tr.status = " + (int)TroubleStatus.Delayed + ") ");
                    if (parm.OrgNode >0)
                    {
                        whereSql.Append(" and tr.id in (select distinct trouble from trouble_eqp where org_node=" + parm.OrgNode + ") ");
                    }
                }
                else if (parm.MenuView == TroubleView.MyPreCheck)
                {
                    whereSql.Append(" and (tr.status = " + (int)TroubleStatus.Repaired)
                    .Append(" or tr.status = " + (int)TroubleStatus.Delayed)
                    .Append(" or (tr.status = " + (int)TroubleStatus.PendingApproval)
                    .Append(" && tr.last_operation = " + (int)TroubleOperation.Unpass+")) ");
                }
                else if (parm.MenuView == TroubleView.MyCheck)
                {
                    whereSql.Append(" and tr.status = " + (int)TroubleStatus.PendingApproval);
                }
                if (parm.RepairCompany>0 && parm.MenuView != TroubleView.MyProcessing)
                {
                    whereSql.Append(" and tr.id in (select distinct trouble from trouble_eqp where org=" + parm.RepairCompany+") ");
                }
                ret.RepairCompany = parm.RepairCompany;
                if (parm.StartTime != null)
                {
                    whereSql.Append(" and tr.happening_time >= '" + parm.StartTime + "' ");
                }
                if (parm.EndTime != null)
                {
                    whereSql.Append(" and tr.happening_time <= '" + parm.EndTime + "' ");
                }
                sql.Append(whereSql).Append(" order by tr." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var tmp = await c.QueryAsync<TroubleReport>(sql.ToString());
                if (tmp.Count() > 0)
                {
                    sql.Clear();
                    sql.Append("SELECT count(*) FROM trouble_report tr ")
                        .Append(" where 1=1 ").Append(whereSql);
                    ret.total = await c.QueryFirstOrDefaultAsync<int>(sql.ToString());
                    ret.rows = tmp.ToList();
                }
                else
                {
                    ret.rows = new List<TroubleReport>();
                    ret.total = 0 ;
                }
                return ret;
            });
        }

        public async Task<int> UpdateStatus(string[] ids, int userID, TroubleStatus status,TroubleOperation operation)
        {
            return await WithConnection(async c =>
            {
                string sql = " update trouble_report " +
                        " set status=@status,last_operation=@operation,updated_by=@userID,updated_time=@time ";
                if (status==TroubleStatus.Processing && operation==TroubleOperation.Assign)
                {
                    sql += ",accepted_time=@time ";
                }
                sql += " where id in @ids";
                int ret = await c.ExecuteAsync(sql,
                    new { ids, userID, time = DateTime.Now, status, operation});
                return ret;
            });
        }
        public async Task<int> Update(TroubleReport troubleReport, TroubleHistory troubleHistory)
        {
            return await WithConnection(async c =>
            {
                string sql;
                IDbTransaction trans = c.BeginTransaction();
                try
                {
                    var result = await c.ExecuteAsync(" update trouble_report " +
                        " set code=@Code,happening_time=@HappeningTime,reported_time=@ReportedTime,line=@Line, " +
                        " start_location=@StartLocation,start_location_by=@StartLocationBy,start_location_path=@StartLocationPath, " +
                        " end_location=@EndLocation,end_location_by=@EndLocationBy,urgent_repair_order=@UrgentRepairOrder, " +
                        " level=@Level,reported_company=@ReportedCompany,reported_company_path=@ReportedCompanyPath, " +
                        " reported_by=@ReportedBy,description=@Desc,last_operation=@LastOperation," +
                        " updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", troubleReport, trans);
                    List<TroubleEqp> eqps = JsonConvert.DeserializeObject<List<TroubleEqp>>(troubleReport.Eqps);
                    sql = "delete from trouble_eqp where trouble=@id";
                    int ret = await c.ExecuteAsync(sql, new { id = troubleReport.ID }, trans);
                    sql = "insert into trouble_eqp values (0,@Trouble,@Org,@Eqp,null,null,null,null)";
                    ret = await c.ExecuteAsync(sql, eqps, trans);
                    if (!string.IsNullOrWhiteSpace(troubleReport.UploadFiles))
                    {
                        sql = "delete from upload_file_relation where entity_id=@id and system_resource=@sr";
                        ret = await c.ExecuteAsync(sql, new { id = troubleReport.ID, sr = SystemResource.TroubleReport }, trans);
                        List<object> objs = new List<object>();
                        JArray jobj = JsonConvert.DeserializeObject<JArray>(troubleReport.UploadFiles);
                        foreach (var obj in jobj)
                        {
                            foreach (var item in obj["ids"].ToString().Split(','))
                            {
                                objs.Add(new
                                {
                                    ePlanID = troubleReport.ID,
                                    fileID = Convert.ToInt32(item),
                                    type = Convert.ToInt32(obj["type"]),
                                    systemResource = (int)SystemResource.TroubleReport
                                });
                            }
                        }
                        sql = "insert into upload_file_relation values (0,@ePlanID,@fileID,@type,@systemResource)";
                        ret = await c.ExecuteAsync(sql, objs, trans);
                    }
                    sql = "insert into trouble_history " +
                    " values (0,@Trouble,@OrgTop,@Operation,@Content,@CreatedBy,@CreatedTime)";
                    await c.QueryFirstOrDefaultAsync<int>(sql, troubleHistory, trans);
                    trans.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            });
        }
        public async Task<TroubleReport> Save(TroubleReport troubleReport)
        {
            return await WithConnection(async c =>
            {
                string sql;
                IDbTransaction trans = c.BeginTransaction();
                try
                {
                    sql = " insert into trouble_report " +
                        " values (0,@Code,@HappeningTime,@ReportedTime,@Line,@StartLocation,@StartLocationBy,@StartLocationPath, " +
                        " @EndLocation,@EndLocationBy,@UrgentRepairOrder,@Level,@ReportedCompany,@ReportedCompanyPath,@ReportedBy," +
                        " @Desc,@Status,@LastOperation,@CreatedBy,@CreatedTime,@AcceptedTime,@UpdatedTime,@UpdatedBy); ";
                    sql += "SELECT LAST_INSERT_ID()";
                    int newid = await c.QueryFirstOrDefaultAsync<int>(sql, troubleReport, trans);
                    troubleReport.ID = newid;
                    List<TroubleEqp> eqps = JsonConvert.DeserializeObject<List<TroubleEqp>>(troubleReport.Eqps);
                    foreach (var item in eqps)
                    {
                        item.Trouble = newid;
                    }
                    sql = "insert into trouble_eqp values (0,@Trouble,@Org,@Eqp,null,null,null,null)";
                    int ret = await c.ExecuteAsync(sql, eqps, trans);
                    if (!string.IsNullOrWhiteSpace(troubleReport.UploadFiles))
                    {
                        List<object> objs = new List<object>();
                        JArray jobj = JsonConvert.DeserializeObject<JArray>(troubleReport.UploadFiles);
                        foreach (var obj in jobj)
                        {
                            foreach (var item in obj["ids"].ToString().Split(','))
                            {
                                objs.Add(new
                                {
                                    ePlanID = newid,
                                    fileID = Convert.ToInt32(item),
                                    type = Convert.ToInt32(obj["type"]),
                                    systemResource = (int)SystemResource.TroubleReport
                                });
                            }
                        }
                        sql = "insert into upload_file_relation values (0,@ePlanID,@fileID,@type,@systemResource)";
                        ret = await c.ExecuteAsync(sql, objs, trans);
                    }

                    sql = " insert into trouble_deal " +
                        " values (0,@Trouble,@OrgTop,@DealBy,@ArrivedTime,@FinishedTime,@Process,@SparePartsReplace, " +
                        " @RepairEvaluation,@RepairReason,@IsSure,@UnpassReason,@SureBy,@SureTime," +
                        " @CreatedBy,@CreatedTime,@UpdateBy,@UpdateTime)";
                    //以后需要改成子状态，显示的时候也多个子状态一起显示
                    //TroubleDeal troubleDeal = new TroubleDeal();
                    //foreach (var item in eqps.Select(a=>a.Org).Distinct())
                    //{
                    //    troubleDeal.Trouble = newid;
                    //    troubleDeal.OrgTop = item;

                    //}
                    //await c.QueryFirstOrDefaultAsync<int>(sql, troubleDeal);

                    sql = "insert into trouble_history " +
                    " values (0,@trouble,@OrgTop,@operation,@content,@user,@time)";
                    object myObj = new
                    {
                        trouble = newid,
                        OrgTop=0,
                        operation = TroubleOperation.NewTrouble,
                        content = troubleReport.Code,
                        user = troubleReport.CreatedBy,
                        time=troubleReport.CreatedTime
                    };
                    ret = await c.ExecuteAsync(sql, myObj, trans);
                    trans.Commit();

                    return troubleReport;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            });
        }
        public async Task<List<TroubleReport>> ListAll()
        {
            return await WithConnection(async c =>
            {
                string sql = "select * from trouble_report tr ";
                var tmp = await c.QueryAsync<TroubleReport>(sql);
                if (tmp.Count() > 0)
                {
                    return tmp.ToList();
                }
                else
                {
                    return new List<TroubleReport>();
                }
            });
        }

        #endregion

        #region get 辅助查询
        public async Task<string> GetLineCodeByID(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = " select code from metro_line where id=@id";
                string ret = await c.QueryFirstOrDefaultAsync<string>(sql,new { id});
                return ret;
            });
        }
        public async Task<string> GetNodeCodeByID(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = " select attr_value from org_node_property_value " +
                " where node_id=@id and node_attr='Code'";
                string ret = await c.QueryFirstOrDefaultAsync<string>(sql, new { id });
                return ret;
            });
        }
        public async Task<string> GetLastNumByDate(DateTime dt)
        {
            return await WithConnection(async c =>
            {
                string sql = " select code from trouble_report " +
                " where date_format(reported_time,'%Y-%m-%d')=@dt " +
                "  ORDER BY RIGHT(code,3) DESC limit 1";
                string ret = await c.QueryFirstOrDefaultAsync<string>(sql, new { dt=dt.ToString("yyyy-MM-dd") });
                return ret;
            });
        }
        public async Task<int> GetOrgNodeByUser(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = " select org_node_id from org_user where user_id=@id and is_del=0";
                int ret = await c.QueryFirstOrDefaultAsync<int>(sql, new { id });
                return ret;
            });
        }

        #endregion

        #region 调度员/值班员接口 未接报、未处理(目前没有这一过程)、未修复、未完结、七十二小时外未修复的数量统计
        public async Task<int> GetNoByStatus(AttandenceStatus attandenceStatus)
        {
            return await WithConnection(async c =>
            {
                string sql = " select count(id) from trouble_report ";
                switch (attandenceStatus)
                {
                    case AttandenceStatus.UnReported:
                        sql += " where status="+(int)TroubleStatus.NewTrouble+
                        " and last_operation!="+ TroubleOperation.RepairReject;
                        break;
                    case AttandenceStatus.UnRepaired:
                        sql += " where status=" + (int)TroubleStatus.Processing;
                        break;
                    case AttandenceStatus.UnFinished:
                        sql += " where status=" + (int)TroubleStatus.Repaired;
                        break;
                    case AttandenceStatus.UnRepaired72:
                        sql += " where status=" + (int)TroubleStatus.Processing +
                        " and timestampdiff(MINUTE,accepted_time,now())>4320";
                        break;
                }
                int ret = await c.QueryFirstOrDefaultAsync<int>(sql);
                return ret;
            });
        }
        #endregion

        #region trouble_eqp
        public async Task<List<TroubleEqp>> ListEqpByTrouble(int trouble,int topOrg=0,int orgNode=0)
        {
            return await WithConnection(async c =>
            {
                string sql = " select a.*,ot.name,e.eqp_name,e.team_path from trouble_eqp a " +
                " left join org_tree ot on ot.id=a.org " +
                " left join equipment e on e.id =a.eqp " +
                " where a.trouble="+ trouble;
                if (topOrg>0)
                {
                    sql += " and a.org="+topOrg;
                }
                if (orgNode > 0)
                {
                    sql += " and a.org_node="+orgNode;
                }
                var ret = await c.QueryAsync<TroubleEqp>(sql);
                if (ret.Count()>0)
                {
                    return ret.ToList();
                }
                else
                {
                    return new List<TroubleEqp>();
                }
            });
        }
        public async Task<int> UpdateTroubleEqp(List<TroubleEqp> troubleEqp)
        {
            return await WithConnection(async c =>
            {
                string sql = "Update trouble_eqp " +
                    " set org_node=@OrgNode,org_path=@OrgPath,assigned_by=@AssignedBy,assigned_time=@AssignedTime" +
                    " where id=@ID";
                return await c.ExecuteAsync(sql, troubleEqp);
            });
        }
        /// <summary>
        /// 故障报给了哪几个单位
        /// </summary>
        /// <param name="trouble"></param>
        /// <returns></returns>
        public async Task<List<int>> ListOrgTopByTrouble(int trouble)
        {
            return await WithConnection(async c =>
            {
                string sql = " select distinct a.org from trouble_eqp a " +
                " where a.trouble=" + trouble;
                var ret = await c.QueryAsync<int>(sql);
                if (ret.Count() > 0)
                {
                    return ret.ToList();
                }
                else
                {
                    return new List<int>();
                }
            });
        }
        /// <summary>
        /// 故障接报的这些单位最后的操作记录
        /// </summary>
        /// <param name="trouble"></param>
        /// <returns></returns>
        public async Task<List<TroubleHistory>> ListOperationByTroubles(IEnumerable<int> trouble)
        {
            return await WithConnection(async c =>
            {
                string sql = " select operation,org_top from trouble_history where org_top IN " +
                "(select MAX(org_top) from trouble_history where org_top in @trouble " +
                " GROUP BY org_top)";
                var ret = await c.QueryAsync<TroubleHistory>(sql,new { trouble});
                if (ret.Count() > 0)
                {
                    return ret.ToList();
                }
                else
                {
                    return new List<TroubleHistory>();
                }
            });
        }

        #endregion

        #region history
        public async Task<int> SaveHistory(TroubleHistory troubleHistory)
        {
            return await WithConnection(async c =>
            {
                string sql = "insert into trouble_history " +
                    " values (0,@Trouble,@OrgTop,@Operation,@Content,@CreatedBy,@CreatedTime)";
                return await c.ExecuteAsync(sql, troubleHistory);
            });
        }

        public async Task<int> SaveHistory(List<TroubleHistory> troubleHistory)
        {
            return await WithConnection(async c =>
            {
                string sql = "insert into trouble_history " +
                    " values (0,@Trouble,@OrgTop,@Operation,@Content,@CreatedBy,@CreatedTime)";
                return await c.ExecuteAsync(sql, troubleHistory);
            });
        }
        public async Task<List<TroubleHistory>> ListHistoryByTrouble(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT th.*,tr.code,dt.name,u.user_name,ot.name as otname from trouble_history th " +
                    " LEFT JOIN trouble_report tr on tr.id=th.trouble " +
                    " LEFT JOIN dictionary_tree dt on dt.id=th.operation " +
                    " LEFT JOIN org_tree ot on ot.id=th.org_top " +
                    " LEFT JOIN user u on u.id = th.created_by where th.trouble=@id";
                var tmp= await c.QueryAsync<TroubleHistory>(sql, new { id });
                if (tmp.Count()>0)
                {
                    return tmp.ToList();
                }
                else
                {
                    return new List<TroubleHistory>();
                }
            });
        }
        #endregion

        #region trouble_deal
        public async Task<TroubleDeal> SaveDeal(TroubleDeal troubleDeal)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into trouble_deal " +
                        " values (0,@Trouble,@OrgTop,@DealBy,@ArrivedTime,@FinishedTime,@Process,@SparePartsReplace, " +
                        " @RepairEvaluation,@RepairReason,@IsSure,@UnpassReason,@SureBy,@SureTime," +
                        " @CreatedBy,@CreatedTime,@UpdateBy,@UpdateTime); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, troubleDeal);
                troubleDeal.ID = newid;
                return troubleDeal;
            });
        }

        public async Task<int> UpdateDeal(TroubleDeal troubleDeal)
        {
            return await WithConnection(async c =>
            {
                string sql = " update trouble_deal " +
                        " set org_top=@OrgTop,deal_by=@DealBy,arrived_time=@ArrivedTime," +
                        " finished_time=@FinishedTime,process=@Process,spareparts_replace=@SparePartsReplace, " +
                        " repair_evaluation=@RepairEvaluation,repair_reason=@RepairReason," +
                        " update_by=@UpdateBy,update_time=@UpdateTime ";
                return await c.ExecuteAsync(sql, troubleDeal);
            });
        }

        public async Task<int> UpdateDealResult(int id, int userID, TroubleStatus status, TroubleOperation operation, string unpassReason = "")
        {
            return await WithConnection(async c =>
            {
                string sql = " update trouble_deal set ";
                if (status == TroubleStatus.PendingApproval && operation == TroubleOperation.Unpass)
                {
                    sql += "is_sure=2,sure_by=@userID,sure_time=@time,unpass_reason=@reason ";
                }
                else if (status == TroubleStatus.Finished && operation == TroubleOperation.Pass)
                {
                    sql += "is_sure=1,sure_by=@userID,sure_time=@time ";
                }
                sql += " where id=@id";
                return await c.ExecuteAsync(sql,
                    new { id, userID, time = DateTime.Now, status, operation,reason=unpassReason });
            });
        }


        public async Task<List<TroubleDeal>> ListDealByTrouble(int trouble, int topOrg = 0)
        {
            return await WithConnection(async c =>
            {
                string sql = " select DISTINCT a.*,ot.name,u1.user_name,u2.user_name as dname,u3.user_name as uname, " +
                " u4.user_name as sname from trouble_deal a " +
                " left join org_tree ot on ot.id=a.org_top "+
                " left join user u1 on u1.id=a.created_by " +
                " left join user u2 on u2.id=a.deal_by " +
                " left join user u3 on u3.id=a.update_by " +
                " left join user u4 on u4.id=a.sure_by " +
                " where a.trouble=" + trouble;
                if (topOrg > 0)
                {
                    sql += " and a.org_top=" + topOrg;
                }
                var ret = await c.QueryAsync<TroubleDeal>(sql);
                if (ret.Count() > 0)
                {
                    return ret.ToList();
                }
                else
                {
                    return new List<TroubleDeal>();
                }
            });
        }

        #endregion
    }
}         
