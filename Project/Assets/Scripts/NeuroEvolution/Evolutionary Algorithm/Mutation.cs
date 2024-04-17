using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Mutation
{
    private readonly EASettings _eaSettings;
    public Mutation(EASettings eaSettings)
    {
        _eaSettings = eaSettings;
    }
    
    public void MutateAllChildren(List<float[][,]> childWeights, List<float[][]> childBiases) 
    {
        // For each network in the list
        for (var i = 0; i < childWeights.Count; i++)
        {
            //for each layer in the network
            for (var j = 0; j < childWeights[i].Length; j++)
            {
                // for each neuron in the layer
                for (var k = 0; k < childWeights[i][j].GetLength(1); k++)
                {
                    // for each weight coming from a node in the previous layer
                    for (var m = 0; m < childWeights[i][j].GetLength(0); m++)
                    {
                        if (Random.Range(0f, 1f) <= _eaSettings.GetMutationRate())
                            childWeights[i][j][m, k] = ApplyWeightMutation(childWeights[i][j][m, k], _eaSettings.GetMutationType());
                    }
                }

                // for each bias in the layer
                for (var k = 0; k < childBiases[i][j].Length; k++)
                {
                    if (Random.Range(0f, 1f) <= _eaSettings.GetMutationRate())
                        childBiases[i][j][k] = ApplyBiasMutation(childBiases[i][j][k], _eaSettings.GetMutationType());
                }
            }
        }
    }
    
    public void MutateIndividual(float[][,] weights, float[][] biases, float mutationRate, EASettings.MutationType mutationType)
    {
        //for each layer in the network
        for (var j = 0; j < weights.Length; j++)
        {
            // for each neuron in the layer
            for (var k = 0; k < weights[j].GetLength(1); k++)
                // for each weight coming from a node in the previous layer
            for (var m = 0; m < weights[j].GetLength(0); m++)
                if (Random.Range(0f, 1f) <= mutationRate)
                    weights[j][m, k] = ApplyWeightMutation(weights[j][m, k], mutationType);
    
            // for each bias in the layer
            for (var k = 0; k < biases[j].Length; k++)
                if (Random.Range(0f, 1f) <= mutationRate)
                    biases[j][k] = ApplyBiasMutation(biases[j][k], mutationType);
        }
    }
    
    private float ApplyWeightMutation(float weight, EASettings.MutationType mutationType)
    {
        switch (mutationType) 
        {
            case EASettings.MutationType.AddOrSubtractZeroToOne:
                weight += Random.Range(-1f, 1f);
                break;
            case EASettings.MutationType.ChangeByPercentage:
                weight *= Random.Range(0f, 2f);
                break;
            case EASettings.MutationType.MultiplyByNegativeOne:
                weight *= -1f;
                break;
            case EASettings.MutationType.Replace:
                weight = Random.Range(-1f, 1f);
                break;
            default:
                throw new Exception("Invalid weight mutation type");
        }

        return weight;
    }

    private float ApplyBiasMutation(float bias, EASettings.MutationType mutationType)
    {
        switch (mutationType) 
        {
            case EASettings.MutationType.AddOrSubtractZeroToOne:
                bias += Random.Range(-1f, 1f);
                break;
            case EASettings.MutationType.ChangeByPercentage:
                bias *= Random.Range(0f, 2f);
                break;
            case EASettings.MutationType.MultiplyByNegativeOne:
                bias *= -1;
                break;
            case EASettings.MutationType.Replace:
                bias = Random.Range(-1f, 1f);
                break;
            default:
                throw new Exception("Invalid bias mutation type");
        }

        return bias;
    }
}
