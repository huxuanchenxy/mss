using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Dapper.FastCrud;

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
                var result = await c.QueryFirstOrDefaultAsync<TroubleReport>(
                    "select tr.*,e.eqp_name,e.eqp_code,dt.`name` as typeName,dt1.name," +
                    "u.user_name,u1.user_name as cname,u2.user_name as aname " +
                    "from trouble_report tr " +
                    "LEFT JOIN equipment e on e.id=tr.eqp " +
                    "LEFT JOIN dictionary_tree dt on dt.ID=tr.type " +
                    "LEFT JOIN dictionary_tree dt1 on dt1.ID=tr.`status` " +
                    "LEFT JOIN `user` u on u.id=tr.reported_by " +
                    "LEFT JOIN `user` u1 on u1.id=tr.created_by " +
                    "LEFT JOIN `user` u2 on u2.id=tr.accepted_by " +
                    "WHERE tr.id = @id", new { id = id });
                return result;
            });
        }
        public async Task<TroubleReportView> ListPage(TroubleReportParm parm)
        {
            return await WithConnection(async c =>
            {
                TroubleReportView ret = new TroubleReportView();
                StringBuilder sql = new StringBuilder();
                sql.Append("select tr.*,e.eqp_name,e.eqp_code,dt.`name` as typeName,dt1.name,")
                .Append("u.user_name,u1.user_name as cname,dt2.name as lname,ml.line_name, ")
                .Append("ot.name as rcname,ot1.name as acname,u.mobile ")
                .Append("from trouble_report tr ")
                .Append("LEFT JOIN metro_line ml on ml.id=tr.line ")
                .Append("LEFT JOIN equipment e on e.id=tr.eqp ")
                .Append("LEFT JOIN dictionary_tree dt on dt.ID=tr.type ")
                .Append("LEFT JOIN dictionary_tree dt1 on dt1.ID=tr.`status` ")
                .Append("LEFT JOIN dictionary_tree dt2 on dt2.ID=tr.level ")
                .Append("LEFT JOIN `user` u on u.id=tr.reported_by ")
                .Append("LEFT JOIN `user` u1 on u1.id=tr.created_by ")
                .Append("LEFT JOIN org_tree ot on ot.id=tr.reported_company ")
                .Append("LEFT JOIN org_tree ot1 on ot1.id=tr.accepted_company where 1=1 ");
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
                    sql.Append("SELECT count(*) FROM construction_plan_import_common c ")
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
    }
}         
