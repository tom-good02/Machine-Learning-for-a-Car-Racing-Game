
using System.Collections.Generic;

[System.Serializable]
public class ModelData
{
    public string modelName;
    
    public int[] activationFunctions;
    public int[] numOfNeurons;
    public int numOfLayers;
    public float[][,] weights;
    public float[][] biases;

    public int numOfRaycasts;
    public bool useMaxGenerations;
    public int maxGenerations;
    public int maxSteps;
    
    public int populationSize;
    public bool isCrossover;
    public int crossoverType;
    public bool isMutation;
    public int mutationType;
    public float mutationRate;
    public int selectionType;
    public int tourSize;
    public int elitism;
    public float ssr_percent;

    public int generations;
    public float lapTime;
    public float fitness;
    
    public List<float> averageFitnessEachGeneration;
    public List<float> bestFitnessEachGeneration;
    public List<float> step;
}
