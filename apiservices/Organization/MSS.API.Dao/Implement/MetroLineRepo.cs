using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using Dapper.FastCrud;
using MSS.API.Model.DTO;
using MSS.API.Dao.Common;

namespace MSS.API.Dao.Implement
{
    public class MetroLineRepo : BaseRepo, IMetroLineRepo<MetroLine>
    {
        public MetroLineRepo(DapperOptions options) : base(options) { }

        public async Task<MetroLine> SaveLine(MetroLine line)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO metro_line (code,line_name, description, created_time, created_by, updated_time, updated_by, is_del)"
                            + " Values (@code,@LineName, @Description, @CreatedTime, @CreatedBy, @UpdatedTime, @UpdatedBy, @IsDel);";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, line);
                line.ID = newid;
                return line;
            });
        }

        public async Task<bool> CheckExist(MetroLine line)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM metro_line WHERE (line_name=@LineName or code=@Code) and is_del!=1 and ID != @ID";

                MetroLine exist = await c.QueryFirstOrDefaultAsync<MetroLine>(sql, line);
                return exist != null ? true : false;
            });
        }

        public async Task<MetroLine> UpdateLine(MetroLine line)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE metro_line SET code=@Code, line_name = @LineName, description = @Description,"
                            + " updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE ID = @ID;";
                await c.ExecuteAsync(sql, line);
                return line;
            });
        }

        public async Task<int> DeleteLine(List<MetroLine> line)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE metro_line SET is_del = 1,"
                            + " updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE ID = @ID;";
                int affectedRows = await c.ExecuteAsync(sql, line);
                return affectedRows;
            });
        }

        public async Task<bool> CheckUsing(List<int> ids)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM tb_config_bigarea WHERE MetroLineID in @ids";

                MetroLine exist = await c.QueryFirstOrDefaultAsync<MetroLine>(sql, new { ids});
                return exist != null ? true : false;
            });
        }

        public async Task<MetroLine> GetMetroLineByID(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM metro_line WHERE is_del!=1 and ID = @ID";

                MetroLine line = await c.QueryFirstOrDefaultAsync<MetroLine>(sql,
                new
                {
                    ID = id
                });
                return line;
            });
        }

        // 分页查询
        public async Task<PageData<MetroLine>> ListMetroLineByPage(MetroLineParam param)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlCount = new StringBuilder();

                sql.Append("SELECT a.*, b.user_name");
                sqlCount.Append("SELECT COUNT(*)");

                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" FROM metro_line a");
                whereSql.Append(" JOIN user b ON a.updated_by = b.id where a.is_del != 1");

                if (!string.IsNullOrEmpty(param.LineName))
                {
                    whereSql.Append(" AND a.line_name like '%" + param.LineName + "%'");
                }

                sql.Append(whereSql)
                   .Append(" order by a." + param.sort + " " + param.order)
                   .Append(" limit " + (param.page - 1) * param.rows + "," + param.rows);
                sqlCount.Append(whereSql);
                var data = await c.QueryAsync<MetroLine>(sql.ToString());
                int total = await c.QueryFirstOrDefaultAsync<int>(sqlCount.ToString());

                PageData<MetroLine> ret = new PageData<MetroLine>();
                ret.rows = data.ToList();
                ret.total = total;

                return ret;
            });
        }

        public async Task<List<MetroLine>> ListAllMetroLine()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * from metro_line"
                    + " WHERE is_del != 1";
                List<MetroLine> data = (await c.QueryAsync<MetroLine>(sql)).ToList();
                return data;
            });
        }

    }
}
