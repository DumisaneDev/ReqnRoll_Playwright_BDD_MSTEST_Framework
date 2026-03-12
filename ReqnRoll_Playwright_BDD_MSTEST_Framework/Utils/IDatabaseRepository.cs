using System.Threading.Tasks;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    /// <summary>
    /// Defines a contract for database operations, ensuring they are easy to use and verify.
    /// </summary>
    internal interface IDatabaseRepository
    {
        #region Deletes a record of type T by its unique identifier or a specific condition.
        /// <summary>
        /// Deletes a record of type T by its unique identifier or a specific condition.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The identifier for the record to be deleted.</param>
        /// <returns>A task representing the asynchronous operation, returning true if the deletion was successful.</returns>
         #endregion
        Task<bool> DeleteAsync<T>(object id) where T : class;

        #region Deletes a record based on a specific table name and condition.
        /// <summary>
        /// Deletes a record based on a specific table name and condition.
        /// Useful for non-ORM direct deletions or when using raw SQL.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="condition">The WHERE clause condition (e.g., "Email = 'test@example.com'").</param>
        /// <returns>A task representing the asynchronous operation, returning true if one or more rows were affected.</returns>
        #endregion
        Task<bool> DeleteAsync(string tableName, string condition);

        #region Executes a raw SQL command and returns a boolean indicating success.
        /// <summary>
        /// Executes a raw SQL command and returns a boolean indicating success.
        /// </summary>
        /// <param name="sql">The SQL command to execute.</param>
        /// <returns>True if the command was successful.</returns>
        #endregion
        Task<bool> ExecuteSqlAsync(string sql);
    }
}
