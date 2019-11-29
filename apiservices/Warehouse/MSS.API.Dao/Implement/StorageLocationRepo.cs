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
    public class StorageLocationRepo : BaseRepo, IStorageLocationRepo<StorageLocation>
    {
        public StorageLocationRepo(DapperOptions options) : base(options) { }

        public async Task<StorageLocation> Save(StorageLocation storageLocation)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into storage_location " +
                    " values (0,@Name,@Warehouse,@Row,@Column,@Des, " +
                    " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, storageLocation);
                storageLocation.ID = newid;
                return storageLocation;
            });
        }

        public async Task<int> Update(StorageLocation storageLocation)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update storage_location " +
                    " set `column`=@Column,name=@Name,warehouse=@Warehouse,row=@Row,des=@Des, " +
                    " updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", storageLocation);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids,int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update storage_location set is_del=" + (int)IsDeleted.yes+
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }

        public async Task<object> GetPageByParm(StorageLocationQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,w.name as wname,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM storage_location a ")
                .Append(" left join warehouse w on a.warehouse=w.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no);
                if (!string.IsNullOrWhiteSpace(parm.Name))
                {
                    whereSql.Append(" and a.name like '%" + parm.Name + "%' ");
                }
                if (parm.Warehouse!=null)
                {
                    whereSql.Append(" and a.warehouse=" + parm.Warehouse);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List< StorageLocation > ets= (await c.QueryAsync<StorageLocation>(sql.ToString())).ToList();
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from storage_location a " + whereSql.ToString());
                return new {rows=ets,total=total };
            });
        }

        public async Task<StorageLocation> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,w.name as wname,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM storage_location a ")
                .Append(" left join warehouse w on a.warehouse=w.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ")
                .Append(" WHERE a.id = @id ");
                var result = await c.QueryFirstOrDefaultAsync<StorageLocation>(
                    sql.ToString(), new { id = id });
                return result;
            });
        }

        public async Task<List<StorageLocation>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<StorageLocation>(
                    "SELECT sl.*,w.name as wname FROM storage_location sl " +
                    "left join warehouse w on sl.warehouse=w.id " +
                    "where sl.is_del=" + (int)IsDeleted.no)).ToList();
                return result;
            });
        }

        public async Task<List<StorageLocation>> ListByWarehouse(int warehouse)
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<StorageLocation>(
                    "SELECT * FROM storage_location where warehouse=@warehouse and is_del=@isDel" +
                    " order by name",
                    new { isDel = (int)IsDeleted.no,warehouse})).ToList();
                return result;
            });
        }

        public async Task<bool> HasByWarehouse(string[] warehouse)
        {
            return await WithConnection(async c =>
            {
                int result = await c.QueryFirstOrDefaultAsync<int>(
                    "SELECT count(*) FROM storage_location where warehouse in @warehouse and is_del=" + (int)IsDeleted.no,new { warehouse });
                return result>0;
            });
        }

    }
}
