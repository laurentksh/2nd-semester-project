using System;
using System.Collections.Generic;
using System.Text;
using _2ndSemesterProject.Models;
using _2ndSemesterProject.Models.Database;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _2ndSemesterProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public DbSet<CloudFile> Files { get; set; }
        public DbSet<CloudFolder> Folders { get; set; }
        public DbSet<FolderSharedAccess> FolderSharedAccesses { get; set; }
        public DbSet<FileSharedAccess> FileSharedAccesses { get; set; }

        public DbSet<AccountPlan> AccountPlans { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options = default) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CloudFolder>()
                .HasOne(p => p.Parent)
                .WithMany(pp => pp.Childs)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CloudFolder>()
                .HasOne(o => o.Owner)
                .WithMany(f => f.Folders)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FolderSharedAccess>()
                .HasOne(s => s.Sender)
                .WithMany(s => s.FolderSharedAccessesSenders)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FolderSharedAccess>()
                .HasOne(r => r.Receiver)
                .WithMany(r => r.FolderSharedAccessesReceiver)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FileSharedAccess>()
                .HasOne(s => s.Sender)
                .WithMany(s => s.FileSharedAccessesSenders)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FileSharedAccess>()
                .HasOne(r => r.Receiver)
                .WithMany(r => r.FileSharedAccessesReceiver)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
