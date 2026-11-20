using System.ComponentModel.DataAnnotations;

namespace BeFit.Models;

public class TypCwiczenia
{
    public int Id { get; set; }

    [Display(Name = "Nazwa ćwiczenia")]
    [Required(ErrorMessage = "Podaj nazwę ćwiczenia.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nazwa musi mieć od {2} do {1} znaków.")]
    public string Nazwa { get; set; } = string.Empty;

    [Display(Name = "Opis")]
    [StringLength(500, ErrorMessage = "Opis może mieć najwyżej {1} znaków.")]
    [DataType(DataType.MultilineText)]
    public string? Opis { get; set; }
}
