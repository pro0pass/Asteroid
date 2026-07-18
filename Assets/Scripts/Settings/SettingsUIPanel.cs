using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Asteroid.Menu
{
    /// <summary>
    /// Settings UI Panel
    /// Displays all settings and allows user configuration
    /// Includes "Revert to Old Menu" button
    /// </summary>
    public class SettingsUIPanel : MonoBehaviour
    {
        [SerializeField] private SettingsManager settingsManager;
        [SerializeField] private MenuManager3D newMenuManager;
        [SerializeField] private GameObject oldMenuPrefab; // Reference to old menu prefab
        
        [Header("Movement Settings")]
        [SerializeField] private Slider movementSpeedSlider;
        [SerializeField] private Toggle joystickToggle;
        [SerializeField] private Toggle seatedModeToggle;
        [SerializeField] private Slider seatedSpeedSlider;
        
        [Header("Control Settings")]
        [SerializeField] private Toggle instantTriggerToggle;
        [SerializeField] private Slider triggerDeadzoneSlider;
        [SerializeField] private Slider gripDeadzoneSlider;
        [SerializeField] private Slider hapticIntensitySlider;
        [SerializeField] private Toggle hapticFeedbackToggle;
        
        [Header("Display Settings")]
        [SerializeField] private Slider screenBrightnessSlider;
        [SerializeField] private Toggle vrPassthroughToggle;
        [SerializeField] private Slider vrPassthroughOpacitySlider;
        [SerializeField] private Dropdown framerateDropdown;
        
        [Header("Eye & Hand Tracking")]
        [SerializeField] private Toggle eyeTrackingToggle;
        [SerializeField] private Slider gazeConfidenceSlider;
        [SerializeField] private Toggle handTrackingToggle;
        [SerializeField] private Slider handTrackingConfidenceSlider;
        
        [Header("Developer Settings")]
        [SerializeField] private Toggle wirelessDebugToggle;
        [SerializeField] private Toggle developerUIToggle;
        [SerializeField] private Toggle performanceMonitoringToggle;
        [SerializeField] private Toggle inputDebuggingToggle;
        
        [Header("Audio Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle spatialAudioToggle;
        
        [Header("Menu Settings")]
        [SerializeField] private Toggle useNewMenuToggle;
        [SerializeField] private Slider menuScaleSlider;
        [SerializeField] private Toggle menuAnimationsToggle;
        
        [Header("Accessibility Settings")]
        [SerializeField] private Toggle colorBlindToggle;
        [SerializeField] private Dropdown colorBlindTypeDropdown;
        [SerializeField] private Toggle subtitlesToggle;
        [SerializeField] private Slider subtitleSizeSlider;
        [SerializeField] private Toggle motionSicknessToggle;
        [SerializeField] private Toggle vignettingToggle;
        [SerializeField] private Slider vignetteIntensitySlider;
        
        [Header("Action Buttons")]
        [SerializeField] private Button resetButton;
        [SerializeField] private Button revertToOldMenuButton;
        [SerializeField] private Button closeButton;
        
        [Header("Confirmation Dialog")]
        [SerializeField] private GameObject confirmationDialogPrefab;
        
        private void Start()
        {
            if (settingsManager == null)
                settingsManager = SettingsManager.Instance;
            
            InitializeUI();
            SetupListeners();
        }
        
        private void InitializeUI()
        {
            var settings = settingsManager.GetSettings();
            
            // Movement
            if (movementSpeedSlider != null)
                movementSpeedSlider.value = settings.movementSpeed;
            if (joystickToggle != null)
                joystickToggle.isOn = settings.joystickEnabled;
            if (seatedModeToggle != null)
                seatedModeToggle.isOn = settings.seatedModeEnabled;
            if (seatedSpeedSlider != null)
                seatedSpeedSlider.value = settings.seatedSpeedMultiplier;
            
            // Control
            if (instantTriggerToggle != null)
                instantTriggerToggle.isOn = settings.instantTriggerResponse;
            if (triggerDeadzoneSlider != null)
                triggerDeadzoneSlider.value = settings.triggerDeadzone;
            if (gripDeadzoneSlider != null)
                gripDeadzoneSlider.value = settings.gripDeadzone;
            if (hapticIntensitySlider != null)
                hapticIntensitySlider.value = settings.hapticIntensity;
            if (hapticFeedbackToggle != null)
                hapticFeedbackToggle.isOn = settings.hapticFeedbackEnabled;
            
            // Display
            if (screenBrightnessSlider != null)
                screenBrightnessSlider.value = settings.screenBrightness;
            if (vrPassthroughToggle != null)
                vrPassthroughToggle.isOn = settings.vrPassthroughEnabled;
            if (vrPassthroughOpacitySlider != null)
                vrPassthroughOpacitySlider.value = settings.vrPassthroughOpacity;
            
            // Eye & Hand Tracking
            if (eyeTrackingToggle != null)
                eyeTrackingToggle.isOn = settings.eyeTrackingEnabled;
            if (gazeConfidenceSlider != null)
                gazeConfidenceSlider.value = settings.gazeConfidenceThreshold;
            if (handTrackingToggle != null)
                handTrackingToggle.isOn = settings.handTrackingEnabled;
            if (handTrackingConfidenceSlider != null)
                handTrackingConfidenceSlider.value = settings.handTrackingConfidence;
            
            // Developer
            if (wirelessDebugToggle != null)
                wirelessDebugToggle.isOn = settings.wirelessDebuggingEnabled;
            if (developerUIToggle != null)
                developerUIToggle.isOn = settings.developerUIEnabled;
            if (performanceMonitoringToggle != null)
                performanceMonitoringToggle.isOn = settings.performanceMonitoringEnabled;
            if (inputDebuggingToggle != null)
                inputDebuggingToggle.isOn = settings.inputDebuggingEnabled;
            
            // Audio
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = settings.masterVolume;
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = settings.musicVolume;
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = settings.sfxVolume;
            if (spatialAudioToggle != null)
                spatialAudioToggle.isOn = settings.spatialAudioEnabled;
            
            // Menu
            if (useNewMenuToggle != null)
                useNewMenuToggle.isOn = settings.useNewMetaQuestMenu;
            if (menuScaleSlider != null)
                menuScaleSlider.value = settings.menuScale;
            if (menuAnimationsToggle != null)
                menuAnimationsToggle.isOn = settings.menuAnimationsEnabled;
            
            // Accessibility
            if (colorBlindToggle != null)
                colorBlindToggle.isOn = settings.colorBlindMode;
            if (colorBlindTypeDropdown != null)
                colorBlindTypeDropdown.value = GetColorBlindTypeIndex(settings.colorBlindType);
            if (subtitlesToggle != null)
                subtitlesToggle.isOn = settings.subtitlesEnabled;
            if (subtitleSizeSlider != null)
                subtitleSizeSlider.value = settings.subtitleSize;
            if (motionSicknessToggle != null)
                motionSicknessToggle.isOn = settings.motionSicknessReduction;
            if (vignettingToggle != null)
                vignettingToggle.isOn = settings.vignettingEnabled;
            if (vignetteIntensitySlider != null)
                vignetteIntensitySlider.value = settings.vignetteIntensity;
        }
        
        private void SetupListeners()
        {
            // Movement
            if (movementSpeedSlider != null)
                movementSpeedSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("movement_speed", v));
            if (joystickToggle != null)
                joystickToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("joystick_enabled", b));
            if (seatedModeToggle != null)
                seatedModeToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("seated_mode", b));
            if (seatedSpeedSlider != null)
                seatedSpeedSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("seated_speed", v));
            
            // Control
            if (instantTriggerToggle != null)
                instantTriggerToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("instant_trigger", b));
            if (triggerDeadzoneSlider != null)
                triggerDeadzoneSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("trigger_deadzone", v));
            if (gripDeadzoneSlider != null)
                gripDeadzoneSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("grip_deadzone", v));
            if (hapticIntensitySlider != null)
                hapticIntensitySlider.onValueChanged.AddListener(v => settingsManager.SetSetting("haptic_intensity", v));
            if (hapticFeedbackToggle != null)
                hapticFeedbackToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("haptic_feedback", b));
            
            // Display
            if (screenBrightnessSlider != null)
                screenBrightnessSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("screen_brightness", v));
            if (vrPassthroughToggle != null)
                vrPassthroughToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("vr_passthrough", b));
            if (vrPassthroughOpacitySlider != null)
                vrPassthroughOpacitySlider.onValueChanged.AddListener(v => settingsManager.SetSetting("vr_passthrough_opacity", v));
            
            // Eye & Hand Tracking
            if (eyeTrackingToggle != null)
                eyeTrackingToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("eye_tracking", b));
            if (gazeConfidenceSlider != null)
                gazeConfidenceSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("gaze_confidence", v));
            if (handTrackingToggle != null)
                handTrackingToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("hand_tracking", b));
            
            // Developer
            if (wirelessDebugToggle != null)
                wirelessDebugToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("wireless_debug", b));
            if (developerUIToggle != null)
                developerUIToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("developer_ui", b));
            if (performanceMonitoringToggle != null)
                performanceMonitoringToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("perf_monitoring", b));
            if (inputDebuggingToggle != null)
                inputDebuggingToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("input_debug", b));
            
            // Audio
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("master_volume", v));
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("music_volume", v));
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("sfx_volume", v));
            if (spatialAudioToggle != null)
                spatialAudioToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("spatial_audio", b));
            
            // Menu
            if (useNewMenuToggle != null)
                useNewMenuToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("use_new_menu", b));
            if (menuScaleSlider != null)
                menuScaleSlider.onValueChanged.AddListener(v => settingsManager.SetSetting("menu_scale", v));
            if (menuAnimationsToggle != null)
                menuAnimationsToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("menu_animations", b));
            
            // Accessibility
            if (colorBlindToggle != null)
                colorBlindToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("colorblind_mode", b));
            if (subtitlesToggle != null)
                subtitlesToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("subtitles", b));
            if (motionSicknessToggle != null)
                motionSicknessToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("motion_sickness", b));
            if (vignettingToggle != null)
                vignettingToggle.onValueChanged.AddListener(b => settingsManager.SetSetting("vignetting", b));
            
            // Buttons
            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetClicked);
            if (revertToOldMenuButton != null)
                revertToOldMenuButton.onClick.AddListener(OnRevertToOldMenuClicked);
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
        }
        
        private int GetColorBlindTypeIndex(string type)
        {
            return type switch
            {
                "deuteranopia" => 1,
                "protanopia" => 2,
                "tritanopia" => 3,
                _ => 0
            };
        }
        
        private string GetColorBlindTypeFromIndex(int index)
        {
            return index switch
            {
                1 => "deuteranopia",
                2 => "protanopia",
                3 => "tritanopia",
                _ => "none"
            };
        }
        
        private void OnResetClicked()
        {
            ShowConfirmation("Reset all settings to defaults?", () =>
            {
                settingsManager.ResetToDefaults();
                InitializeUI();
                Debug.Log("Settings reset to defaults");
            });
        }
        
        private void OnRevertToOldMenuClicked()
        {
            ShowConfirmation("Switch to classic menu? This will restart the menu system.", () =>
            {
                Debug.Log("Reverting to old menu...");
                settingsManager.SetSetting("use_new_menu", false);
                
                // Disable new menu
                if (newMenuManager != null)
                    newMenuManager.gameObject.SetActive(false);
                
                // Enable old menu
                if (oldMenuPrefab != null)
                {
                    GameObject oldMenu = Instantiate(oldMenuPrefab);
                    oldMenu.SetActive(true);
                    Debug.Log("Old menu restored");
                }
                
                // Close settings panel
                gameObject.SetActive(false);
            });
        }
        
        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }
        
        private void ShowConfirmation(string message, System.Action onConfirm)
        {
            if (confirmationDialogPrefab != null)
            {
                GameObject dialogObj = Instantiate(confirmationDialogPrefab, transform.parent);
                var dialog = dialogObj.GetComponent<ConfirmationDialog>();
                if (dialog != null)
                {
                    dialog.Show(message, onConfirm, () => Destroy(dialogObj));
                }
            }
            else
            {
                // Fallback: directly confirm
                onConfirm?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Simple confirmation dialog
    /// </summary>
    public class ConfirmationDialog : MonoBehaviour
    {
        [SerializeField] private Text messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        private System.Action onConfirm;
        private System.Action onCancel;
        
        public void Show(string message, System.Action confirm, System.Action cancel)
        {
            if (messageText != null)
                messageText.text = message;
            
            onConfirm = confirm;
            onCancel = cancel;
            
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmClicked);
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClicked);
        }
        
        private void OnConfirmClicked()
        {
            onConfirm?.Invoke();
        }
        
        private void OnCancelClicked()
        {
            onCancel?.Invoke();
        }
    }
}
