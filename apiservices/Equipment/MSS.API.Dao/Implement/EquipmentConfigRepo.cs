﻿using System;
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
    //目前为了设备能选中，只用了getall，其余需要重新修改代码
    public class EquipmentConfigRepo : BaseRepo, IEquipmentConfigRepo<EquipmentConfig>
    {
        public EquipmentConfigRepo(DapperOptions options) : base(options) { }

        public async Task<EquipmentConfig> Save(EquipmentConfig obj)
        {
            return await WithConnection(async c =>
            {
                string sql = " INSERT INTO equipment_config " +
                    " values (0,@Reminder,@BeforeDead,@BeforeMaintainMiddle,@BeforeMaintainBig,@TextTemplate,@Published, " +
                    " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, obj);
                obj.ID = newid;
                return obj;
            });
        }

        public async Task<int> Update(EquipmentConfig obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" UPDATE equipment_config " +
                    " set reminder=@Reminder,before_dead=@BeforeDead,before_maintain_middle=@BeforeMaintainMiddle,before_maintain_big=@BeforeMaintainBig,text_template=@TextTemplate, " +
                    " published=@Published,updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", obj);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids,int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update equipment_config set is_del=" + (int)IsDeleted.yes+
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }

        

        public async Task<EquipmentConfig> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<EquipmentConfig>(
                    "SELECT * FROM equipment_config WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<List<EquipmentConfig>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<EquipmentConfig>(
                    "SELECT * FROM equipment_config WHERE is_del = 0 ")).ToList();
                return result;
            });
        }
    }
}
