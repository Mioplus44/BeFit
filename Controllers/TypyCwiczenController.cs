using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers;

public class TypyCwiczenController : Controller
{
    private const string Admin = DbSeeder.RolaAdmin;
    private readonly ApplicationDbContext _db;

    public TypyCwiczenController(ApplicationDbContext db) => _db = db;

    [AllowAnonymous]
    public async Task<IActionResult> Index()
        => View(await _db.TypyCwiczen.OrderBy(t => t.Nazwa).ToListAsync());

    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var typ = await _db.TypyCwiczen.FirstOrDefaultAsync(t => t.Id == id);
        return typ == null ? NotFound() : View(typ);
    }

    [Authorize(Roles = Admin)]
    public IActionResult Create() => View(new TypCwiczenia());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Admin)]
    public async Task<IActionResult> Create([Bind("Nazwa,Opis")] TypCwiczenia model)
    {
        await SprawdzUnikalnosc(model, null);
        if (!ModelState.IsValid) return View(model);

        _db.TypyCwiczen.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = Admin)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var typ = await _db.TypyCwiczen.FindAsync(id);
        return typ == null ? NotFound() : View(typ);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Admin)]
    public async Task<IActionResult> Edit(int id, [Bind("Nazwa,Opis")] TypCwiczenia model)
    {
        var typ = await _db.TypyCwiczen.FindAsync(id);
        if (typ == null) return NotFound();

        await SprawdzUnikalnosc(model, id);
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        typ.Nazwa = model.Nazwa;
        typ.Opis = model.Opis;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = Admin)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var typ = await _db.TypyCwiczen.FirstOrDefaultAsync(t => t.Id == id);
        return typ == null ? NotFound() : View(typ);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = Admin)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var typ = await _db.TypyCwiczen.FindAsync(id);
        if (typ == null) return NotFound();

        if (await _db.WykonaneCwiczenia.AnyAsync(w => w.TypCwiczeniaId == id))
        {
            TempData["Blad"] = "Nie można usunąć tego typu ćwiczenia, ponieważ został już użyty w wykonanych ćwiczeniach.";
            return RedirectToAction(nameof(Index));
        }

        _db.TypyCwiczen.Remove(typ);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task SprawdzUnikalnosc(TypCwiczenia model, int? id)
    {
        if (string.IsNullOrWhiteSpace(model.Nazwa)) return;
        var istnieje = await _db.TypyCwiczen.AnyAsync(t => t.Nazwa == model.Nazwa && t.Id != id);
        if (istnieje)
        {
            ModelState.AddModelError(nameof(model.Nazwa), "Ćwiczenie o takiej nazwie już istnieje.");
        }
    }
}
