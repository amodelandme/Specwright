namespace Specwright.Core.Capabilities.Abstractions;

public interface ICapability<in TRequest, TResult>
    where TRequest : class
    where TResult : class
{
    string Id { get; }

    string Name { get; }

    CapabilityCategory Category { get; }

    CapabilityExecutionType ExecutionType { get; }

    Task<CapabilityResult<TResult>> ExecuteAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
