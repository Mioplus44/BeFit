using System.Security.Claims;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers;

[Authorize]
public class StatystykiController : Controller
{
    private readonly ApplicationDbContext _db;

    public StatystykiController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var od = DateTime.Now.AddDays(-28);

        var dane = await _db.WykonaneCwiczenia
            .Include(w => w.TypCwiczenia)
            .Where(w => w.UserId == uid
                        && w.SesjaTreningowa!.UserId == uid
                        && w.SesjaTreningowa.Poczatek >= od)
            .ToListAsync();

        var statystyki = dane
            .GroupBy(w => new { w.TypCwiczeniaId, w.TypCwiczenia!.Nazwa })
            .Select(g => new StatystykaCwiczenia
            {
                Nazwa = g.Key.Nazwa,
                LiczbaWykonan = g.Count(),
                LacznaLiczbaPowtorzen = g.Sum(w => w.LiczbaSerii * w.LiczbaPowtorzen),
                SrednieObciazenie = g.Average(w => w.Obciazenie),
                MaksymalneObciazenie = g.Max(w => w.Obciazenie)
            })
            .OrderBy(s => s.Nazwa)
            .ToList();

        return View(statystyki);
    }
}
