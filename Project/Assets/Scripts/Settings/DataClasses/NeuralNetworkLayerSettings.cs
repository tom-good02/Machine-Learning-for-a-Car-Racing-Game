[System.Serializable]
public class NeuralNetworkLayerSettings
{
    private NNLayer[] _layers;
    
    private int _numOfLayers;
    private int[] _numOfNeurons;
    private int[] _activationFunctions;

    public NeuralNetworkLayerSettings(int numOfLayers, int[] numOfNeurons, int[] activationFunctions)
    {
        _numOfLayers = numOfLayers;
        _numOfNeurons = numOfNeurons;
        _activationFunctions = activationFunctions;
        _layers = new NNLayer[numOfLayers];
        for (var i = 0; i < numOfLayers; i++)
            _layers[i] = new NNLayer(numOfNeurons[i], (NeuralNetwork.ActivationFunction)activationFunctions[i]);
    }
    
    public int GetNumOfLayers() 
    {
        return _numOfLayers;
    }
    
    public int[] GetNumOfNeurons() 
    {
        return _numOfNeurons;
    }
    
    public int[] GetActivationFunctions() 
    {
        return _activationFunctions;
    }

    // public void AddLayer(int numNeurons, NeuralNetwork.ActivationFunction activationFunction) 
    // {
    //     if (_layers == null)
    //     {
    //         _layers = new NNLayer[1];
    //         _layers[0] = new NNLayer(numNeurons, activationFunction);
    //         return;
    //     }
    //     
    //     var newLayers = new NNLayer[_layers.Length + 1];
    //     for (var i = 0; i < _layers.Length; i++) 
    //         newLayers[i] = _layers[i];
    //     newLayers[_layers.Length] = new NNLayer(numNeurons, activationFunction);
    //     _layers = newLayers;
    // }
    //
    // public void RemoveLayer (int index) 
    // {
    //     if (_layers == null || index < 0 || index >= _layers.Length) 
    //         return;
    //     
    //     var newLayers = new NNLayer[_layers.Length - 1];
    //     for (var i = 0; i < index; i++) 
    //         newLayers[i] = _layers[i];
    //     for (var i = index + 1; i < _layers.Length; i++) 
    //         newLayers[i - 1] = _layers[i];
    //     _layers = newLayers;
    // }
    
    public NNLayer[] GetNeuralNetworkLayers() 
    {
        return _layers;
    }
}
