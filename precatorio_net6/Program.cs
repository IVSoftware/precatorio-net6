using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

var manager = new PrecatorioManager(new PrecatorioRepository());

// Illegal attempt to instantiate Precatorio();
// var doesNotCompile = new Precatorio();

// Illegal attempt to create Precatorio directly
try
{
    var notAllowed = Precatorio.Create(
        initializer: new Dictionary<string, object>
        {
            { "FornecedorId", 123 },
            { "FundoId", 456 },
            { "DestinacaoRecursoId", 798},
            { "Data", DateTime.UtcNow },
            { "Tipo", 0xeeeeffff },
            { "Valor", 1.234m },
            { "Descricao", "This is a test" },
        });
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.Message}{Environment.NewLine}");
}

// Normal create through manager
var precatorio = await manager.CreateAsync(
    init: new Dictionary<string, object>
    {
        { "FornecedorId", 123 },
        { "FundoId", 456 },
        { "DestinacaoRecursoId", 798},
        { "Data", DateTime.UtcNow },
        { "Tipo", 0xeeeeffff },
        { "Valor", 1.234m },
        { "Descricao", "This is a test" },
    });


Console.WriteLine(precatorio?.ToString());
Console.ReadKey();

internal class PrecatorioManager
{
    private PrecatorioRepository precatorioRepository;
    public PrecatorioManager(PrecatorioRepository precatorioRepository) =>
        this.precatorioRepository = precatorioRepository;
    internal async Task<Precatorio?> CreateAsync(Dictionary<string, object> init)
    {
        var created = Precatorio.Create(init);
        await ValidateAsync(created);
        return created;
    }

    private async Task<bool> ValidateAsync(Precatorio? created)
    {
        if(created == null) return false;

        Console.WriteLine("VALIDATING!");
        await Task.Delay(100); // It's up to you what is being awaited here.

        return true; // For example, return pass/fail result
    }
}

internal class Precatorio
{
    public int FornecedorId { get; set; }
    public int FundoId { get; set; }
    public int DestinacaoRecursoId { get; set; }
    public DateTime Data { get; set; }
    public long Tipo { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = String.Empty;
    private Precatorio() { }
    internal static Precatorio? Create(
        Dictionary<string, object> initializer, 
        [CallerMemberName] string? caller = null)
    {
        if (caller == "CreateAsync")
        {
            var created = new Precatorio();
            foreach (var kvp in initializer)
            {
                PropertyInfo? pi = typeof(Precatorio).GetProperty(kvp.Key);
                if (pi == null)
                {
                    System.Diagnostics.Debug.Assert(
                        false,
                        $"Expecting a property named {kvp}");
                }
                else
                {
                    pi.SetValue(created, kvp.Value);
                }
            }
            return created;
        }
        else throw new InvalidOperationException(
            $"Precatorio.Create cannot be called from {caller}."); 
    }
    public override string ToString() =>
$@"FornecedorId = {FornecedorId}
FundoId = {FundoId}
DestinacaoRecursoId = {DestinacaoRecursoId}
Data = {Data}
Tipo = {Tipo}
Valor = {Valor}
Descricao = {Descricao}
";
}