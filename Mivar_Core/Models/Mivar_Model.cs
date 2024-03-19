namespace Mivar_Core.Models
{
    public class Mivar_Model
    {
        public FinalReached FinalReached;
        public Dictionary<string, Mivar_Class> classes = new();
        public Dictionary<string, Mivar_Parameter> parameters = new();
        public Dictionary<string, Mivar_Rule> rules = new();
        public Dictionary<string, Mivar_Relation> relations = new();
        public Dictionary<string, object> results = new();

        private int unknownParams_count = 0;
        private List<Mivar_Rule> rules_to_solve = new();

        private List<string> final_params_ids = new();

        public Mivar_Model()
        {
            FinalReached = () =>
            {
                unknownParams_count -= 1;
            };
        }
        public void SolveModel(List<(string, object)> initial_params, List<string> results)
        {
            ClearResults();
            InitResultParameters(results);
            var temp_params = InitKnownParameters(initial_params);
            var temp_rules = GetRules(temp_params);
            rules_to_solve.AddRange(temp_rules);

            if (temp_rules.Count == 0)
            {
                Console.WriteLine("По входным данным нельзя получить ни одно новое значение");
                return;
            }

            while (unknownParams_count != 0)
            {
                temp_params = GetParams(temp_rules);
                temp_rules = GetRules(temp_params);
                if (temp_rules.Count == 0)
                {
                    break;
                }
                rules_to_solve.AddRange(temp_rules);
            }

            if (unknownParams_count == 0)
            {
                Console.WriteLine("Алгоритм получен!");
                PrintResults();
            }
            else
            {
                // Попытаться найти самое близкое к решению правило (правила)
                //
            }

            Console.WriteLine("Решение закончено!");

        }

        private void ClearResults()
        {
            results.Clear();
            final_params_ids.Clear();
            rules_to_solve.Clear();
            foreach (var rule in rules)
            {
                rule.Value.ResetInputsCount();
            }
            foreach (var parameter in parameters)
            {
                parameter.Value.SetAsNotReached();
            }
        }

        public void SolveModel_Test1()
        {
            ClearResults();

            InitResultParameters_Test1();

            var temp_params = InitKnownParameters_Test1();
            var temp_rules = GetRules(temp_params);
            rules_to_solve.AddRange(temp_rules);

            while (unknownParams_count != 0)
            {
                temp_params = GetParams(temp_rules);
                temp_rules = GetRules(temp_params);
                if (temp_rules.Count == 0)
                {
                    break;
                }
                rules_to_solve.AddRange(temp_rules);
            }

            if (unknownParams_count == 0)
            {
                Console.WriteLine("Алгоритм получен!");
                PrintResults();
            }
            else
            {
                // Попытаться найти самое близкое к решению правило (правила)
                //
            }

            Console.WriteLine("Решение закончено!");

        }

        public void SolveModel_Test2()
        {
            ClearResults();

            InitResultParameters_Test2();

            var temp_params = InitKnownParameters_Test2();
            var temp_rules = GetRules(temp_params);
            rules_to_solve.AddRange(temp_rules);

            while (unknownParams_count != 0)
            {
                temp_params = GetParams(temp_rules);
                temp_rules = GetRules(temp_params);
                if (temp_rules.Count == 0)
                {
                    break;
                }
                rules_to_solve.AddRange(temp_rules);
            }

            if (unknownParams_count == 0)
            {
                Console.WriteLine("Алгоритм получен!");
                PrintResults();
            }
            else
            {
                // Попытаться найти самое близкое к решению правило (правила)
                //
            }

            Console.WriteLine("Решение закончено!");

        }

        private void PrintResults()
        {

            Console.WriteLine("\t\tУсловия:");
            Console.WriteLine(new string('_', 33));
            Console.WriteLine(string.Format("|{0,15}|{1,15}|", "Имя", "Значение"));
            foreach (var init_param in results)
            {
                Console.WriteLine(string.Format("|{0,15}|{1,15}|", parameters[init_param.Key].shortName, init_param.Value));
            }
            
            Console.WriteLine(new string('_', 33));
            Console.WriteLine("Нахождение результата...");
            
            SolveAlgorythm(rules_to_solve);
            
            Console.WriteLine("\t\tРезультаты получены:");
            Console.WriteLine(new string('_', 33));
            Console.WriteLine(string.Format("|{0,15}|{1,15}|", "Имя", "Значение"));
            foreach (var final_param in final_params_ids)
            {
                Console.WriteLine(string.Format("|{0,15}|{1,15}|", parameters[final_param].shortName, results[final_param]));
            }
            
            Console.WriteLine(new string('_', 33));
        }

        private void SolveAlgorythm(List<Mivar_Rule> rules_to_solve)
        {
            foreach (var rule in rules_to_solve)
            {
                rule.Solve(this);
            }
        }

        private List<Mivar_Parameter> InitKnownParameters(List<(string, object)> init_values)
        {
            List<string> keys = GetIDsByNames(init_values.Select(p => p.Item1).ToList());
            var result = GetParamsByKeys(keys);
            result.ForEach(param =>
            {
                param.condition = ParameterCondition.Initial;
                param.value = init_values.Where(p => p.Item1 == param.shortName).First().Item2;
                results.Add(param.id, param.value);
            });
            return result;
        }

        private List<Mivar_Parameter> GetParams(List<Mivar_Rule> step1_r)
        {
            List<string> step1_p_keys = GetParamKeysByRules(step1_r);
            var step1_p = GetParamsByKeys(step1_p_keys);
            return step1_p;
        }

        private static List<string> GetParamKeysByRules(List<Mivar_Rule> step1_r)
        {
            var a1 = step1_r.Select(rule => rule.result_ids.Keys.ToList());
            var a2 = a1.Aggregate((a, b) => a.Union(b).ToList());
            return a2;
        }

        private List<Mivar_Rule> GetRules(List<Mivar_Parameter> params_list)
        {
            return params_list.Select(x => x.SetAsReached()).Aggregate((a, b) => a.UnionBy(b, l => l.id).ToList());
        }

        private void InitResultParameters(List<string> result_keys)
        {
            var result_ids = GetIDsByNames(result_keys);
            final_params_ids.AddRange(result_ids);
            foreach (var final_param in final_params_ids)
            {
                var param = parameters[final_param];
                param.condition = ParameterCondition.Final;
                param.finalReached_delegate = FinalReached;
            }
            unknownParams_count = final_params_ids.Count;
        }

        private List<Mivar_Parameter> GetParamsByKeys(List<string> step0_p_keys)
        {
            return step0_p_keys.Select(key => parameters[key]).ToList();
        }

        private List<Mivar_Parameter> InitKnownParameters_Test1()
        {
            // example of initializing parameters
            List<string> step0_p_keys = new()
        {
            "94f1c97c-fda1-440d-bdcf-0e6cb302034f",
            "bafa6be8-e858-4c67-8b49-44a422dc4717",
            "2faa4983-f235-42be-8c4f-0e44f62228ac",
            "fe0858a6-bc64-4e9c-929e-6338984ca62c",
        };
            var result = GetParamsByKeys(step0_p_keys);
            result.ForEach(param =>
            {
                param.condition = ParameterCondition.Initial;
                param.value = param.id == "fe0858a6-bc64-4e9c-929e-6338984ca62c" ? 0 : 1;
                results.Add(param.id, param.value);
            });
            return result;
        }

        private void InitResultParameters_Test1()
        {
            final_params_ids.Add("853490f9-d8c2-4366-a840-7dc325bceba6");
            final_params_ids.Add("6b14a9df-4c5a-495f-949b-3064ef63624d");

            foreach (var final_param in final_params_ids)
            {
                var param = parameters[final_param];
                param.condition = ParameterCondition.Final;
                param.finalReached_delegate = FinalReached;
            }
            unknownParams_count = final_params_ids.Count;
        }

        private List<Mivar_Parameter> InitKnownParameters_Test2()
        {
            List<string> names = new()
        {
            "1,0_type0",
            "0,0",
            "type0",
            "1,1_type0",
        };
            List<string> step0_p_keys = GetIDsByNames(names);
            // example of initializing parameters
            var result = GetParamsByKeys(step0_p_keys);
            result.ForEach(param =>
            {
                param.condition = ParameterCondition.Initial;
                param.value = param.shortName == "0,0" ? 0 : 1;
                results.Add(param.id, param.value);
            });
            return result;
        }

        private List<string> GetIDsByNames(List<string> names)
        {
            List<string> result = new();
            foreach (var name in names)
            {
                result.Add(parameters.Where(p => p.Value.shortName == name).First().Value.id);
            }
            return result;
        }

        private void InitResultParameters_Test2()
        {
            final_params_ids.Add("a79320e3-b897-4475-a4f2-635cc3457650");
            final_params_ids.Add("6b14a9df-4c5a-495f-949b-3064ef63624d");

            foreach (var final_param in final_params_ids)
            {
                var param = parameters[final_param];
                param.condition = ParameterCondition.Final;
                param.finalReached_delegate = FinalReached;
            }
            unknownParams_count = final_params_ids.Count;
        }
    }
}