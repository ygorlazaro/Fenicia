namespace Fenicia.Web.Models
{
    public class ModulesResponse
    {
        public List<Module> Data { get; set; } = new();
        public int Status { get; set; }
        public MessageInfo Message { get; set; } = new();
    }

    public class Module
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Amount { get; set; }
        public int Type { get; set; }
        public List<Submodule> Submodules { get; set; } = new();
    }

    public class Submodule
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
    }

    public class MessageInfo
    {
        public string Message { get; set; } = string.Empty;
    }
}
