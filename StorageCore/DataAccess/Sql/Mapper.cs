using System;
using System.Data;
using StorageCore.Domain.Entities;
using StorageCore.Extensions;

namespace StorageCore.DataAccess.Sql
{
    public static class Mapper
    {
        public static User MapToUser(IDataReader reader)
        {
            return new User
            {
                Id = reader.Get<int>("id"),
                Email = reader.Get<string>("Email"),
                EmailConfirmed = reader.Get<bool>("EmailConfirmed"),
                NormalizedEmail = reader.Get<string>("NormalizedEmail"),
                PhoneNumber = reader.Get<string>("PhoneNumber"),
                PhoneNumberConfirmed = reader.Get<bool>("PhoneNumberConfirmed"),
                PasswordHash = reader.Get<string>("PasswordHash"),
                Created = reader.Get<DateTime>("Created"),
                Updated = reader.Get<DateTime>("Updated")
            };
        }

        public static Role MapToRole(IDataReader reader)
        {
            return new Role
            {
                Id = reader.Get<int>("Id"),
                Name = reader.Get<string>("Name"),
                NormalizedName = reader.Get<string>("NormalizedName")
            };
        }
    }
}
