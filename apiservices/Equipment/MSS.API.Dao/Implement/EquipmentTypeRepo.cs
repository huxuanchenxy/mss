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
    public class EquipmentTypeRepo : BaseRepo, IEquipmentTypeRepo<EquipmentType>
    {
        public EquipmentTypeRepo(DapperOptions options) : base(options) { }

        public async Task<EquipmentType> Save(EquipmentType eqpType)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into equipment_type " +
                    " values (0,@TName,@Model,@Desc,@PWorking,@PDrawings, " +
                    " @PInstall,@PUser,@PRegulations, " +
                    " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.ExecuteAsync(sql, eqpType);
                eqpType.ID = newid;
                return eqpType;
            });
        }

        public async Task<int> Update(EquipmentType eqpType)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update equipment_type " +
                    " set type_name=@TName,model=@Model,description=@Desc,path_working_instruction=@PWorking, " +
                    " path_technical_drawings=@PDrawings,path_installation_manual=@PInstall,path_user_guide=@PUser, " +
                    " path_regulations=@PRegulations,updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", eqpType);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids,int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update equipment_type set is_del=" + (int)IsDeleted.yes+
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }

        public async Task<object> GetPageByParm(EqpTypeQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM equipment_type a ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no);
                if (!string.IsNullOrWhiteSpace(parm.SearchName))
                {
                    whereSql.Append(" and a.type_name like '%" + parm.SearchName + "%' ");
                }
                if (!string.IsNullOrWhiteSpace(parm.SearchDesc))
                {
                    whereSql.Append(" and a.description like '%" + parm.SearchDesc + "%' ");
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List< EquipmentType > ets= (await c.QueryAsync<EquipmentType>(sql.ToString())).ToList();
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from equipment_type a " + whereSql.ToString());
                return new {rows=ets,total=total };
            });
        }

        public async Task<EquipmentType> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<EquipmentType>(
                    "SELECT * FROM equipment_type WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<List<EquipmentType>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<EquipmentType>(
                    "SELECT * FROM equipment_type")).ToList();
                return result;
            });
        }
    }
}
