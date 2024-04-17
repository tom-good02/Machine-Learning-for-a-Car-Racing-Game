using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{

    private List<CheckpointSingle> _checkpoints;

    private void Awake() {
        var checkpointsTransform = transform.Find("Checkpoints");
        _checkpoints = new List<CheckpointSingle>();
        
        foreach(Transform checkpointTransform in checkpointsTransform) {
            var checkpointSingle = checkpointTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetTrackCheckpoints(this);
            _checkpoints.Add(checkpointSingle);
        }
    }

    public void CarThroughCheckpoint(CheckpointSingle cs, GameObject car)
    {
        if (car.CompareTag("Player"))
        {
            var playerCarController = car.GetComponent<PlayerCarController>();
            playerCarController.ThroughCheckpoint(_checkpoints.IndexOf(cs), 
                _checkpoints.IndexOf(cs) == _checkpoints.Count - 2, 
                _checkpoints.IndexOf(cs) == _checkpoints.Count - 1);
        }
        else if (car.CompareTag("PopulationMember"))
        {
            var aiCarController = car.GetComponent<AICarController>();
            aiCarController.ThroughCheckpoint(_checkpoints.IndexOf(cs));
        }
        else if (car.CompareTag("Agent"))
        {
            var driverAgent = car.GetComponent<DriverAgent>();
            driverAgent.ThroughCheckpoint(_checkpoints.IndexOf(cs));
        }
    }
}
