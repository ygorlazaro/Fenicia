using Fenicia.Integration.RunCommandTool;
using Fenicia.Integration.RunCommandTool.Providers;

using Spectre.Console;

const string baseUrl = "http://localhost:5144";

var godEmail = AnsiConsole.Prompt(new TextPrompt<string>("Enter god email: "));
var godPassword = AnsiConsole.Prompt(new TextPrompt<string>("Enter god password: ").Secret());

AnsiConsole.WriteLine("Getting token...");

var tokenProvider = new TokenProvider(baseUrl);
var godToken = await tokenProvider.DoLoginAsync(godEmail, godPassword);

AnsiConsole.Background = Color.DarkBlue;
AnsiConsole.Clear();

AnsiConsole.WriteLine("Token received!");

SelectionOption option;
var choices = new SelectionOption[]
{
    new (0, "-- EXIT --"),
    new (1, "1 - Create new user with new company"),
    new (2, "2 - Create modules database for a company")
};

var userProvider = new UserProvider(baseUrl);
var userMock = userProvider.CreateUserMock();

do
{
    option = AnsiConsole.Prompt(new SelectionPrompt<SelectionOption>()
                        .Title("What to do?").AddChoices(choices).UseConverter(x => x.description));

    switch (option.id)
    {
        case 1:
            var createdUser = await userProvider.CreateNewUserAsync(userMock);
            var userToken = await tokenProvider.DoLoginAsync(userMock.Email, userMock.Password);

            AnsiConsole.WriteLine($"User {userMock.Email} created with ID {createdUser.Id}");

            userProvider.SetToken(userToken.AccessToken);

            AnsiConsole.WriteLine("Searching modules...");

            var moduleProvider = new ModuleProvider(baseUrl);
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

            await tokenProvider.DoLoginAsync(userMock.Email, userMock.Password);

            break;

        case 2:
            var cnpj = AnsiConsole.Prompt(new TextPrompt<string>("Enter company's CNPJ: "));
            var migrationProvider = new MigrationProvider(baseUrl, godToken.AccessToken);

            Console.WriteLine("Creating companies databases...");

            await migrationProvider.CreateDatabasesAsync(cnpj);

            break;
    }
}
while (option.id != 0);
