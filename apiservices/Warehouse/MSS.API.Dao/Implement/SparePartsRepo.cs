using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using MSS.API.Common;

namespace MSS.API.Dao.Implement
{
    public class SparePartsRepo : BaseRepo, ISparePartsRepo<SpareParts>
    {
        public SparePartsRepo(DapperOptions options) : base(options) { }

        public async Task<SpareParts> Save(SpareParts spareParts)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into spare_parts " +
                    " values (0,@Name,@Type,@Model,@Unit,@EqpType,@PlanPrice,@EnglishDes,@Remark, " +
                    " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, spareParts);
                spareParts.ID = newid;
                return spareParts;
            });
        }

        public async Task<int> Update(SpareParts spareParts)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update spare_parts " +
                    " set name=@Name,type=@Type,model=@Model,unit=@Unit,eqp_type=@EqpType, " +
                    " plan_price=@PlanPrice,english_des=@EnglishDes,remark=@Remark, " +
                    " updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", spareParts);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids,int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update spare_parts set is_del=" + (int)IsDeleted.yes+
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }

        public async Task<object> GetPageByParm(SparePartsQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,et.type_name,dt.name as tname,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM spare_parts a ")
                .Append(" left join equipment_type et on a.eqp_type=et.id ")
                .Append(" left join dictionary_tree dt on a.type=dt.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no);
                if (!string.IsNullOrWhiteSpace(parm.SearchName))
                {
                    whereSql.Append(" and a.name like '%" + parm.SearchName + "%' ");
                }
                if (parm.SearchType!=null)
                {
                    whereSql.Append(" and a.type=" + parm.SearchType);
                }
                if (parm.SearchEqpType != null)
                {
                    whereSql.Append(" and a.eqp_type=" + parm.SearchEqpType);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List< SpareParts > ets= (await c.QueryAsync<SpareParts>(sql.ToString())).ToList();
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from spare_parts a " + whereSql.ToString());
                return new {rows=ets,total=total };
            });
        }

        public async Task<SpareParts> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,et.type_name,dt.name as tname,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM spare_parts a ")
                .Append(" left join equipment_type et on a.eqp_type=et.id ")
                .Append(" left join dictionary_tree dt on a.type=dt.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ")
                .Append(" WHERE a.id = @id ");
                var result = await c.QueryFirstOrDefaultAsync<SpareParts>(
                    sql.ToString(), new { id = id });
                return result;
            });
        }

        public async Task<List<SpareParts>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<SpareParts>(
                    "SELECT * FROM spare_parts")).ToList();
                return result;
            });
        }
    }
}
