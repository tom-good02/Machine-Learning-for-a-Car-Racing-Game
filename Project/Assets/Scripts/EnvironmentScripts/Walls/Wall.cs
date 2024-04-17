using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        var car = other.gameObject.transform.parent.gameObject;

        if (car.CompareTag("Player"))
        {
            var playerCarController = car.GetComponent<PlayerCarController>();
            playerCarController.WallHit();
        } 
        else if (car.CompareTag("PopulationMember"))
        {
            var aiCarController = car.GetComponent<AICarController>();
            aiCarController.WallHit();   
        }
        else if (car.CompareTag("Agent"))
        {
            var driverAgent = car.GetComponent<DriverAgent>();
            driverAgent.WallHit();
        }
    }
}
