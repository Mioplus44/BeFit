namespace BeFit.Models;

public class StatystykaCwiczenia
{
    public string Nazwa { get; set; } = string.Empty;
    public int LiczbaWykonan { get; set; }
    public int LacznaLiczbaPowtorzen { get; set; }
    public decimal SrednieObciazenie { get; set; }
    public decimal MaksymalneObciazenie { get; set; }
}
