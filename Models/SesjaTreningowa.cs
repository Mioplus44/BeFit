using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BeFit.Models;

public class SesjaTreningowa : IValidatableObject
{
    public int Id { get; set; }

    [BindNever]
    public string? UserId { get; set; }

    [ValidateNever]
    public IdentityUser? User { get; set; }

    [Display(Name = "Początek sesji")]
    [Required(ErrorMessage = "Podaj datę i godzinę rozpoczęcia sesji.")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    public DateTime Poczatek { get; set; }

    [Display(Name = "Koniec sesji")]
    [Required(ErrorMessage = "Podaj datę i godzinę zakończenia sesji.")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    public DateTime Koniec { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Poczatek.Year < 2000 || Poczatek.Year > 2100)
        {
            yield return new ValidationResult("Początek sesji musi mieścić się w latach 2000–2100.", new[] { nameof(Poczatek) });
        }
        if (Koniec <= Poczatek)
        {
            yield return new ValidationResult("Koniec sesji musi być późniejszy niż jej początek.", new[] { nameof(Koniec) });
        }
        else if (Koniec - Poczatek > TimeSpan.FromHours(24))
        {
            yield return new ValidationResult("Sesja nie może trwać dłużej niż 24 godziny.", new[] { nameof(Koniec) });
        }
    }
}
