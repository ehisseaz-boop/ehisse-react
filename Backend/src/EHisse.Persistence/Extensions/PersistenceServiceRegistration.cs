using System;
using EHisse.Application.Interfaces;
using EHisse.Persistence.Context;
using EHisse.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EHisse.Persistence.Extensions;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connStr = configuration.GetConnectionString("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("DATABASE_URL") 
                ?? "Host=aws-0-eu-west-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.xtlimzohxsvcnuclzbxt;Password=EHisseler2026!Secured#Db;SSL Mode=Require;Trust Server Certificate=true";

            options.UseNpgsql(connStr);
            options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        });

        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<ICatalogService, CatalogService>();

        return services;
    }
}
