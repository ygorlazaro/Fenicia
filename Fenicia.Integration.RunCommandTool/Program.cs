#pragma warning disable SA1200 // Using directives should be placed correctly
using Fenicia.Common.Database.Responses;
using Fenicia.Integration.RunCommandTool;
using Fenicia.Integration.RunCommandTool.Providers;

using Spectre.Console;
#pragma warning restore SA1200 // Using directives should be placed correctly

var baseUrl = "http://localhost:5144";

var godEmail = AnsiConsole.Prompt(new TextPrompt<string>("Enter god email: "));
var godPassword = AnsiConsole.Prompt(new TextPrompt<string>("Enter god password: ").Secret());
var godCnpj = AnsiConsole.Prompt(new TextPrompt<string>("Enter god cnpj: "));

AnsiConsole.WriteLine("Getting token...");

var tokenProvider = new TokenProvider(baseUrl);
var godToken = await tokenProvider.DoLoginAsync(godEmail, godPassword, godCnpj);

AnsiConsole.Background = Color.DarkBlue;
AnsiConsole.Clear();

AnsiConsole.WriteLine("Token received!");

var option = new SelectionOption(0, string.Empty);
var choices = new SelectionOption[]
{
    new (0, "-- EXIT --"),
    new (1, "1 - Create new user with new company"),
    new (2, "2 - Create modules database for a company")
};

var userProvider = new UserProvider(baseUrl);
var userMock = userProvider.CreateUserMock();
var userToken = new TokenResponse();

do
{
    option = AnsiConsole.Prompt(new SelectionPrompt<SelectionOption>()
                        .Title("What to do?").AddChoices(choices).UseConverter(x => x.description));

    switch (option.id)
    {
        case 1:
            var createdUser = await userProvider.CreateNewUserAsync(userMock);
            userToken = await tokenProvider.DoLoginAsync(userMock.Email, userMock.Password, userMock.Company.Cnpj);

            AnsiConsole.WriteLine($"User {userMock.Email} created with ID {createdUser.Id}");

            userProvider.SetToken(userToken.AccessToken);

            AnsiConsole.WriteLine("Searching modules...");

            var moduleProvider = new ModuleProvider(baseUrl, userToken.AccessToken);
            var modules = await moduleProvider.GetModulesAsync();

            AnsiConsole.WriteLine($"Found {modules.Count} modules.");

            var answer = AnsiConsole.Prompt(new TextPrompt<bool>("Create a new order?")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(x => x ? "y" : "n"));

            if (answer)
            {
                Console.WriteLine("Creating order...");

                var orderProvider = new OrderProvider(baseUrl, userToken.AccessToken);

                await orderProvider.CreateOrderAsync(modules);

                Console.WriteLine("Order created");
            }

            userToken = await tokenProvider.DoLoginAsync(userMock.Email, userMock.Password, userMock.Company.Cnpj);

            break;

        case 2:
            var cnpj = AnsiConsole.Prompt(new TextPrompt<string>("Enter company's CNPJ: "));
            var migrationProvider = new MigrationProvider(baseUrl, godToken.AccessToken);

            Console.WriteLine("Creating companies databases...");

            await migrationProvider.CreateDatabasesAsync(cnpj);

            break;
        default:
            break;
    }
}
while (option.id != 0);
