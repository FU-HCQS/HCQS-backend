using HCQS.BackEnd.API.Installers;
using HCQS.BackEnd.DAL.Data;
using Microsoft.EntityFrameworkCore;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.InstallerServicesInAssembly(builder.Configuration);

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});
//}
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

//app.UseHangfireDashboard();

app.MapControllers();
ApplyMigration();
using (var scope = app.Services.CreateScope())
{
    //var serviceProvider = scope.ServiceProvider;
    //var workerService = serviceProvider.GetRequiredService<WorkerService>();
    //workerService.Start();
}
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<HCQSDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}