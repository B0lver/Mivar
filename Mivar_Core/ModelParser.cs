using Mivar_Core.Models;
using System.Data;
using System.Xml.Linq;

namespace Mivar_Core
{
    public class ModelParser
    {
        private Mivar_Model result_model;

        public ModelParser()
        {
            result_model = new();
        }
        public Mivar_Model ParseXml(XDocument xmlDoc)
        {
            result_model = new Mivar_Model();

            var classes = xmlDoc.Descendants("class").Distinct();
            result_model.classes = ParseClasses(classes);

            var parameters = classes.Descendants("parameter").Distinct();
            result_model.parameters = ParseParameters(parameters);

            var rules = classes.Descendants("rule").Distinct();
            result_model.rules = ParseRules(rules);

            var relations = xmlDoc.Descendants("relation").Distinct();
            result_model.relations = ParseRelations(relations);

            return result_model;
        }
        private Dictionary<string, Mivar_Class> ParseClasses(IEnumerable<XElement> classes)
        {
            Dictionary<string, Mivar_Class> result_classes = new();
            foreach (var c in classes)
            {
                Mivar_Class mivar_class = new();
                mivar_class.id = c.Attribute("id").Value;
                mivar_class.shortName = c.Attribute("shortName").Value;
                result_classes.Add(mivar_class.id, mivar_class);
            }
            return result_classes;
        }

        private Dictionary<string, Mivar_Parameter> ParseParameters(IEnumerable<XElement> parameters)
        {
            Dictionary<string, Mivar_Parameter> result_parameters = new();
            foreach (var p in parameters)
            {
                Mivar_Parameter parameter = new();
                parameter.id = p.Attribute("id").Value;
                parameter.shortName = p.Attribute("shortName").Value;
                parameter.type = p.Attribute("type").Value;
                result_parameters.Add(parameter.id, parameter);
            }
            return result_parameters;
        }

        private Dictionary<string, Mivar_Rule> ParseRules(IEnumerable<XElement> rules)
        {
            Dictionary<string, Mivar_Rule> result_rules = new();
            foreach (var r_ in rules)
            {
                Mivar_Rule rule = new();
                rule.id = r_.Attribute("id").Value;
                rule.shortName = r_.Attribute("shortName").Value;
                rule.relation = r_.Attribute("relation").Value;

                var initId = r_.Attribute("initId").Value.Split(';');
                List<(string, string)> inputs = initId.Select(ExtractGuidFromId).ToList();

                var resultId = r_.Attribute("resultId").Value.Split(';');
                List<(string, string)> outputs = resultId.Select(ExtractGuidFromId).ToList();

                inputs.ForEach(elem =>
                {
                    rule.init_ids.Add(elem.Item2, elem.Item1);
                    result_model.parameters[elem.Item2].rules.Add(rule);
                    rule.ResetInputsCount();
                });

                outputs.ForEach(elem => rule.result_ids.Add(elem.Item2, elem.Item1));

                result_rules.Add(rule.id, rule);
            }

            return result_rules;
        }
        private Dictionary<string, Mivar_Relation> ParseRelations(IEnumerable<XElement> relations)
        {
            Dictionary<string, Mivar_Relation> result_relations = new();
            foreach (var r in relations)
            {
                Mivar_Relation relation = new();
                relation.id = r.Attribute("id").Value;
                relation.shortName = r.Attribute("shortName").Value;
                relation.relationType = r.Attribute("relationType").Value;

                var inObj = r.Attribute("inObj").Value.Split(';');
                List<(string, string)> inputs = inObj.Select(ExtractGuidFromId).ToList();

                var outObj = r.Attribute("outObj").Value.Split(';');
                List<(string, string)> outputs = outObj.Select(ExtractGuidFromId).ToList();

                inputs.ForEach(elem => relation.initial_objects.Add(elem.Item1, elem.Item2));
                outputs.ForEach(elem => relation.output_objects.Add(elem.Item1, elem.Item2));

                relation.js_code = r.Value;
                result_relations.Add(relation.id, relation);
            }

            return result_relations;
        }
        private static (string, string) ExtractGuidFromId(string id)
        {
            // Assuming the ID is in the format "x:GUID" or "y:GUID"
            // This method extracts the GUID part
            int index = id.IndexOf(':');
            if (index >= 0)
            {
                return (id.Substring(0, index), id.Substring(index + 1));
            }
            return ("x", id); // Return the original ID if no ':' is found
        }
    }
}