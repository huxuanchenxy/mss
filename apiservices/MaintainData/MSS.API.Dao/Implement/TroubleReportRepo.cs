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

namespace MSS.API.Dao.Implement
{
    public class TroubleReportRepo :BaseRepo, ITroubleReportRepo<TroubleReport>
    {
        public TroubleReportRepo(DapperOptions options) :base(options)
            {
            } 

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
                .Append("ot.name as rcname,u.mobile,u2.user_name as uname ")
                .Append("from trouble_report tr ")
                .Append("LEFT JOIN metro_line ml on ml.id=tr.line ")
                //.Append("LEFT JOIN equipment e on e.id=tr.eqp ")
                //.Append("LEFT JOIN dictionary_tree dt on dt.ID=tr.type ")
                .Append("LEFT JOIN dictionary_tree dt1 on dt1.ID=tr.`status` ")
                .Append("LEFT JOIN dictionary_tree dt2 on dt2.ID=tr.level ")
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
                sql.Append(whereSql).Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows); ;
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

        public async Task<int> UpdateStatus(string[] ids, int userID, TroubleStatus status)
        {
            return await WithConnection(async c =>
            {
                string sql = " update trouble_report " +
                        " set status=@status,updated_by=@userID,updated_time=@time where id in @ids ";
                int ret = await c.ExecuteAsync(sql, 
                    new { ids, userID, time = DateTime.Now,status});
                return ret;
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
                        " @Desc,@Status,@CreatedBy,@CreatedTime,@AcceptedTime,@UpdatedTime,@UpdatedBy); ";
                    sql += "SELECT LAST_INSERT_ID()";
                    int newid = await c.QueryFirstOrDefaultAsync<int>(sql, troubleReport, trans);
                    troubleReport.ID = newid;
                    List<TroubleEqp> eqps = JsonConvert.DeserializeObject<List<TroubleEqp>>(troubleReport.Eqps);
                    foreach (var item in eqps)
                    {
                        item.Trouble = newid;
                    }
                    sql = "insert into trouble_eqp values (0,@Trouble,@Org,@Eqp)";
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
        public async Task<List<TroubleEqp>> ListEqpByTrouble(int trouble)
        {
            return await WithConnection(async c =>
            {
                string sql = " select a.*,ot.name,e.eqp_name from trouble_eqp a " +
                " left join org_tree ot on ot.id=a.org " +
                " left join equipment e on e.id =a.eqp " +
                " where trouble=@trouble";
                var ret = await c.QueryAsync<TroubleEqp>(sql, new { trouble });
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

        public async Task<int> Update(TroubleReport troubleReport)
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
                        " reported_by=@ReportedBy,description=@Desc," +
                        " updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", troubleReport, trans);
                    List<TroubleEqp> eqps = JsonConvert.DeserializeObject<List<TroubleEqp>>(troubleReport.Eqps);
                    sql = "delete from trouble_eqp where trouble=@id";
                    int ret = await c.ExecuteAsync(sql, new { id = troubleReport.ID}, trans);
                    sql = "insert into trouble_eqp values (0,@Trouble,@Org,@Eqp)";
                    ret = await c.ExecuteAsync(sql, eqps, trans);
                    if (!string.IsNullOrWhiteSpace(troubleReport.UploadFiles))
                    {
                        sql = "delete from upload_file_relation where entity_id=@id and system_resource=@sr";
                        ret = await c.ExecuteAsync(sql, new { id = troubleReport.ID,sr=SystemResource.TroubleReport }, trans);
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
    }
}         
