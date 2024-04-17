using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
    private EASettings _eaSettings;
    private AICarSettings _aiCarSettings;
    [Header("AI Car Settings")]
    [Range(1f, 20f)]
    [SerializeField] private float deathTimerLength = 3f;
    [Range(1f, 1000f)]
    [SerializeField] private float globalDeathTimer = 100f;
    [Range(0.1f, 1f)]
    [SerializeField] private float threshold = 0.5f;
    private NeuralNetworkLayerSettings _nnLayerSettings;
    private CarControllerSettings _carControllerSettings;
    [Header("Car Controller Settings")]
    [Range(1, 200)]
    [SerializeField] private int maxSpeed = 140;
    [Range(1, 10)]
    [SerializeField] private int accelerationMultiplier = 3;
    [Range(0.1f, 1f)]
    [SerializeField] private float decelerationMultiplier = 0.2f;
    [Range(0.1f, 1f)]
    [SerializeField] private float turningMultiplier = 0.5f;
    [Range(1, 1000)]
    [SerializeField] private int brakeForce = 600;
    [Range(1, 90)]
    [SerializeField] private int maxTurnAngle = 30;
    private PlayerCarSettings _playerCarSettings;
    private RaycastSettings _raycastSettings;
    
    [Tooltip("GameObject to Enable")]
    [SerializeField] private GameObject myGameObject;
    
    [SerializeField] private EA ea;
    
    [SerializeField] private DataPersistenceManager dataPersistenceManager;
    
    [SerializeField] private ParameterValueManager parameterValueManager;

    private void Awake()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.GetInt(PlayerPrefKeys.LoadModel, 0) == 1)
        {
            var modelName = PlayerPrefs.GetString(PlayerPrefKeys.ModelName, "DefaultName");
            var modelData = dataPersistenceManager.LoadEANeuralNetworkModel(modelName);
            modelData.modelName = modelData.modelName.Substring(0, modelData.modelName.LastIndexOf(" ", StringComparison.Ordinal));
            
            if (modelData != null)
            {
                _eaSettings = new EASettings(modelData.populationSize, modelData.isCrossover,
                    (EASettings.CrossoverType) modelData.crossoverType, modelData.isMutation,
                    (EASettings.MutationType) modelData.mutationType, modelData.mutationRate,
                    (EASettings.SelectionType) modelData.selectionType, modelData.tourSize, modelData.elitism,
                    modelData.ssr_percent, true, modelData.weights, modelData.biases);
                _aiCarSettings = new AICarSettings(deathTimerLength, globalDeathTimer, threshold);
                _nnLayerSettings = new NeuralNetworkLayerSettings(modelData.numOfLayers, modelData.numOfNeurons,
                    modelData.activationFunctions);
                _carControllerSettings = new CarControllerSettings(maxSpeed, accelerationMultiplier, decelerationMultiplier,
                    turningMultiplier, brakeForce, maxTurnAngle);
                _playerCarSettings = new PlayerCarSettings();
                _raycastSettings = new RaycastSettings(modelData.numOfRaycasts, false);
                ea.SetTerminationCriteria(modelData.useMaxGenerations, modelData.maxGenerations, modelData.maxSteps);
                
                var averageFitnessEachGeneration = modelData.averageFitnessEachGeneration;
                var bestFitnessEachGeneration = modelData.bestFitnessEachGeneration;
                var step = modelData.step;
                
                ea.SetGeneration(modelData.generations + 1);
                ea.SetLoadedFitnessData(bestFitnessEachGeneration, averageFitnessEachGeneration, step, modelData.generations);
                
                PlayerPrefs.SetInt(PlayerPrefKeys.LoadModel, 0);
                PlayerPrefs.Save();
                
                if (myGameObject != null)
                    myGameObject.SetActive(true);
                
                SetParameterMenuValues();
                
                return;
            }
        }
        
        // Otherwise load from PlayerPrefs
        LoadEASettings();
        LoadAICarSettings();
        LoadNeuralNetworkLayerSettings();
        LoadCarSettings();
        LoadPlayerCarSettings();
        LoadRaycastSettings();
        LoadTerminationConditionSettings();
        if (myGameObject != null)
            myGameObject.SetActive(true);
        
        SetParameterMenuValues();
    }

    private void LoadEASettings()
    {
        var populationSize = PlayerPrefs.GetInt(PlayerPrefKeys.PopulationSize, 40);
        var isCrossover = PlayerPrefs.GetInt(PlayerPrefKeys.IsCrossover, 1) == 1;
        var crossoverType = PlayerPrefs.GetInt(PlayerPrefKeys.CrossoverType, 0);
        var isMutation = PlayerPrefs.GetInt(PlayerPrefKeys.IsMutation, 1) == 1;
        var mutationType = PlayerPrefs.GetInt(PlayerPrefKeys.MutationType, 0);
        var mutationRate = PlayerPrefs.GetFloat(PlayerPrefKeys.MutationRate, 0.4f);
        var selectionType = PlayerPrefs.GetInt(PlayerPrefKeys.SelectionType, 0);
        var tourSize = PlayerPrefs.GetInt(PlayerPrefKeys.TourSize, 30);
        var elitism = PlayerPrefs.GetInt(PlayerPrefKeys.Elitism, 10);
        var ssrPercent = PlayerPrefs.GetFloat(PlayerPrefKeys.SteadyStatePercentage, 0.5f);

        _eaSettings = new EASettings(populationSize, isCrossover, (EASettings.CrossoverType)crossoverType, isMutation,
            (EASettings.MutationType)mutationType, mutationRate, (EASettings.SelectionType)selectionType, tourSize,
            elitism, ssrPercent);
    }
    
    private void LoadAICarSettings()
    {
        _aiCarSettings = new AICarSettings(deathTimerLength, globalDeathTimer, threshold);
    }

    private void LoadNeuralNetworkLayerSettings()
    {
        var numOfLayers = PlayerPrefs.GetInt(PlayerPrefKeys.NumOfLayers, 0);
        var numOfNodes = Array.ConvertAll(PlayerPrefs.GetString(PlayerPrefKeys.NumOfNodes, "5,5,5,5,5,5,5,5,5,5")
            .Split(','), int.Parse);
        var activationFunctions = Array.ConvertAll(PlayerPrefs.GetString(PlayerPrefKeys.ActivationFunctions, "0,0,0,0,0,0,0,0,0,0")
            .Split(','), int.Parse);

        _nnLayerSettings = new NeuralNetworkLayerSettings(numOfLayers, numOfNodes, activationFunctions);
    }

    private void LoadCarSettings()
    { 
        _carControllerSettings = new CarControllerSettings(maxSpeed, accelerationMultiplier, decelerationMultiplier, turningMultiplier, brakeForce, maxTurnAngle);
    }
    
    private void LoadPlayerCarSettings()
    {
        _playerCarSettings = new PlayerCarSettings();
    }
    
    private void LoadRaycastSettings()
    {
        var numOfRaycasts = PlayerPrefs.GetInt(PlayerPrefKeys.NumOfRaycasts, 5);
        _raycastSettings = new RaycastSettings(numOfRaycasts, false);
    }

    private void LoadTerminationConditionSettings()
    {
        var terminationCondition = PlayerPrefs.GetInt(PlayerPrefKeys.TerminationCondition, 0);
        var stepsValue = PlayerPrefs.GetInt(PlayerPrefKeys.StepsValue, 100000);
        var generationsValue = PlayerPrefs.GetInt(PlayerPrefKeys.GenerationsValue, 100);
        // if 0 then max steps, if 1 then max generations
        ea.SetTerminationCriteria(terminationCondition == 1, generationsValue, stepsValue);
    }
    
    private void SetParameterMenuValues()
    {
        parameterValueManager.SetEAValues(_eaSettings);
        parameterValueManager.SetNNValues(_nnLayerSettings);
        parameterValueManager.SetInputValues(_raycastSettings.GetNumOfRaycasts());
    }
    
    public EASettings GetEASettings() 
    {
        return _eaSettings;
    }
    
    public AICarSettings GetAICarSettings() 
    {
        return _aiCarSettings;
    }
    
    public NeuralNetworkLayerSettings GetNeuralNetworkLayerSettings() 
    {
        return _nnLayerSettings;
    }
    
    public CarControllerSettings GetCarControllerSettings() 
    {
        return _carControllerSettings;
    }
    
    public PlayerCarSettings GetPlayerCarSettings() 
    {
        return _playerCarSettings;
    }
    
    public RaycastSettings GetRaycastSettings() 
    {
        return _raycastSettings;
    }
}
