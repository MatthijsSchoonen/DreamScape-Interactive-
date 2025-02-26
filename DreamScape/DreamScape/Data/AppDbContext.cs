using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DreamScape.Model;
using Microsoft.EntityFrameworkCore;

namespace DreamScape.Data
{
    class AppDbContext : DbContext
    {
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Model.Type> Types { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Rarity> Rarities { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Trade> Trades { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Go to the App.config.example file and then follow Instructions

            optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=;database=DreamScape", ServerVersion.Parse("8.0.30"));
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, Name = "Pending" },
                new Status { Id = 2, Name = "Declined" },
                new Status { Id = 3, Name = "Accepted" }
            );

            modelBuilder.Entity<Model.Type>().HasData(
                new Model.Type { Id = 1, Name = "Weapon" },
                new Model.Type { Id = 2, Name = "Armor" },
                new Model.Type { Id = 3, Name = "Consumable" }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            );

            modelBuilder.Entity<Rarity>().HasData(
                new Rarity { Id = 1, Name = "Common" },
                new Rarity { Id = 2, Name = "Uncommon" },
                new Rarity { Id = 3, Name = "Rare" },
                new Rarity { Id = 4, Name = "Epic" },
                new Rarity { Id = 5, Name = "Legendary" }
            );

            modelBuilder.Entity<User>().HasData(
               new User { Id = 1, UserName = "admin", Email = "admin@example.com", Password = SecureHasher.Hash("admin"), RoleId = 1 },
               new User { Id = 2, UserName = "user", Email = "user@example.com", Password = SecureHasher.Hash("user"), RoleId = 2 },
               new User { Id = 3, UserName = "user2", Email = "user2@example.com", Password = SecureHasher.Hash("user"), RoleId = 2 }
           );

            modelBuilder.Entity<Item>().HasData(
                new Item { Id = 1, Name = "Sword", TypeId = 1, RarityId = 1, Power = 10, Speed = 5, Durability = 100, Magic = "None" },
                new Item { Id = 2, Name = "Shield", TypeId = 2, RarityId = 2, Power = 5, Speed = 2, Durability = 80, Magic = "None" },
                new Item { Id = 3, Name = "Potion of Light", TypeId = 3, RarityId = 4, Power = 10, Speed = 5, Durability = 1, Magic = "None" }
            );

            modelBuilder.Entity<PasswordReset>().HasData(
                new PasswordReset { Id = 1, UserId = 1, Code = "resetcode1" },
                new PasswordReset { Id = 2, UserId = 2, Code = "resetcode2" }
            );

            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { Id = 1, UserId = 1, ItemId = 1, Count = 1 ,IsForTrade = true },
                new Inventory { Id = 2, UserId = 2, ItemId = 2, Count = 1 }
            );

            modelBuilder.Entity<Trade>().HasData(
                new Trade { Id = 1, SellerId = 1, SellItemId = [1], TradeItemId = [2], BuyerId = 2, StatusId = 1 }
            );
        }
    }
}
