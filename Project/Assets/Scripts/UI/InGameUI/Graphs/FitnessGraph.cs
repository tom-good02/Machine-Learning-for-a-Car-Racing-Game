using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitnessGraph : MonoBehaviour
{
    [SerializeField] private RectTransform _graphContainer;
    [SerializeField] private GameObject xAxis;
    [SerializeField] private GameObject yAxis;
    
    [SerializeField] private Sprite pointSprite;
    
    [SerializeField] private RectTransform _bestPointsContainer;
    [SerializeField] private RectTransform _averagePointsContainer;
    [SerializeField] private RectTransform _bestConnectionsContainer;
    [SerializeField] private RectTransform _averageConnectionsContainer;
    
    private float _maxFitness = float.MinValue;
    private float _minFitness = float.MaxValue;
    
    private int _numOfGenerations;
    private List<float> _bestFitnesses = new();
    private List<RectTransform> _bestPoints = new();
    private List<float> _averageFitnesses = new();
    private List<RectTransform> _averagePoints = new();

    private List<RectTransform> _bestConnections = new();
    private List<RectTransform> _averageConnections = new();

    public void CreateNewGraphFromBestAndAverageFitnessFromData(List<float> bestFitnesses, List<float> averageFitnesses, int numGenerations)
    {
        _bestFitnesses = new List<float>(bestFitnesses);
        _averageFitnesses = new List<float>(averageFitnesses);
        _numOfGenerations = numGenerations;
        
        CreateNewGraphFromStoredData();
    }

    private void CreateNewGraphFromStoredData()
    {
        // Clear the graph
        foreach (var point in _bestPoints)
            Destroy(point.gameObject);
        foreach (var point in _averagePoints)
            Destroy(point.gameObject);
        foreach (var connection in _bestConnections)
            Destroy(connection.gameObject);
        foreach (var connection in _averageConnections)
            Destroy(connection.gameObject);
        
        // Update min and max fitness
        for (var i = 0; i < _numOfGenerations; i++)
        {
            if (_bestFitnesses[i] > _maxFitness)
                _maxFitness = _bestFitnesses[i];
            if (_bestFitnesses[i] < _minFitness)
                _minFitness = _bestFitnesses[i];
            if (_averageFitnesses[i] > _maxFitness)
                _maxFitness = _averageFitnesses[i];
            if (_averageFitnesses[i] < _minFitness)
                _minFitness = _averageFitnesses[i];
        }
        
        // Create graph
        var rect = _graphContainer.rect;
        var pointSpacing = 0.9f * rect.width / (_numOfGenerations - 1);
        for (var i = 0; i < _numOfGenerations; i++)
        {
            var xPosition = _numOfGenerations == 1 ? 0.9f * rect.width / 2 : i * pointSpacing;
            var yPosition = Mathf.Lerp(0.05f * rect.height, rect.height * 0.95f, Mathf.InverseLerp(_minFitness, _maxFitness, _bestFitnesses[i]));
            
            var bestPoint = new GameObject("point", typeof(Image));
            var bestPointImage = bestPoint.GetComponent<Image>();
            bestPointImage.sprite = pointSprite;
            bestPoint.transform.SetParent(_bestPointsContainer, false);
            var bestPointRect = bestPoint.GetComponent<RectTransform>();
            bestPointRect.sizeDelta = new Vector2(10, 10);
            bestPointRect.anchorMax = new Vector2(0, 0);
            bestPointRect.anchorMin = new Vector2(0, 0);
            _bestPoints.Add(bestPointRect);
            bestPointRect.anchoredPosition = new Vector2(0.05f * rect.width + xPosition, yPosition);
        
            yPosition = Mathf.Lerp(0.05f * rect.height, rect.height * 0.95f, Mathf.InverseLerp(_minFitness, _maxFitness, _averageFitnesses[i]));
            
            var averagePoint = new GameObject("point", typeof(Image));
            var averagePointImage = averagePoint.GetComponent<Image>();
            averagePointImage.sprite = pointSprite;
            averagePoint.transform.SetParent(_averagePointsContainer, false);
            var averagePointRect = averagePoint.GetComponent<RectTransform>();
            averagePointRect.sizeDelta = new Vector2(10, 10);
            averagePointRect.anchorMax = new Vector2(0, 0);
            averagePointRect.anchorMin = new Vector2(0, 0);
            _averagePoints.Add(averagePointRect);
            averagePointRect.anchoredPosition = new Vector2(0.05f * rect.width + xPosition, yPosition);

            // Create connection
            if (i > 0)
            {
                var startPoint = _bestPoints[i - 1].anchoredPosition;
                var endPoint = _bestPoints[i].anchoredPosition;
                var dir = (endPoint - startPoint).normalized;
                var distance = Vector2.Distance(startPoint, endPoint);
                var connection = new GameObject("connection", typeof(Image));
                connection.GetComponent<Image>().color = new Color(1, 0.6862745f, 0, 1);
                connection.transform.SetParent(_bestConnectionsContainer, false);
                var connectionRect = connection.GetComponent<RectTransform>();
                connectionRect.anchorMin = new Vector2(0, 0);
                connectionRect.anchorMax = new Vector2(0, 0);
                connectionRect.sizeDelta = new Vector2(distance, 3);
                connectionRect.anchoredPosition = startPoint + dir * (distance * 0.5f);
                connectionRect.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
                _bestConnections.Add(connectionRect);
                
                startPoint = _averagePoints[i - 1].anchoredPosition;
                endPoint = _averagePoints[i].anchoredPosition;
                dir = (endPoint - startPoint).normalized;
                distance = Vector2.Distance(startPoint, endPoint);
                connection = new GameObject("connection", typeof(Image));
                connection.GetComponent<Image>().color = new Color(0, 1, 0.6617508f, 1);
                connection.transform.SetParent(_averageConnectionsContainer, false);
                connectionRect = connection.GetComponent<RectTransform>();
                connectionRect.anchorMin = new Vector2(0, 0);
                connectionRect.anchorMax = new Vector2(0, 0);
                connectionRect.sizeDelta = new Vector2(distance, 3);
                connectionRect.anchoredPosition = startPoint + dir * (distance * 0.5f);
                connectionRect.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
                _averageConnections.Add(connectionRect);
            }
        }
    }


    public void UpdateGraph(float bestFitnessOfGeneration, float averageFitnessOfGeneration)
    {
        // Add points to the lists
        _bestFitnesses.Add(bestFitnessOfGeneration);
        _averageFitnesses.Add(averageFitnessOfGeneration);
        
        // Increment generation counter
        _numOfGenerations++;
        
        // Create the new points
        var bestPoint = new GameObject("point", typeof(Image));
        var bestPointImage = bestPoint.GetComponent<Image>();
        bestPointImage.sprite = pointSprite;
        bestPoint.transform.SetParent(_bestPointsContainer, false);
        var bestPointRect = bestPoint.GetComponent<RectTransform>();
        bestPointRect.sizeDelta = new Vector2(10, 10);
        bestPointRect.anchorMax = new Vector2(0, 0);
        bestPointRect.anchorMin = new Vector2(0, 0);
        _bestPoints.Add(bestPointRect);
        
        var averagePoint = new GameObject("point", typeof(Image));
        var averagePointImage = averagePoint.GetComponent<Image>();
        averagePointImage.sprite = pointSprite;
        averagePoint.transform.SetParent(_averagePointsContainer, false);
        var averagePointRect = averagePoint.GetComponent<RectTransform>();
        averagePointRect.sizeDelta = new Vector2(10, 10);
        averagePointRect.anchorMax = new Vector2(0, 0);
        averagePointRect.anchorMin = new Vector2(0, 0);
        _averagePoints.Add(averagePointRect);
        
        // Create the new connections
        if (_numOfGenerations > 1)
        {
            var bestConnection = new GameObject("connection", typeof(Image));
            bestConnection.GetComponent<Image>().color = new Color(1, 0.6862745f, 0, 1);
            bestConnection.transform.SetParent(_bestConnectionsContainer, false);
            var bestConnectionRect = bestConnection.GetComponent<RectTransform>();
            bestConnectionRect.anchorMax = new Vector2(0, 0);
            bestConnectionRect.anchorMin = new Vector2(0, 0);

            var averageConnection = new GameObject("connection", typeof(Image));
            averageConnection.GetComponent<Image>().color = new Color(0, 1, 0.6617508f, 1);
            averageConnection.transform.SetParent(_averageConnectionsContainer, false);
            var averageConnectionRect = averageConnection.GetComponent<RectTransform>();
            averageConnectionRect.anchorMax = new Vector2(0, 0);
            averageConnectionRect.anchorMin = new Vector2(0, 0);
            
            // Add their rect transforms to the lists
            _bestConnections.Add(bestConnectionRect);
            _averageConnections.Add(averageConnectionRect);
            
        }
        
        // Update max and min fitness values
        if (bestFitnessOfGeneration > _maxFitness)
            _maxFitness = bestFitnessOfGeneration;
        if (bestFitnessOfGeneration < _minFitness)
            _minFitness = bestFitnessOfGeneration;
        if (averageFitnessOfGeneration > _maxFitness)
            _maxFitness = averageFitnessOfGeneration;
        if (averageFitnessOfGeneration < _minFitness)
            _minFitness = averageFitnessOfGeneration;
        
        // Calculate the spacing between points
        var rect = _graphContainer.rect;
        var pointSpacing = 0.9f * rect.width / (_numOfGenerations - 1);
        for (var i = 0; i < _numOfGenerations; i++)
        {
            var xPosition = _numOfGenerations == 1 ? 0.9f * rect.width / 2 : i * pointSpacing;
            var yPosition = Mathf.Lerp(0.05f * rect.height, rect.height * 0.95f, Mathf.InverseLerp(_minFitness, _maxFitness, _bestFitnesses[i]));
            _bestPoints[i].anchoredPosition = new Vector2(0.05f * rect.width + xPosition, yPosition);
        
            yPosition = Mathf.Lerp(0.05f * rect.height, rect.height * 0.95f, Mathf.InverseLerp(_minFitness, _maxFitness, _averageFitnesses[i]));
            _averagePoints[i].anchoredPosition = new Vector2(0.05f * rect.width + xPosition, yPosition);

            // Update connection position
            if (i > 0)
            {
                var startPoint = _bestPoints[i - 1].anchoredPosition;
                var endPoint = _bestPoints[i].anchoredPosition;
                var dir = (endPoint - startPoint).normalized;
                var distance = Vector2.Distance(startPoint, endPoint);
                var connectionRect = _bestConnections[i - 1];
                connectionRect.sizeDelta = new Vector2(distance, 3);
                connectionRect.anchoredPosition = startPoint + dir * (distance * 0.5f);
                connectionRect.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));

                startPoint = _averagePoints[i - 1].anchoredPosition;
                endPoint = _averagePoints[i].anchoredPosition;
                dir = (endPoint - startPoint).normalized;
                distance = Vector2.Distance(startPoint, endPoint);
                connectionRect = _averageConnections[i - 1];
                connectionRect.sizeDelta = new Vector2(distance, 3);
                connectionRect.anchoredPosition = startPoint + dir * (distance * 0.5f);
                connectionRect.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
            }
        }
    }
    
    private static float GetAngleFromVectorFloat(Vector3 dir) {
        dir = dir.normalized;
        var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
}
