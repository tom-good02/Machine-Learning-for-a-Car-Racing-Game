[System.Serializable]
public class NNLayer 
{
    private int numNeurons;
    private NeuralNetwork.ActivationFunction activationFunction;

    public NNLayer(int numNeurons, NeuralNetwork.ActivationFunction activationFunction) 
    {
        this.numNeurons = numNeurons;
        this.activationFunction = activationFunction;
    }
    
    public int GetNumNeurons() 
    {
        return numNeurons;
    }

    public void SetNumNeurons(int num) 
    {
        numNeurons = num;
    }

    public NeuralNetwork.ActivationFunction GetActivationFunction() 
    {
        return activationFunction;
    }

    public void SetActivationFunction(NeuralNetwork.ActivationFunction af) 
    {
        activationFunction = af;
    }
}