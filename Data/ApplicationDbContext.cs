using BeFit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TypCwiczenia> TypyCwiczen => Set<TypCwiczenia>();
    public DbSet<SesjaTreningowa> SesjeTreningowe => Set<SesjaTreningowa>();
    public DbSet<WykonaneCwiczenie> WykonaneCwiczenia => Set<WykonaneCwiczenie>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TypCwiczenia>().HasIndex(t => t.Nazwa).IsUnique();

        builder.Entity<SesjaTreningowa>()
            .HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId)
            .IsRequired().OnDelete(DeleteBehavior.Cascade);

        builder.Entity<WykonaneCwiczenie>()
            .HasOne(w => w.User).WithMany().HasForeignKey(w => w.UserId)
            .IsRequired().OnDelete(DeleteBehavior.Cascade);

        builder.Entity<WykonaneCwiczenie>()
            .HasOne(w => w.TypCwiczenia).WithMany().HasForeignKey(w => w.TypCwiczeniaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WykonaneCwiczenie>()
            .HasOne(w => w.SesjaTreningowa).WithMany().HasForeignKey(w => w.SesjaTreningowaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
