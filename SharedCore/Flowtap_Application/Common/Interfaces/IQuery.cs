using MediatR;

namespace Flowtap_Application.Common.Interfaces;

/// <summary>Marker interface for queries — excluded from TransactionBehavior.</summary>
public interface IQuery<out TResponse> : IRequest<TResponse> { }
