using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject saveConfirmed;
    [SerializeField] private GameObject confirmQuit;
    [SerializeField] private GameObject confirmHome;
    [SerializeField] private DataPersistenceManager dataPersistenceManager;
    [SerializeField] private GameObject parametersMenu;
    private bool _isPaused;
    
    [Header("Parameter menu button highlights")]
    [SerializeField] private GameObject EAParameterHighlight;
    [SerializeField] private GameObject NNParameterHighlight;
    [SerializeField] private GameObject InputParameterHighlight;
    
    [Header("Parameter menus")]
    [SerializeField] private GameObject EAParameters;
    [SerializeField] private GameObject NNParameters;
    [SerializeField] private GameObject InputParameters;
    
    
    private void Update()
    {
        if (parametersMenu.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                parametersMenu.SetActive(false);
                pauseMenu.SetActive(true);
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
            pauseMenu.SetActive(_isPaused);
        }
    }
    
    public void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void Save()
    {
        dataPersistenceManager.SaveEANeuralNetworkModel();
        saveConfirmed.SetActive(true);
    }

    public void ViewParameters()
    {
        pauseMenu.SetActive(false);
        parametersMenu.SetActive(true);
    }
    
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void SaveAndQuit()
    {
        dataPersistenceManager.SaveEANeuralNetworkModel();
        Quit();
    }
    
    public void Home()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeMenu");
    }
    
    public void SaveAndHome()
    {
        dataPersistenceManager.SaveEANeuralNetworkModel();
        Home();
    }
    
    public void Back()
    {
        parametersMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    
    public void ShowEAParameters()
    {
        NNParameters.SetActive(false);
        InputParameters.SetActive(false);
        EAParameters.SetActive(true);
        EAParameterHighlight.SetActive(true);
        NNParameterHighlight.SetActive(false);
        InputParameterHighlight.SetActive(false);
    }
    
    public void ShowNNParameters()
    {
        EAParameters.SetActive(false);
        InputParameters.SetActive(false);
        NNParameters.SetActive(true);
        EAParameterHighlight.SetActive(false);
        NNParameterHighlight.SetActive(true);
        InputParameterHighlight.SetActive(false);
    }
    
    public void ShowInputParameters()
    {
        EAParameters.SetActive(false);
        NNParameters.SetActive(false);
        InputParameters.SetActive(true);
        EAParameterHighlight.SetActive(false);
        NNParameterHighlight.SetActive(false);
        InputParameterHighlight.SetActive(true);
    }

    public void HideSaveConfirmedScreen()
    {
        saveConfirmed.SetActive(false);
    }
    
    public void ShowConfirmQuitScreen()
    {
        confirmQuit.SetActive(true);
    }
    
    public void HideConfirmQuitScreen()
    {
        confirmQuit.SetActive(false);
    }
    
    public void ShowConfirmHomeScreen()
    {
        confirmHome.SetActive(true);
    }
    
    public void HideConfirmHomeScreen()
    {
        confirmHome.SetActive(false);
    }
}