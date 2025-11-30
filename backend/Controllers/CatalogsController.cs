using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Adres.Prueba.Api.Data;

namespace Adres.Prueba.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CatalogsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/catalogs
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var unidades = await _context.UnidadesAdministrativas
            .OrderBy(u => u.Nombre)
            .Select(u => u.Nombre)
            .ToListAsync();

        var tipos = await _context.TiposBienServicio
            .OrderBy(t => t.Nombre)
            .Select(t => t.Nombre)
            .ToListAsync();

        return Ok(new
        {
            unidadesAdministrativas = unidades,
            tiposBienServicio = tipos
        });
    }

    // GET /api/catalogs.xml
    [HttpGet("xml")]
    [Produces("application/xml")]
    public async Task<IActionResult> GetXml()
    {
        var unidades = await _context.UnidadesAdministrativas
            .OrderBy(u => u.Nombre)
            .Select(u => u.Nombre)
            .ToListAsync();

        var tipos = await _context.TiposBienServicio
            .OrderBy(t => t.Nombre)
            .Select(t => t.Nombre)
            .ToListAsync();

        string Escape(string value) =>
            value.Replace("&", "&amp;")
                 .Replace("<", "&lt;")
                 .Replace(">", "&gt;")
                 .Replace(""", "&quot;")
                 .Replace("'", "&apos;");

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<catalogos>");
        sb.AppendLine("  <unidadesAdministrativas>");
        foreach (var u in unidades)
        {
            sb.AppendLine($"    <unidad>{Escape(u)}</unidad>");
        }
        sb.AppendLine("  </unidadesAdministrativas>");
        sb.AppendLine("  <tiposBienServicio>");
        foreach (var t in tipos)
        {
            sb.AppendLine($"    <tipo>{Escape(t)}</tipo>");
        }
        sb.AppendLine("  </tiposBienServicio>");
        sb.AppendLine("</catalogos>");

        return Content(sb.ToString(), "application/xml", Encoding.UTF8);
    }
}
