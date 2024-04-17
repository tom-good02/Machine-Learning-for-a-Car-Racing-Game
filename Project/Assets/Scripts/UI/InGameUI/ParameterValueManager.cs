using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ParameterValueManager : MonoBehaviour
{
    [Header("EA Parameters")]
    [SerializeField] private TMP_Text populationSizeText;
    [SerializeField] private TMP_Text crossoverEnabledText;
    [SerializeField] private TMP_Text crossoverTypeText;
    [SerializeField] private TMP_Text mutationEnabledText;
    [SerializeField] private TMP_Text mutationTypeText;
    [SerializeField] private TMP_Text mutationRateText;
    [SerializeField] private TMP_Text selectionTypeText;
    [SerializeField] private TMP_Text tourSizeText;
    [SerializeField] private TMP_Text elitismText;
    [SerializeField] private TMP_Text ssrPercentText;
    
    [Header("NN Parameters")]
    [SerializeField] private List<TMP_Text> hiddenLayerNumTexts;
    [SerializeField] private List<TMP_Text> hiddenLayerNodeCountTexts;
    [SerializeField] private List<TMP_Text> hiddenLayerActivationFunctionTexts;
    
    [Header("Input Parameters")]
    [SerializeField] private TMP_Text numOfSensorsText;

    public void SetEAValues(EASettings eaSettings)
    {
        populationSizeText.text = eaSettings.GetPopulationSize().ToString();
        crossoverEnabledText.text = eaSettings.GetIsCrossover().ToString();
        crossoverTypeText.text = eaSettings.GetCrossoverType() switch
        {
            EASettings.CrossoverType.NeuronSwap => "Neuron Swap",
            EASettings.CrossoverType.WeightSwap => "Weight Swap",
            EASettings.CrossoverType.BiasSwap => "Bias Swap",
            EASettings.CrossoverType.LayerSwap => "Layer Swap",
            _ => crossoverTypeText.text
        };
        mutationEnabledText.text = eaSettings.GetIsMutation().ToString();
        mutationTypeText.text = eaSettings.GetMutationType() switch
        {
            EASettings.MutationType.AddOrSubtractZeroToOne => "+/- 0-1",
            EASettings.MutationType.ChangeByPercentage => "Change by %",
            EASettings.MutationType.MultiplyByNegativeOne => "Multiply by -1",
            EASettings.MutationType.Replace => "Replace",
            _ => mutationTypeText.text
        };
        mutationRateText.text = eaSettings.GetMutationRate().ToString(CultureInfo.CurrentCulture);
        selectionTypeText.text = eaSettings.GetSelectionType() switch
        {
            EASettings.SelectionType.RouletteWheel => "Roulette Wheel",
            EASettings.SelectionType.Tournament => "Tournament",
            _ => selectionTypeText.text
        };
        tourSizeText.text = eaSettings.GetTourSize().ToString();
        elitismText.text = eaSettings.GetElitism().ToString();
        ssrPercentText.text = eaSettings.GetSteadyStateReplacementPercent().ToString(CultureInfo.CurrentCulture);
    }

    public void SetNNValues(NeuralNetworkLayerSettings neuralNetworkLayerSettings)
    {
        for (var i = 0; i < neuralNetworkLayerSettings.GetNumOfLayers(); i++)
        {
            hiddenLayerNumTexts[i].enabled = true;
            hiddenLayerNodeCountTexts[i].enabled = true;
            hiddenLayerActivationFunctionTexts[i].enabled = true;
            
            hiddenLayerNumTexts[i].text = (i + 1).ToString();
            hiddenLayerNodeCountTexts[i].text = neuralNetworkLayerSettings.GetNumOfNeurons()[i].ToString();
            var activationFunction = (NeuralNetwork.ActivationFunction) neuralNetworkLayerSettings.GetActivationFunctions()[i];
            hiddenLayerActivationFunctionTexts[i].text = activationFunction.ToString();
        }
    }
    
    public void SetInputValues(int numOfSensors)
    {
        numOfSensorsText.text = numOfSensors.ToString();
    }
}
