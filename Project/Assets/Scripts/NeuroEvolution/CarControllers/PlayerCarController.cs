using UnityEngine;

public class PlayerCarController : ParentCarController 
{
    
    // TODO: Never fully implemented
    
    public new void FixedUpdate() 
    {
        base.FixedUpdate();
        CheckForInputs();
        UpdateCarMovement();
        UpdateWheelMeshes();
    }

    private void CheckForInputs() 
    {
        SetLeft(Input.GetKey(KeyCode.A));
        SetRight(Input.GetKey(KeyCode.D));
        SetForward(Input.GetKey(KeyCode.W));
        SetBackward(Input.GetKey(KeyCode.S));
    }
    
    public void ThroughCheckpoint(int checkpointIndex, bool penultimateCheckpoint, bool finalCheckpoint)
    {
        switch (finalCheckpoint)
        {
            case true when !GetPenultimateCheckpointPassed():
                return;
            case true:
                EndLapTime();
                // TODO: something related to the fact the lap has been completed
                print("Lap finished: " + GetLapTime() + " seconds");
                return;
        }
        
        if (IncrementCurrentCheckpoint(checkpointIndex))
        {
            print("Checkpoint " + checkpointIndex + " reached");
            if (penultimateCheckpoint)
                SetPenultimateCheckpointPassed(true);
        
            if (checkpointIndex == 0 && !GetPenultimateCheckpointPassed())
                StartLapTime();

            return;
        }
        
        // otherwise car has hit the wrong checkpoint (i.e. gone backwards)
        // TODO: kill car? End something? Do something related to the fact the wrong checkpoint has been hit
        print("Wrong checkpoint reached");
    }

    public void WallHit()
    {
        print("Player hit wall");
    }
}
