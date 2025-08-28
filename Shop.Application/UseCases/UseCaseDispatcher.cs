using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Interfaces.CQRS;
using Shop.Application.Interfaces.CQRS.Commands;
using Shop.Application.Interfaces.CQRS.Queries;

namespace Shop.Application.UseCases
{
    public class UseCaseDispatcher : IUseCaseDispatcher
    {
        private readonly IServiceProvider _provider;

        public UseCaseDispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
        {
            var handler = _provider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            return await handler.HandleAsync(command);
        }

        public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = _provider.GetRequiredService<ICommandHandler<TCommand>>();
            await handler.HandleAsync(command);
        }

        public async Task<TResult> DispatchQueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            var handler = _provider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return await handler.HandleAsync(query);
        }
    }
}
