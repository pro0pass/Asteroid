using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Asteroid.Menu
{
    /// <summary>
    /// Main menu manager for the 3D interface
    /// Handles menu navigation, cosmetic animations, and layout
    /// </summary>
    public class MenuManager3D : MonoBehaviour
    {
        [Header("Menu Configuration")]
        [SerializeField] private float menuScale = 1f;
        [SerializeField] private Color menuBackgroundColor = Color.black;
        [SerializeField] private float smoothTransitionSpeed = 5f;
        
        [Header("3D References")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private Transform menuRoot;
        [SerializeField] private ParticleSystem cosmicShowerParticles;
        
        [Header("Menu Items")]
        [SerializeField] private Button storeButton;
        [SerializeField] private Button allAppsButton;
        [SerializeField] private Button apkSignerButton;
        [SerializeField] private Button terminalButton;
        [SerializeField] private Button settingsButton;
        
        [Header("App Icons")]
        [SerializeField] private Transform appIconContainer;
        [SerializeField] private GameObject appIconPrefab;
        
        private List<AppIconUI> loadedApps = new List<AppIconUI>();
        private CanvasGroup canvasGroup;
        private RectTransform menuRectTransform;
        private bool isMenuOpen = true;
        
        private void Start()
        {
            InitializeMenu();
            SetupEventHandlers();
            LoadPreloadedApps();
            PlayCosmicAnimation();
        }
        
        private void InitializeMenu()
        {
            canvasGroup = mainCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = mainCanvas.gameObject.AddComponent<CanvasGroup>();
            
            menuRectTransform = menuRoot.GetComponent<RectTransform>();
            
            // Set menu to bottom-right position
            menuRectTransform.anchorMin = new Vector2(1, 0);
            menuRectTransform.anchorMax = new Vector2(1, 0);
            menuRectTransform.anchoredPosition = new Vector2(-20, 20);
            
            // Apply menu scale
            menuRoot.localScale = Vector3.one * menuScale;
            
            // Set background
            Image backgroundImage = mainCanvas.GetComponent<Image>();
            if (backgroundImage != null)
                backgroundImage.color = menuBackgroundColor;
        }
        
        private void SetupEventHandlers()
        {
            storeButton.onClick.AddListener(OnStoreClicked);
            allAppsButton.onClick.AddListener(OnAllAppsClicked);
            apkSignerButton.onClick.AddListener(OnAPKSignerClicked);
            terminalButton.onClick.AddListener(OnTerminalClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
        }
        
        private void LoadPreloadedApps()
        {
            // Load preloaded apps with logos
            string[] preloadedApps = { "Asteroid Settings", "Meta Quest", "Gallery" };
            
            foreach (string appName in preloadedApps)
            {
                GameObject iconObj = Instantiate(appIconPrefab, appIconContainer);
                AppIconUI iconUI = iconObj.GetComponent<AppIconUI>();
                
                if (iconUI != null)
                {
                    iconUI.SetApp(appName, LoadAppIcon(appName));
                    loadedApps.Add(iconUI);
                }
            }
        }
        
        private Sprite LoadAppIcon(string appName)
        {
            // Load icon from Resources folder
            return Resources.Load<Sprite>($"Icons/{appName}");
        }
        
        private void PlayCosmicAnimation()
        {
            if (cosmicShowerParticles != null)
                cosmicShowerParticles.Play();
        }
        
        public void SetMenuScale(float scale)
        {
            menuScale = Mathf.Clamp01(scale);
            menuRoot.localScale = Vector3.one * menuScale;
        }
        
        public void ToggleMenu()
        {
            isMenuOpen = !isMenuOpen;
            float targetAlpha = isMenuOpen ? 1f : 0f;
            StartCoroutine(SmoothFadeMenu(targetAlpha));
        }
        
        private System.Collections.IEnumerator SmoothFadeMenu(float targetAlpha)
        {
            while (Mathf.Abs(canvasGroup.alpha - targetAlpha) > 0.01f)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * smoothTransitionSpeed);
                yield return null;
            }
            canvasGroup.alpha = targetAlpha;
        }
        
        // Menu Button Handlers
        private void OnStoreClicked()
        {
            Debug.Log("Opening Store...");
            // Load Store scene or panel
        }
        
        private void OnAllAppsClicked()
        {
            Debug.Log("Opening All Apps...");
            // Load All Apps scene
        }
        
        private void OnAPKSignerClicked()
        {
            Debug.Log("Opening APK Signer...");
            // Load APK Signer scene
        }
        
        private void OnTerminalClicked()
        {
            Debug.Log("Opening Terminal...");
            // Load Terminal scene
        }
        
        private void OnSettingsClicked()
        {
            Debug.Log("Opening Settings...");
            // Load Settings scene
        }
        
        private void Update()
        {
            HandleControllerInput();
        }
        
        private void HandleControllerInput()
        {
            // Handle Meta Quest controller input
            if (OVRInput.GetDown(OVRInput.Button.Start))
                ToggleMenu();
        }
    }
}
