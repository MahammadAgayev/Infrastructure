using System;
using System.Data.Common;

namespace StorageCore.DataAccess
{
    public interface IDbContext
    {
        DbTransaction Transaction { get; }
    }
}