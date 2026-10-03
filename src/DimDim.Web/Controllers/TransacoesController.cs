using DimDim.Web.Data;
using DimDim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Controllers;

public class TransacoesController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<TransacoesController> _logger;

    public TransacoesController(AppDbContext context, ILogger<TransacoesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Transacoes
    public async Task<IActionResult> Index()
    {
        var transacoes = _context.Transacoes
            .Include(t => t.Cliente)
            .OrderByDescending(t => t.DataTransacao);

        return View(await transacoes.ToListAsync());
    }

    // GET: Transacoes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var transacao = await _context.Transacoes
            .Include(t => t.Cliente)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transacao is null) return NotFound();

        return View(transacao);
    }

    // GET: Transacoes/Create
    public IActionResult Create()
    {
        ViewData["ClienteId"] = new SelectList(_context.Clientes.OrderBy(c => c.Nome), "Id", "Nome");
        return View();
    }

    // POST: Transacoes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ClienteId,Descricao,Valor,Tipo,Status")] Transacao transacao)
    {
        if (ModelState.IsValid)
        {
            transacao.DataTransacao = DateTime.UtcNow;
            _context.Add(transacao);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Transação criada: Id={TransacaoId}, ClienteId={ClienteId}, Valor={Valor}",
                transacao.Id, transacao.ClienteId, transacao.Valor);
            return RedirectToAction(nameof(Index));
        }

        ViewData["ClienteId"] = new SelectList(_context.Clientes.OrderBy(c => c.Nome), "Id", "Nome", transacao.ClienteId);
        return View(transacao);
    }

    // GET: Transacoes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var transacao = await _context.Transacoes.FindAsync(id);
        if (transacao is null) return NotFound();

        ViewData["ClienteId"] = new SelectList(_context.Clientes.OrderBy(c => c.Nome), "Id", "Nome", transacao.ClienteId);
        return View(transacao);
    }

    // POST: Transacoes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,Descricao,Valor,Tipo,Status,DataTransacao")] Transacao transacao)
    {
        if (id != transacao.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(transacao);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Transação atualizada: Id={TransacaoId}, Status={Status}", transacao.Id, transacao.Status);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Transacoes.AnyAsync(t => t.Id == transacao.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["ClienteId"] = new SelectList(_context.Clientes.OrderBy(c => c.Nome), "Id", "Nome", transacao.ClienteId);
        return View(transacao);
    }

    // GET: Transacoes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var transacao = await _context.Transacoes
            .Include(t => t.Cliente)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transacao is null) return NotFound();

        return View(transacao);
    }

    // POST: Transacoes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var transacao = await _context.Transacoes.FindAsync(id);
        if (transacao is not null)
        {
            _context.Transacoes.Remove(transacao);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Transação excluída: Id={TransacaoId}", id);
        }
        return RedirectToAction(nameof(Index));
    }
}
