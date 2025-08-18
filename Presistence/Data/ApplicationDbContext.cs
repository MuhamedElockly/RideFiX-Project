using Domain.Entities.CoreEntites.CarMaintenance_Entities;
using Domain.Entities.CoreEntites.EmergencyEntities;
using Domain.Entities.credit;
using Domain.Entities.e_Commerce;
using Domain.Entities.IdentityEntities;
using Domain.Entities.PaymentEntites;
using Domain.Entities.Reporting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Presistence.Data.Configurations;
using Presistence.Migrations;


namespace Presistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyRefference).Assembly);
        }
      
        public DbSet<Technician> technicians { get; set; }
        public DbSet<Car> cars { get; set; }
        public DbSet<CarMaintenanceRecord> carMaintenanceRecords { get; set; }
        public DbSet<MaintenanceTypes> MaintenanceTypes { get; set; }
        public DbSet<CarOwner> carOwners { get; set; }
        public DbSet<TCategory> categories { get; set; }

        public DbSet<ChatSession> chatSessions { get; set; }
        public DbSet<Message> messages { get; set; }

        public DbSet<EmergencyRequest> emergencyRequests { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<EmergencyRequestTechnicians> EmergencyRequestTechnicians { get; set; }
        public DbSet<UserConnectionIds> UserConnectionIds { get; set; }

        public DbSet<Product> products { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }
        public DbSet<Category> pCategory { get; set; }
        public DbSet<Report> reports { get; set; }
        public DbSet<Rate> ProductRate { get; set; }
        public DbSet<CoinTopUp> CoinTopUps { get; set; }

        public DbSet<CoinChargeEntity> CoinChargeEntities { get; set; }

    }
}
