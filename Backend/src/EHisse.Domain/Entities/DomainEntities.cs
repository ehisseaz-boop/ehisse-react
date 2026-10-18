using EHisse.Domain.Common;
using EHisse.Domain.Enums;

namespace EHisse.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Pin { get; set; } // FİN kod (şəxsiyyət vəsiqəsi)
    public string? IdCardImageUrl { get; set; } // Vəsiqə şəkli URL-i

    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public string? RejectionReason { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public SellerProfile? SellerProfile { get; set; }
}

public class SellerProfile : BaseEntity
{
    public int UserId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string? Phone { get; set; }
    public string? WhatsAppNumber { get; set; }
    public string? Address { get; set; }
    public decimal Rating { get; set; }
    public bool ShowContactInfoInAds { get; set; } = true;
    public bool IsPhoneVerified { get; set; } = true;

    public User User { get; set; } = null!;
}

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class DeviceBrand : BaseEntity
{
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}

public class CategoryBrand
{
    public int CategoryId { get; set; }
    public int BrandId { get; set; }

    public Category Category { get; set; } = null!;
    public DeviceBrand Brand { get; set; } = null!;
}

public class DeviceModel : BaseEntity
{
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public string Name { get; set; } = null!;
    public string ModelCode { get; set; } = null!;
    public int ReleaseYear { get; set; }
    public bool IsActive { get; set; }

    public Category Category { get; set; } = null!;
    public DeviceBrand Brand { get; set; } = null!;
}

public class SparePartType : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Key { get; set; } = null!;
    public bool IsPortable { get; set; }
    public bool IsAccessory { get; set; }
}

public class CategoryPartType
{
    public int CategoryId { get; set; }
    public int PartTypeId { get; set; }

    public Category Category { get; set; } = null!;
    public SparePartType PartType { get; set; } = null!;
}

public class QualityType : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class SparePart : BaseEntity
{
    public int SellerId { get; set; }
    public int DeviceModelId { get; set; }
    public int PartTypeId { get; set; }
    public int QualityTypeId { get; set; }

    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string Currency { get; set; } = "AZN";

    public int StockQuantity { get; set; }
    public int MinOrderQuantity { get; set; } = 1;
    public int WarrantyMonths { get; set; }
    public int ViewCount { get; set; }

    public ApprovalStatus ModerationStatus { get; set; } = ApprovalStatus.Pending;
    public string? RejectionReason { get; set; }
    public bool IsUpdateSubmission { get; set; }

    public byte[]? RowVersion { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationAddress { get; set; }

    public SellerProfile Seller { get; set; } = null!;
    public DeviceModel DeviceModel { get; set; } = null!;
    public SparePartType PartType { get; set; } = null!;
    public QualityType QualityType { get; set; } = null!;

    public ICollection<SparePartImage> Images { get; set; } = new List<SparePartImage>();
    public ICollection<SparePartReview> Reviews { get; set; } = new List<SparePartReview>();
}

public class SparePartImage : BaseEntity
{
    public int SparePartId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }

    public SparePart SparePart { get; set; } = null!;
}

public class SparePartSaleAudit : BaseEntity
{
    public int SparePartId { get; set; }
    public int SellerId { get; set; }
    public int? PerformedByUserId { get; set; }
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public SparePart SparePart { get; set; } = null!;
}

public class SparePartReview : BaseEntity
{
    public int SparePartId { get; set; }
    public int AdminId { get; set; }
    public ApprovalStatus Status { get; set; }
    public string? Reason { get; set; }
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

    public SparePart SparePart { get; set; } = null!;
}

public class ApprovalHistory : BaseEntity
{
    public int TargetUserId { get; set; }
    public int AdminId { get; set; }
    public ApprovalStatus Status { get; set; }
    public string? Reason { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;
}

public class ActivityLog : BaseEntity
{
    public int UserId { get; set; }
    public string Action { get; set; } = null!;
    public string? EntityName { get; set; }
    public int? EntityId { get; set; }
    public string? Details { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
}
