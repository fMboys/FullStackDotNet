using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
public class LogiTrackContext : IdentityDbContext<IdentityUser>
{
public DbSet<InventoryItem> InventoryItems { get; set; }
public DbSet<Order> Orders { get; set; }
protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite("Data Source=logitrack.db");
}