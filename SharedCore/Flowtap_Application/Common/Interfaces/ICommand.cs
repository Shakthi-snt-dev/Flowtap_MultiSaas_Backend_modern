using MediatR;

namespace Flowtap_Application.Common.Interfaces;

/// <summary>Marker interface for commands — enables TransactionBehavior targeting.</summary>
public interface ICommand<out TResponse> : IRequest<TResponse> { }

/// <summary>Marker interface for commands that return no value.</summary>
public interface ICommand : IRequest { }
