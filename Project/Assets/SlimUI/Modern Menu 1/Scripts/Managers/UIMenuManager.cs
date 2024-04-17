using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu{
	public class UIMenuManager : MonoBehaviour {
		private Animator CameraObject;

		// campaign button sub menu
        [Header("MENUS")]
        [Tooltip("The Menu for when the MAIN menu buttons")]
        public GameObject mainMenu;
        [Tooltip("THe first list of buttons")]
        public GameObject firstMenu;
        [Tooltip("The Menu for when the PLAY button is clicked")]
        public GameObject playMenu;
        [Tooltip("The Menu for when the EXIT button is clicked")]
        public GameObject exitMenu;
        [Tooltip("Optional 4th Menu")]
        public GameObject extrasMenu;

        public GameObject loadMenu;

        public enum Theme {custom1, custom2, custom3};
        [Header("THEME SETTINGS")]
        public Theme theme;
        // private int themeIndex;
        public ThemedUIData themeController;

        [Header("PANELS")]
        [Tooltip("The UI Panel parenting all sub menus")]
        public GameObject mainCanvas;
        [Tooltip("The UI Panel that holds the CONTROLS window tab")]
        public GameObject PanelControls;
        [Tooltip("The UI Panel that holds the VIDEO window tab")]
        public GameObject PanelVideo;
        [Tooltip("The UI Panel that holds the GAME window tab")]
        public GameObject PanelGame;
        [Tooltip("The UI Panel that holds the KEY BINDINGS window tab")]
        public GameObject PanelKeyBindings;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for MOVEMENT")]
        public GameObject PanelMovement;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for COMBAT")]
        public GameObject PanelCombat;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for GENERAL")]
        public GameObject PanelGeneral;
        
        // my new panels
        [Tooltip("The UI Panel that holds the METHOD AND MAP window tab")]
        public GameObject PanelMethodAndMap;
        [Tooltip("The UI Panel that holds the EVOLUTIONARY ALGORITHM window tab")]
        public GameObject PanelEA;
        [Tooltip("The UI Panel that holds the NEURAL NETWORK window tab")]
        public GameObject PanelNN;
        [Tooltip("The UI Panel that holds the INPUT window tab")]
        public GameObject PanelGeneralSettings;
        [Tooltip("The UI Panel that holds the REINFORCEMENT LEARNING window tab")]
        public GameObject PanelRL;
        

        // highlights in settings screen
        [Header("SETTINGS SCREEN")]
        public GameObject settingsCanvas;
        [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
        public GameObject lineGame;
        [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
        public GameObject lineVideo;
        [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
        public GameObject lineControls;
        [Tooltip("Highlight Image for when KEY BINDINGS Tab is selected in Settings")]
        public GameObject lineKeyBindings;
        [Tooltip("Highlight Image for when MOVEMENT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineMovement;
        [Tooltip("Highlight Image for when COMBAT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineCombat;
        [Tooltip("Highlight Image for when GENERAL Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineGeneral;
        
        [Header("NEW GAME SCREEN")]
        public GameObject newGameCanvas;
        [Tooltip("")] 
        public GameObject lineMethodAndMap;
        [Tooltip("")] 
        public GameObject lineEASettings;
        [Tooltip("")] 
        public GameObject lineNNSettings;
        [Tooltip("")]
        public GameObject lineInputSettings;
        [Tooltip("")]
        public GameObject lineRLSettings;


        [Header("LOADING SCREEN")]
		[Tooltip("If this is true, the loaded scene won't load until receiving user input")]
		public bool waitForInput = true;
        public GameObject loadingMenu;
		[Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;
        public TMP_Text loadPromptText;
        public TMP_Text loadingText;
		public KeyCode userPromptKey;

		[Header("SFX")]
        [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
        public AudioSource hoverSound;
        [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
        public AudioSource sliderSound;
        [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
        public AudioSource swooshSound;

		void Start(){
			CameraObject = transform.GetComponent<Animator>();

			playMenu.SetActive(false);
			exitMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			loadMenu.SetActive(false);
			firstMenu.SetActive(true);
			mainMenu.SetActive(true);

			SetThemeColors();
		}

		void SetThemeColors()
		{
			switch (theme)
			{
				case Theme.custom1:
					themeController.currentColor = themeController.custom1.graphic1;
					themeController.textColor = themeController.custom1.text1;
					// themeIndex = 0;
					break;
				case Theme.custom2:
					themeController.currentColor = themeController.custom2.graphic2;
					themeController.textColor = themeController.custom2.text2;
					// themeIndex = 1;
					break;
				case Theme.custom3:
					themeController.currentColor = themeController.custom3.graphic3;
					themeController.textColor = themeController.custom3.text3;
					// themeIndex = 2;
					break;
				default:
					Debug.Log("Invalid theme selected.");
					break;
			}
		}

		public void PlayCampaign(){
			exitMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			loadMenu.SetActive(false);
			playMenu.SetActive(true);
		}
		
		public void PlayCampaignMobile(){
			exitMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			playMenu.SetActive(true);
			mainMenu.SetActive(false);
		}

		public void ReturnMenu(){
			playMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			loadMenu.SetActive(false);
			exitMenu.SetActive(false);
			mainMenu.SetActive(true);
		}

		public void LoadScene(string scene){
			if(scene != ""){
				StartCoroutine(LoadAsynchronously(scene));
			}
		}

		public void  DisablePlayCampaign(){
			playMenu.SetActive(false);
		}
		
		public void ShowNewGameCanvas()
		{
			settingsCanvas.SetActive(false);
			newGameCanvas.SetActive(true);
		}
		
		public void ShowSettingsCanvas()
		{
			newGameCanvas.SetActive(false);
			settingsCanvas.SetActive(true);
		}
		
		public void Position2(){
			DisablePlayCampaign();
			CameraObject.SetFloat("SettingsAnimate",1);
		}

		public void Position1(){
			CameraObject.SetFloat("SettingsAnimate",0);
		}

		void DisableGeneralSettingsPanels(){
			PanelControls.SetActive(false);
			lineControls.SetActive(false);
			PanelVideo.SetActive(false);
			lineVideo.SetActive(false);
			PanelGame.SetActive(false);
			lineGame.SetActive(false);
			PanelKeyBindings.SetActive(false);
			lineKeyBindings.SetActive(false);

			PanelMovement.SetActive(false);
			lineMovement.SetActive(false);
			PanelCombat.SetActive(false);
			lineCombat.SetActive(false);
			PanelGeneral.SetActive(false);
			lineGeneral.SetActive(false);
		}

		void DisableNewGamePanels()
		{
			// My new panels
			PanelMethodAndMap.SetActive(false);
			lineMethodAndMap.SetActive(false);
			PanelGeneralSettings.SetActive(false);
			lineInputSettings.SetActive(false);
			PanelEA.SetActive(false);
			lineEASettings.SetActive(false);
			PanelNN.SetActive(false);
			lineNNSettings.SetActive(false);
			PanelRL.SetActive(false);
			lineRLSettings.SetActive(false);
		}
		
		// my new code for panels
		public void MethodAndMapPanel()
		{
			DisableNewGamePanels();
			PanelMethodAndMap.SetActive(true);
			lineMethodAndMap.SetActive(true);
		}
		
		public void EASettingsPanel()
		{
			DisableNewGamePanels();
			PanelEA.SetActive(true);
			lineEASettings.SetActive(true);
		}
		
		public void NNSettingsPanel()
		{
			DisableNewGamePanels();
			PanelNN.SetActive(true);
			lineNNSettings.SetActive(true);
		}
		
		public void InputPanel()
        {
            DisableNewGamePanels();
            PanelGeneralSettings.SetActive(true);
            lineInputSettings.SetActive(true);
        }
		
		public void RLSettingsPanel()
		{
			DisableNewGamePanels();
			PanelRL.SetActive(true);
			lineRLSettings.SetActive(true);
		}
		
		public void GamePanel(){
			DisableGeneralSettingsPanels();
			PanelGame.SetActive(true);
			lineGame.SetActive(true);
		}

		public void VideoPanel(){
			DisableGeneralSettingsPanels();
			PanelVideo.SetActive(true);
			lineVideo.SetActive(true);
		}

		public void ControlsPanel(){
			DisableGeneralSettingsPanels();
			PanelControls.SetActive(true);
			lineControls.SetActive(true);
		}

		public void KeyBindingsPanel(){
			DisableGeneralSettingsPanels();
			MovementPanel();
			PanelKeyBindings.SetActive(true);
			lineKeyBindings.SetActive(true);
		}

		public void MovementPanel(){
			DisableGeneralSettingsPanels();
			PanelKeyBindings.SetActive(true);
			PanelMovement.SetActive(true);
			lineMovement.SetActive(true);
		}

		public void CombatPanel(){
			DisableGeneralSettingsPanels();
			PanelKeyBindings.SetActive(true);
			PanelCombat.SetActive(true);
			lineCombat.SetActive(true);
		}

		public void GeneralPanel(){
			DisableGeneralSettingsPanels();
			PanelKeyBindings.SetActive(true);
			PanelGeneral.SetActive(true);
			lineGeneral.SetActive(true);
		}

		public void PlayHover(){
			hoverSound.Play();
		}

		public void PlaySFXHover(){
			sliderSound.Play();
		}

		public void PlaySwoosh(){
			swooshSound.Play();
		}

		// Are You Sure - Quit Panel Pop Up
		public void AreYouSure(){
			exitMenu.SetActive(true);
			if(extrasMenu) extrasMenu.SetActive(false);
			loadMenu.SetActive(false);
			DisablePlayCampaign();
		}

		public void AreYouSureMobile(){
			exitMenu.SetActive(true);
			if(extrasMenu) extrasMenu.SetActive(false);
			mainMenu.SetActive(false);
			DisablePlayCampaign();
		}

		public void ExtrasMenu(){
			playMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(true);
			exitMenu.SetActive(false);
		}
		
		public void LoadMenu(){
			playMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			loadMenu.SetActive(true);
			exitMenu.SetActive(false);
		}

		public void QuitGame(){
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}

		// Load Bar synching animation
		private IEnumerator LoadAsynchronously(string sceneName){ // scene name is just the name of the current scene being loaded
			var operation = SceneManager.LoadSceneAsync(sceneName);
			operation.allowSceneActivation = false;
			// mainCanvas.SetActive(false);
			newGameCanvas.SetActive(false);
			loadingMenu.SetActive(true);
			mainCanvas.SetActive(false);

			while (!operation.isDone){
				float progress = Mathf.Clamp01(operation.progress / .95f);
				loadingBar.value = progress;
				loadingText.text = "Loading...  " + (progress * 100f).ToString("F0") + "%";

				if (operation.progress >= 0.9f && waitForInput){
					loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
					loadingText.text = "Loaded";
					loadingBar.value = 1;

					if (Input.GetKeyDown(userPromptKey)){
						operation.allowSceneActivation = true;
					}
                }else if(operation.progress >= 0.9f && !waitForInput){
					operation.allowSceneActivation = true;
				}

				yield return null;
			}
		}
	}
}