namespace EHisseler.Web.Models
{
    /// <summary>
    /// Elanın kabinetdə/admin paneldə göstərilən vahid vəziyyəti.
    /// Moderasiya statusu, aktivlik, stok və silinmə ayrı saxlanılır — burada birləşdirilir.
    /// </summary>
    public static class AdStatusHelper
    {
        public const string Active = "Active";
        public const string Pending = "Pending";
        public const string Rejected = "Rejected";
        public const string OutOfStock = "OutOfStock";
        public const string Deactivated = "Deactivated";
        public const string Deleted = "Deleted";

        public static string GetStatus(SparePart p)
        {
            if (p.IsDeleted) return Deleted;
            if (p.ModerationStatus == ApprovalStatus.Pending) return Pending;
            if (p.ModerationStatus == ApprovalStatus.Rejected) return Rejected;
            if (p.StockQuantity <= 0) return OutOfStock;
            if (!p.IsActive) return Deactivated;
            return Active;
        }

        public static string GetText(string status) => status switch
        {
            Active => "Aktiv",
            Pending => "Yoxlamadadır",
            Rejected => "Rədd edilib",
            OutOfStock => "Stok bitib",
            Deactivated => "Deaktiv edilib",
            Deleted => "Silinib",
            _ => status
        };

        /// <summary>İctimai saytda (axtarış, siyahı, detal) görünmə şərti.</summary>
        public static bool IsPublic(SparePart p) =>
            !p.IsDeleted && p.IsActive && p.ModerationStatus == ApprovalStatus.Approved && p.StockQuantity > 0;
    }
}
