namespace Adres.Prueba.Api.Models;

public class AcquisitionHistory
{
    public int Id { get; set; }
    public int AcquisitionId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    public Acquisition Acquisition { get; set; } = null!;
}
