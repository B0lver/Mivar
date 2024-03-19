namespace Mivar_Core.Models
{
    public enum ParameterState
    {
        Reached,
        NotReached,
    }

    public enum ParameterCondition
    {
        Initial,
        Intermediate,
        Final,
    }
    public delegate void FinalReached();
    public class Mivar_Parameter
    {
        public ParameterState state = ParameterState.NotReached;
        public ParameterCondition condition = ParameterCondition.Intermediate;
        public string id = string.Empty;
        public string shortName = string.Empty;
        public string type = string.Empty;
        public object value = null;

        public List<Mivar_Rule> rules = new();

        public FinalReached? finalReached_delegate;

        public List<Mivar_Rule> SetAsReached()
        {
            if (state is ParameterState.Reached)
                return null;
            state = ParameterState.Reached;
            if (condition is ParameterCondition.Final)
            {
                finalReached_delegate?.Invoke();
            }
            return rules
                .Select(r => r.DecreaseParamsToReadyCount())
                .Where(r => r is not null)
                .ToList() ?? new();
        }

        public void SetAsNotReached()
        {
            state = ParameterState.NotReached;
        }
    }
}