namespace Fenicia.Web.Services;

public static class Money
{
    private static readonly System.Globalization.CultureInfo PtBr = new("pt-BR");

    public static string Format(decimal value)
    {
        return value.ToString("C", PtBr);
    }
}
