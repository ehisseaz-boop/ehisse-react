namespace EHisseler.Web.Models
{
    /// <summary>
    /// Elan axtarışı üçün filter parametrləri.
    /// HomeController.Index, AjaxController.Search və Admin.AllAds
    /// hamısı bu DTO-nu QueryBuilder extension-larla işlədir.
    /// </summary>
    public class SparePartSearchQuery
    {
        // ── Mətn axtarışı ──────────────────────────────────────────────────────
        /// <summary>
        /// Boşluqla ayrılmış söz(lər). Title / Brand / Model üzrə AND-axtarış.
        /// </summary>
        public string? SearchString { get; set; }

        // ── Kateqoriya/cihaz filtrləri ──────────────────────────────────────────
        public int? CategoryId   { get; set; }
        public int? BrandId      { get; set; }
        public int? ModelId      { get; set; }
        public int? PartTypeId   { get; set; }
        public int? QualityTypeId{ get; set; }
        public int? SellerId     { get; set; }

        // ── Qiymət filtrləri ────────────────────────────────────────────────────
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // ── Status filtrləri ────────────────────────────────────────────────────
        /// <summary>Yalnız stokda olan məhsullar. Default: true.</summary>
        public bool InStockOnly { get; set; } = true;

        /// <summary>
        /// Null = hamısı (admin üçün).
        /// Dəyər varsa yalnız həmin status göstərilir.
        /// </summary>
        public ApprovalStatus? Status { get; set; } = ApprovalStatus.Approved;

        /// <summary>Silinmiş elanlar göstərilsin? Default: false (göstərilməsin).</summary>
        public bool IncludeDeleted { get; set; } = false;
        public bool IsActiveOnly { get; set; } = true;

        // ── Sıralama ────────────────────────────────────────────────────────────
        /// <summary>price_asc | price_desc | date_asc | date_desc</summary>
        public string SortOrder { get; set; } = "date_desc";

        // ── Səhifələmə ──────────────────────────────────────────────────────────
        public int PageNumber { get; set; } = 1;
        public int PageSize   { get; set; } = 20;
    }
}
