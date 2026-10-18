using EHisse.Application.DTOs;
using EHisse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EHisse.Persistence.Repositories;

public static class SparePartQueryExtensions
{
    public static IQueryable<SparePart> ApplySearch(
        this IQueryable<SparePart> query,
        SparePartSearchQuery filter)
    {
        var builder = new DynamicQueryBuilder<SparePart>();

        if (!filter.IncludeDeleted)
            builder.And(p => !p.IsDeleted);

        if (filter.IsActiveOnly)
            builder.And(p => p.IsActive);

        if (filter.Status.HasValue)
            builder.And(p => p.ModerationStatus == filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchString))
        {
            var terms = filter.SearchString.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var raw in terms)
            {
                var term = $"%{raw.ToLower()}%";
                builder.And(p =>
                    EF.Functions.Like(p.Title.ToLower(), term) ||
                    (p.DeviceModel != null && EF.Functions.Like(p.DeviceModel.Name.ToLower(), term)) ||
                    (p.DeviceModel != null && p.DeviceModel.Brand != null && EF.Functions.Like(p.DeviceModel.Brand.Name.ToLower(), term))
                );
            }
        }

        if (filter.CategoryId.HasValue)
            builder.And(p => p.DeviceModel.CategoryId == filter.CategoryId.Value);

        if (filter.BrandId.HasValue)
            builder.And(p => p.DeviceModel.BrandId == filter.BrandId.Value);

        if (filter.ModelId.HasValue)
            builder.And(p => p.DeviceModelId == filter.ModelId.Value);

        if (filter.PartTypeId.HasValue)
            builder.And(p => p.PartTypeId == filter.PartTypeId.Value);

        if (filter.QualityTypeId.HasValue)
            builder.And(p => p.QualityTypeId == filter.QualityTypeId.Value);

        if (filter.SellerId.HasValue)
            builder.And(p => p.SellerId == filter.SellerId.Value);

        if (filter.MinPrice.HasValue)
            builder.And(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            builder.And(p => p.Price <= filter.MaxPrice.Value);

        if (filter.InStockOnly)
            builder.And(p => p.StockQuantity > 0);

        query = builder.Apply(query);

        query = filter.SortOrder switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "views_desc" => query.OrderByDescending(p => p.ViewCount).ThenByDescending(p => p.CreatedDate),
            "date_asc" => query.OrderBy(p => p.CreatedDate),
            _ => query.OrderByDescending(p => p.CreatedDate),
        };

        return query;
    }

    public static IQueryable<SparePart> ApplyStatusFilter(
        this IQueryable<SparePart> query,
        SparePartSearchQuery filter)
    {
        var builder = new DynamicQueryBuilder<SparePart>();
        if (!filter.IncludeDeleted) builder.And(p => !p.IsDeleted);
        if (filter.IsActiveOnly) builder.And(p => p.IsActive);
        if (filter.Status.HasValue) builder.And(p => p.ModerationStatus == filter.Status.Value);
        return builder.Apply(query);
    }

    public static IQueryable<SparePart> ApplyTextSearch(
        this IQueryable<SparePart> query,
        string? searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString)) return query;

        var builder = new DynamicQueryBuilder<SparePart>();
        var terms = searchString.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var raw in terms)
        {
            var term = $"%{raw.ToLower()}%";
            builder.And(p =>
                EF.Functions.Like(p.Title.ToLower(), term) ||
                (p.DeviceModel != null && EF.Functions.Like(p.DeviceModel.Name.ToLower(), term)) ||
                (p.DeviceModel != null && p.DeviceModel.Brand != null && EF.Functions.Like(p.DeviceModel.Brand.Name.ToLower(), term))
            );
        }
        return builder.Apply(query);
    }
}
