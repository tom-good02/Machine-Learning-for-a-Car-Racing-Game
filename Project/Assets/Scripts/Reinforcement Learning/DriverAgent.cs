using UnityEngine;

public class DriverAgent : CarScript
{
    [SerializeField] private float deathTimerLength = 3f;
    [SerializeField] private float globalDeathTimerLength = 100f;
    
    private Transform _checkpoints;
    [SerializeField] private Raycasts raycasts;
    private float _deathTimer;
    private float _globalDeathTimer;
    private bool _dead;
    private float _lastCheckpointTime;
    private int _nextCheckpointIndex;
    private int _totalCheckpointsReached;

    private AgentManager _agent;

    public new void Awake()
    {
        _deathTimer = deathTimerLength;
        _globalDeathTimer = globalDeathTimerLength;
    }
    
    public void SetNewAgent(AgentManager agent)
    {
        _agent = agent;
    }
    
    public void SetCheckpoints(Transform checkpoints)
    {
        _checkpoints = checkpoints;
    }
    
    public new void FixedUpdate()
    {
        if (_dead)
            return;
        
        base.FixedUpdate();
        if (CountdownTimer())
        {
            if (_dead)
                return;
            _agent.AddReward(-4 * deathTimerLength);
            _dead = true;
            _agent.EndEpisode();
            return;
        }
        
        UpdateCarMovement();
        UpdateWheelMeshes();
    }
    
    private bool CountdownTimer() 
    {
        _deathTimer -= Time.fixedDeltaTime;
        _globalDeathTimer -= Time.fixedDeltaTime;
        return _deathTimer <= 0 || _globalDeathTimer <= 0;
    }
    
    public Raycasts GetRaycasts()
    {
        return raycasts;
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
                _agent.AddReward(deathTimerLength);
                _lastCheckpointTime = Time.fixedTime;
            }
            else
            {
                var timeAsIntSinceLastCheckpoint = Time.fixedTime - _lastCheckpointTime;
                if (timeAsIntSinceLastCheckpoint < 0.1f)
                    timeAsIntSinceLastCheckpoint = 0.1f;
                _agent.AddReward(10 * deathTimerLength + deathTimerLength / timeAsIntSinceLastCheckpoint);
                _lastCheckpointTime = Time.fixedTime;
            }
            
            _totalCheckpointsReached++;
            if (_totalCheckpointsReached == _checkpoints.childCount + 1)
            {
                EndLapTime();
                _agent.AddReward(globalDeathTimerLength * 10 / GetLapTime());
                _agent.EndEpisode();
                return;
            }
            
            _nextCheckpointIndex++;
            _deathTimer = deathTimerLength;
            if (_nextCheckpointIndex == _checkpoints.childCount)
                _nextCheckpointIndex = 0;
            return;
        }
        
        _agent.AddReward(-4 * deathTimerLength);
        _agent.EndEpisode();
    }
    
    public void WallHit()
    {
        if (_dead)
            return;
        _dead = true;
        _agent.AddReward(-4 * deathTimerLength);
        _agent.EndEpisode();
    }
    
    public void SetNextCheckpointIndex(int index)
    {
        _nextCheckpointIndex = index;
    }
}
