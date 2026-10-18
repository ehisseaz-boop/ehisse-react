using EHisse.Domain.Enums;

namespace EHisse.Application.DTOs;

public class SparePartSearchQuery
{
    public string? SearchString { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? ModelId { get; set; }
    public int? PartTypeId { get; set; }
    public int? QualityTypeId { get; set; }
    public int? SellerId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool InStockOnly { get; set; } = true;
    public ApprovalStatus? Status { get; set; } = ApprovalStatus.Approved;
    public bool IncludeDeleted { get; set; } = false;
    public bool IsActiveOnly { get; set; } = true;
    public string SortOrder { get; set; } = "date_desc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string FirstName, string LastName, string Phone, string? Email, string Password, UserRole Role);
public record ForgotPasswordRequest(string Username);
public record ResetPasswordRequest(string Username, string NewPassword);

public record PartInput(
    int DeviceModelId, int PartTypeId, int QualityTypeId,
    string? Title, string? Description, string? SKU,
    decimal Price, decimal? DiscountPrice, string? Currency,
    int StockQuantity, int MinOrderQuantity, int WarrantyMonths,
    double? Latitude = null, double? Longitude = null, string? LocationAddress = null);

public record PriceInput(decimal NewPrice);
public record SettingsInput(string CompanyName, string ContactPerson, string? Phone, string? WhatsAppNumber, string? Address, bool ShowContactInfoInAds);

public record ReasonInput(string? Reason);
public record CategoryInput(string Name, int DisplayOrder = 0, bool IsActive = true);
public record BrandInput(string Name, bool IsActive = true);
public record AssignBrandInput(int CategoryId, int BrandId);
public record ModelInput(string Name, string? ModelCode, int ReleaseYear, int CategoryId, int BrandId, bool IsActive = true);
public record PartTypeInput(string Name, string? Key, bool IsAccessory = false, bool IsPortable = false);
public record AssignPartTypeInput(int CategoryId, int PartTypeId);

public record AdminDashboardStats(
    int CountUsers,
    int CountParts,
    int ActiveRequests,
    int PendingUsers,
    int PendingNewAds,
    int PendingUpdateAds,
    int PendingRestockAds
);
