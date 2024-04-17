using TMPro;
using UnityEngine;

public class SavedModelListTile : MonoBehaviour
{
    [SerializeField] private TMP_Text modelName;
    [SerializeField] private TMP_Text generations;
    
    private ModelData _modelData;
    private LoadModelScrollViewManager _loadModelScrollViewManager;
    
    public void SetModelName(string name)
    {
        modelName.text = name;
    }
    
    public void SetGenerations(int gen)
    {
        generations.text = "Generations: " + gen;
    }
    
    public void SetModelData(ModelData modelData)
    {
        _modelData = modelData;
    }
    
    public void SetLoadModelScrollViewManager(LoadModelScrollViewManager loadModelScrollViewManager)
    {
        _loadModelScrollViewManager = loadModelScrollViewManager;
    }

    public void StartTraining()
    {
        _loadModelScrollViewManager.StartTraining(_modelData);
    }
}
