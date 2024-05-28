using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Transactions;

public abstract class RepositoryBase<TContext> where TContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    protected RepositoryBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
    /// <summary>
    /// Ejecuta una operación de consulta dentro de un ámbito de transacción suprimida.
    /// </summary>
    /// <typeparam name="TResult">El tipo del resultado devuelto por la operación.</typeparam>
    /// <param name="operation">La operación a ejecutar, que toma un DbContext y devuelve un Task de TResult.</param>
    /// <param name="ct">El token de cancelación para observar mientras se espera a que se complete la tarea.</param>
    /// <returns>Un Task que representa el resultado de la operación.</returns>
    protected async Task<TResult> ExecuteQueryAsync<TResult>(Func<TContext, Task<TResult>> operation, CancellationToken ct = default)
    {
        // Comprobar si el token ya ha sido cancelado antes de comenzar
        ct.ThrowIfCancellationRequested();
        // Suprime el contexto de transacción ambiental para la operación
        using (var tx = new TransactionScope(
            TransactionScopeOption.Suppress,
            new TransactionOptions
            {
                Timeout = TimeSpan.FromMinutes(5),
                IsolationLevel = IsolationLevel.ReadCommitted
            },
            TransactionScopeAsyncFlowOption.Enabled))
        // Crea un nuevo ámbito para obtener una instancia de DbContext del proveedor de servicios
        using (var scope = _serviceProvider.CreateScope())
        {
            // Resuelve la instancia de DbContext del ámbito
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            // Ejecuta la operación proporcionada con el DbContext resuelto
            var currentTransaction = Transaction.Current;
            if (currentTransaction != null)
            {
                using (ct.Register(currentTransaction.Rollback))
                {
                    // Ejecuta la operación proporcionada con el DbContext resuelto y el CancellationToken
                    return await operation(dbContext);
                }
            }
            else
            {
                // Ejecuta la operación proporcionada con el DbContext resuelto sin registrar el token de cancelación
                return await operation(dbContext);
            }
        }
    }
    /// <summary>
    /// Ejecuta una operación de comando dentro de un nuevo ámbito de transacción.
    /// </summary>
    /// <typeparam name="TResult">El tipo del resultado devuelto por la operación.</typeparam>
    /// <param name="operation">La operación a ejecutar, que toma un DbContext y devuelve un Task de TResult.</param>
    /// <param name="ct">El token de cancelación para observar mientras se espera a que se complete la tarea.</param>
    /// <returns>Un Task que representa el resultado de la operación.</returns>
    protected async Task<TResult> ExecuteCommandAsync<TResult>(Func<TContext, Task<TResult>> operation, CancellationToken ct = default)
    {
        // Comprobar si el token ya ha sido cancelado antes de comenzar
        ct.ThrowIfCancellationRequested();
        // Crea un nuevo ámbito de transacción para la operación
        using (var tx = new TransactionScope(
            TransactionScopeOption.RequiresNew,
            new TransactionOptions
            {
                Timeout = TimeSpan.FromMinutes(5),
                IsolationLevel = IsolationLevel.ReadCommitted
            },
            TransactionScopeAsyncFlowOption.Enabled))
        // Crea un nuevo ámbito para obtener una instancia de DbContext del proveedor de servicios
        using (var scope = _serviceProvider.CreateScope())
        {
            // Resuelve la instancia de DbContext del ámbito
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            // Verifica si la transacción actual no es nula y registra el token de cancelación
            var currentTransaction = Transaction.Current;
            if (currentTransaction != null)
            {
                using (ct.Register(currentTransaction.Rollback))
                {
                    // Ejecuta la operación proporcionada con el DbContext resuelto y el CancellationToken
                    var res = await operation(dbContext);
                    tx.Complete();
                    return res;
                }
            }
            else
            {
                // Ejecuta la operación proporcionada con el DbContext resuelto sin registrar el token de cancelación
                var res = await operation(dbContext);
                tx.Complete();
                return res;
            }
        }
    }
    /// <summary>
    /// Ejecuta una operación asincrónica sin resultado dentro de un ámbito de transacción.
    /// </summary>
    /// <param name="operation">Función que representa la operación a ejecutar.</param>
    /// <param name="ct">Token de cancelación opcional.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    protected async Task ExecuteCommandAsync(Func<TContext, Task> operation, CancellationToken ct = default)
    {
        // Comprobar si el token ya ha sido cancelado antes de comenzar
        ct.ThrowIfCancellationRequested();
        // Crea un nuevo ámbito de transacción para la operación
        using (var tx = new TransactionScope(
            TransactionScopeOption.RequiresNew,
            new TransactionOptions
            {
                Timeout = TimeSpan.FromMinutes(5),
                IsolationLevel = IsolationLevel.ReadCommitted
            },
            TransactionScopeAsyncFlowOption.Enabled))
        // Crea un nuevo ámbito para obtener una instancia de DbContext del proveedor de servicios
        using (var scope = _serviceProvider.CreateScope())
        {
            // Resuelve la instancia de DbContext del ámbito
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            // Ejecuta la operación proporcionada con el DbContext resuelto
            var currentTransaction = Transaction.Current;
            if (currentTransaction != null)
            {
                using (ct.Register(currentTransaction.Rollback))
                {
                    // Ejecuta la operación proporcionada con el DbContext resuelto y el CancellationToken
                    await operation(dbContext);
                    tx.Complete();
                }
            }
            else
            {
                // Ejecuta la operación proporcionada con el DbContext resuelto sin registrar el token de cancelación
                await operation(dbContext);
                tx.Complete();
            }
        }
    }
}
