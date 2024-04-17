using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentManager : Agent
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private SpawnPointManager spawnPointManager;
    [SerializeField] private Transform checkpoints;
    [SerializeField] private GameObject environment;
    private GameObject _agent;
    private DriverAgent _driverAgent;
    
    public override void Initialize()
    {
        CreateAgent();
    }
    
    public override void OnEpisodeBegin()
    {
        Destroy(_agent);
        CreateAgent();
    }
    
    private void CreateAgent()
    {
        var spawnPoint = spawnPointManager.GetSpawnPoint();
        var position = spawnPoint.spawnPoint.position;
        var rotation = spawnPoint.spawnPoint.rotation;
        // Add some randomness to the position
        position += spawnPoint.spawnPoint.TransformDirection(new Vector3(Random.Range(-3f, 3f), 0f, 0f));
        
        var nextCheckpointIndex = spawnPoint.nextCheckpoint.GetSiblingIndex();
        
        _agent = Instantiate(agentPrefab, position, rotation, environment.transform);
        _driverAgent = _agent.GetComponent<DriverAgent>();
        _driverAgent.SetCheckpoints(checkpoints);
        _driverAgent.SetNewAgent(this);
        _driverAgent.SetNextCheckpointIndex(nextCheckpointIndex);
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_driverAgent.GetThrottle());
        sensor.AddObservation(_driverAgent.GetTurning());
        sensor.AddObservation(_driverAgent.GetCurrentSpeed());
        sensor.AddObservation(_driverAgent.GetCurrentTurnAngle());
        
        var raycasts = _driverAgent.GetRaycasts();
        for(var i = 0; i < raycasts.GetNumOfRaycasts(); i++)
            sensor.AddObservation(raycasts.GetHits()[i].collider != null ? raycasts.GetHits()[i].distance : 1000f);
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        var forwards = actions.DiscreteActions[0];
        var left = actions.DiscreteActions[1];
        var right = actions.DiscreteActions[2];
        var backwards = actions.DiscreteActions[3];
        
        _driverAgent.SetForward(forwards == 1);
        _driverAgent.SetLeft(left == 1);
        _driverAgent.SetRight(right == 1);
        _driverAgent.SetBackward(backwards == 1);
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.W) ? 1 : 0;
        discreteActions[1] = Input.GetKey(KeyCode.A) ? 1 : 0;
        discreteActions[2] = Input.GetKey(KeyCode.D) ? 1 : 0;
        discreteActions[3] = Input.GetKey(KeyCode.S) ? 1 : 0;
    }
    
    
    
}