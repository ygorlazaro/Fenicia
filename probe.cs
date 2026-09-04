using System;
using System.Reflection;
var asm = Assembly.LoadFrom("/home/ygor/.nuget/packages/mudblazor/9.9.0/lib/net10.0/MudBlazor.dll");
foreach (var t in asm.GetExportedTypes())
{
    if (t.Name.Contains("Dialog", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine(t.FullName);
    }
}
