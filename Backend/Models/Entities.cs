using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EHisseler.Web.Models
{
    public enum UserRole
    {
        Admin = 1,
        Usta = 2,
        Anbardar = 3
    }

    public enum ApprovalStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!; // Admin üçün login və ya mobil nömrə
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Phone { get; set; } // Azərbaycan mobil nömrəsi (+994XXXXXXXXX)
        public string Email { get; set; } = string.Empty; // Könüllü — boş ola bilər

        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
        public string? RejectionReason { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public SellerProfile? SellerProfile { get; set; }
    }

    public class SellerProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string ContactPerson { get; set; } = null!;
        public string? Phone { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Address { get; set; }
        public decimal Rating { get; set; }
        public bool ShowContactInfoInAds { get; set; } = true;
        public bool IsPhoneVerified { get; set; } = true;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class DeviceBrand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CategoryBrand
    {
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }

    public class DeviceModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Name { get; set; } = null!;
        public string ModelCode { get; set; } = null!;
        public int ReleaseYear { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;
        [ForeignKey("BrandId")]
        public DeviceBrand Brand { get; set; } = null!;
    }

    public class SparePartType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Key { get; set; } = null!;
        public bool IsPortable { get; set; }
        public bool IsAccessory { get; set; }
    }

    public class CategoryPartType
    {
        public int CategoryId { get; set; }
        public int PartTypeId { get; set; }
    }

    public class QualityType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class SparePart
    {
        public int Id { get; set; }
        public int SellerId { get; set; }

        [Required(ErrorMessage = "Model seçilməlidir")]
        public int DeviceModelId { get; set; }

        [Required(ErrorMessage = "Hissə növü seçilməlidir")]
        public int PartTypeId { get; set; }

        public int QualityTypeId { get; set; }

        [Required(ErrorMessage = "Ad boş ola bilməz")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Adın uzunluğu 3 ilə 200 simvol arasında olmalıdır")]
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "SKU boş ola bilməz")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "SKU uzunluğu 2 ilə 50 simvol arasında olmalıdır")]
        public string SKU { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Qiymət sıfırdan böyük olmalıdır")]
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public string Currency { get; set; } = "AZN";

        [ConcurrencyCheck]
        public int StockQuantity { get; set; }
        public int MinOrderQuantity { get; set; } = 1;

        [Range(0, 120, ErrorMessage = "Zəmanət müddəti mənfi ola bilməz")]
        public int WarrantyMonths { get; set; }

        public int ViewCount { get; set; }

        public ApprovalStatus ModerationStatus { get; set; } = ApprovalStatus.Pending;
        public string? RejectionReason { get; set; }

        /// <summary>
        /// true — elan əvvəl dərc olunub və yoxlamaya dəyişiklik (yenilənmə) kimi düşüb.
        /// false — ilk dəfə yaradılan (yeni) elan.
        /// </summary>
        public bool IsUpdateSubmission { get; set; }

        [ConcurrencyCheck]
        public byte[]? RowVersion { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LocationAddress { get; set; }

        [ForeignKey("SellerId")]
        public SellerProfile Seller { get; set; } = null!;
        [ForeignKey("DeviceModelId")]
        public DeviceModel DeviceModel { get; set; } = null!;
        [ForeignKey("PartTypeId")]
        public SparePartType PartType { get; set; } = null!;
        [ForeignKey("QualityTypeId")]
        public QualityType QualityType { get; set; } = null!;

        public ICollection<SparePartImage> Images { get; set; } = new List<SparePartImage>();
        public ICollection<SparePartReview> Reviews { get; set; } = new List<SparePartReview>();
    }

    public class SparePartImage
    {
        public int Id { get; set; }
        public int SparePartId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }

        [ForeignKey("SparePartId")]
        public SparePart SparePart { get; set; } = null!;
    }

    public class SparePartSaleAudit
    {
        public int Id { get; set; }
        public int SparePartId { get; set; }
        /// <summary>Satıcı profili (SellerProfile.Id)</summary>
        public int SellerId { get; set; }
        /// <summary>Əməliyyatı edən istifadəçi (User.Id)</summary>
        public int? PerformedByUserId { get; set; }
        public int Quantity { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("SparePartId")]
        public SparePart SparePart { get; set; } = null!;
    }

    public class SparePartReview
    {
        public int Id { get; set; }
        public int SparePartId { get; set; }
        public int AdminId { get; set; }
        public ApprovalStatus Status { get; set; }
        public string? Reason { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        [ForeignKey("SparePartId")]
        public SparePart SparePart { get; set; } = null!;
    }

    public class ApprovalHistory
    {
        public int Id { get; set; }
        public int TargetUserId { get; set; }
        public int AdminId { get; set; }
        public ApprovalStatus Status { get; set; }
        public string? Reason { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.Now;
    }

    public class ActivityLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = null!;
        public string? EntityName { get; set; }
        public int? EntityId { get; set; }
        public string? Details { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
