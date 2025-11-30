namespace Adres.Prueba.Api.Models;

public class Acquisition
{
    public int Id { get; set; }
    public decimal Presupuesto { get; set; }
    public string Unidad { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime FechaAdquisicion { get; set; }
    public string Proveedor { get; set; } = string.Empty;
    public string? Documentacion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<AcquisitionHistory> Histories { get; set; } = new List<AcquisitionHistory>();
}
