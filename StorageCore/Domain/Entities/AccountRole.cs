namespace StorageCore.Domain.Entities
{
    public class AccountRole 
    {
        public int Id { get; set; }
        public Account User { get; set; }
        public Role Role { get; set; }
    }
}