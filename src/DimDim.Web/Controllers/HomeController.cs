using System.Diagnostics;
using DimDim.Web.Data;
using DimDim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var resumo = new HomeSummaryViewModel
        {
            TotalClientes = await _context.Clientes.CountAsync(),
            TotalTransacoes = await _context.Transacoes.CountAsync(),
            SomaValores = await _context.Transacoes.SumAsync(t => (decimal?)t.Valor) ?? 0m
        };

        _logger.LogInformation(
            "Resumo exibido: {TotalClientes} clientes, {TotalTransacoes} transações, soma {SomaValores}",
            resumo.TotalClientes, resumo.TotalTransacoes, resumo.SomaValores);

        return View(resumo);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
