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
    public class WarehouseRepo : BaseRepo, IWarehouseRepo<Warehouse>
    {
        public WarehouseRepo(DapperOptions options) : base(options) { }

        public async Task<Warehouse> Save(Warehouse warehouse)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into warehouse " +
                    " values (0,@Name,@Mobile,@Contact,@Address, " +
                    " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, warehouse);
                warehouse.ID = newid;
                return warehouse;
            });
        }

        public async Task<int> Update(Warehouse warehouse)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update warehouse " +
                    " set name=@Name,mobile=@Mobile,contact=@Contact, " +
                    " address=@Address,updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", warehouse);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids,int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update warehouse set is_del=" + (int)IsDeleted.yes+
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }

        public async Task<object> GetPageByParm(WarehouseQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM warehouse a ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no);
                if (!string.IsNullOrWhiteSpace(parm.SearchName))
                {
                    whereSql.Append(" and a.name like '%" + parm.SearchName + "%' ");
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List< Warehouse > ets= (await c.QueryAsync<Warehouse>(sql.ToString())).ToList();
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from warehouse a " + whereSql.ToString());
                return new {rows=ets,total=total };
            });
        }

        public async Task<Warehouse> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<Warehouse>(
                    "SELECT * FROM warehouse WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<List<Warehouse>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<Warehouse>(
                    "SELECT * FROM warehouse where is_del=" + (int)IsDeleted.no)).ToList();
                return result;
            });
        }
    }
}
