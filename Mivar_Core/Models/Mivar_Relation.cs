namespace Mivar_Core.Models
{
    public class Mivar_Relation
    {
        public string id = string.Empty;
        public string shortName = string.Empty;
        // name - type
        public Dictionary<string, string> initial_objects = new();
        // name - type
        public Dictionary<string, string> output_objects = new();
        public string relationType = "prog";//string.Empty;

        public string js_code = string.Empty;
    }
}