using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Adres.Prueba.Api.Data;
using Adres.Prueba.Api.Models;

namespace Adres.Prueba.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcquisitionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AcquisitionsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/acquisitions
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? unidad,
        [FromQuery] string? tipo,
        [FromQuery] string? proveedor,
        [FromQuery] string? estado,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta)
    {
        var query = _context.Acquisitions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(unidad))
        {
            query = query.Where(a => a.Unidad.Contains(unidad));
        }
        if (!string.IsNullOrWhiteSpace(tipo))
        {
            query = query.Where(a => a.Tipo.Contains(tipo));
        }
        if (!string.IsNullOrWhiteSpace(proveedor))
        {
            query = query.Where(a => a.Proveedor.Contains(proveedor));
        }
        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (estado.Equals("ACTIVO", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(a => a.Activo);
            }
            else if (estado.Equals("INACTIVO", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(a => !a.Activo);
            }
        }
        if (fechaDesde.HasValue)
        {
            query = query.Where(a => a.FechaAdquisicion.Date >= fechaDesde.Value.Date);
        }
        if (fechaHasta.HasValue)
        {
            query = query.Where(a => a.FechaAdquisicion.Date <= fechaHasta.Value.Date);
        }

        var result = await query
            .OrderByDescending(a => a.FechaAdquisicion)
            .ToListAsync();

        return Ok(result);
    }

    // GET /api/acquisitions/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var acq = await _context.Acquisitions.FindAsync(id);
        if (acq == null) return NotFound(new { error = "No encontrado" });
        return Ok(acq);
    }

    public class AcquisitionRequest
    {
        public decimal Presupuesto { get; set; }
        public string Unidad { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal ValorUnitario { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public string Proveedor { get; set; } = string.Empty;
        public string? Documentacion { get; set; }
    }

    private string? Validate(AcquisitionRequest body)
    {
        if (body.Presupuesto < 0) return "El campo 'presupuesto' debe ser mayor o igual a 0.";
        if (string.IsNullOrWhiteSpace(body.Unidad)) return "El campo 'unidad' es obligatorio.";
        if (string.IsNullOrWhiteSpace(body.Tipo)) return "El campo 'tipo' es obligatorio.";
        if (body.Cantidad <= 0) return "El campo 'cantidad' debe ser mayor a 0.";
        if (body.ValorUnitario < 0) return "El campo 'valorUnitario' debe ser mayor o igual a 0.";
        if (string.IsNullOrWhiteSpace(body.Proveedor)) return "El campo 'proveedor' es obligatorio.";
        if (body.FechaAdquisicion == default) return "La fecha de adquisición no es válida.";
        return null;
    }

    private async Task AddHistoryAsync(int acquisitionId, string action, string summary)
    {
        _context.Histories.Add(new AcquisitionHistory
        {
            AcquisitionId = acquisitionId,
            Action = action,
            Summary = summary,
            Timestamp = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    // POST /api/acquisitions
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AcquisitionRequest request)
    {
        var error = Validate(request);
        if (error != null) return BadRequest(new { error });

        var valorTotal = request.Cantidad * request.ValorUnitario;

        var acquisition = new Acquisition
        {
            Presupuesto = request.Presupuesto,
            Unidad = request.Unidad.Trim(),
            Tipo = request.Tipo.Trim(),
            Cantidad = request.Cantidad,
            ValorUnitario = request.ValorUnitario,
            ValorTotal = valorTotal,
            FechaAdquisicion = request.FechaAdquisicion,
            Proveedor = request.Proveedor.Trim(),
            Documentacion = request.Documentacion?.Trim(),
            Activo = true
        };

        _context.Acquisitions.Add(acquisition);
        await _context.SaveChangesAsync();

        await AddHistoryAsync(acquisition.Id, "CREADO", $"Registro creado con proveedor {acquisition.Proveedor}");

        return CreatedAtAction(nameof(GetById), new { id = acquisition.Id }, acquisition);
    }

    // PUT /api/acquisitions/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AcquisitionRequest request)
    {
        var error = Validate(request);
        if (error != null) return BadRequest(new { error });

        var acquisition = await _context.Acquisitions.FindAsync(id);
        if (acquisition == null) return NotFound(new { error = "No encontrado" });

        var valorTotal = request.Cantidad * request.ValorUnitario;

        acquisition.Presupuesto = request.Presupuesto;
        acquisition.Unidad = request.Unidad.Trim();
        acquisition.Tipo = request.Tipo.Trim();
        acquisition.Cantidad = request.Cantidad;
        acquisition.ValorUnitario = request.ValorUnitario;
        acquisition.ValorTotal = valorTotal;
        acquisition.FechaAdquisicion = request.FechaAdquisicion;
        acquisition.Proveedor = request.Proveedor.Trim();
        acquisition.Documentacion = request.Documentacion?.Trim();

        await _context.SaveChangesAsync();
        await AddHistoryAsync(acquisition.Id, "ACTUALIZADO", "Campos de la adquisición actualizados");

        return Ok(acquisition);
    }

    public class StatusRequest
    {
        public bool Activo { get; set; }
    }

    // PATCH /api/acquisitions/{id}/status
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusRequest request)
    {
        var acquisition = await _context.Acquisitions.FindAsync(id);
        if (acquisition == null) return NotFound(new { error = "No encontrado" });

        acquisition.Activo = request.Activo;
        await _context.SaveChangesAsync();
        await AddHistoryAsync(acquisition.Id, "ESTADO", $"Estado cambiado a {(request.Activo ? "ACTIVO" : "INACTIVO")}");

        return Ok(acquisition);
    }

    // GET /api/acquisitions/{id}/history
    [HttpGet("{id:int}/history")]
    public async Task<IActionResult> GetHistory(int id)
    {
        var exists = await _context.Acquisitions.AnyAsync(a => a.Id == id);
        if (!exists) return NotFound(new { error = "No encontrado" });

        var history = await _context.Histories
            .Where(h => h.AcquisitionId == id)
            .OrderByDescending(h => h.Timestamp)
            .ToListAsync();

        return Ok(history);
    }
}
