
using Jint;
using Mivar_Core.Models;

public class Mivar_Rule
{
    public static Engine JSEngine = new();
    public string id = string.Empty;//"fd82fb21-939e-4841-8ae2-019211b6bce5";
    public string shortName = string.Empty; 
    public string relation = string.Empty;//"f76ff82d-98cb-4cf3-8937-0d6f85416796";
    public Dictionary<string, string> init_ids = new(); // (id, name)
    public Dictionary<string, string> result_ids = new();
    public int params_toReady = 0;


    public Mivar_Rule? DecreaseParamsToReadyCount()
    {
        if (params_toReady == 0)
            return null;
        params_toReady -= 1;
        if (params_toReady == 0)
        {
            return this;
        }
        return null;
    }

    public void Solve(Mivar_Model model)
    {
        try
        {
            // Add parameters to the engine's global scope
            foreach (var in_param in init_ids)
            {
                JSEngine.SetValue(in_param.Value, model.results[in_param.Key]);
            }

            foreach (var out_param in result_ids)
            {
                JSEngine.SetValue(out_param.Value, 0);
            }

            // Execute the JavaScript code
            JSEngine.Execute(model.relations[relation].js_code);

            // PRINT RESULTS

            foreach (var out_param in result_ids)
            {
                var new_val = JSEngine.GetValue(out_param.Value).ToObject();
                if (model.results.TryGetValue(out_param.Key, out object prev_val))
                {
                    //model.results[out_param.Key] = new_val;
                }
                else
                {
                    model.results.Add(out_param.Key, new_val);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing JavaScript code: {ex.Message}");
        }
    }

    public void ResetInputsCount()
    {
        this.params_toReady = init_ids.Count;
    }
}
