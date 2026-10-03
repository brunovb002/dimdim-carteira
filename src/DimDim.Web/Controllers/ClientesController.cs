using DimDim.Web.Data;
using DimDim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Controllers;

public class ClientesController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(AppDbContext context, ILogger<ClientesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Clientes
    public async Task<IActionResult> Index()
    {
        return View(await _context.Clientes.OrderBy(c => c.Nome).ToListAsync());
    }

    // GET: Clientes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var cliente = await _context.Clientes
            .Include(c => c.Transacoes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente is null) return NotFound();

        return View(cliente);
    }

    // GET: Clientes/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Clientes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,Email,Telefone")] Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            cliente.DataCadastro = DateTime.UtcNow;
            _context.Add(cliente);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Cliente criado: Id={ClienteId}, Nome={Nome}", cliente.Id, cliente.Nome);
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }

    // GET: Clientes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();

        return View(cliente);
    }

    // POST: Clientes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Telefone,DataCadastro")] Cliente cliente)
    {
        if (id != cliente.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cliente atualizado: Id={ClienteId}, Nome={Nome}", cliente.Id, cliente.Nome);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Clientes.AnyAsync(c => c.Id == cliente.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }

    // GET: Clientes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        if (cliente is null) return NotFound();

        return View(cliente);
    }

    // POST: Clientes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is not null)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Cliente excluído: Id={ClienteId}, Nome={Nome}", id, cliente.Nome);
        }
        return RedirectToAction(nameof(Index));
    }
}
