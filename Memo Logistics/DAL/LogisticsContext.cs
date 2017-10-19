using Memo_Logistics.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Memo_Logistics.DAL
{
    public class LogisticsContext : DbContext
    {

        public DbSet<Logistics> Logistics { get; set; }
        public DbSet<Trucks> Trucks { get; set; }
        public DbSet<Drivers> Drivers { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Trucks>()
               .HasMany(c => c.Drivers).
               WithMany(i => i.Trucks)
               .Map(t => t.MapLeftKey("TruckID")
                   .MapRightKey("DriverID")
                   .ToTable("TruckDrivers"));

            modelBuilder.Entity<Customers>().MapToStoredProcedures();

            modelBuilder.Entity<Logistics>().MapToStoredProcedures();

        }
    }
}