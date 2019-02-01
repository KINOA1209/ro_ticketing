using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ticketing_api.Infrastructure;
using ticketing_api.Infrastructure.Identity;
using ticketing_api.Models;
using ticketing_api.Models.Views;

namespace ticketing_api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly string _user;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, UserResolverService userService)
            : base(options)
        {
            this._user = userService?.GetUser();
        }

        public DbSet<AppUser> AppUser { get; set; }
        private DbSet<Audit> Audit { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Driver> Driver { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<RigLocation> RigLocation { get; set; }
        public DbSet<ShippingPaper> ShippingPaper { get; set; }
        public DbSet<TicketPaper> TicketPaper { get; set; }
        public DbSet<Well> Well { get; set; }
        public DbSet<Market> Market { get; set; }
        public DbSet<County> County { get; set; }
        public DbSet<MarketTax> MarketTax { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<JobType> JobType { get; set; }
        public DbSet<Setting> Setting { get; set; }
        public DbSet<Truck> Truck { get; set; }
        public DbSet<IdentityRole> AspNetRoles { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<IdentityUserRole<string>> UserRole { get; set; }
        public DbSet<Unit> Unit { get; set; }
        public DbSet<PaymentTerm> PaymentTerm { get; set; }
        public DbSet<TicketProduct> TicketProduct { get; set; }
        public DbSet<Log> OrderLog { get; set; }
        public DbSet<TicketImage> TicketImage { get; set; }
        public DbSet<TicketPaperImage> TicketPaperImage { get; set; }
        public DbSet<ShippingPaperImage> ShippingPaperImage { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<Tax> Tax { get; set; }
        public DbSet<CategoryTax> CategoryTax { get; set; }
        public DbSet<Vehicle> Vehicle { get; set; }
        public DbSet<RigLocationNote> RigLocationNote { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<CustomerNote> CustomerNote { get; set; }
        public DbSet<OrderDirection> OrderDirection { get; set; }
        public DbSet<SalesRep> SalesRep { get; set; }
        public DbSet<TicketTax> TicketTax { get; set; }
        public DbSet<ProductTax> ProductTax { get; set; }
        public DbSet<AppUserLocation> AppUserLocation { get; set; }
        public DbSet<LoadOrigin> LoadOrigin { get; set; }
        public DbSet<MarketSpecialHandling> MarketSpecialHandling { get; set; }
        public DbSet<Carrier> Carrier { get; set; }
        public DbSet<Refinery> Refinery { get; set; }
        public DbSet<BillOfLading> BillOfLading { get; set; }
        public DbSet<Terminal> Terminal { get; set; }
        public DbSet<RefineryAddress> RefineryAddress { get; set; }
        public DbSet<BillOfLadingStatus> BillOfLadingStatus { get; set; }
        public DbSet<RequestDeliveryTimeSlot> RequestDeliveryTimeSlot { get; set; }
        public DbSet<BillOfLadingProduct> BillOfLadingProduct { get; set; }
        public DbSet<BillOfLadingImage> BillOfLadingImage { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplate { get; set; }

        //public Func<DateTime> TimestampProvider { get; } = () => DateTime.UtcNow;
        public DateTime TimestampProvider = DateTime.UtcNow;

        public override int SaveChanges()
        {
            this.AuditEntities();
            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges();
            OnAfterSaveChanges(auditEntries);
            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.AuditEntities();
            var auditEntries = OnBeforeSaveChanges();
            //var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            var result = base.SaveChanges();
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            //convert deletes to soft deletes
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted))
            {
                if (entry.Entity is ISoftDeletable)
                {
                    entry.Property("IsDeleted").CurrentValue = true;
                    entry.State = EntityState.Modified;
                }
            }

            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry) { TableName = entry.Metadata.Relational().TableName, ChangedBy = _user };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                Audit.Add(auditEntry.ToAudit());
            }

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                // Save the Audit entry
                Audit.Add(auditEntry.ToAudit());
            }

            return SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Make sure you add above line
            modelBuilder.Entity<ApplicationUser>().ToTable("aspnetusers");
            modelBuilder.Entity<IdentityRole>().ToTable("aspnetroles");
            modelBuilder.Entity<Module>().ToTable("module");
            modelBuilder.Entity<Permission>().ToTable("permission");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("aspnetuserroles");
            modelBuilder.Entity<TicketProduct>().ToTable("ticketproduct");
            //modelBuilder.Entity<IdentityUserLogin<ApplicationUser>>().ToTable("aspnetuserlogins");

            //lowercase table names to match db
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.Relational().TableName = entityType.Relational().TableName.ToLower();
            }

            //filter out soft delete items
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(ISoftDeletable).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType).Property<Boolean>("IsDeleted");
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(
                    Expression.Call(typeof(EF), nameof(EF.Property), new[] { typeof(bool) }, parameter, Expression.Constant("IsDeleted")),
                    Expression.Constant(false));
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
            }

            //add audit fields
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(IAuditable).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("CreatedAt");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("UpdatedAt");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>("CreatedBy");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>("UpdatedBy");
            }
        }

        /// <summary>
        /// Method that will set the Audit properties for every added or modified Entity marked with the 
        /// IAuditable interface.
        /// </summary>
        private void AuditEntities()
        {
            // For every changed entity marked as IAditable set the values for the audit properties
            foreach (EntityEntry<IAuditable> entry in ChangeTracker.Entries<IAuditable>())
            {
                // If the entity was added.
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedBy").CurrentValue = _user;
                    entry.Property("CreatedAt").CurrentValue = TimestampProvider;
                }
                else if (entry.State == EntityState.Modified) // If the entity was updated
                {
                    entry.Property("UpdatedBy").CurrentValue = _user;
                    entry.Property("UpdatedAt").CurrentValue = TimestampProvider;
                }
            }
        }

    }
}
