using System.Security.Claims;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers;

[Authorize]
public class WykonaneCwiczeniaController : Controller
{
    private readonly ApplicationDbContext _db;

    public WykonaneCwiczeniaController(ApplicationDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private IQueryable<WykonaneCwiczenie> Moje()
    {
        var uid = UserId;
        return _db.WykonaneCwiczenia
            .Include(w => w.TypCwiczenia)
            .Include(w => w.SesjaTreningowa)
            .Where(w => w.UserId == uid);
    }

    public async Task<IActionResult> Index()
        => View(await Moje().OrderByDescending(w => w.SesjaTreningowa!.Poczatek).ToListAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var cwiczenie = await Moje().FirstOrDefaultAsync(w => w.Id == id);
        return cwiczenie == null ? NotFound() : View(cwiczenie);
    }

    public async Task<IActionResult> Create()
    {
        await ZaladujListy();
        return View(new WykonaneCwiczenie { LiczbaSerii = 3, LiczbaPowtorzen = 10 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("TypCwiczeniaId,SesjaTreningowaId,Obciazenie,LiczbaSerii,LiczbaPowtorzen")] WykonaneCwiczenie model)
    {
        await SprawdzPowiazania(model);
        if (!ModelState.IsValid)
        {
            await ZaladujListy(model.TypCwiczeniaId, model.SesjaTreningowaId);
            return View(model);
        }

        model.UserId = UserId;
        _db.WykonaneCwiczenia.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var cwiczenie = await Moje().FirstOrDefaultAsync(w => w.Id == id);
        if (cwiczenie == null) return NotFound();

        await ZaladujListy(cwiczenie.TypCwiczeniaId, cwiczenie.SesjaTreningowaId);
        return View(cwiczenie);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("TypCwiczeniaId,SesjaTreningowaId,Obciazenie,LiczbaSerii,LiczbaPowtorzen")] WykonaneCwiczenie model)
    {
        var cwiczenie = await Moje().FirstOrDefaultAsync(w => w.Id == id);
        if (cwiczenie == null) return NotFound();

        await SprawdzPowiazania(model);
        if (!ModelState.IsValid)
        {
            model.Id = id;
            await ZaladujListy(model.TypCwiczeniaId, model.SesjaTreningowaId);
            return View(model);
        }

        cwiczenie.TypCwiczeniaId = model.TypCwiczeniaId;
        cwiczenie.SesjaTreningowaId = model.SesjaTreningowaId;
        cwiczenie.Obciazenie = model.Obciazenie;
        cwiczenie.LiczbaSerii = model.LiczbaSerii;
        cwiczenie.LiczbaPowtorzen = model.LiczbaPowtorzen;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var cwiczenie = await Moje().FirstOrDefaultAsync(w => w.Id == id);
        return cwiczenie == null ? NotFound() : View(cwiczenie);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cwiczenie = await Moje().FirstOrDefaultAsync(w => w.Id == id);
        if (cwiczenie == null) return NotFound();

        _db.WykonaneCwiczenia.Remove(cwiczenie);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Typ musi istnieć, a sesja musi należeć do zalogowanego użytkownika.
    private async Task SprawdzPowiazania(WykonaneCwiczenie model)
    {
        var uid = UserId;
        if (!await _db.TypyCwiczen.AnyAsync(t => t.Id == model.TypCwiczeniaId))
        {
            ModelState.AddModelError(nameof(model.TypCwiczeniaId), "Wybierz istniejący typ ćwiczenia.");
        }
        if (!await _db.SesjeTreningowe.AnyAsync(s => s.Id == model.SesjaTreningowaId && s.UserId == uid))
        {
            ModelState.AddModelError(nameof(model.SesjaTreningowaId), "Wybierz jedną ze swoich sesji treningowych.");
        }
    }

    private async Task ZaladujListy(int? typ = null, int? sesja = null)
    {
        var uid = UserId;
        var typy = await _db.TypyCwiczen.OrderBy(t => t.Nazwa).ToListAsync();
        var sesje = await _db.SesjeTreningowe
            .Where(s => s.UserId == uid)
            .OrderByDescending(s => s.Poczatek)
            .ToListAsync();

        ViewBag.Typy = new SelectList(typy, nameof(TypCwiczenia.Id), nameof(TypCwiczenia.Nazwa), typ);
        ViewBag.Sesje = new SelectList(
            sesje.Select(s => new
            {
                s.Id,
                Opis = $"{s.Poczatek:dd.MM.yyyy HH:mm} – {s.Koniec:HH:mm}"
            }),
            "Id", "Opis", sesja);
    }
}
