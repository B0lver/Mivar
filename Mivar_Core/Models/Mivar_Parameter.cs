public class Mivar_Parameter
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

    public ParameterState state = ParameterState.NotReached;
    public ParameterCondition condition = ParameterCondition.Intermediate;
    public string id = string.Empty;
    public string shortName = string.Empty;
    public string type = string.Empty;
    public object value = null;

    public List<Mivar_Rule> rules = new();

    public delegate void FinalReached();

    public FinalReached? finalReached_delegate;

    public List<Mivar_Rule> SetAsReached()
    {
        if (this.state is ParameterState.Reached)
            return null;
        this.state = ParameterState.Reached;
        if (this.condition is ParameterCondition.Final)
        {
            this.finalReached_delegate?.Invoke();
        }
        return rules
            .Select(r =>r.DecreaseParamsToReadyCount())
            .Where(r => r is not null)
            .ToList() ?? new();
    }

    public void SetAsNotReached()
    {
        this.state = ParameterState.NotReached;
    }
}
