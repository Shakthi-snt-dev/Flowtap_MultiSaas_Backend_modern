using System.Threading;
using System.Threading.Tasks;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Organization.Store.Commands.DeleteStore;

public class DeleteStoreCommandHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteStoreCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteStoreCommand request, CancellationToken ct)
    {
        var store = await db.Stores.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (store == null) return Result<Unit>.Failure("Store not found.");

        store.IsActive = false;

        await db.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
