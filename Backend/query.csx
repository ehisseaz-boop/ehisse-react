using EHisseler.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlite(""Data Source=ehisseler.db"");
using var db = new AppDbContext(optionsBuilder.Options);

foreach(var cat in db.Categories.ToList()) {
    Console.WriteLine($""{cat.Id} - {cat.Name}"");
}
