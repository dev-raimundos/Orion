namespace Api.Diagnostics;

public static class StatusEndpointExtensions
{
    private const string BuildConfiguration =
#if DEBUG
        "Debug";
#else
        "Release";
#endif

    public static IEndpointRouteBuilder MapStatusPage(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => Results.Content(
            $"""
            <!DOCTYPE html>
            <html lang="pt-br">
            <head>
                <meta charset="utf-8" />
                <title>Orion API</title>
            </head>
            <body>
                <h1>Orion API está no ar</h1>
                <p>Build: {BuildConfiguration}</p>
            </body>
            </html>
            """,
            "text/html"));

        return app;
    }
}
