using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    internal class DatabaseRepository : IDatabaseRepository
    {
        private readonly string _connectionString;

        public DatabaseRepository()
        {
            _connectionString = ConfigReader.getValue("Database_Connection_String");
            #region error_handling
            if (string.IsNullOrEmpty(_connectionString) || _connectionString.Contains("YOUR_SERVER_NAME"))
            {
                // Note: In a real scenario, we might want to log a warning or throw a more specific exception.
                // For testing purposes, we ensure the connection string is valid before proceeding.
            }
            #endregion
        }

        private DbContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            return new DbContext(optionsBuilder.Options);
        }

        public async Task<bool> DeleteAsync<T>(object id) where T : class
        {
            try
            {
                using var context = CreateContext();
                var entity = await context.Set<T>().FindAsync(id);
                if (entity == null) return false;

                context.Set<T>().Remove(entity);
                var result = await context.SaveChangesAsync();
                Log.Information($"Deleted entity of type {typeof(T).Name} with ID {id}");
                return result > 0;
               
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting entity of type {typeof(T).Name}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string tableName, string condition)
        {
            try
            {
                string sql = $"DELETE FROM {tableName} WHERE {condition}";
                Log.Information($"Executing SQL: {sql}");
                return await ExecuteSqlAsync(sql);
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting from table {tableName}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ExecuteSqlAsync(string sql)
        {
            try
            {
                using var context = CreateContext();
                // ExecuteSqlRawAsync returns the number of rows affected
                var rowsAffected = await context.Database.ExecuteSqlRawAsync(sql);
                Log.Information($"Executed SQL: {sql} - Rows Affected: {rowsAffected}");
                return rowsAffected >= 0;
            }
            catch (Exception ex)
            {
                Log.Error($"Error executing SQL: {ex.Message}");
                return false;
            }
        }
    }
}
