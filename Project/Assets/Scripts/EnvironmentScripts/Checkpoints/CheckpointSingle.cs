using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{

    private TrackCheckpoints trackCheckpoints;

    private void OnTriggerEnter(Collider other) {
        var car = other.gameObject.transform.parent.gameObject;
        
        trackCheckpoints.CarThroughCheckpoint(this, car);
    }

    public void SetTrackCheckpoints(TrackCheckpoints tc) {
        trackCheckpoints = tc;
    }
}
