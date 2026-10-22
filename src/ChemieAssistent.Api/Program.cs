using System.Text.Json.Serialization;
using ChemieAssistent.Api;
using ChemieAssistent.Domain;
using ChemieAssistent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Falha na inicialização se a conexão com o banco não estiver configurada.
var connectionString = builder.Configuration.GetConnectionString("ChemieAssistent")
    ?? throw new InvalidOperationException("ConnectionString 'ChemieAssistent' não configurada.");

// Registra os serviços utilizados pelo endpoint.
// O repositório pode ser Singleton porque não mantém conexões ou estado entre consultas.
builder.Services.AddSingleton<IRepositorioElementos>(new RepositorioElementosSql(connectionString));
builder.Services.AddSingleton<ProcessadorEquacao>();

// Serializa enums como texto no JSON, facilitando a leitura das respostas da API.
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Restringe as origens e os métodos HTTP permitidos ao frontend.
var origens = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddPolicy("Front", p =>
    p.WithOrigins(origens).AllowAnyHeader().WithMethods("POST")));

// Padroniza as respostas de erros inesperados com ProblemDetails.
builder.Services.AddProblemDetails();

var app = builder.Build();

// A ordem do pipeline importa: exceções são tratadas antes da execução dos endpoints.
app.UseExceptionHandler();
app.UseCors("Front");

app.MapPost("/api/calcular", (CalcularRequest req, ProcessadorEquacao processador) =>
{
    try
    {
        var entradas = req.Massas?.Select(m => m.ParaDominio()).ToList();
        var relatorio = processador.Processar(req.Equacao ?? "", entradas);
        return Results.Ok(relatorio.ParaDto());
    }
    catch (DominioException ex)
    {
        // Converte erros de domínio em respostas HTTP 400, preservando o código do erro.
        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Entrada inválida",
            detail: ex.Message,
            extensions: new Dictionary<string, object?> { ["codigo"] = ex.Codigo.ToString() });
    }
});

app.MapGet("/", () => "Chemie Assistent API no ar.");

app.Run();