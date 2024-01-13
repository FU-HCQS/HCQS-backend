using HCQS.BackEnd.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HCQS.BackEnd.DAL.Data
{
    public class HCQSDbContext : IdentityDbContext<Account>
    {
        public HCQSDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<ConstructionMaterial> ConstructionMaterials { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractProgressPayment> ContractProgressPayment { get; set; }
        public DbSet<ExportPriceMaterial> ExportPriceMaterials { get; set; }
        public DbSet<ImportExportInventoryHistory> ImportExportInventoryHistorys { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialHistory> MaterialHistories { get; set; }
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
        public DbSet<Worker> Workers { get; set; }
        public DbSet<WorkerForProject> WorkerForProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}