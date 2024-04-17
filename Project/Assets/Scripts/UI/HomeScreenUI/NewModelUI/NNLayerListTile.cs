using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NNLayerListTile : MonoBehaviour
{
    [Header("LAYER SETTINGS")]
    [SerializeField] private Slider numOfNodesSlider;
    [SerializeField] private TMP_Text numOfNodesText;
    [SerializeField] private TMP_Dropdown activationFunctionDropdown;
    
    private UINewModelSettingsManager _uiNewModelSettingsManager;

    public void SetUIManager(UINewModelSettingsManager uiNewModelSettingsManager)
    {
        _uiNewModelSettingsManager = uiNewModelSettingsManager;
    }
    
    public void SetNumOfNodes(int value)
    {
        numOfNodesSlider.value = value;
        numOfNodesText.text = "" + value;
    }
    
    public void SetActivationFunction(int value)
    {
        activationFunctionDropdown.value = value;
    }

    public void NumOfNodesChanged()
    {
        var numOfNodes = (int) numOfNodesSlider.value;
        numOfNodesText.text = "" + numOfNodes;
        _uiNewModelSettingsManager.NumOfNodesChanged(this, numOfNodes);
    }
    
    public void ActivationFunctionChanged()
    {
        _uiNewModelSettingsManager.ActivationFunctionChanged(this, activationFunctionDropdown.value);
    }
}
