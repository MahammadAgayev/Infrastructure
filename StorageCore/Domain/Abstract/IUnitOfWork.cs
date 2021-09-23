namespace StorageCore.Domain.Abstract
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }
        public IRoleRepository RoleRepository { get; }

        void CreateTransaction();

        void Commit();
        void Rollback();
    }
}