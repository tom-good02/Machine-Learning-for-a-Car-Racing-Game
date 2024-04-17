using System;
using UnityEngine;
using Unity.Mathematics;

public class NeuralNetwork : MonoBehaviour
{

    public enum ActivationFunction 
    {
        ReLU,
        Tanh,
        Sigmoid,
        None
    }

    [SerializeField] private Layer[] _layers;
    public const int NumOutputNeurons = 4;
    public const int NumAdditionalInputs = 4;
    
    private float _fitness = 0;

    public void Awake() 
    {
        // get NNLayers from settings
        var settings = GameObject.Find("Settings").GetComponent<Settings>();
        var nnLayers = settings.GetNeuralNetworkLayerSettings().GetNeuralNetworkLayers();
        
        // create layers (+ 1 for the output layer)
        _layers = new Layer[nnLayers.Length + 1];

        // number of inputs
        var numInputs = settings.GetRaycastSettings().GetNumOfRaycasts() + NumAdditionalInputs;

        // if number of layers == 1, straight to output layer
        if(_layers.Length == 1) 
            _layers[0] = new Layer(numInputs, NumOutputNeurons, ActivationFunction.Sigmoid);
        // otherwise create hidden layers layers
        else 
        {
            // first hidden layer
            _layers[0] = new Layer(numInputs, nnLayers[0].GetNumNeurons(), nnLayers[0].GetActivationFunction());
            // second onwards hidden layers
            for(var i = 1; i < nnLayers.Length; i++) 
                _layers[i] = new Layer(nnLayers[i - 1].GetNumNeurons(), nnLayers[i].GetNumNeurons(), nnLayers[i].GetActivationFunction());

            _layers[^1] = new Layer(nnLayers[^1].GetNumNeurons(), NumOutputNeurons, ActivationFunction.Sigmoid);
        }
    }

    public float[] Brain(float[] inputs) 
    {
        // feed forward through layers
        _layers[0].FeedForward(inputs);
        _layers[0].ApplyActivationFunction();

        for(var i = 1; i < _layers.Length; i++) 
        {
            _layers[i].FeedForward(_layers[i - 1].GetOutputs());
            _layers[i].ApplyActivationFunction();
        }
        
        return _layers[^1].GetOutputs();
    }

    public void AddFitness(float fitness)
    {
        _fitness += fitness;
    }

    public float GetFitness() 
    {
        return _fitness;
    }

    public void ResetFitness() 
    {
        _fitness = 0;
    }

    public void GetCopyOfWeightsAndBiases(out float[][,] weightArray, out float[][] biasArray) 
    {
        weightArray = new float[_layers.Length][,];
        biasArray = new float[_layers.Length][];
        
        for (var i = 0; i < _layers.Length; i++) 
        {
            var l = _layers[i];
            var tempWeights = l.GetWeights();
            var tempBiases = l.GetBiases();
            
            weightArray[i] = new float[tempWeights.GetLength(0), tempWeights.GetLength(1)];
            biasArray[i] = new float[tempBiases.Length];
            
            Array.Copy(tempWeights, weightArray[i], tempWeights.Length);
            Array.Copy(tempBiases, biasArray[i], tempBiases.Length);
        }
    }

    public void SetWeightsAndBiases(float[][,] weights, float[][] biases) 
    {
        for (var i = 0; i < _layers.Length; i++) 
        {
            _layers[i].SetWeights(weights[i]);
            _layers[i].SetBiases(biases[i]);
        }
    }

    [Serializable] private class Layer 
    {
        private int _numInputNeurons;
        private int _numNeurons;
        private ActivationFunction _activationFunction;

        [SerializeField] private float[,] _weights;
        [SerializeField] private float[] _biases;
        private float[] _outputs;

        public Layer(int numInputNeurons, int numNeurons, ActivationFunction activationFunction) 
        {
            _numInputNeurons = numInputNeurons;
            _numNeurons = numNeurons;
            _activationFunction = activationFunction;

            _weights = new float[_numInputNeurons, _numNeurons];
            _biases = new float[_numNeurons];
            _outputs = new float[_numNeurons];

            InitialiseWeights();
        }

        public float[] GetOutputs() 
        {
            return _outputs;
        }

        public int GetNumNeurons() 
        {
            return _numNeurons;
        }

        public int GetNumInputNeurons() 
        {
            return _numInputNeurons;
        }

        public float[,] GetWeights() 
        {
            return _weights;
        }

        public float[] GetBiases() 
        {
            return _biases;
        }

        public void SetWeights(float[,] weights) 
        {
            _weights = weights;
        }
        
        public void SetBiases(float[] biases) 
        {
            _biases = biases;
        }

        public void FeedForward(float[] inputs) 
        {
            for(var i = 0; i < _numNeurons; i++) 
            {
                var sum = 0f;
                for(var j = 0; j < _numInputNeurons; j++) 
                {
                    sum += inputs[j] * _weights[j, i];
                }
                sum += _biases[i];
                _outputs[i] = (float) Math.Tanh(sum);
            }
        }

        public void ApplyActivationFunction() 
        {
            switch(_activationFunction) {
                case ActivationFunction.ReLU:
                    ApplyReLU();
                    break;
                case ActivationFunction.Tanh:
                    ApplyTanh();
                    break;
                case ActivationFunction.Sigmoid:
                    ApplySigmoid();
                    break;
                case ActivationFunction.None:
                    break;
            }
        }

        private void ApplyReLU() 
        {
            for(var i = 0; i < _numNeurons; i++) 
            {
                _outputs[i] = Mathf.Max(0, _outputs[i]);
            }
        }

        private void ApplyTanh() 
        {
            for(var i = 0; i < _numNeurons; i++) 
            {
                _outputs[i] = math.tanh(_outputs[i]);
            }
        }

        private void ApplySigmoid() {
            for(var i = 0; i < _numNeurons; i++) 
            {
                var k = Mathf.Exp(_outputs[i]);
                _outputs[i] = k / (1.0f + k);
            }
        }

        // Initialise weights with Xavier Uniform Weight initialisation 
        private void InitialiseWeights() 
        {
            for(var i = 0; i < _numNeurons; i++) 
            {
                for(var j = 0; j < _numInputNeurons; j++) 
                {
                    _weights[j, i] = (float) Math.Sqrt(6.0 / (_numInputNeurons + _numNeurons)) * UnityEngine.Random.Range(-1.0f, 1.0f);
                }
            }
        }

    }
}
