using System;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] private EA ea;
    
    private FileDataHandler _fileDataHandler;

    public ModelData LoadEANeuralNetworkModel(string modelName)
    {
        _fileDataHandler = new FileDataHandler();
        return _fileDataHandler.LoadModel(modelName);
    }
    
    public void SaveEANeuralNetworkModel()
    {
        var modelName = PlayerPrefs.GetString(PlayerPrefKeys.ModelName, "DefaultName");
        if (modelName.Contains(" "))
            modelName = modelName.Substring(0, modelName.LastIndexOf(" ", StringComparison.Ordinal));
        
        modelName += " " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        _fileDataHandler = new FileDataHandler();
        var modelData = ea.GetData();
        modelData.modelName = modelName;
        _fileDataHandler.SaveModel(modelData, modelName);
    }
}


