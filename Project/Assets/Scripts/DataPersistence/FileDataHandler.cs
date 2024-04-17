using System;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FileDataHandler
{
    private string _dataDirPath;
    
    public FileDataHandler()
    {
        _dataDirPath = Path.Combine(Application.persistentDataPath, "models");
    }

    public ModelData LoadModel(string fileName)
    {
        var fullPath = Path.Combine(_dataDirPath, fileName);
        if (!File.Exists(fullPath)) 
            return null;
        ModelData data;
        try
        {
            string dataToLoad;
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }
            data = JsonConvert.DeserializeObject<ModelData>(dataToLoad);
        }
        catch (Exception e)
        {
            throw new Exception("Error loading data from " + fullPath + ": " + e.Message);
        }

        return data;
    }

    public void SaveModel(ModelData modelData, string fileName)
    {
        var myData = modelData;
        var fullPath = Path.Combine(_dataDirPath, fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            var dataToStore = JsonConvert.SerializeObject(myData, Formatting.Indented);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error saving data to " + fullPath + ": " + e.Message);
        }
    }
    
    public ModelData[] LoadAllModels()
    {
        var files = Directory.GetFiles(_dataDirPath);
        var models = new ModelData[files.Length];
        for (var i = 0; i < files.Length; i++)
        {
            models[i] = LoadModel(files[i]);
        }

        return models;
    }
    
    public void DeleteModel(string fileName)
    {
        var fullPath = Path.Combine(_dataDirPath, fileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
