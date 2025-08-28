﻿using Shop.Application.Interfaces.CQRS.Commands;
using Shop.Application.Interfaces.CQRS.Queries;


namespace Shop.Application.Interfaces.CQRS
{
    public interface IUseCaseDispatcher
    {
        Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;
        Task<TResult> DispatchQueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
}
