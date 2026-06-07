using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Repository
{
    public class OrphanHousingDbContext : DbContext
    {
        public OrphanHousingDbContext()
        {

        }
        public OrphanHousingDbContext(DbContextOptions<OrphanHousingDbContext> options)
            : base(options) { }

        public DbSet<Person> Persons => Set<Person>();
        public DbSet<Apartment> Apartments => Set<Apartment>();
        public DbSet<Contract> Contracts => Set<Contract>();
        public DbSet<CommissionDecision> CommissionDecisions => Set<CommissionDecision>();
        public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();
        public DbSet<UtilityDebt> UtilityDebts => Set<UtilityDebt>();
        public DbSet<ApartmentStatusHistory> ApartmentStatusHistories => Set<ApartmentStatusHistory>();
        public DbSet<Application> Applications => Set<Application>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>()
                .HasIndex(x => new
                { 
                    x.SurName, 
                    x.FirstName, 
                    x.LastName 
                });

            modelBuilder.Entity<Apartment>().
                HasIndex(x => x.Address);

            modelBuilder.Entity<Contract>()
                .HasIndex(x => x.ContractNumber); 
            
            modelBuilder.Entity<CommissionDecision>()
                .HasIndex(x => x.DecisionNumber);

            modelBuilder.Entity<Contract>()
                .HasOne(x => x.PreviousContract)
                .WithMany()
                .HasForeignKey(x => x.PreviousContractId)
                .OnDelete(DeleteBehavior.Restrict);

            //ENUMS
            modelBuilder.Entity<Contract>()
               .Property(x => x.ContractType)
               .HasConversion<string>();

            modelBuilder.Entity<Contract>()
                .Property(x => x.Status)
                .HasConversion<string>();

            modelBuilder.Entity<CommissionDecision>()
                .Property(x => x.DecisionType)
                .HasConversion<string>();

            modelBuilder.Entity<CommissionDecision>()
                .Property(x => x.Result)
                .HasConversion<string>();

            modelBuilder.Entity<Application>()
                .Property(x => x.ApplicationType)
                .HasConversion<string>();

            modelBuilder.Entity<Application>()
                .Property(x => x.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Apartment>()
                .Property(x => x.CurrentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<ApartmentStatusHistory>()
                .Property(x => x.Status)
                .HasConversion<string>();

            modelBuilder.Entity<FamilyMember>()
                .Property(x => x.RelationshipType)
                .HasConversion<string>();

            modelBuilder.Entity<UtilityDebt>()
                .Property(x => x.Status)
                .HasConversion<string>();
        }

    }
}
