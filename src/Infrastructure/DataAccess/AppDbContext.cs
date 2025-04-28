using Core.Abstractions;
using Core.Domain.Entities;
using Infrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess;

public class AppDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator _mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
        : base(options) => _mediator = mediator;
    
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<Order> Orders => Set<Order>();
}