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
    public class UploadFileRepo : BaseRepo, IUploadFileRepo<UploadFile>
    {
        public UploadFileRepo(DapperOptions options) : base(options) { }

        public async Task<UploadFile> Save(UploadFile uploadFile)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into upload_file " +
                    " values (0,@FileName,@FilePath); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, uploadFile);
                uploadFile.ID = newid;
                return uploadFile;
            });
        }

        public async Task<int> Delete(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Delete from upload_file WHERE id=@id ", new { id = id});
                return result;
            });
        }

        public async Task<UploadFile> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<UploadFile>(
                    "SELECT * FROM upload_file WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<List<UploadFile>> ListByIDs(string ids)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<UploadFile>(
                    "SELECT uf.*,ufr.type,dt.name as TypeName,dt1.name as SystemResourceName,ufr.system_resource FROM upload_file uf " +
                    "left join upload_file_relation ufr on ufr.file_id=uf.id " +
                    "left join dictionary_tree dt on ufr.type=dt.id " +
                    "left join dictionary_tree dt1 on ufr.system_resource=dt1.id " +
                    "WHERE uf.id in @ids", new { ids = ids.Split(',') });
                if (result!=null && result.Count()>0)
                {
                    return result.ToList();
                }
                else
                {
                    return null;
                }
            });
        }
        public async Task<List<UploadFile>> ListByEntity(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<UploadFile>(
                    "SELECT uf.*,ufr.type,dt.name as TypeName,dt1.name as SystemResourceName,ufr.system_resource FROM upload_file uf " +
                    "left join upload_file_relation ufr on ufr.file_id=uf.id " +
                    "left join dictionary_tree dt on ufr.type=dt.id " +
                    "left join dictionary_tree dt1 on ufr.system_resource=dt1.id " +
                    "WHERE ufr.entity_id=@id", new { id = id });
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

        public async Task<List<UploadFile>> ListAll()
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<UploadFile>(
                    "SELECT * FROM upload_file");
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
    }
}
