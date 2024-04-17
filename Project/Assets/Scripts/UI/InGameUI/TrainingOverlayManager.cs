using UnityEngine;

public class TrainingOverlayManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup networkOverlay;
    [SerializeField] private CanvasGroup fitnessGraphOverlay;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ToggleNetworkOverlay();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ToggleGraphOverlay();
    }

    private void ToggleNetworkOverlay()
    {
        networkOverlay.alpha = networkOverlay.alpha == 0 ? 1 : 0;
    }
    
    private void ToggleGraphOverlay()
    {
        fitnessGraphOverlay.alpha = fitnessGraphOverlay.alpha == 0 ? 1 : 0;
    }
    

}
