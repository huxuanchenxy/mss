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
    public class EquipmentRepairHistoryRepo : BaseRepo, IEquipmentRepairHistoryRepo<EquipmentRepairHistory>
    {
        public EquipmentRepairHistoryRepo(DapperOptions options) : base(options) { }

        public async Task<EquipmentRepairHistory> Save(EquipmentRepairHistory equipmentRepairHistory)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into equipment_repair_history " +
                    " values (0,@Trouble,@Eqp,@EqpPath,@Desc, " +
                    " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, equipmentRepairHistory);
                equipmentRepairHistory.ID = newid;
                return equipmentRepairHistory;
            });
        }

        public async Task<int> Update(EquipmentRepairHistory equipmentRepairHistory)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update equipment_repair_history " +
                    " set trouble=@Trouble,eqp=@Eqp,eqp_path=@EqpPath,desc=@Desc, " +
                    " updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", equipmentRepairHistory);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" delete from equipment_repair_history " +
                " WHERE id in @ids ");
                return result;
            });
        }

        public async Task<object> GetPageByParm(EquipmentRepairHistoryQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,dt.name as tname,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM equipment_repair_history a ")
                .Append(" left join dictionary_tree dt on a.type=dt.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no);
                if (parm.Trouble!=null)
                {
                    whereSql.Append(" and a.trouble =" + parm.Trouble);
                }
                if (parm.Eqp != null)
                {
                    whereSql.Append(" and a.eqp=" + parm.Eqp);
                }
                if (!string.IsNullOrWhiteSpace(parm.Desc))
                {
                    whereSql.Append(" and a.desc like '%" + parm.Desc + "%' ");
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List<EquipmentRepairHistory> ets = (await c.QueryAsync<EquipmentRepairHistory>(sql.ToString())).ToList();
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from equipment_repair_history a " + whereSql.ToString());
                return new { rows = ets, total = total };
            });
        }

        public async Task<EquipmentRepairHistory> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<EquipmentRepairHistory>(
                    "SELECT * FROM equipment_repair_history WHERE id = @id", new { id = id });
                return result;
            });
        }
    }
}
