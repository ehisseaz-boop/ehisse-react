
using EHisseler.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlite("Data Source=ehisseler.db");
using var db = new AppDbContext(optionsBuilder.Options);
Console.WriteLine(db.SpareParts.Count());
foreach(var sp in db.SpareParts.ToList()) {
    Console.WriteLine($"{sp.Id} - {sp.Title} - {sp.Status} - {sp.IsDeleted}");
}

