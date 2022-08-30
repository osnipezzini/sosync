using SOCore;
using SOCore.Exceptions;
using SOCore.Services;
using Microsoft.EntityFrameworkCore;

using SOCore.Utils;
using SOFramework;
using SOLogging;
using SOSync.Common;
using SOSync.Service;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");

#if DEBUG
Environment.SetEnvironmentVariable("SOLOGLEVEL", "10");
Environment.SetEnvironmentVariable("SODEBUG", "1");
Environment.SetEnvironmentVariable("SOTECHDEV", "1");
#endif


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSOCore();
        services.AddSOFramework();
        services.AddHostedService<Worker>();
        services.ConfigureCommonServices();
    })
    .UseSOLogging()
    .Build();

var serviceScope = host.Services.CreateScope();


checkLicense:
var licenseService = serviceScope.ServiceProvider.GetRequiredService<ILicenseService>();
if (licenseService.HasLicense && licenseService.License.IsValid)
    await host.RunAsync();
else
{
    Console.WriteLine("Licença do sistema inválida, insira os dados para efetuar o registro.");
    Console.WriteLine();
    Console.Write("CPF / CNPJ: ");
    var document = StringUtils.FormatCpfCnpj(Console.ReadLine());
    Console.Write("Digite a senha: ");
    var password = string.Empty;
    ConsoleKey pressedKey;
    do
    {
        var keyInfo = Console.ReadKey(intercept: true);
        pressedKey = keyInfo.Key;

        if (pressedKey is ConsoleKey.Backspace && password.Length > 0)
        {
            Console.Write("\b \b");
            password = password[0..^1];
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            Console.Write("*");
            password += keyInfo.KeyChar;
        }
    } while (pressedKey is not ConsoleKey.Enter);
    Console.WriteLine();

    try
    {
        await licenseService.RegisterDeviceAsync(document, password);
    }
    catch (LicenseNotFoundException)
    {
        Console.WriteLine("Dados inválidos, verifique os dados informados e tente novamente!");
        Console.WriteLine("Pressione qualquer tecla para continuar ...");
        Console.ReadKey();
    }
    goto checkLicense;
}

await host.RunAsync();