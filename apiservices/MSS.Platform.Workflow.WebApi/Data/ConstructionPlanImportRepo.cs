﻿using Dapper;
using MSS.API.Common;
using MSS.Platform.Workflow.WebApi.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Coded By admin 2019/9/27 11:18:53
namespace MSS.Platform.Workflow.WebApi.Data
{
    public interface IConstructionPlanImportRepo<T>
    {
        Task<object> ListPage(ConstructionPlanImportParm parm);
        Task<object> ListPageCommon(ConstructionPlanCommonParm parm);
        int BulkLoad(DataTable table);
        Task<int> Save(ConstructionPlanImportCommon obj);
        Task<int> UpdateCommonStatus(int id, int userID);
        Task<int> Delete(List<int> ids, string tableName);
        Task<int> Delete(List<int> ids);
        Task<List<ConstructionPlanImportCommon>> ListByYearAndCompany(int year, int company);
        Task<ConstructionPlanImportCommon> GetByID(int id);
        Task<List<ConstructionPlanYear>> ListYearByQuery(int id);
        Task<List<ConstructionPlanMonth>> ListMonthByQuery(int id);
        Task<List<QueryItem>> ListAllLines();
        Task<List<QueryItem>> ListAllOrgByType(OrgType orgType);
        Task<List<QueryItem>> ListAllEqpTypes();
        Task<List<QueryItem>> ListAllLocations();
        Task<List<QueryItem>> ListDictionarysByParent(int parent);
    }

    public class ConstructionPlanImportRepo : BaseRepo, IConstructionPlanImportRepo<ConstructionPlanYear>
    {
        private readonly MySqlConnection con;
        public ConstructionPlanImportRepo(DapperOptions options) : base(options)
        {
            con = new MySqlConnection(options.ConnectionString);
        }
        public async Task<object> ListPage(ConstructionPlanImportParm parm)
        {
            return await WithConnection(async c =>
            {
                object ret=null;
                IEnumerable<object> retTmp;
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT id FROM construction_plan_import_common ")
                .Append(" where year="+ parm.SearchYear)
                .Append(" and company='"+parm.SearchCompany+"'");
                var ids= await c.QueryAsync<int>(sql.ToString());
                if (ids.Count() <= 0) return ret;
                string tableName = parm.IsYear ? "construction_plan_year" : "construction_plan_month";
                sql.Clear();
                sql.Append("SELECT * FROM " + tableName)
                .Append(" where query in @ids ")
                //.Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                if (parm.IsYear)
                {
                    retTmp = await c.QueryAsync<ConstructionPlanYear>(sql.ToString(),new { ids });
                }
                else
                {
                    retTmp = await c.QueryAsync<ConstructionPlanMonth>(sql.ToString(), new { ids });
                }
                if (retTmp.Count() > 0)
                {
                    sql.Clear();
                    sql.Append("select count(*) FROM "+tableName+" where query in @ids ");
                    int total = await c.QueryFirstOrDefaultAsync<int>(
                        sql.ToString(), new { ids });
                    ret = new { rows = retTmp.ToList(), total };
                }
                return ret;
            });
        }
        public async Task<object> ListPageCommon(ConstructionPlanCommonParm parm)
        {
            return await WithConnection(async c =>
            {
                object ret = null;
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT c.*,o.name,u.user_name FROM construction_plan_import_common c ")
                .Append("left join org_tree o on c.company=o.id ")
                .Append("left join user u on c.created_by=u.id ")
                .Append(" where 1=1");
                StringBuilder whereSql = new StringBuilder();
                if (parm.Year!=null)
                {
                    whereSql.Append(" and c.year="+parm.Year);
                }
                if (parm.Line != null)
                {
                    whereSql.Append(" and c.line=" + parm.Line);
                }
                if (parm.Company!=null)
                {
                    whereSql.Append(" and c.company=" + parm.Company);
                }
                if (parm.Department != null)
                {
                    whereSql.Append(" and c.department=" + parm.Department);
                }
                sql.Append(whereSql).Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows); ;
                var tmp = await c.QueryAsync<ConstructionPlanImportCommon>(sql.ToString());
                if (tmp.Count() > 0)
                {
                    sql.Clear();
                    sql.Append("SELECT count(*) FROM construction_plan_import_common c ")
                        .Append(" where 1=1 ").Append(whereSql);
                    int total= await c.QueryFirstOrDefaultAsync<int>(sql.ToString());
                    ret = new { rows = tmp.ToList(), total };
                }
                else
                {
                    ret = new { rows = new List<ConstructionPlanImportCommon>(), total = 0 };
                }
                return ret;
            });
        }


        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="_mySqlConnection"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int BulkLoad(DataTable table)
        {
            ToCsv(table);
            var columns = table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList();
            MySqlBulkLoader bulk = new MySqlBulkLoader(con)
            {
                FieldTerminator = ",",
                FieldQuotationCharacter = '"',
                EscapeCharacter = '"',
                LineTerminator = "\r\n",
                FileName = @"../uploads/" + table.TableName + ".csv",
                NumberOfLinesToSkip = 0,
                TableName = table.TableName,
            };
            bulk.Columns.AddRange(columns);
            int ret= bulk.Load();
            //删除临时的csv文件
            File.Delete(@"\\10.89.36.63\uploads\"+ table.TableName + ".csv");
            return ret;
        }

        public async Task<int> Save(ConstructionPlanImportCommon obj)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into construction_plan_import_common " +
                        " values (0,@Year,@Department,@DepartmentName,@Line,@LineName, " +
                        " @Company,@IsCreatedMonth,@CreatedTime,@CreatedBy,@ImportTime,@ImportBy); ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, obj);
                return newid;
            });
        }
        public async Task<int> UpdateCommonStatus(int id,int userID)
        {
            return await WithConnection(async c =>
            {
                string sql = " update construction_plan_import_common " +
                        " set is_created_month =1,created_by=@userID,created_time=@time where id=@id ";
                int ret = await c.QueryFirstOrDefaultAsync<int>(sql,new { id,userID,time=DateTime.Now });
                return ret;
            });
        }

        public async Task<int> Delete(List<int> ids)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Delete from construction_plan_import_common WHERE id=@ids ", new { ids });
                return result;
            });
        }
        public async Task<int> Delete(List<int> ids,string tableName)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Delete from "+tableName+" WHERE query=@ids ", new { ids });
                return result;
            });
        }
        public async Task<List<ConstructionPlanImportCommon>> ListByYearAndCompany(int year,int company)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<ConstructionPlanImportCommon>(
                    " SELECT * FROM construction_plan_import_common " +
                    " where year=@year and company=@company", new { year,company });
                if (result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return new List<ConstructionPlanImportCommon>();
                }
            });
        }
        public async Task<ConstructionPlanImportCommon> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                return await c.QueryFirstOrDefaultAsync<ConstructionPlanImportCommon>(
                    " SELECT * FROM construction_plan_import_common " +
                    " where id=@id", new { id });
            });
        }
        public async Task<List<ConstructionPlanYear>> ListYearByQuery(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<ConstructionPlanYear>(
                    " SELECT * FROM construction_plan_year " +
                    " where query=@id", new { id });
                if (result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return new List<ConstructionPlanYear>();
                }
            });
        }

        public async Task<List<ConstructionPlanMonth>> ListMonthByQuery(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<ConstructionPlanMonth>(
                    " SELECT * FROM construction_plan_month " +
                    " where query=@id", new { id });
                if (result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return new List<ConstructionPlanMonth>();
                }
            });
        }
        public async Task<List<QueryItem>> ListAllLines()
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<QueryItem>(
                    "SELECT id,line_name as name FROM metro_line where is_del=@IsDel",new {IsDel=IsDeleted.no });
                if (result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return new List<QueryItem>();
                }
            });
        }

        public async Task<List<QueryItem>> ListAllOrgByType(OrgType orgType)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<QueryItem>(
                    "SELECT id,name FROM org_tree where node_type=@type and is_del=@IsDel",
                    new { type= orgType, IsDel = IsDeleted.no });
                if (result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return new List<QueryItem>();
                }
            });
        }

        public async Task<List<QueryItem>> ListAllEqpTypes()
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<QueryItem>(
                    "SELECT id,type_name as name FROM equipment_type where is_del=@IsDel", new { IsDel = IsDeleted.no });
                if (result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return new List<QueryItem>();
                }
            });
        }

        public async Task<List<QueryItem>> ListAllLocations()
        {
            return await WithConnection(async c =>
            {
                string sql = "select AreaName as name,id,1 as LocationBy from tb_config_bigarea UNION " +
                "select AreaName as name,id,2 as LocationBy from tb_config_midarea";
                var result = await c.QueryAsync<QueryItem>(sql);
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
        public async Task<List<QueryItem>> ListDictionarysByParent(int parent)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<QueryItem>(
                    "SELECT id,name FROM dictionary_tree where parent_id=@parent", new { parent });
                if (result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return new List<QueryItem>();
                }
            });
        }

        private void ToCsv(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }

            File.WriteAllText(@"\\10.89.36.63\uploads\" + table.TableName + ".csv", sb.ToString());
        }
    }
}



