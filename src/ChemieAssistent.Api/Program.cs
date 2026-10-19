using System.Text.Json.Serialization;
using ChemieAssistent.Api;
using ChemieAssistent.Domain;
using ChemieAssistent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1) Lê a connection string do appsettings.json. Se faltar, falha logo na inicialização.
var connectionString = builder.Configuration.GetConnectionString("ChemieAssistent")
    ?? throw new InvalidOperationException("ConnectionString 'ChemieAssistent' não configurada.");

// 2) Injeção de dependência: registra o que a aplicação precisa.
//    Quando alguém pedir IRepositorioElementos, entrega o repositório SQL.
//    ProcessadorEquacao recebe o repositório automaticamente pelo construtor.
//    Singleton = uma única instância para toda a aplicação (seguro aqui porque
//    o repositório abre uma conexão nova a cada consulta e não guarda estado).
builder.Services.AddSingleton<IRepositorioElementos>(new RepositorioElementosSql(connectionString));
builder.Services.AddSingleton<ProcessadorEquacao>();

// 3) Enums saem como texto no JSON ("Corretos") em vez de número (1).
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// 4) CORS: libera só as origens do appsettings, só o método POST.
var origens = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddPolicy("Front", p =>
    p.WithOrigins(origens).AllowAnyHeader().WithMethods("POST")));

// 5) Formato padrão (ProblemDetails) para respostas de erro.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Pipeline: a ordem importa.
app.UseExceptionHandler();   // exceção inesperada -> 500 em ProblemDetails
app.UseCors("Front");

// 6) O endpoint. Recebe JSON, devolve JSON.
app.MapPost("/api/calcular", (CalcularRequest req, ProcessadorEquacao processador) =>
{
    try
    {
        var entradas = req.Massas?.Select(m => m.ParaDominio()).ToList();
        var relatorio = processador.Processar(req.Equacao ?? "", entradas);
        return Results.Ok(relatorio.ParaDto());                 // 200
    }
    catch (DominioException ex)                                 // erro do usuário
    {
        return Results.Problem(                                 // 400
            statusCode: StatusCodes.Status400BadRequest,
            title: "Entrada inválida",
            detail: ex.Message,
            extensions: new Dictionary<string, object?> { ["codigo"] = ex.Codigo.ToString() });
    }
});

app.MapGet("/", () => "Chemie Assistent API no ar.");

app.Run();