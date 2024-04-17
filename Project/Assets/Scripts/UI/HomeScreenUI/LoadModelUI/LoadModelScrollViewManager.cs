using System;
using System.Collections.Generic;
using SlimUI.ModernMenu;
using UnityEngine;

public class LoadModelScrollViewManager : MonoBehaviour
{
    [SerializeField] private GameObject modelListPrefab;
    [SerializeField] private Transform content;
    
    [SerializeField] private UIMenuManager uiMenuManager;
    
    private List<GameObject> _modelList = new();

    public void Awake()
    {
        var modelData = LoadModels();
        print(modelData.Length);
        foreach (var model in modelData)
        {
            var modelList = Instantiate(modelListPrefab, content);
            var modelListManager = modelList.GetComponent<SavedModelListTile>();
            modelListManager.SetModelName(model.modelName);
            modelListManager.SetGenerations(model.generations);
            modelListManager.SetModelData(model);
            modelListManager.SetLoadModelScrollViewManager(this);
            _modelList.Add(modelList);
        }
    }

    private ModelData[] LoadModels()
    {
        var fileDataHandler = new FileDataHandler();
        return fileDataHandler.LoadAllModels();
    }

    public void StartTraining(ModelData modelData)
    {
        PlayerPrefs.SetString(PlayerPrefKeys.ModelName, modelData.modelName);
        PlayerPrefs.SetInt(PlayerPrefKeys.LoadModel, 1);
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

