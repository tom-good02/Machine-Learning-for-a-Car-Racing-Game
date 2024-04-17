using System;
using System.Collections.Generic;
using SlimUI.ModernMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINewModelSettingsManager : MonoBehaviour
{
    [Header("UIMenuManager")]
    [SerializeField] private UIMenuManager uiMenuManager;
    
    [Header("BUTTONS")] 
    [SerializeField] private GameObject EAButtons;
    [SerializeField] private GameObject RLButtons;
    
    [Header("MODEL NAME")]
    [SerializeField] private TMP_InputField modelNameInputField;
    
    [Header("MAP AND METHOD SETTINGS")] 
    [SerializeField] private GameObject rlHighlight;
    [SerializeField] private GameObject eaHighlight;
    [SerializeField] private GameObject easyHighlight;
    [SerializeField] private GameObject mediumHighlight;
    [SerializeField] private GameObject hardHighlight;
    [SerializeField] private GameObject silverstoneHighlight;
    [SerializeField] private Image loadEasyHighlight;
    [SerializeField] private Image loadMediumHighlight;
    [SerializeField] private Image loadHardHighlight;
    [SerializeField] private Image loadSilverstoneHighlight;
    
    [Header("EA SETTINGS")]
    [SerializeField] private Slider populationSlider;
    [SerializeField] private TextMeshPro populationSizeText;
    private int _isCrossover;
    [SerializeField] private TextMeshPro crossoverBtnText;
    [SerializeField] private TMP_Dropdown crossoverTypeDropdown;
    private int _isMutation;
    [SerializeField] private TextMeshPro mutationBtnText;
    [SerializeField] private TMP_Dropdown mutationTypeDropdown;
    [SerializeField] private Slider mutationRateSlider;
    [SerializeField] private TextMeshPro mutationRateText;
    [SerializeField] private TMP_Dropdown selectionTypeDropdown;
    [SerializeField] private Slider tourSizeSlider;
    [SerializeField] private TextMeshPro tourSizeText;
    [SerializeField] private Slider elitismSlider;
    [SerializeField] private TextMeshPro elitismText;
    [SerializeField] private Slider steadyStatePercentageSlider;
    [SerializeField] private TextMeshPro steadyStatePercentageText;

    [Header("NN SETTINGS")]
    [SerializeField] private Slider numOfLayersSlider;
    [SerializeField] private TextMeshPro numOfLayersText;
    [SerializeField] private Transform scrollViewContent;
    private List<NNLayerListTile> _nnLayerUIManagers = new();
    private int[] _numOfNodes;
    private int[] _activationFunctions;

    [Header("GENERAL SETTINGS")] 
    [SerializeField] private Slider numOfRaycastsSlider;
    [SerializeField] private TextMeshPro numOfRaycastsText;
    [SerializeField] private TMP_Dropdown terminationConditionDropdown;
    [SerializeField] private Slider stepsSlider;
    [SerializeField] private TextMeshPro stepsText;
    [SerializeField] private Slider generationsSlider;
    [SerializeField] private TextMeshPro generationsText;
    
    [Header("REINFORCEMENT LEARNING UNAVAILABLE")]
    [SerializeField] private GameObject rlUnavailable;
    [SerializeField] private GameObject modelNameInput;
    
    
    public void Start()
    {
        var method = PlayerPrefs.GetInt(PlayerPrefKeys.Method, 0);
        if (method is > 1 or < 0)
            method = 0;
        switch (method)
        {
            case 0:
                RLButtons.SetActive(false);
                EAButtons.SetActive(true);
                rlHighlight.SetActive(false);
                eaHighlight.SetActive(true);
                break;
            case 1:
                EAButtons.SetActive(false);
                RLButtons.SetActive(true);
                eaHighlight.SetActive(false);
                rlHighlight.SetActive(true);
                break;
        }
        
        var trackDifficulty = PlayerPrefs.GetInt(PlayerPrefKeys.TrackDifficulty, 0);
        if (trackDifficulty is > 3 or < 0)
            trackDifficulty = 0;
        switch (trackDifficulty)
        {
            case 0:
                easyHighlight.SetActive(true);
                loadEasyHighlight.enabled = true;
                mediumHighlight.SetActive(false);
                loadMediumHighlight.enabled = false;
                hardHighlight.SetActive(false);
                loadHardHighlight.enabled = false;
                silverstoneHighlight.SetActive(false);
                loadSilverstoneHighlight.enabled = false;
                break;
            case 1:
                easyHighlight.SetActive(false);
                loadEasyHighlight.enabled = false;
                mediumHighlight.SetActive(true);
                loadMediumHighlight.enabled = true;
                hardHighlight.SetActive(false);
                loadHardHighlight.enabled = false;
                silverstoneHighlight.SetActive(false);
                loadSilverstoneHighlight.enabled = false;
                break;
            case 2:
                easyHighlight.SetActive(false);
                loadEasyHighlight.enabled = false;
                mediumHighlight.SetActive(false);
                loadMediumHighlight.enabled = false;
                hardHighlight.SetActive(true);
                loadHardHighlight.enabled = true;
                silverstoneHighlight.SetActive(false);
                loadSilverstoneHighlight.enabled = false;
                break;
            case 3:
                easyHighlight.SetActive(false);
                loadEasyHighlight.enabled = false;
                mediumHighlight.SetActive(false);
                loadMediumHighlight.enabled = false;
                hardHighlight.SetActive(false);
                loadHardHighlight.enabled = false;
                silverstoneHighlight.SetActive(true);
                loadHardHighlight.enabled = true;
                break;
        }
        
        var populationSize = PlayerPrefs.GetInt(PlayerPrefKeys.PopulationSize, 20);
        if (populationSize < populationSlider.minValue || populationSize > populationSlider.maxValue)
            populationSize = 20;
        populationSlider.value = populationSize;
        populationSizeText.text = "" + populationSize;
        
        var isCrossOver = PlayerPrefs.GetInt(PlayerPrefKeys.IsCrossover, 1);
        if (isCrossOver is > 1 or < 0)
            isCrossOver = 1;
        _isCrossover = isCrossOver;
        crossoverBtnText.text = _isCrossover == 1 ? "on" : "off";
        
        var crossoverType = PlayerPrefs.GetInt(PlayerPrefKeys.CrossoverType, 0);
        if (crossoverType > crossoverTypeDropdown.options.Count - 1 || crossoverType < 0)
            crossoverType = 0;
        crossoverTypeDropdown.value = crossoverType;
        
        var isMutation = PlayerPrefs.GetInt(PlayerPrefKeys.IsMutation, 1);
        if (isMutation is > 1 or < 0)
            isMutation = 0;   
        _isMutation = isMutation;
        mutationBtnText.text = _isMutation == 1 ? "on" : "off";
        
        var mutationType = PlayerPrefs.GetInt(PlayerPrefKeys.MutationType, 0);
        if (mutationType > mutationTypeDropdown.options.Count - 1 || mutationType < 0)
            mutationType = 0;
        mutationTypeDropdown.value = mutationType;
        
        var mutationRate = PlayerPrefs.GetFloat(PlayerPrefKeys.MutationRate, 0.30f);
        if (mutationRate is > 1 or < 0)
            mutationRate = 0.30f;
        mutationRateSlider.value = mutationRate;
        mutationRateText.text = "" + mutationRate;
        
        var selectionType = PlayerPrefs.GetInt(PlayerPrefKeys.SelectionType, 0);
        if (selectionType > selectionTypeDropdown.options.Count - 1 || selectionType < 0)
            selectionType = 0;
        selectionTypeDropdown.value = selectionType;
        
        var tourSize = PlayerPrefs.GetInt(PlayerPrefKeys.TourSize, 1);
        if (tourSize > populationSlider.value || tourSize < 0)
            tourSize = 0;
        tourSizeSlider.maxValue = populationSlider.value;
        tourSizeSlider.value = tourSize;
        tourSizeText.text = "" + tourSize;
        
        var elitism = PlayerPrefs.GetInt(PlayerPrefKeys.Elitism, 0);
        if (elitism > populationSlider.value || elitism < 0)
            elitism = 0;
        elitismSlider.maxValue = populationSlider.value;
        elitismSlider.value = elitism;
        elitismText.text = "" + elitism;
        
        var ssrPercent = PlayerPrefs.GetFloat(PlayerPrefKeys.SteadyStatePercentage, 0.5f);
        if (ssrPercent > 1 || ssrPercent < 0)
            ssrPercent = 0.5f;
        steadyStatePercentageSlider.value = ssrPercent;
        steadyStatePercentageText.text = "" + ssrPercent;
        
        var numOfLayers = PlayerPrefs.GetInt(PlayerPrefKeys.NumOfLayers, 2);
        if (numOfLayers > scrollViewContent.childCount)
            numOfLayers = scrollViewContent.childCount;
        numOfLayersSlider.value = numOfLayers;
        numOfLayersText.text = "" + numOfLayers;
        
        _numOfNodes = Array.ConvertAll(PlayerPrefs.GetString(PlayerPrefKeys.NumOfNodes, "5,5,5,5,5,5,5,5,5,5")
            .Split(','), int.Parse);
        _activationFunctions = Array.ConvertAll(PlayerPrefs.GetString(PlayerPrefKeys.ActivationFunctions, "0,0,0,0,0,0,0,0,0,0")
            .Split(','), int.Parse);
        var childCount = scrollViewContent.childCount;
        if (_numOfNodes.Length != childCount || _activationFunctions.Length != childCount)
        {
            _numOfNodes = new int[childCount];
            _activationFunctions = new int[childCount];
            for (var i = 0; i < childCount; i++)
            {
                _numOfNodes[i] = 5;
                _activationFunctions[i] = 0;
            }
        }
        for (var i = 0; i < scrollViewContent.childCount; i++)
        {
            var layerPanel = scrollViewContent.GetChild(i);
            var nnLayerUIManager = layerPanel.GetComponent<NNLayerListTile>();
            _nnLayerUIManagers.Add(nnLayerUIManager);
            nnLayerUIManager.SetUIManager(this);
            nnLayerUIManager.SetNumOfNodes(_numOfNodes[i]);
            nnLayerUIManager.SetActivationFunction(_activationFunctions[i]);
            scrollViewContent.GetChild(i).gameObject.SetActive(i < numOfLayers);

            layerPanel.Find("LayerNumberText").GetComponent<TMP_Text>().text = "Hidden Layer " + (i + 1);
        }
        
        var numOfRaycasts = PlayerPrefs.GetInt(PlayerPrefKeys.NumOfRaycasts, 5);
        if (numOfRaycasts < numOfRaycastsSlider.minValue || numOfRaycasts > numOfRaycastsSlider.maxValue)
            numOfRaycasts = 5;
        numOfRaycastsSlider.value = numOfRaycasts;
        numOfRaycastsText.text = "" + numOfRaycasts;
        
        var modelName = PlayerPrefs.GetString(PlayerPrefKeys.ModelName, "DefaultName");
        if (modelName.Contains(" "))
            modelName = modelName.Substring(0, modelName.LastIndexOf(" ", StringComparison.Ordinal));
        modelNameInputField.text = modelName;
        
        var terminationCondition = PlayerPrefs.GetInt(PlayerPrefKeys.TerminationCondition, 0);
        if (terminationCondition > terminationConditionDropdown.options.Count - 1 || terminationCondition < 0)
            terminationCondition = 0;
        terminationConditionDropdown.value = terminationCondition;
        
        var steps = PlayerPrefs.GetInt(PlayerPrefKeys.StepsValue, 30000);
        if (steps < stepsSlider.minValue * 60 * 50 || steps > stepsSlider.maxValue * 50 * 60)
            steps = 30000;
        var minutes = steps / 50 / 60;
        stepsSlider.value = minutes;
        stepsText.text = "" + minutes + " min";
        
        var generations = PlayerPrefs.GetInt(PlayerPrefKeys.GenerationsValue, 100);
        if (generations < generationsSlider.minValue || generations > generationsSlider.maxValue)
            generations = 100;
        generationsSlider.value = generations;
        generationsText.text = "" + generations;
    }
    
    public void SetModelName()
    {
        // Only allow alphanumeric characters and underscores
        modelNameInputField.text = new string(Array.FindAll(modelNameInputField.text.ToCharArray(), c => (char.IsLetterOrDigit(c) || c == '_')));
        if (modelNameInputField.text.Length > 20)
            modelNameInputField.text = modelNameInputField.text.Substring(0, 20);
        if (modelNameInputField.text.Length != 0)
            PlayerPrefs.SetString(PlayerPrefKeys.ModelName, modelNameInputField.text);
        if (modelNameInputField.text.Length == 0)
            PlayerPrefs.SetString(PlayerPrefKeys.ModelName, "DefaultName");
    }
    
    public void SetEAMethod()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.Method, 0);
        rlHighlight.SetActive(false);
        eaHighlight.SetActive(true);
        RLButtons.SetActive(false);
        EAButtons.SetActive(true);
    }
    
    public void SetRLMethod()
    { 
        rlUnavailable.SetActive(true);
        modelNameInput.SetActive(false);
    }
    
    public void CloseRLUnavailable()
    {
        rlUnavailable.SetActive(false);
        modelNameInput.SetActive(true);
    }
    
    public void SetEasyMap()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.TrackDifficulty, 0);
        easyHighlight.SetActive(true);
        loadEasyHighlight.enabled = true;
        mediumHighlight.SetActive(false);
        loadMediumHighlight.enabled = false;
        hardHighlight.SetActive(false);
        loadHardHighlight.enabled = false;
        silverstoneHighlight.SetActive(false);
        loadSilverstoneHighlight.enabled = false;
    }
    
    public void SetMediumMap()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.TrackDifficulty, 1);
        easyHighlight.SetActive(false);
        loadEasyHighlight.enabled = false;
        mediumHighlight.SetActive(true);
        loadMediumHighlight.enabled = true;
        hardHighlight.SetActive(false);
        loadHardHighlight.enabled = false;
        silverstoneHighlight.SetActive(false);
        loadSilverstoneHighlight.enabled = false;
    }
    
    public void SetHardMap()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.TrackDifficulty, 2);
        easyHighlight.SetActive(false);
        loadEasyHighlight.enabled = false;
        mediumHighlight.SetActive(false);
        loadMediumHighlight.enabled = false;
        hardHighlight.SetActive(true);
        loadHardHighlight.enabled = true;
        silverstoneHighlight.SetActive(false);
        loadSilverstoneHighlight.enabled = false;
    }
    
    public void SetSilverstoneMap()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.TrackDifficulty, 3);
        easyHighlight.SetActive(false);
        loadEasyHighlight.enabled = false;
        mediumHighlight.SetActive(false);
        loadMediumHighlight.enabled = false;
        hardHighlight.SetActive(false);
        loadHardHighlight.enabled = false;
        silverstoneHighlight.SetActive(true);
        loadHardHighlight.enabled = true;
    }

    public void PopulationSizeChanged()
    {
        var value = populationSlider.value;
        PlayerPrefs.SetInt(PlayerPrefKeys.PopulationSize, (int) value);
        populationSizeText.text = "" + value;
        
        tourSizeSlider.maxValue = value;
        elitismSlider.maxValue = value;
    }
    
    public void CrossoverToggleChanged()
    {
        _isCrossover = _isCrossover == 1 ? 0 : 1;
        PlayerPrefs.SetInt(PlayerPrefKeys.IsCrossover, _isCrossover);
        crossoverBtnText.text = _isCrossover == 1 ? "on" : "off";
    }

    public void CrossoverTypeChanged()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.CrossoverType, crossoverTypeDropdown.value);
    }
    
    public void MutationToggleChanged()
    {
        _isMutation = _isMutation == 1 ? 0 : 1;
        PlayerPrefs.SetInt(PlayerPrefKeys.IsMutation, _isMutation);
        mutationBtnText.text = _isMutation == 1 ? "on" : "off";
    }
    
    public void MutationTypeChanged()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.MutationType, mutationTypeDropdown.value);
    }
    
    public void MutationRateChanged()
    {
        var value = Mathf.Round(mutationRateSlider.value * 100) / 100f;
        PlayerPrefs.SetFloat(PlayerPrefKeys.MutationRate, value);
        mutationRateText.text = "" + value;
    }
    
    public void SelectionTypeChanged()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.SelectionType, selectionTypeDropdown.value);
    }
    
    public void TourSizeChanged()
    {
        var tourSize = (int) tourSizeSlider.value;
        PlayerPrefs.SetInt(PlayerPrefKeys.TourSize, tourSize);
        tourSizeText.text = "" + tourSize;
    }
    
    public void ElitismChanged()
    {
        var elitism = (int) elitismSlider.value;
        PlayerPrefs.SetInt(PlayerPrefKeys.Elitism, elitism);
        elitismText.text = "" + elitism;
    }
    
    public void SteadyStatePercentageChanged()
    {
        var value = Mathf.Round(steadyStatePercentageSlider.value * 100) / 100f;
        PlayerPrefs.SetFloat(PlayerPrefKeys.SteadyStatePercentage, value);
        steadyStatePercentageText.text = "" + value;
    }
    
    public void NumOfLayersChanged()
    {
        var numOfLayers = (int) numOfLayersSlider.value;
        PlayerPrefs.SetInt(PlayerPrefKeys.NumOfLayers, numOfLayers);
        numOfLayersText.text = "" + numOfLayers;
        for (var i = 0; i < scrollViewContent.childCount; i++)
        {
            scrollViewContent.GetChild(i).gameObject.SetActive(i < numOfLayers);
        }
    }

    public void NumOfNodesChanged(NNLayerListTile nnLayerListTile, int numOfNodes)
    {
        _numOfNodes[_nnLayerUIManagers.IndexOf(nnLayerListTile)] = numOfNodes;
        PlayerPrefs.SetString(PlayerPrefKeys.NumOfNodes, string.Join(",", _numOfNodes));
    }
    
    public void ActivationFunctionChanged(NNLayerListTile nnLayerListTile, int activationFunction)
    {
        _activationFunctions[_nnLayerUIManagers.IndexOf(nnLayerListTile)] = activationFunction;
        PlayerPrefs.SetString(PlayerPrefKeys.ActivationFunctions, string.Join(",", _activationFunctions));
    }
    
    public void NumOfRaycastsChanged()
    {
        var value = (int) numOfRaycastsSlider.value;
        PlayerPrefs.SetInt(PlayerPrefKeys.NumOfRaycasts, value);
        numOfRaycastsText.text = "" + value;
    }
    
    public void TerminationConditionChanged()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.TerminationCondition, terminationConditionDropdown.value);
    }
    
    public void StepsChanged()
    {
        var minutes = (int) stepsSlider.value;
        stepsText.text = "" + minutes + " min";
        var steps = minutes * 60 * 50;
        PlayerPrefs.SetInt(PlayerPrefKeys.StepsValue, steps);
    }
    
    public void GenerationsChanged()
    {
        var value = (int) generationsSlider.value;
        PlayerPrefs.SetInt(PlayerPrefKeys.GenerationsValue, value);
        generationsText.text = "" + value;
    }

    public void StartTraining()
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.LoadModel, 0);
        PlayerPrefs.Save();
        var track = PlayerPrefs.GetInt(PlayerPrefKeys.TrackDifficulty, 0);
        switch (track)
        {
            case 0:
                uiMenuManager.LoadScene("EasyEA");
                break;
            case 1:
                uiMenuManager.LoadScene("MediumEA");
                break;
            case 2:
                uiMenuManager.LoadScene("HardEA");
                break;
            case 3:
                uiMenuManager.LoadScene("SilverstoneEA");
                break;
        }
    }



}
