using Specwright.Core.Workflows.Contracts;
using Specwright.Infrastructure.DependencyInjection;
using Specwright.ServiceDefaults;

namespace Specwright.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddSpecwrightServiceDefaults();
        builder.AddSpecwrightInfrastructure();
        builder.Services.AddProblemDetails();

        var app = builder.Build();

        app.UseExceptionHandler();
        await app.InitializeSpecwrightInfrastructureAsync();

        app.MapSpecwrightDefaultEndpoints();

        app.MapGet("/", () => TypedResults.Ok(new
        {
            service = "Specwright.Api",
            status = "running"
        }));

        app.MapPost("/api/executions", RecordExecutionAsync);

        app.Run();
    }

    private static async Task<IResult> RecordExecutionAsync(
        RecordExecutionRequest request,
        IRecordExecutionCapability capability,
        CancellationToken cancellationToken)
    {
        var result = await capability.ExecuteAsync(request, cancellationToken);

        if (result.Output is null)
        {
            var detail = result.Errors.Count == 0
                ? "The execution record could not be persisted."
                : string.Join("; ", result.Errors.Select(error => error.Message));

            return TypedResults.Problem(
                title: "Execution persistence failed",
                detail: detail,
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return TypedResults.Created($"/api/executions/{result.Output.ExecutionRecordId}", result);
    }
}
