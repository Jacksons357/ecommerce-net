using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Infrastructure.Data;

namespace Ecommerce.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EcommerceDbContext _context;
    private IDbContextTransaction? _transaction;

    private IGenericRepository<Usuario>? _usuarios;
    private IGenericRepository<Cliente>? _clientes;
    private IGenericRepository<Produto>? _produtos;
    private IPedidoRepository? _pedidos;
    private IGenericRepository<PedidoItem>? _pedidoItens;
    private IGenericRepository<HistoricoEvento>? _historicoEventos;

    public UnitOfWork(EcommerceDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Usuario> Usuarios =>
        _usuarios ??= new GenericRepository<Usuario>(_context);

    public IGenericRepository<Cliente> Clientes =>
        _clientes ??= new GenericRepository<Cliente>(_context);

    public IGenericRepository<Produto> Produtos =>
        _produtos ??= new GenericRepository<Produto>(_context);

    public IPedidoRepository Pedidos =>
        _pedidos ??= new PedidoRepository(_context);

    public IGenericRepository<PedidoItem> PedidoItens =>
        _pedidoItens ??= new GenericRepository<PedidoItem>(_context);

    public IGenericRepository<HistoricoEvento> HistoricoEventos =>
        _historicoEventos ??= new GenericRepository<HistoricoEvento>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
