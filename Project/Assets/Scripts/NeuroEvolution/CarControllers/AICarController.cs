using UnityEngine;

public class AICarController : ParentCarController
{
    private AICarSettings _aiCarSettings;
    
    private int _id;

    private EA _ea;
    [SerializeField] private NeuralNetwork _neuralNetwork;
    private Raycasts _raycasts;

    private bool _dead;
    private float _deathTimerLength;
    private float _globalDeathTimer;
    
    private int _nextCheckpointIndex;
    private int _totalCheckpointsReached;
    private Transform _checkpoints;
    private float _lastCheckpointTime;
    
    public new void Awake() 
    {
        base.Awake();
        _aiCarSettings = GetSettings().GetAICarSettings();
        _deathTimerLength = _aiCarSettings.GetDeathTimerLength();
        _globalDeathTimer = _aiCarSettings.GetGlobalDeathTimer();
        _neuralNetwork = GetComponent<NeuralNetwork>();
        _raycasts = GetComponent<Raycasts>();
    }

    private new void FixedUpdate()
    {
        if (_dead)
            return;
        
        base.FixedUpdate();
        if (CountdownTimer())
        {
            if (_dead)
                return;
            _neuralNetwork.AddFitness(-4 * _aiCarSettings.GetDeathTimerLength());
            Die();   
        }
        
        HandleNetwork();
        UpdateCarMovement();
        UpdateWheelMeshes();
    }

    private void HandleNetwork() 
    {
        var nnInputs = new float[_raycasts.GetNumOfRaycasts() + 4];
        nnInputs[0] = GetThrottle();
        nnInputs[1] = GetTurning();
        nnInputs[2] = GetCurrentSpeed();
        nnInputs[3] = GetCurrentTurnAngle();

        for(var i = 0; i < _raycasts.GetNumOfRaycasts(); i++) 
        {
            if(_raycasts.GetHits()[i].collider != null) 
                nnInputs[i + 4] = _raycasts.GetHits()[i].distance;
            else 
                nnInputs[i + 4] = 1000f;
        }
        
        var nnOutput = _neuralNetwork.Brain(nnInputs);
        // output[0] is W
        // output[1] is A
        // output[2] is S
        // output[3] is D
        // use output if > threshold
        
        var threshold = _aiCarSettings.GetThreshold();
        
        SetForward(nnOutput[0] > threshold);
        SetLeft(nnOutput[1] > threshold);
        SetBackward(nnOutput[2] > threshold);
        SetRight(nnOutput[3] > threshold);
    }
    
    public void ThroughCheckpoint(int checkpointIndex)
    {
        if (_dead)
            return;
        if (checkpointIndex == _nextCheckpointIndex)
        {
            if (_lastCheckpointTime == 0)
            {
                StartLapTime();
                _neuralNetwork.AddFitness(_aiCarSettings.GetDeathTimerLength());
                _lastCheckpointTime = Time.fixedTime;
            }
            else
            {
                var timeAsIntSinceLastCheckpoint = Time.fixedTime - _lastCheckpointTime;
                if (timeAsIntSinceLastCheckpoint < 0.1f)
                    timeAsIntSinceLastCheckpoint = 0.1f;
                var deathTimerLength = _aiCarSettings.GetDeathTimerLength();
                _neuralNetwork.AddFitness(10 * deathTimerLength + deathTimerLength / timeAsIntSinceLastCheckpoint);
                _lastCheckpointTime = Time.fixedTime;
            }
            
            _totalCheckpointsReached++;
            if (_totalCheckpointsReached == _checkpoints.childCount + 1)
            {
                EndLapTime();
                _neuralNetwork.AddFitness(_aiCarSettings.GetGlobalDeathTimer() * 10 / GetLapTime());
                Die();
                return;
            }
            
            _nextCheckpointIndex++;
            _deathTimerLength = _aiCarSettings.GetDeathTimerLength();
            if (_nextCheckpointIndex == _checkpoints.childCount)
                _nextCheckpointIndex = 0;
            return;
        }
        
        _neuralNetwork.AddFitness(-4 * _aiCarSettings.GetDeathTimerLength());
        Die();
    }

    public void WallHit()
    {
        if (_dead)
            return;
        _neuralNetwork.AddFitness(-4 * _aiCarSettings.GetDeathTimerLength());
        Die();
    }

    private void Die()
    {
        _dead = true;
        GetRigidBody().constraints = RigidbodyConstraints.FreezeAll;
        _ea.NotifyOfDeath();
    }
    
    public bool IsDead()
    {
        return _dead;
    }
    
    private bool CountdownTimer() 
    {
        _deathTimerLength -= Time.fixedDeltaTime;
        _globalDeathTimer -= Time.fixedDeltaTime;
        return _deathTimerLength <= 0 || _globalDeathTimer <= 0;
    }
    
    public void SetId(int id) 
    {
        _id = id;
    }
    
    public int GetId() 
    {
        return _id;
    }
    
    public NeuralNetwork GetNeuralNetwork() 
    {
        return _neuralNetwork;
    }

    public void SetEA(EA ea)
    {
        _ea = ea;
    }

    public void SetCheckpoints(Transform checkpoints)
    {
        _checkpoints = checkpoints;
    }
    
    public void SetNextCheckpointIndex(int nextCheckpointIndex)
    {
        _nextCheckpointIndex = nextCheckpointIndex;
    }
}
