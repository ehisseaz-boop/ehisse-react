using EHisseler.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EHisseler.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Usta,Anbardar")]
    public class SparePartsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SparePartsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{id}/record-sale")]
        public async Task<IActionResult> RecordSale(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var sellerProfile = await _context.SellerProfiles.FirstOrDefaultAsync(s => s.UserId == userId);

            if (sellerProfile == null) return Unauthorized(new { message = "Satıcı profili tapılmadı." });

            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var part = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerProfile.Id && !p.IsDeleted);

                if (part == null)
                    return NotFound(new { message = "Məhsul tapılmadı." });

                if (part.StockQuantity <= 0)
                    return BadRequest(new { message = "Məhsulun stoku bitib." });

                var previousStock = part.StockQuantity;
                part.StockQuantity -= 1;

                if (part.StockQuantity == 0)
                {
                    part.ModerationStatus = ApprovalStatus.Approved;
                    part.IsActive = false;
                }

                var audit = new SparePartSaleAudit
                {
                    SparePartId = part.Id,
                    SellerId = sellerProfile.Id,
                    Quantity = 1,
                    PreviousStock = previousStock,
                    NewStock = part.StockQuantity,
                    Timestamp = DateTime.Now
                };

                _context.SparePartSaleAudits.Add(audit);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return Ok(new { success = true, newStock = part.StockQuantity, isActive = part.IsActive });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return Conflict(new { message = "Eyni vaxtda başqa istifadəçi tərəfindən satış edilib. Zəhmət olmasa yeniləyin." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Satış zamanı xəta baş verdi.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePart(int id, [FromBody] SparePart updatedPart)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var sellerProfile = await _context.SellerProfiles.FirstOrDefaultAsync(s => s.UserId == userId);

            if (sellerProfile == null) return Unauthorized();

            var part = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerProfile.Id && !p.IsDeleted);
            if (part == null) return NotFound();

            bool requiresApproval = false;
            
            if (part.Title != updatedPart.Title ||
                part.DeviceModelId != updatedPart.DeviceModelId ||
                part.PartTypeId != updatedPart.PartTypeId ||
                part.QualityTypeId != updatedPart.QualityTypeId ||
                part.Description != updatedPart.Description ||
                part.Price != updatedPart.Price ||
                part.SKU != updatedPart.SKU)
            {
                requiresApproval = true;
            }

            part.DeviceModelId = updatedPart.DeviceModelId;
            part.PartTypeId = updatedPart.PartTypeId;
            part.QualityTypeId = updatedPart.QualityTypeId;
            part.Title = updatedPart.Title;
            part.Description = updatedPart.Description;
            part.Price = updatedPart.Price;
            part.Currency = updatedPart.Currency ?? "AZN";
            part.StockQuantity = updatedPart.StockQuantity;
            part.MinOrderQuantity = updatedPart.MinOrderQuantity;
            part.WarrantyMonths = updatedPart.WarrantyMonths;
            if (!string.IsNullOrEmpty(updatedPart.SKU))
            {
                part.SKU = updatedPart.SKU;
            }
            part.DiscountPrice = updatedPart.DiscountPrice;
            part.LastModifiedDate = DateTime.Now;

            if (requiresApproval)
            {
                part.ModerationStatus = ApprovalStatus.Pending;
                part.IsActive = false;
            }

            if (part.StockQuantity <= 0 && part.IsActive)
            {
                part.IsActive = false;
            }

            try
            {
                _context.Update(part);
                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Eyni vaxtda dəyişiklik edilib." });
            }
        }
    }
}
