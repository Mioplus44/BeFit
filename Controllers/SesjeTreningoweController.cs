using System.Security.Claims;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers;

[Authorize]
public class SesjeTreningoweController : Controller
{
    private readonly ApplicationDbContext _db;

    public SesjeTreningoweController(ApplicationDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // Zwraca wyłącznie sesje zalogowanego użytkownika.
    private IQueryable<SesjaTreningowa> Moje()
    {
        var uid = UserId;
        return _db.SesjeTreningowe.Where(s => s.UserId == uid);
    }

    public async Task<IActionResult> Index()
        => View(await Moje().OrderByDescending(s => s.Poczatek).ToListAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var sesja = await Moje().FirstOrDefaultAsync(s => s.Id == id);
        return sesja == null ? NotFound() : View(sesja);
    }

    public IActionResult Create()
    {
        var teraz = DateTime.Now;
        var start = new DateTime(teraz.Year, teraz.Month, teraz.Day, teraz.Hour, teraz.Minute, 0);
        return View(new SesjaTreningowa { Poczatek = start, Koniec = start.AddHours(1) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Poczatek,Koniec")] SesjaTreningowa model)
    {
        if (!ModelState.IsValid) return View(model);

        model.UserId = UserId;
        _db.SesjeTreningowe.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var sesja = await Moje().FirstOrDefaultAsync(s => s.Id == id);
        return sesja == null ? NotFound() : View(sesja);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Poczatek,Koniec")] SesjaTreningowa model)
    {
        var sesja = await Moje().FirstOrDefaultAsync(s => s.Id == id);
        if (sesja == null) return NotFound();

        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        sesja.Poczatek = model.Poczatek;
        sesja.Koniec = model.Koniec;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var sesja = await Moje().FirstOrDefaultAsync(s => s.Id == id);
        return sesja == null ? NotFound() : View(sesja);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sesja = await Moje().FirstOrDefaultAsync(s => s.Id == id);
        if (sesja == null) return NotFound();

        _db.SesjeTreningowe.Remove(sesja);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
