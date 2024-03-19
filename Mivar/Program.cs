using Mivar;
using System.Xml.Linq;

class Program
{
    static Mivar_Model model = new();
    static void Main(string[] args)
    {
        // Load the XML file
        XDocument xmlDoc = XDocument.Load("test_v3_17.03.24.xml");

        ModelParser parser = new();
        model = parser.ParseXml(xmlDoc);

        Console.WriteLine("Model loaded");
        Console.WriteLine($"Total classes: {model.classes.Count}");
        Console.WriteLine($"Total rules: {model.rules.Count}");
        Console.WriteLine($"Total parameters: {model.parameters.Count}");
        Console.WriteLine($"Total relatios: {model.relations.Count}");

        List<(string, object)> Test1_inits = new()
        {
            ("1,0_type0", 1),
            ("0,0", 0),
            ("type0", 1),
            ("1,1_type0", 1),
        };

        List<string> Test1_results = new()
        {
            "1,0",
            "1,1",
        };

        List<(string, object)> Test2_inits = new()
        {
            ("0,1_type1", 1),
            ("0,0", 0),
            ("type1", 1),
            ("1,1_type1", 1),
        };

        List<string> Test2_results = new()
        {
            "0,1",
            "1,1",
        };

        model.SolveModel(Test1_inits, Test1_results);
        model.SolveModel(Test2_inits, Test2_results);


        Console.ReadKey();
    }
}
