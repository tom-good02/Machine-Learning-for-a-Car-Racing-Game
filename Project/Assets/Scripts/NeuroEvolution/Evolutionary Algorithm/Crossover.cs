using System;
using System.Linq;
using Random = UnityEngine.Random;

public class Crossover
{
    private readonly EASettings.CrossoverType _crossoverType;
    private readonly bool _isCrossover;
    public Crossover(EASettings eaSettings)
    {
        _crossoverType = eaSettings.GetCrossoverType();
        _isCrossover = eaSettings.GetIsCrossover();
    }
    
    public void PerformCrossover(out float[][,] childOneWeights, out float[][] childOneBiases, 
        out float[][,] childTwoWeights, out float[][] childTwoBiases, 
        float[][,] parentOneWeights, float[][] parentOneBiases,
        float[][,] parentTwoWeights, float[][] parentTwoBiases) 
    {
        
        childOneWeights = new float[parentOneWeights.Length][,];
        childOneBiases = new float[parentOneBiases.Length][];
    
        childTwoWeights = new float[parentOneWeights.Length][,];
        childTwoBiases = new float[parentOneBiases.Length][];
    
        if (_isCrossover)
            RealCrossover(parentOneWeights, parentOneBiases, parentTwoWeights, parentTwoBiases, _crossoverType, 
                childOneWeights, childOneBiases, childTwoWeights, childTwoBiases);
        else
            FakeCrossover(parentOneWeights, parentOneBiases, parentTwoWeights, parentTwoBiases, 
                childOneWeights, childOneBiases, childTwoWeights, childTwoBiases);
    }
    
    private void RealCrossover(float[][,] parentOneWeights, float[][] parentOneBiases,
        float[][,] parentTwoWeights, float[][] parentTwoBiases, 
        EASettings.CrossoverType crossoverType,
        float[][,] childOneWeights, float[][] childOneBiases, 
        float [][,] childTwoWeights, float[][] childTwoBiases)
    {
        for (var j = 0; j < parentOneWeights.Length; j++)
        {
            var tempWeights = parentOneWeights[j];
            var tempBiases = parentOneBiases[j];

            childOneWeights[j] = new float[tempWeights.GetLength(0), tempWeights.GetLength(1)];
            childOneBiases[j] = new float[tempBiases.Length];
        
            Array.Copy(tempWeights, childOneWeights[j], tempWeights.Length);
            Array.Copy(tempBiases, childOneBiases[j], tempBiases.Length);
        
            tempWeights = parentTwoWeights[j];
            tempBiases = parentTwoBiases[j];
        
            childTwoWeights[j] = new float[tempWeights.GetLength(0), tempWeights.GetLength(1)];
            childTwoBiases[j] = new float[tempBiases.Length];
        
            Array.Copy(tempWeights, childTwoWeights[j], tempWeights.Length);
            Array.Copy(tempBiases, childTwoBiases[j], tempBiases.Length);
        }
        
        switch (crossoverType)
        {
            case EASettings.CrossoverType.NeuronSwap:
                NeuronSwap(childOneWeights, childTwoWeights);
                break;
            case EASettings.CrossoverType.WeightSwap:
                WeightSwap(childOneWeights, childTwoWeights);
                break;
            case EASettings.CrossoverType.BiasSwap:
                BiasSwap(childOneBiases, childTwoBiases);
                break;
            case EASettings.CrossoverType.LayerSwap:
                LayerSwap(childOneWeights, childOneBiases, 
                    childTwoWeights, childTwoBiases);
                break;
            default:
                throw new Exception("Invalid crossover type");
        }
    }

    private void FakeCrossover(float[][,] parentOneWeights, float[][] parentOneBiases,
        float[][,] parentTwoWeights, float[][] parentTwoBiases, 
        float[][,] childOneWeights, float[][] childOneBiases, float [][,] childTwoWeights, float[][] childTwoBiases)
    {
        float[][,] parentWeights;
        float[][] parentBiases;
        if (Random.Range(0, 2) == 0)
        {
            parentWeights = parentOneWeights;
            parentBiases = parentOneBiases;
        }
        else
        {
            parentWeights = parentTwoWeights;
            parentBiases = parentTwoBiases;
        }
        
        for (var j = 0; j < parentWeights.Length; j++)
        {
            var tempWeights = parentWeights[j];
            var tempBiases = parentBiases[j];

            childOneWeights[j] = new float[tempWeights.GetLength(0), tempWeights.GetLength(1)];
            childOneBiases[j] = new float[tempBiases.Length];
            
            Array.Copy(tempWeights, childOneWeights[j], tempWeights.Length);
            Array.Copy(tempBiases, childOneBiases[j], tempBiases.Length);
            
            childTwoWeights[j] = new float[tempWeights.GetLength(0), tempWeights.GetLength(1)];
            childTwoBiases[j] = new float[tempBiases.Length];
            
            Array.Copy(tempWeights, childTwoWeights[j], tempWeights.Length);
            Array.Copy(tempBiases, childTwoBiases[j], tempBiases.Length);
        }
    }

    private void NeuronSwap(float[][,] childOneWeights, float[][,] childTwoWeights)
    {
        // Pick a neuron and swap it between the childrens weights
        var totalNeurons = childOneWeights.Sum(layer => layer.GetLength(1));
        totalNeurons += NeuralNetwork.NumOutputNeurons;
        
        var neuronToSwap = Random.Range(0, totalNeurons);
        var neuronCounter = 0;
        
        // for each layer in the network
        for (var i = 0; i < childOneWeights.Length; i++)
        {
            // for each neuron in the layer
            for (var j = 0; j < childOneWeights[i].GetLength(1); j++)
            {
                if (neuronCounter == neuronToSwap)
                {
                    for (var k = 0; k < childOneWeights[i].GetLength(0); k++)
                        (childOneWeights[i][k, j], childTwoWeights[i][k, j]) = (childTwoWeights[i][k, j], childOneWeights[i][k, j]);
                    return;
                }

                neuronCounter++;
            }
        }
    }

    private void WeightSwap(float[][,] childOneWeights, float[][,] childTwoWeights)
    {
        // Pick a weight and swap it between the children's weights
        var totalWeights = childOneWeights.Sum(layer => layer.Length);
        
        var weightToSwap = Random.Range(0, totalWeights);
        var weightCounter = 0;
        
        // for each layer in the network
        for (var i = 0; i < childOneWeights.Length; i++)
        {
            // for each neuron in the layer
            for (var j = 0; j < childOneWeights[i].GetLength(1); j++)
            {
                // for each weight coming from a node in the previous layer
                for (var k = 0; k < childOneWeights[i].GetLength(0); k++)
                {
                    if (weightCounter == weightToSwap)
                    {
                        (childOneWeights[i][k, j], childTwoWeights[i][k, j]) = (childTwoWeights[i][k, j], childOneWeights[i][k, j]);
                        return;
                    }

                    weightCounter++;
                }
            }
        }
    }

    private void BiasSwap(float[][] childOneBiases, float[][] childTwoBiases)
    {
        // Pick a bias and swap it between the children's biases
        var totalBiases = childOneBiases.Sum(layer => layer.Length);
        
        var biasToSwap = Random.Range(0, totalBiases);
        var biasCounter = 0;
        
        // for each layer in the network
        for (var i = 0; i < childOneBiases.Length; i++)
        {
            // for each bias in the layer
            for (var j = 0; j < childOneBiases[i].Length; j++)
            {
                if (biasCounter == biasToSwap)
                {
                    (childOneBiases[i][j], childTwoBiases[i][j]) = (childTwoBiases[i][j], childOneBiases[i][j]);
                    return;
                }

                biasCounter++;
            }
        }
    }

    private void LayerSwap(float[][,] childOneWeights, float[][] childOneBiases, 
        float[][,] childTwoWeights, float[][] childTwoBiases)
    {
        // pick a layer in the networks and swap it between the children
        var layerToSwap = Random.Range(0, childOneWeights.Length);
        
        (childOneWeights[layerToSwap], childTwoWeights[layerToSwap]) = 
            (childTwoWeights[layerToSwap], childOneWeights[layerToSwap]);
        (childOneBiases[layerToSwap], childTwoBiases[layerToSwap]) =
            (childTwoBiases[layerToSwap], childOneBiases[layerToSwap]);
    }
}
