using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BeFit.Models;

public class WykonaneCwiczenie
{
    public int Id { get; set; }

    [BindNever]
    public string? UserId { get; set; }

    [ValidateNever]
    public IdentityUser? User { get; set; }

    [Display(Name = "Typ ćwiczenia")]
    [Range(1, int.MaxValue, ErrorMessage = "Wybierz typ ćwiczenia.")]
    public int TypCwiczeniaId { get; set; }

    [ValidateNever]
    public TypCwiczenia? TypCwiczenia { get; set; }

    [Display(Name = "Sesja treningowa")]
    [Range(1, int.MaxValue, ErrorMessage = "Wybierz sesję treningową.")]
    public int SesjaTreningowaId { get; set; }

    [ValidateNever]
    public SesjaTreningowa? SesjaTreningowa { get; set; }

    [Display(Name = "Obciążenie (kg)")]
    [Required(ErrorMessage = "Podaj użyte obciążenie.")]
    [Range(0.0, 1000.0, ErrorMessage = "Obciążenie musi mieścić się w zakresie od {1} do {2} kg.")]
    [DisplayFormat(DataFormatString = "{0:0.##}")]
    public decimal Obciazenie { get; set; }

    [Display(Name = "Liczba serii")]
    [Required(ErrorMessage = "Podaj liczbę serii.")]
    [Range(1, 100, ErrorMessage = "Liczba serii musi mieścić się w zakresie od {1} do {2}.")]
    public int LiczbaSerii { get; set; }

    [Display(Name = "Liczba powtórzeń w serii")]
    [Required(ErrorMessage = "Podaj liczbę powtórzeń.")]
    [Range(1, 1000, ErrorMessage = "Liczba powtórzeń musi mieścić się w zakresie od {1} do {2}.")]
    public int LiczbaPowtorzen { get; set; }
}
