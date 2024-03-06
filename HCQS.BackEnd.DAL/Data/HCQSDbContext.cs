using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HCQS.BackEnd.DAL.Data
{
    public class HCQSDbContext : IdentityDbContext<Account>
    {
        public HCQSDbContext()
        {
        }

        public HCQSDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<ProgressConstructionMaterial> ProgressConstructionMaterials { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ConstructionConfigValue> ConstructionConfigValues { get; set; }
        
        public DbSet<ContractProgressPayment> ContractProgressPayment { get; set; }
        public DbSet<ExportPriceMaterial> ExportPriceMaterials { get; set; }
        public DbSet<ImportExportInventoryHistory> ImportExportInventoryHistorys { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<SupplierPriceQuotation> SupplierPriceQuotations { get; set; }
        public DbSet<MaterialSupplier> MaterialSuppliers { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentResponse> PaymentResponses { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationDetail> QuotationDetails { get; set; }
        public DbSet<SampleProject> SampleProjects { get; set; }
        public DbSet<StaticFile> StaticFiles { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<WorkerPrice> WorkerPrices { get; set; }
        public DbSet<WorkerForProject> WorkerForProjects { get; set; }
        public DbSet<SupplierPriceDetail> SupplierPriceDetails { get; set; }
        public DbSet<QuotationDealing> QuotationDealings { get; set; }
        public DbSet<ContractVerificationCode> ContractVerificationCodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", true, true)
                            .Build();
            string cs = config["ConnectionStrings:Host"];
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(cs);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
              new IdentityRole { Id = "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21", Name = Permission.ADMIN, NormalizedName = Permission.ADMIN.ToLower() });

            builder.Entity<IdentityRole>().HasData(
               new IdentityRole { Id = "2f28c722-04c9-41fd-85e4-eaa506acda38", Name = Permission.STAFF, NormalizedName = Permission.STAFF.ToLower() });

            builder.Entity<IdentityRole>().HasData(
                       new IdentityRole { Id = "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f", Name = Permission.CUSTOMER, NormalizedName = Permission.CUSTOMER.ToLower() });
        }
    }
}