using System.Data.Common;
using System.Threading;

namespace StorageCore.DataAccess
{
    public class DbContext : IDbContext
    {
        public ThreadLocal<DbTransaction> DbTransaction { get; private set; }

        DbTransaction IDbContext.DbTransaction => DbTransaction.Value;

        public void SetTransaction(DbTransaction dbTransaction)
        {
            this.DbTransaction.Value = dbTransaction;
        }
    }
}