using EHisseler.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlite("Data Source=ehisseler.db");
using var db = new AppDbContext(optionsBuilder.Options);

var parts = db.SpareParts.ToList();
int count = 0;
foreach (var part in parts)
{
    // Make half of them today, half of them in the past
    if (count % 2 == 0) {
        part.CreatedDate = DateTime.UtcNow; // Today
    } else {
        part.CreatedDate = DateTime.UtcNow.AddDays(- (count + 1)).AddHours(count); // Past
    }
    count++;
}
db.SaveChanges();
Console.WriteLine($"Updated {count} parts with sample dates.");
