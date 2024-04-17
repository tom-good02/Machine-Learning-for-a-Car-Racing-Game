public class EASettings
{
    public enum SelectionType
    {
        Tournament,
        RouletteWheel
    }
    
    public enum MutationType
    {
        AddOrSubtractZeroToOne,
        ChangeByPercentage,
        MultiplyByNegativeOne,
        Replace
    }

    public enum CrossoverType
    {
        NeuronSwap,
        WeightSwap,
        BiasSwap,
        LayerSwap
    }
    
    // minimum population size is 2, max is 100
    private int _populationSize;
    
    private bool _isCrossover;
    private CrossoverType _crossoverType;
    
    private bool _isMutation;
    private MutationType _mutationType;
    private float _mutationRate;
    
    private SelectionType _selectionType;
    // ideally the range would be 1, populationSize. But for now it's 1, 100
    private int _tourSize;
    // ideally the range would be 0, populationSize. But for now it's 0, 100
    // zero indicated no elitism
    // the best X members of the population to keep each generation
    private int _elitism;
    // The percentage of the remaining population after elitism to replace with offspring
    // Dependent on crossover being enabled
    // The remaining population is replaced by the best X members of the old generation, after subjecting them to mutation
    private float _steadyStateReplacementPercent;
    
    private bool _loadWeightsAndBiases;
    private float[][,] _initialWeights;
    private float[][] _initialBiases;

    public EASettings(int populationSize, bool isCrossover, CrossoverType crossoverType, bool isMutation, 
        MutationType mutationType, float mutationRate, SelectionType selectionType, 
        int tourSize, int elitism, float steadyStateReplacementPercent)
    {
        _populationSize = populationSize;
        _isCrossover = isCrossover;
        _crossoverType = crossoverType;
        _isMutation = isMutation;
        _mutationType = mutationType;
        _mutationRate = mutationRate;
        _selectionType = selectionType;
        _tourSize = tourSize;
        _elitism = elitism;
        _steadyStateReplacementPercent = steadyStateReplacementPercent;
    }
    
    public EASettings(int populationSize, bool isCrossover, CrossoverType crossoverType, bool isMutation, 
        MutationType mutationType, float mutationRate, SelectionType selectionType, 
        int tourSize, int elitism, float steadyStateReplacementPercent, bool loadWeightsAndBiases,
        float[][,] initialWeights, float[][] initialBiases)
    {
        _populationSize = populationSize;
        _isCrossover = isCrossover;
        _crossoverType = crossoverType;
        _isMutation = isMutation;
        _mutationType = mutationType;
        _mutationRate = mutationRate;
        _selectionType = selectionType;
        _tourSize = tourSize;
        _elitism = elitism;
        _steadyStateReplacementPercent = steadyStateReplacementPercent;
        _loadWeightsAndBiases = loadWeightsAndBiases;
        _initialWeights = initialWeights;
        _initialBiases = initialBiases;
    }
    
    public int GetPopulationSize() 
    {
        return _populationSize;
    }
    
    public void SetPopulationSize(int size) 
    {
        _populationSize = size;
    }
    
    public bool GetIsCrossover() 
    {
        return _isCrossover;
    }
    
    public void SetIsCrossover(bool value) 
    {
        _isCrossover = value;
    }
    
    public bool GetIsMutation() 
    {
        return _isMutation;
    }
    
    public void SetIsMutation(bool value) 
    {
        _isMutation = value;
    }
    
    public MutationType GetMutationType() 
    {
        return _mutationType;
    }
    
    public void SetMutationType(MutationType type) 
    {
        _mutationType = type;
    }
    
    public float GetMutationRate() 
    {
        return _mutationRate;
    }
    
    public void SetMutationRate(float rate) 
    {
        _mutationRate = rate;
    }
    
    public CrossoverType GetCrossoverType() 
    {
        return _crossoverType;
    }
    
    public void SetCrossoverType(CrossoverType type) 
    {
        _crossoverType = type;
    }
    
    public SelectionType GetSelectionType() 
    {
        return _selectionType;
    }
    
    public void SetSelectionType(SelectionType type) 
    {
        _selectionType = type;
    }
    
    public int GetTourSize() 
    {
        return _tourSize;
    }
    
    public void SetTourSize(int size) 
    {
        _tourSize = size;
    }
    
    public int GetElitism() 
    {
        return _elitism;
    }
    
    public void SetElitism(int value) 
    {
        _elitism = value;
    }
    
    public float GetSteadyStateReplacementPercent() 
    {
        return _steadyStateReplacementPercent;
    }
    
    public void SetSteadyStateReplacementPercent(float percent) 
    {
        _steadyStateReplacementPercent = percent;
    }
    
    public bool GetLoadWeightsAndBiases() 
    {
        return _loadWeightsAndBiases;
    }
    
    public void SetLoadWeightsAndBiases(bool value) 
    {
        _loadWeightsAndBiases = value;
    }
    
    public float[][,] GetInitialWeights() 
    {
        return _initialWeights;
    }
    
    public void SetInitialWeights(float[][,] weights) 
    {
        _initialWeights = weights;
    }
    
    public float[][] GetInitialBiases() 
    {
        return _initialBiases;
    }
    
    public void SetInitialBiases(float[][] biases) 
    {
        _initialBiases = biases;
    }
}
