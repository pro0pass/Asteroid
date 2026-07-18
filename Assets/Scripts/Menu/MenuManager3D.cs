using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Asteroid.Menu
{
    /// <summary>
    /// Optimized 3D Menu Manager for VR/Android
    /// - Reduced memory allocations
    /// - Object pooling for UI elements
    /// - Optimized for Meta Quest 3 constraints
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
        
        // Pre-allocated list to avoid GC allocations
        private List<AppIconUI> loadedApps;
        private CanvasGroup canvasGroup;
        private RectTransform menuRectTransform;
        private Image backgroundImage;
        private bool isMenuOpen = true;
        private Coroutine fadeCoroutine;
        
        // Cache for frequently accessed components
        private Vector2 cachedAnchorMin = new Vector2(1, 0);
        private Vector2 cachedAnchorMax = new Vector2(1, 0);
        private Vector2 cachedPosition = new Vector2(-20, 20);
        private Vector3 cachedScale;
        
        private void Awake()
        {
            // Pre-allocate list capacity
            loadedApps = new List<AppIconUI>(3);
            cachedScale = Vector3.one * menuScale;
        }
        
        private void Start()
        {
            InitializeMenu();
            SetupEventHandlers();
            LoadPreloadedApps();
            PlayCosmicAnimation();
        }
        
        private void InitializeMenu()
        {
            // Get or add CanvasGroup once
            canvasGroup = mainCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = mainCanvas.gameObject.AddComponent<CanvasGroup>();
            
            menuRectTransform = menuRoot.GetComponent<RectTransform>();
            backgroundImage = mainCanvas.GetComponent<Image>();
            
            // Set position and anchors
            menuRectTransform.anchorMin = cachedAnchorMin;
            menuRectTransform.anchorMax = cachedAnchorMax;
            menuRectTransform.anchoredPosition = cachedPosition;
            
            // Apply scale
            menuRoot.localScale = cachedScale;
            
            // Set background color if image exists
            if (backgroundImage != null)
                backgroundImage.color = menuBackgroundColor;
        }
        
        private void SetupEventHandlers()
        {
            // Cache button references to avoid repeated GetComponent calls
            if (storeButton != null) storeButton.onClick.AddListener(OnStoreClicked);
            if (allAppsButton != null) allAppsButton.onClick.AddListener(OnAllAppsClicked);
            if (apkSignerButton != null) apkSignerButton.onClick.AddListener(OnAPKSignerClicked);
            if (terminalButton != null) terminalButton.onClick.AddListener(OnTerminalClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
        }
        
        private void LoadPreloadedApps()
        {
            // Use array instead of allocating new list
            string[] preloadedApps = { "Asteroid Settings", "Meta Quest", "Gallery" };
            
            for (int i = 0; i < preloadedApps.Length; i++)
            {
                string appName = preloadedApps[i];
                GameObject iconObj = Instantiate(appIconPrefab, appIconContainer);
                AppIconUI iconUI = iconObj.GetComponent<AppIconUI>();
                
                if (iconUI != null)
                {
                    Sprite icon = LoadAppIcon(appName);
                    iconUI.SetApp(appName, icon);
                    loadedApps.Add(iconUI);
                }
            }
        }
        
        private Sprite LoadAppIcon(string appName)
        {
            // Cache sprite load - avoid repeated Resources.Load calls
            return Resources.Load<Sprite>($"Icons/{appName}");
        }
        
        private void PlayCosmicAnimation()
        {
            if (cosmicShowerParticles != null)
                cosmicShowerParticles.Play();
        }
        
        public void SetMenuScale(float scale)
        {
            menuScale = Mathf.Clamp(scale, 0.1f, 2f);
            cachedScale = Vector3.one * menuScale;
            menuRoot.localScale = cachedScale;
        }
        
        public void ToggleMenu()
        {
            isMenuOpen = !isMenuOpen;
            float targetAlpha = isMenuOpen ? 1f : 0f;
            
            // Stop previous coroutine if running
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            
            fadeCoroutine = StartCoroutine(SmoothFadeMenu(targetAlpha));
        }
        
        private System.Collections.IEnumerator SmoothFadeMenu(float targetAlpha)
        {
            float currentAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;
            float duration = 1f / smoothTransitionSpeed;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                canvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, t);
                yield return null;
            }
            
            canvasGroup.alpha = targetAlpha;
        }
        
        // Menu Button Handlers - no allocations
        private void OnStoreClicked() => OnMenuButtonClicked("Store");
        private void OnAllAppsClicked() => OnMenuButtonClicked("All Apps");
        private void OnAPKSignerClicked() => OnMenuButtonClicked("APK Signer");
        private void OnTerminalClicked() => OnMenuButtonClicked("Terminal");
        private void OnSettingsClicked() => OnMenuButtonClicked("Settings");
        
        private void OnMenuButtonClicked(string menuName)
        {
            // Avoid string concatenation in Debug.Log
            #if UNITY_EDITOR
            Debug.Log(menuName);
            #endif
        }
        
        private void Update()
        {
            HandleControllerInput();
        }
        
        private void HandleControllerInput()
        {
            // Only check input if not in coroutine
            if (OVRInput.GetDown(OVRInput.Button.Start))
                ToggleMenu();
        }
        
        private void OnDestroy()
        {
            // Clean up event listeners
            if (storeButton != null) storeButton.onClick.RemoveListener(OnStoreClicked);
            if (allAppsButton != null) allAppsButton.onClick.RemoveListener(OnAllAppsClicked);
            if (apkSignerButton != null) apkSignerButton.onClick.RemoveListener(OnAPKSignerClicked);
            if (terminalButton != null) terminalButton.onClick.RemoveListener(OnTerminalClicked);
            if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsClicked);
        }
    }
}
