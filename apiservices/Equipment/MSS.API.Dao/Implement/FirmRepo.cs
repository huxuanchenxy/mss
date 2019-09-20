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
    public class FirmRepo : BaseRepo, IFirmRepo<Firm>
    {
        public FirmRepo(DapperOptions options) : base(options) { }

        public async Task<Firm> Save(Firm firm)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into firm " +
                    " values (0,@Name,@Type,@Mobile,@Contact,@Address, " +
                    " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, firm);
                firm.ID = newid;
                return firm;
            });
        }

        public async Task<int> Update(Firm firm)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update firm " +
                    " set name=@Name,type=@Type,mobile=@Mobile,contact=@Contact, " +
                    " address=@Address,updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", firm);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids,int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update firm set is_del=" + (int)IsDeleted.yes+
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }

        public async Task<object> GetPageByParm(FirmQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,dt.name as tname,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM firm a ")
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
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List< Firm > ets= (await c.QueryAsync<Firm>(sql.ToString())).ToList();
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from firm a " + whereSql.ToString());
                return new {rows=ets,total=total };
            });
        }

        public async Task<Firm> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<Firm>(
                    "SELECT * FROM firm WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<List<Firm>> ListByType(int? type)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM firm where is_del=" + (int)IsDeleted.no;
                if (type!=null)
                {
                    sql += " and type="+type;
                }
                var result = (await c.QueryAsync<Firm>(
                    sql)).ToList();
                return result;
            });
        }

        public async Task<List<Firm>> ListAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<Firm>(
                    "SELECT * FROM firm where is_del=" + (int)IsDeleted.no)).ToList();
                return result;
            });
        }
    }
}
