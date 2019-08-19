using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using MSS.API.Model.DTO;

namespace MSS.API.Dao.Implement
{
    public class GlobalDataManagerRepo : BaseRepo, IGlobalDataManagerRepo
    {
        public GlobalDataManagerRepo(DapperOptions options) : base(options) { }

        public async Task<List<StatisticsDimension>> ListEqpDimension()
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT distinct a.id as eqp_id, a.eqp_name, a.eqp_type as eqp_type_id,")
                .Append(" et.type_name as eqp_type_name, a.supplier as supplier_id,")
                .Append(" f1.name as supplier_name, a.manufacturer as manufacturer_id, ")
                .Append(" f2.name as manufacturer_name, a.sub_system as sub_system_id, d.name as sub_system_name,")
                .Append(" a.team as team_id, ot.name as team_name, a.team_path, a.top_org as top_org_id,")
                .Append(" a.location, a.location_by, a.location_path")
                .Append(" FROM equipment a ")
                .Append(" left join equipment_type et on et.id=a.eqp_type ")
                .Append(" left join org_tree ot on ot.id=a.team ")
                .Append(" left join firm f1 on f1.id=a.Supplier ")
                .Append(" left join firm f2 on f2.id=a.Manufacturer ")
                .Append(" left join dictionary_tree d on a.sub_system=d.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" where a.is_del=0");
                
                sql.Append(whereSql);
                List<StatisticsDimension> list = (await c.QueryAsync<StatisticsDimension>(sql.ToString())).ToList();

                return list;
            });
        }

        public async Task<List<AllArea>> GetAllArea()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sub_code_name as AreaName,sub_code as id,0 as TableName " +
                "from dictionary where code='metro_area' UNION " +
                "select AreaName,id,1 as TableName from tb_config_bigarea UNION " +
                "select AreaName,id,2 as TableName from tb_config_midarea";
                var result = await c.QueryAsync<AllArea>(sql);
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return null;
                }
            });
        }

        public async Task<int> SaveStatisticsDimension(List<StatisticsDimension> data)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO statistics_dimension (eqp_id, eqp_name, eqp_type_id,")
                    .Append(" eqp_type_name, supplier_id, supplier_name, manufacturer_id,")
                    .Append(" manufacturer_name, sub_system_id, sub_system_name, location_level1,")
                    .Append(" location_level1_name, location_level2, location_level2_name,location_level3,")
                    .Append(" location_level3_name, top_org_id, top_org_name, team_id, team_name, team_path)")
                    .Append(" Values (@EqpID, @EqpName, @EqpTypeID, @EqpTypeName, @SupplierID,")
                    .Append(" @SupplierName, @ManufacturerID, @ManufacturerName, @SubSystemID,")
                    .Append(" @SubSystemName, @LocationLevel1, @LocationLevel1Name, @LocationLevel2,")
                    .Append(" @LocationLevel2Name, @LocationLevel3, @LocationLevel3Name, @TopOrgID,")
                    .Append(" @TopOrgName, @TeamID, @TeamName, @TeamPath);");
                
                int affectedRows = await c.ExecuteAsync(sql.ToString(), data);
                return affectedRows;
            });
        }

        public async Task<List<StatisticsDimension>> ListStatisticsDimension()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM statistics_dimension";
                var list = await c.QueryAsync<StatisticsDimension>(sql);

                return list.ToList();
            });
        }
    }
}
