using System.Net.Http.Headers;
using System.Text.Json;
using Fenicia.Common.DTOs.Basic.Address;
using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.State;
using Fenicia.Web.Components.Shared;
using Fenicia.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic;

public partial class Customer : ComponentBase
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private List<GetAllStateResponse> _states = [];
    private CustomerFormData _add = new();
    private CustomerFormData _edit = new();

    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = default!;

    [Inject]
    private ICompanyContextService CompanyContext { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        return LoadStatesAsync();
    }

    private static AddressCommand? BuildAddress(CustomerFormData model)
    {
        if (string.IsNullOrWhiteSpace(model.Street)
            || string.IsNullOrWhiteSpace(model.Number)
            || string.IsNullOrWhiteSpace(model.ZipCode)
            || string.IsNullOrWhiteSpace(model.City)
            || model.StateId is null)
        {
            return null;
        }

        return new AddressCommand(
            model.Street!,
            model.Number!,
            model.Complement,
            model.Neighborhood,
            model.ZipCode!,
            model.StateId.Value,
            model.City!,
            model.Country);
    }

    private async Task LoadStatesAsync()
    {
        try
        {
            var client = HttpClientFactory.CreateClient("FeniciaBasic");

            var token = await CompanyContext.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var companyId = await CompanyContext.GetSelectedCompanyIdAsync();
            if (companyId.HasValue)
            {
                client.DefaultRequestHeaders.Remove("CompanyId");
                client.DefaultRequestHeaders.Add("CompanyId", companyId.Value.ToString());
            }

            var response = await client.GetAsync("state");
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<GetAllStateResponse>>(body, _jsonOptions);
                _states = items ?? [];
            }
        }
        catch
        {
            _states = [];

            throw;
        }
    }

    private void ResetForm()
    {
        _add = new CustomerFormData();
    }

    private void LoadForm(GetAllCustomerResponse? item)
    {
        _edit = item is null
            ? new CustomerFormData()
            : new CustomerFormData
            {
                Name = item.Name,
                Email = item.Email,
                PhoneNumber = item.PhoneNumber,
                Document = item.Document,
                Street = item.Address?.Street,
                Number = item.Address?.Number,
                Complement = item.Address?.Complement,
                Neighborhood = item.Address?.Neighborhood,
                ZipCode = item.Address?.ZipCode,
                City = item.Address?.City,
                StateId = item.Address is { StateId: var sid } && sid != Guid.Empty ? sid : null,
                Country = item.Address?.Country
            };
    }

    private object CreatePayload(CrudModalContext<GetAllCustomerResponse> ctx)
    {
        var model = ctx.IsAdd ? _add : _edit;
        return ctx.IsAdd
            ? new AddCustomerCommand(
                model.Name ?? string.Empty,
                model.Email,
                model.Document,
                model.PhoneNumber,
                BuildAddress(model))
            : new UpdateCustomerCommand(
                ctx.Id,
                model.Name ?? string.Empty,
                model.Email,
                model.Document,
                model.PhoneNumber,
                BuildAddress(model));
    }
}
