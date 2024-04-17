using System;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using Random = UnityEngine.Random;

public class EA : MonoBehaviour
{
    private const float Tolerance = 0.0001f;
    
    // Maximum training steps
    // private const float MaxSteps = 100000;
    private bool _useMaxGenerations;
    private int _maxGenerations;
    private int _maxSteps;
    private int _currentStep;
    
    [SerializeField] private DataPersistenceManager dataPersistenceManager;

    [SerializeField] private TMP_Text generationCounter;
    [SerializeField] private NeuralNetworkGraph neuralNetworkGraph;
    [SerializeField] private GameObject finishedTrainingScreen;
    
    [SerializeField] private Settings settings;
    private EASettings _eaSettings;
    private Mutation _mutation;
    private Crossover _crossover;
    private ParentSelection _selection;
    
    [SerializeField] private FitnessGraph fitnessGraph;
    
    [SerializeField] private SpawnPointManager spawnPointManager;
    [SerializeField] private Transform checkpoints;
    
    [SerializeField] private GameObject car;
    [SerializeField] private Transform carParent;
    
    private List<AICarController> _aiCarControllers = new();
    
    private int _deadCars;
    private bool _hasStarted;

    private int _generation = 1;

    private List<float[][,]> _currentGenWeights = new();
    private List<float[][]> _currentGenBiases = new();
    private List<float> _currentGenFitnesses = new();
    
    private int _totalFitness;

    private List<float[][,]> _childWeights = new();
    private List<float[][]> _childBiases = new();
    
    private List<float[][,]> _newGenWeights = new();
    private List<float[][]> _newGenBiases = new();

    private float[][,] _bestWeights;
    private float[][] _bestBiases;
    private float _bestLaptime;
    private float _bestFitness = float.MinValue;
    
    private List<float> _averageFitnessEachGeneration = new();
    private List<float> _bestFitnessEachGeneration = new();
    private List<float> _step = new();
    
    private Transform _mainCameraTransform;
    [SerializeField] private Vector3 defaultCameraPos;
    [SerializeField] private Vector3 defaultCameraRot;
    private CameraController _mainCameraController;

    public void Start()
    {
        _eaSettings = settings.GetEASettings();
        _mutation = new Mutation(_eaSettings);
        _crossover = new Crossover(_eaSettings);
        _selection = new ParentSelection(_eaSettings);

        var cameraMain = Camera.main;
        if (cameraMain)
        {
            _mainCameraController = cameraMain.GetComponent<CameraController>();
            _mainCameraTransform = cameraMain.transform;
        }

        StartEA();
        _hasStarted = true;
    }

    public void FixedUpdate()
    {
        _currentStep++;
        if (!_useMaxGenerations && _currentStep >= _maxSteps)
        {
            // End training
            print("Max steps reached");
            EndTraining();
        }
    }

    public void SetGeneration(int generation)
    {
        _generation = generation;
    }
    
    public void SetLoadedFitnessData(List<float> bestFitnessEachGeneration, List<float> averageFitnessEachGeneration, List<float> step, int generation)
    {
        _bestFitnessEachGeneration = bestFitnessEachGeneration;
        _averageFitnessEachGeneration = averageFitnessEachGeneration;
        _step = step;
        fitnessGraph.CreateNewGraphFromBestAndAverageFitnessFromData(bestFitnessEachGeneration, averageFitnessEachGeneration, generation);
    }

    public void Update()
    {
        // Update camera to point at best (alive) member
        // Find highest fitness car right now
        // Update camera to point at that car
        if (!_hasStarted)
            return;
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (_mainCameraController.enabled)
            {  
                _mainCameraController.enabled = false;
                _mainCameraTransform.position = defaultCameraPos;
                _mainCameraTransform.rotation = Quaternion.Euler(defaultCameraRot);
            }
            else
            {
                _mainCameraController.enabled = true;
            }
        }
        _mainCameraController.CameraTarget = FindCameraTarget();
    }

    private Transform FindCameraTarget()
    {
        var bestFitness = float.MinValue;
        AICarController bestAICarController = null;
        foreach (var aiCarController in _aiCarControllers)
        {
            if (aiCarController.IsDead())
                continue;
            var fitness = aiCarController.GetNeuralNetwork().GetFitness();
            if (fitness <= bestFitness) 
                continue;
            bestFitness = fitness;
            bestAICarController = aiCarController;
        }
        
        if (!bestAICarController)
            throw new Exception("Error in finding best car");
        
        return bestAICarController.transform;
    }
    
    private void StartEA()
    {
        var nnSettings = settings.GetNeuralNetworkLayerSettings();
        var nnHiddenLayers = nnSettings.GetNumOfNeurons()[..nnSettings.GetNumOfLayers()];
        neuralNetworkGraph.CreateGraph(nnHiddenLayers, 
            settings.GetRaycastSettings().GetNumOfRaycasts() + NeuralNetwork.NumAdditionalInputs, 
            NeuralNetwork.NumOutputNeurons);
        FirstGeneration();
    }
    
    private void FirstGeneration() 
    {
        generationCounter.text = "" + _generation;
        var spawnPoint = spawnPointManager.GetSpawnPoint();
        var position = spawnPoint.spawnPoint.position;
        var rotation = spawnPoint.spawnPoint.rotation;
        // Add some randomness to the position
        position += spawnPoint.spawnPoint.TransformDirection(new Vector3(Random.Range(-3f, 3f), 0f, 0f));
        
        var nextCheckpointIndex = spawnPoint.nextCheckpoint.GetSiblingIndex();
        for (var i = 0; i < _eaSettings.GetPopulationSize(); i++)
        {
            SpawnCar(i, position, rotation, nextCheckpointIndex);
            if (_eaSettings.GetLoadWeightsAndBiases())
                _aiCarControllers[i].GetNeuralNetwork().SetWeightsAndBiases(_eaSettings.GetInitialWeights(), _eaSettings.GetInitialBiases());
        }
        // Update neural network graph with a neural network from the population
        _aiCarControllers[0].GetNeuralNetwork().GetCopyOfWeightsAndBiases(out var weights, out var biases);
        _bestWeights = weights;
        _bestBiases = biases;
        neuralNetworkGraph.UpdateGraph(weights);
    }

    private void SpawnCar(int carID, Vector3 position, Quaternion rotation, int nextCheckpointIndex) 
    {
        var newCar = Instantiate(car, position, rotation, carParent);
        newCar.name = newCar.name + " " + carID;
        var aiCarController = newCar.GetComponent<AICarController>();
        aiCarController.SetEA(this);
        aiCarController.SetId(carID);
        aiCarController.SetCheckpoints(checkpoints);
        aiCarController.SetNextCheckpointIndex(nextCheckpointIndex);
        
        _aiCarControllers.Add(aiCarController);
    }

    public void NotifyOfDeath() 
    {
        _deadCars++;
        if (_deadCars != _eaSettings.GetPopulationSize())
            return;
        NextGeneration();
    }
    
    private void ClearLists() 
    {
        _currentGenWeights.Clear();
        _currentGenBiases.Clear();
        _currentGenFitnesses.Clear();
        
        _childWeights.Clear();
        _childBiases.Clear();
        
        _newGenWeights.Clear();
        _newGenBiases.Clear();
    }

    private void NextGeneration() 
    {
        print(_currentStep);
        StoreCurrentGen();
        StoreBestMemberAndFitnessesOfGeneration();
        
        Reproduce();
        _generation++;
        
        if (_useMaxGenerations && _generation >= _maxGenerations + 1)
        {
            // End training
            print("Max generations reached");
            EndTraining();
            return;
        }
        
        generationCounter.text = "" + _generation;
        
        _deadCars = 0;
        ResetCarsAndGo();
        
        ClearLists();
    }

    // Elitism: Keep X best population members each generation
    // Steady State: Percent of population (after elitism members are removed) to be replaced by offspring and removed
    //               If crossover is not enabled, the offspring are just copies of the parents, (then
    //                   subjected to mutation) with the parent to be copied for each offspring chosen randomly from the 
    //                   two selected parents
    // Generational replacement with mutation: The remaining population is replaced by mutated copies of themselves
    // E.g. In a population of 10, we might keep the best 1 (elitism). Then breed offspring, replace and remove the
    //     worst 5 with these offspring (steady state). Finally, take the remaining 4 members and mutate them, replacing
    //     themselves in the population
    private void  Reproduce() 
    {
        var populationSize = _eaSettings.GetPopulationSize();
        var steadyStateReplacement = _eaSettings.GetSteadyStateReplacementPercent();
        var elitism = _eaSettings.GetElitism();
        if (elitism > populationSize)
            elitism = populationSize;
        var numOfOffspringToCreate = (int) ((populationSize - elitism) * steadyStateReplacement);
        
        CreateOffspring(numOfOffspringToCreate);
        TakeEliteMembers(elitism);
        ReplaceWorstMembersWithOffspring(numOfOffspringToCreate);
        AddRemainingMembersToNewGen();
    }

    private void CreateOffspring(int numOfOffspringToCreate)
    {
        // Create offspring
        _selection.SelectParents(out var parentOneIndex, out var parentTwoIndex, _currentGenFitnesses);
        for (var i = 0; i < numOfOffspringToCreate; i++)
        {
            _crossover.PerformCrossover(out var childOneWeights, out var childOneBiases, 
                out var childTwoWeights, out var childTwoBiases, 
                _currentGenWeights[parentOneIndex], _currentGenBiases[parentOneIndex],
                _currentGenWeights[parentTwoIndex], _currentGenBiases[parentTwoIndex]);
            if (numOfOffspringToCreate - i == 1)
            {
                if (Random.Range(0, 2) == 0)
                {
                    _childWeights.Add(childOneWeights);
                    _childBiases.Add(childOneBiases);
                }
                else
                {
                    _childWeights.Add(childTwoWeights);
                    _childBiases.Add(childTwoBiases);
                }
            }
            else
            {
                _childWeights.Add(childOneWeights);
                _childBiases.Add(childOneBiases);
                _childWeights.Add(childTwoWeights);
                _childBiases.Add(childTwoBiases);
            }
        }
        
        if (_eaSettings.GetIsMutation())
            _mutation.MutateAllChildren(_childWeights, _childBiases);
    }

    private void TakeEliteMembers(int elitism)
    {
        // Select the best X (elitism) members of the current generation and add them to the new generation
        for (var i = 0; i < elitism; i++)
        {
            var bestIndex = 0;
            var bestFitness = float.MinValue;
            for (var j = 0; j < _currentGenFitnesses.Count; j++)
            {
                if (IsNewFitnessLarger(bestFitness, _currentGenFitnesses[j]))
                {
                    bestIndex = j;
                    bestFitness = _currentGenFitnesses[j];
                }
            }
            
            _newGenWeights.Add(_currentGenWeights[bestIndex]);
            _newGenBiases.Add(_currentGenBiases[bestIndex]);
            
            _currentGenWeights.RemoveAt(bestIndex);
            _currentGenBiases.RemoveAt(bestIndex);
            _currentGenFitnesses.RemoveAt(bestIndex);
        }
    }

    private void ReplaceWorstMembersWithOffspring(int numToReplace)
    {
        // take the worst members of the remaining old generation and replace them with the created offspring
        // remove replaced members from the old generation
        for (var i = 0; i < numToReplace; i++)
        {
            var worstIndex = 0;
            var worstFitness = float.MaxValue;
            for (var j = 0; j < _currentGenFitnesses.Count; j++)
            {
                if (IsNewFitnessLarger(worstFitness, _currentGenFitnesses[j]))
                    continue;
                worstIndex = j;
                worstFitness = _currentGenFitnesses[j];
            }
            _newGenWeights.Add(_childWeights[i]);
            _newGenBiases.Add(_childBiases[i]);
        
            _currentGenWeights.RemoveAt(worstIndex);
            _currentGenBiases.RemoveAt(worstIndex);
            _currentGenFitnesses.RemoveAt(worstIndex);
        }   
    }
    
    private void AddRemainingMembersToNewGen()
    {
        // Add the remaining members of the old generation to the new generation after subjecting them to mutation
        for (var i = 0; i < _currentGenFitnesses.Count; i++)
        {
            if (_eaSettings.GetIsMutation())
                _mutation.MutateIndividual(_currentGenWeights[i], _currentGenBiases[i], 
                    _eaSettings.GetMutationRate(), _eaSettings.GetMutationType());
            _newGenWeights.Add(_currentGenWeights[i]);
            _newGenBiases.Add(_currentGenBiases[i]);
        }
    }

    private void StoreBestMemberAndFitnessesOfGeneration()
    {
        // Find and save best member
        // Store the weights and biases of each cars neural network
        var bestFitness = float.MinValue;
        AICarController bestAICarController = null;
        var totalFitness = 0f;
        foreach (var aiCarController in _aiCarControllers)
        {
            var fitness = aiCarController.GetNeuralNetwork().GetFitness();
            totalFitness += fitness;
            
            if (IsNewFitnessLarger(bestFitness, fitness))
            {
                bestFitness = fitness;
                bestAICarController = aiCarController;
            }
        }
        
        if (!bestAICarController)
            throw new Exception("Error in saving best car");
        
        _bestFitnessEachGeneration.Add(bestFitness);
        var averageFitness = totalFitness / _eaSettings.GetPopulationSize();
        _averageFitnessEachGeneration.Add(averageFitness);
        _step.Add(_currentStep);
        
        // Update Fitness Graph
        fitnessGraph.UpdateGraph(bestFitness, averageFitness);
        if (IsNewFitnessLarger(_bestFitness, bestFitness))
        {
            _bestFitness = bestFitness;
            _bestLaptime = bestAICarController.GetLapTime();
            bestAICarController.GetNeuralNetwork().GetCopyOfWeightsAndBiases(out _bestWeights, out _bestBiases);
            
            // Update Neural Network Graph
            neuralNetworkGraph.UpdateGraph(_bestWeights);
        }
        else
        {
            bestAICarController.GetNeuralNetwork().GetCopyOfWeightsAndBiases(out var weights, out var biases);
            
            // Update Neural Network Graph
            neuralNetworkGraph.UpdateGraph(weights);
        }
    }

    private bool IsNewFitnessLarger(float currentFitness, float newFitness)
    {
        return newFitness > currentFitness || (Math.Abs(newFitness - currentFitness) < Tolerance && Random.Range(0, 2) == 0);
    }

    private void StoreCurrentGen()
    {
        // Store the weights and biases of each cars neural network
        foreach (var aiCarController in _aiCarControllers)
        {
            var neuralNetwork = aiCarController.GetNeuralNetwork();
            neuralNetwork.GetCopyOfWeightsAndBiases(out var weights, out var biases);
            _currentGenWeights.Add(weights);
            _currentGenBiases.Add(biases);
            _currentGenFitnesses.Add(neuralNetwork.GetFitness());
        }
    }

    private void ResetCarsAndGo()
    {
        // Destroy all current cars
        foreach (var aiCarController in _aiCarControllers)
            Destroy(aiCarController.gameObject);
        _aiCarControllers.Clear();
        
        // Create new population
        var spawnPoint = spawnPointManager.GetSpawnPoint();
        var position = spawnPoint.spawnPoint.position;
        var rotation = spawnPoint.spawnPoint.rotation;
        // Add some randomness to the position
        position += spawnPoint.spawnPoint.TransformDirection(new Vector3(Random.Range(-3f, 3f), 0f, 0f));
        
        var nextCheckpointIndex = spawnPoint.nextCheckpoint.GetSiblingIndex();
        for (var i = 0; i < _eaSettings.GetPopulationSize(); i++)
        {
            SpawnCar(i, position, rotation, nextCheckpointIndex);
            _aiCarControllers[i].GetNeuralNetwork().SetWeightsAndBiases(_newGenWeights[i], _newGenBiases[i]);
        }
    }

    public ModelData GetData()
    {
        var data = new ModelData();

        var neuralNetworkLayerSettings = settings.GetNeuralNetworkLayerSettings();
        
        data.activationFunctions = neuralNetworkLayerSettings.GetActivationFunctions();
        data.numOfNeurons = neuralNetworkLayerSettings.GetNumOfNeurons();
        data.numOfLayers = neuralNetworkLayerSettings.GetNumOfLayers();
        data.weights = _bestWeights;
        data.biases = _bestBiases;
        
        data.numOfRaycasts = settings.GetRaycastSettings().GetNumOfRaycasts();
        data.useMaxGenerations = _useMaxGenerations;
        data.maxGenerations = _maxGenerations;
        data.maxSteps = _maxSteps;

        data.populationSize = _eaSettings.GetPopulationSize();
        data.isCrossover = _eaSettings.GetIsCrossover();
        data.crossoverType = (int) _eaSettings.GetCrossoverType();
        data.isMutation = _eaSettings.GetIsMutation();
        data.mutationType = (int)_eaSettings.GetMutationType();
        data.mutationRate = _eaSettings.GetMutationRate();
        data.selectionType = (int) _eaSettings.GetSelectionType();
        data.tourSize = _eaSettings.GetTourSize();
        data.elitism = _eaSettings.GetElitism();
        data.ssr_percent = _eaSettings.GetSteadyStateReplacementPercent();

        data.generations = _generation - 1; 
        data.lapTime = _bestLaptime;
        data.fitness = _bestFitness;
        
        data.bestFitnessEachGeneration = _bestFitnessEachGeneration;
        data.averageFitnessEachGeneration = _averageFitnessEachGeneration;
        data.step = _step;

        return data;
    }
    
    public void SetTerminationCriteria(bool useMaxGenerations, int maxGenerations, int maxSteps)
    {
        _useMaxGenerations = useMaxGenerations;
        _maxGenerations = maxGenerations;
        _maxSteps = maxSteps;
    }

    private void EndTraining()
    {
        // End training
        Time.timeScale = 0;
        finishedTrainingScreen.SetActive(true);
    }
}
