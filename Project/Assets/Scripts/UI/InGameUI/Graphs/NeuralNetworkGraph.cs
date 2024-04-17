using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeuralNetworkGraph : MonoBehaviour
{
    [Tooltip("The sprite to use for the nodes")]
    [SerializeField] private Sprite nodeSprite;
    
    private RectTransform _networkContainer;
    private RectTransform _connectionsContainer;
    private RectTransform _nodeContainer;
    
    private List<List<GameObject>> _nodes = new();
    private List<List<List<GameObject>>> _newConnections = new();
    
    private void Awake()
    {
        _networkContainer = GetComponent<RectTransform>();
        _connectionsContainer = transform.Find("Connections").GetComponent<RectTransform>();
        _nodeContainer = transform.Find("Nodes").GetComponent<RectTransform>();
    }

    public void UpdateGraph(float[][,] weights)
    {
        for (var i = 0; i < weights.Length; i++)
        {
            for (var j = 0; j < weights[i].GetLength(0); j++)
            {
                for (var k = 0; k < weights[i].GetLength(1); k++)
                {
                    var color = weights[i][j, k] < 0
                        ? new Color(1, 0, 0, Mathf.Abs(weights[i][j, k]))
                        : new Color(0, 1, 0, Mathf.Abs(weights[i][j, k]));
                    _newConnections[i][j][k].GetComponent<Image>().color = color;
                }
            }
        }
    }

    public void CreateGraph(int[] nodesPerLayer, int nodesInInputLayer, int nodesInOutputLayer)
    {
        var totalLayers = nodesPerLayer.Length + 1;
        var rect = _networkContainer.rect;
        
        // Find the maximum number of nodes in any single layer
        var maxNodesInLayer = nodesInInputLayer;
        foreach (var layer in nodesPerLayer)
            if (layer > maxNodesInLayer)
                maxNodesInLayer = layer;
        if (nodesInOutputLayer > maxNodesInLayer)
            maxNodesInLayer = nodesInOutputLayer;
        
        // Calculate the size of the nodes
        var sizeOfNode = 0.75f * rect.height / maxNodesInLayer;
        if (sizeOfNode > 40)
            sizeOfNode = 40;
        
        // Create input layer
        var nodeColor = new Color(0.9409347f, 0.5924528f, 1, 1);
        CreateNodes(nodesInInputLayer, 0, totalLayers, sizeOfNode, nodeColor);
        
        // Create hidden layers
        nodeColor = new Color(1, 0.6862745f, 0, 1);
        for (var i = 0; i < nodesPerLayer.Length; i++)
        {
            CreateNodes(nodesPerLayer[i], i + 1, totalLayers, sizeOfNode, nodeColor);
        }
        
        // Create output layer
        nodeColor = new Color(0.3320755f, 0.7926453f, 1, 1);
        CreateNodes(nodesInOutputLayer, nodesPerLayer.Length + 1, totalLayers, sizeOfNode, nodeColor);
        
        // Create connections
        for (var i = 0; i < _nodes.Count - 1; i++)
        {
            var layerConnections = new List<List<GameObject>>();
            for (var j = 0; j < _nodes[i].Count; j++)
            {
                var nodeConnections = new List<GameObject>();
                for (var k = 0; k < _nodes[i + 1].Count; k++)
                {
                    var start = _nodes[i][j].GetComponent<RectTransform>().anchoredPosition;
                    var end = _nodes[i + 1][k].GetComponent<RectTransform>().anchoredPosition;
                    nodeConnections.Add(DrawLine(start, end));
                }
                layerConnections.Add(nodeConnections);
            }
            _newConnections.Add(layerConnections);
        }
    }
    
    private void CreateNodes(int numberOfNodes, int layerNumber, int totalLayers, float sizeOfNode, Color nodeColor)
    {
        var rect = _networkContainer.rect;
        var layerNodes = new List<GameObject>();
        var spacing = rect.height * 0.85f / numberOfNodes;
        if (numberOfNodes % 2 == 0)
        {
            for (var i = 0; i < numberOfNodes / 2; i++)
            {
                layerNodes.Add(CreateNode(new Vector2(
                        rect.width * 0.05f + layerNumber * (rect.width * 0.9f / totalLayers), 
                        rect.height * 0.1f + (rect.height * 0.9f / 2 + (0.5f * spacing + i * spacing))), 
                    sizeOfNode, nodeColor));
                layerNodes.Add(CreateNode(new Vector2(
                        rect.width * 0.05f + layerNumber * (rect.width * 0.9f / totalLayers), 
                        rect.height * 0.1f + (rect.height * 0.9f / 2 - (0.5f * spacing + i * spacing))), 
                    sizeOfNode, nodeColor));
            }
        }
        else
        {
            layerNodes.Add(CreateNode(new Vector2(
                    rect.width * 0.05f + layerNumber * (rect.width * 0.9f / totalLayers), 
                    rect.height * 0.1f + rect.height * 0.9f / 2), 
                sizeOfNode, nodeColor));
            
            for (var i = 0; i < (numberOfNodes - 1) / 2; i++)
            {
                layerNodes.Add(CreateNode(new Vector2(
                        rect.width * 0.05f + layerNumber * (rect.width * 0.9f / totalLayers), 
                        spacing / 2 + rect.height * 0.1f + (rect.height * 0.9f / 2 + (0.5f * spacing + i * spacing))), 
                    sizeOfNode, nodeColor));
                layerNodes.Add(CreateNode(new Vector2(
                        rect.width * 0.05f + layerNumber * (rect.width * 0.9f / totalLayers), 
                        -spacing / 2 + rect.height * 0.1f + (rect.height * 0.9f / 2 - (0.5f * spacing + i * spacing))), 
                    sizeOfNode, nodeColor));
            }
        }
        _nodes.Add(layerNodes);
    }
    
    private GameObject CreateNode(Vector2 anchoredPosition, float size, Color nodeColor)
    {
        var circle = new GameObject("circle", typeof(Image));
        circle.transform.SetParent(_nodeContainer, false);
        circle.GetComponent<Image>().sprite = nodeSprite;
        circle.GetComponent<Image>().color = nodeColor;
        var rectTransform = circle.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(size, size);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return circle;
    }
    
    private GameObject DrawLine(Vector2 startPoint, Vector2 endPoint)
    {
        var line = new GameObject("line", typeof(Image));
        line.transform.SetParent(_connectionsContainer, false);
        line.GetComponent<Image>().color = new Color(1, 0.6862745f, 0, 1);
        var rectTransform = line.GetComponent<RectTransform>();
        var dir = (endPoint - startPoint).normalized;
        var distance = Vector2.Distance(startPoint, endPoint);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 2);
        rectTransform.anchoredPosition = startPoint + dir * (distance * 0.5f);
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        return line;
    }
    
    private static float GetAngleFromVectorFloat(Vector3 dir) {
        dir = dir.normalized;
        var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
}
