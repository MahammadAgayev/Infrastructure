using System;
using System.Data.Common;

namespace StorageCore.DataAccess
{
    public class DbContext : IDbContext
    {
        public DbTransaction Transaction => throw new NotImplementedException();
    }
}