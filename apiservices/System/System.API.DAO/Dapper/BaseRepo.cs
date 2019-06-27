using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.API.DAO.Dapper;

namespace System.API.DAO.Dapper
{
    public abstract class BaseRepo
    {
        private readonly string _ConnectionString;

        protected BaseRepo(DapperOptions options)
        {
            _ConnectionString = DapperOptions.ConnectionString;
        }

        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                using (var connection = new MySqlConnection(_ConnectionString))
                {
                    await connection.OpenAsync();
                    return await getData(connection);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(ex.ToString());
            }
            catch (MySqlException ex)
            {

                throw new Exception(ex.ToString());
            }
        }
    }
}
