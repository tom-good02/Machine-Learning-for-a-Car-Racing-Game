using System;
using TMPro;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] private StartingCheckpoint defaultSpawnPoint;
    [Serializable] public struct StartingCheckpoint
    {
        [SerializeField] public Transform spawnPoint;
        [SerializeField] public Transform nextCheckpoint;
    }
    [SerializeField] private StartingCheckpoint[] startingCheckpoints;
    [SerializeField] private bool randomSpawnPoint;
    
    [SerializeField] private TMP_Text randomSpawnPointText;
    
    public void Awake()
    {
        if (defaultSpawnPoint.spawnPoint == null || defaultSpawnPoint.nextCheckpoint == null)
            throw new Exception("Default spawn point is not set up correctly");
        if (startingCheckpoints.Length == 0)
        {
            startingCheckpoints = new StartingCheckpoint[1];
            startingCheckpoints[0] = defaultSpawnPoint;
        }
        if (randomSpawnPointText != null)
            randomSpawnPointText.text = randomSpawnPoint ? "Random" : "Default";
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            randomSpawnPoint = !randomSpawnPoint;
            randomSpawnPointText.text = randomSpawnPoint ? "Random" : "Default";
        }
    }

    public void SetRandomSpawnPoint(bool randomSpawnPoint)
    {
        this.randomSpawnPoint = randomSpawnPoint;
    }

    public StartingCheckpoint GetSpawnPoint()
    {
        if (!randomSpawnPoint) 
            return defaultSpawnPoint;
        
        var randomIndex = UnityEngine.Random.Range(0, startingCheckpoints.Length);
        return startingCheckpoints[randomIndex];
    }
}
