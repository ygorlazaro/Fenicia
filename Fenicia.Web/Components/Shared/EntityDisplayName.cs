using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Fenicia.Web.Components.Shared;

public class EntityDisplayName : ComponentBase
{
    [Parameter] public object? Entity { get; set; }

    [Parameter] public string PropertyName { get; set; } = "Name";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var text = GetDisplayText();
        builder.AddContent(0, text);
    }

    private string GetDisplayText()
    {
        var entity = Entity;
        if (entity is null)
        {
            return string.Empty;
        }

        var type = entity.GetType();
        var prop = type.GetProperty(PropertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
        if (prop is not null)
        {
            return prop.GetValue(entity)?.ToString() ?? string.Empty;
        }

        var fallback = type.GetProperty("Name", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
        return fallback?.GetValue(entity)?.ToString() ?? string.Empty;
    }
}
