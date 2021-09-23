using System.Data.Common;
using System.Threading;

namespace StorageCore.DataAccess
{
    public interface IDbContext
    {
        DbTransaction DbTransaction { get; }
    }
}
