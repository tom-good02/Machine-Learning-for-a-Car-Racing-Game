
public class TerminationConditionSettings
{
    private bool _useMaxGenerations;
    
    private int _maxGenerations;
    private int _maxSteps;
    
    public TerminationConditionSettings(bool useMaxGenerations, int maxGenerations, int maxSteps)
    {
        _useMaxGenerations = useMaxGenerations;
        _maxGenerations = maxGenerations;
        _maxSteps = maxSteps;
    }
    
    public bool GetUseMaxGenerations() 
    {
        return _useMaxGenerations;
    }
    
    public int GetMaxGenerations() 
    {
        return _maxGenerations;
    }
    
    public int GetMaxSteps() 
    {
        return _maxSteps;
    }
}
