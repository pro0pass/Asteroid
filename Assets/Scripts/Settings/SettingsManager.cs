using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Asteroid.Menu
{
    /// <summary>
    /// Settings Manager for Asteroid
    /// Handles all user preferences and configurations
    /// Persists settings to PlayerPrefs
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }
        
        [System.Serializable]
        public class AsteroidSettings
        {
            // Movement Settings
            public float movementSpeed = 1f;
            public bool joystickEnabled = true;
            public float joystickDeadzone = 0.2f;
            public bool seatedModeEnabled = false;
            public float seatedSpeedMultiplier = 3f;
            
            // Control Settings
            public bool instantTriggerResponse = true;
            public float triggerDeadzone = 0.05f;
            public float gripDeadzone = 0.05f;
            public float hapticIntensity = 0.6f;
            public bool hapticFeedbackEnabled = true;
            
            // Display Settings
            public float screenBrightness = 1f;
            public bool vrPassthroughEnabled = false;
            public float vrPassthroughOpacity = 0.3f;
            public int targetFramerate = 72; // Quest 3 default
            
            // Eye Tracking Settings
            public bool eyeTrackingEnabled = true;
            public float gazeConfidenceThreshold = 0.5f;
            public bool gazeUIEnabled = true;
            
            // Hand Tracking Settings
            public bool handTrackingEnabled = true;
            public float handTrackingConfidence = 0.7f;
            
            // Debugging Settings
            public bool wirelessDebuggingEnabled = false;
            public bool developerUIEnabled = false;
            public bool performanceMonitoringEnabled = false;
            public bool inputDebuggingEnabled = false;
            
            // Audio Settings
            public float masterVolume = 1f;
            public float musicVolume = 0.7f;
            public float sfxVolume = 0.8f;
            public bool spatialAudioEnabled = true;
            
            // Menu Settings
            public bool useNewMetaQuestMenu = true; // Toggle between new and old menu
            public float menuScale = 1f;
            public bool menuAnimationsEnabled = true;
            
            // Accessibility Settings
            public bool colorBlindMode = false;
            public string colorBlindType = "none"; // deuteranopia, protanopia, tritanopia
            public bool subtitlesEnabled = false;
            public float subtitleSize = 1f;
            public bool motionSicknessReduction = false;
            public bool vignettingEnabled = false;
            public float vignetteIntensity = 0.3f;
        }
        
        public AsteroidSettings currentSettings = new AsteroidSettings();
        private Dictionary<string, System.Action<object>> settingChangeCallbacks = new Dictionary<string, System.Action<object>>();
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            LoadSettings();
        }
        
        public void LoadSettings()
        {
            // Load from PlayerPrefs
            currentSettings.movementSpeed = PlayerPrefs.GetFloat("movement_speed", 1f);
            currentSettings.joystickEnabled = PlayerPrefs.GetInt("joystick_enabled", 1) == 1;
            currentSettings.joystickDeadzone = PlayerPrefs.GetFloat("joystick_deadzone", 0.2f);
            currentSettings.seatedModeEnabled = PlayerPrefs.GetInt("seated_mode", 0) == 1;
            currentSettings.seatedSpeedMultiplier = PlayerPrefs.GetFloat("seated_speed", 3f);
            
            currentSettings.instantTriggerResponse = PlayerPrefs.GetInt("instant_trigger", 1) == 1;
            currentSettings.triggerDeadzone = PlayerPrefs.GetFloat("trigger_deadzone", 0.05f);
            currentSettings.gripDeadzone = PlayerPrefs.GetFloat("grip_deadzone", 0.05f);
            currentSettings.hapticIntensity = PlayerPrefs.GetFloat("haptic_intensity", 0.6f);
            currentSettings.hapticFeedbackEnabled = PlayerPrefs.GetInt("haptic_feedback", 1) == 1;
            
            currentSettings.screenBrightness = PlayerPrefs.GetFloat("screen_brightness", 1f);
            currentSettings.vrPassthroughEnabled = PlayerPrefs.GetInt("vr_passthrough", 0) == 1;
            currentSettings.vrPassthroughOpacity = PlayerPrefs.GetFloat("vr_passthrough_opacity", 0.3f);
            currentSettings.targetFramerate = PlayerPrefs.GetInt("target_framerate", 72);
            
            currentSettings.eyeTrackingEnabled = PlayerPrefs.GetInt("eye_tracking", 1) == 1;
            currentSettings.gazeConfidenceThreshold = PlayerPrefs.GetFloat("gaze_confidence", 0.5f);
            currentSettings.gazeUIEnabled = PlayerPrefs.GetInt("gaze_ui", 1) == 1;
            
            currentSettings.handTrackingEnabled = PlayerPrefs.GetInt("hand_tracking", 1) == 1;
            currentSettings.handTrackingConfidence = PlayerPrefs.GetFloat("hand_tracking_confidence", 0.7f);
            
            currentSettings.wirelessDebuggingEnabled = PlayerPrefs.GetInt("wireless_debug", 0) == 1;
            currentSettings.developerUIEnabled = PlayerPrefs.GetInt("developer_ui", 0) == 1;
            currentSettings.performanceMonitoringEnabled = PlayerPrefs.GetInt("perf_monitoring", 0) == 1;
            currentSettings.inputDebuggingEnabled = PlayerPrefs.GetInt("input_debug", 0) == 1;
            
            currentSettings.masterVolume = PlayerPrefs.GetFloat("master_volume", 1f);
            currentSettings.musicVolume = PlayerPrefs.GetFloat("music_volume", 0.7f);
            currentSettings.sfxVolume = PlayerPrefs.GetFloat("sfx_volume", 0.8f);
            currentSettings.spatialAudioEnabled = PlayerPrefs.GetInt("spatial_audio", 1) == 1;
            
            currentSettings.useNewMetaQuestMenu = PlayerPrefs.GetInt("use_new_menu", 1) == 1;
            currentSettings.menuScale = PlayerPrefs.GetFloat("menu_scale", 1f);
            currentSettings.menuAnimationsEnabled = PlayerPrefs.GetInt("menu_animations", 1) == 1;
            
            currentSettings.colorBlindMode = PlayerPrefs.GetInt("colorblind_mode", 0) == 1;
            currentSettings.colorBlindType = PlayerPrefs.GetString("colorblind_type", "none");
            currentSettings.subtitlesEnabled = PlayerPrefs.GetInt("subtitles", 0) == 1;
            currentSettings.subtitleSize = PlayerPrefs.GetFloat("subtitle_size", 1f);
            currentSettings.motionSicknessReduction = PlayerPrefs.GetInt("motion_sickness", 0) == 1;
            currentSettings.vignettingEnabled = PlayerPrefs.GetInt("vignetting", 0) == 1;
            currentSettings.vignetteIntensity = PlayerPrefs.GetFloat("vignette_intensity", 0.3f);
        }
        
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("movement_speed", currentSettings.movementSpeed);
            PlayerPrefs.SetInt("joystick_enabled", currentSettings.joystickEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("joystick_deadzone", currentSettings.joystickDeadzone);
            PlayerPrefs.SetInt("seated_mode", currentSettings.seatedModeEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("seated_speed", currentSettings.seatedSpeedMultiplier);
            
            PlayerPrefs.SetInt("instant_trigger", currentSettings.instantTriggerResponse ? 1 : 0);
            PlayerPrefs.SetFloat("trigger_deadzone", currentSettings.triggerDeadzone);
            PlayerPrefs.SetFloat("grip_deadzone", currentSettings.gripDeadzone);
            PlayerPrefs.SetFloat("haptic_intensity", currentSettings.hapticIntensity);
            PlayerPrefs.SetInt("haptic_feedback", currentSettings.hapticFeedbackEnabled ? 1 : 0);
            
            PlayerPrefs.SetFloat("screen_brightness", currentSettings.screenBrightness);
            PlayerPrefs.SetInt("vr_passthrough", currentSettings.vrPassthroughEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("vr_passthrough_opacity", currentSettings.vrPassthroughOpacity);
            PlayerPrefs.SetInt("target_framerate", currentSettings.targetFramerate);
            
            PlayerPrefs.SetInt("eye_tracking", currentSettings.eyeTrackingEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("gaze_confidence", currentSettings.gazeConfidenceThreshold);
            PlayerPrefs.SetInt("gaze_ui", currentSettings.gazeUIEnabled ? 1 : 0);
            
            PlayerPrefs.SetInt("hand_tracking", currentSettings.handTrackingEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("hand_tracking_confidence", currentSettings.handTrackingConfidence);
            
            PlayerPrefs.SetInt("wireless_debug", currentSettings.wirelessDebuggingEnabled ? 1 : 0);
            PlayerPrefs.SetInt("developer_ui", currentSettings.developerUIEnabled ? 1 : 0);
            PlayerPrefs.SetInt("perf_monitoring", currentSettings.performanceMonitoringEnabled ? 1 : 0);
            PlayerPrefs.SetInt("input_debug", currentSettings.inputDebuggingEnabled ? 1 : 0);
            
            PlayerPrefs.SetFloat("master_volume", currentSettings.masterVolume);
            PlayerPrefs.SetFloat("music_volume", currentSettings.musicVolume);
            PlayerPrefs.SetFloat("sfx_volume", currentSettings.sfxVolume);
            PlayerPrefs.SetInt("spatial_audio", currentSettings.spatialAudioEnabled ? 1 : 0);
            
            PlayerPrefs.SetInt("use_new_menu", currentSettings.useNewMetaQuestMenu ? 1 : 0);
            PlayerPrefs.SetFloat("menu_scale", currentSettings.menuScale);
            PlayerPrefs.SetInt("menu_animations", currentSettings.menuAnimationsEnabled ? 1 : 0);
            
            PlayerPrefs.SetInt("colorblind_mode", currentSettings.colorBlindMode ? 1 : 0);
            PlayerPrefs.SetString("colorblind_type", currentSettings.colorBlindType);
            PlayerPrefs.SetInt("subtitles", currentSettings.subtitlesEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("subtitle_size", currentSettings.subtitleSize);
            PlayerPrefs.SetInt("motion_sickness", currentSettings.motionSicknessReduction ? 1 : 0);
            PlayerPrefs.SetInt("vignetting", currentSettings.vignettingEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("vignette_intensity", currentSettings.vignetteIntensity);
            
            PlayerPrefs.Save();
        }
        
        public void ResetToDefaults()
        {
            PlayerPrefs.DeleteAll();
            currentSettings = new AsteroidSettings();
            SaveSettings();
        }
        
        public AsteroidSettings GetSettings() => currentSettings;
        
        public void SetSetting(string key, object value)
        {
            switch (key.ToLower())
            {
                case "movement_speed":
                    currentSettings.movementSpeed = (float)value;
                    break;
                case "joystick_enabled":
                    currentSettings.joystickEnabled = (bool)value;
                    break;
                case "seated_mode":
                    currentSettings.seatedModeEnabled = (bool)value;
                    break;
                case "eye_tracking":
                    currentSettings.eyeTrackingEnabled = (bool)value;
                    break;
                case "hand_tracking":
                    currentSettings.handTrackingEnabled = (bool)value;
                    break;
                case "use_new_menu":
                    currentSettings.useNewMetaQuestMenu = (bool)value;
                    break;
                case "wireless_debug":
                    currentSettings.wirelessDebuggingEnabled = (bool)value;
                    break;
                case "developer_ui":
                    currentSettings.developerUIEnabled = (bool)value;
                    break;
                case "master_volume":
                    currentSettings.masterVolume = (float)value;
                    break;
                case "haptic_feedback":
                    currentSettings.hapticFeedbackEnabled = (bool)value;
                    break;
                case "colorblind_mode":
                    currentSettings.colorBlindMode = (bool)value;
                    break;
                case "motion_sickness":
                    currentSettings.motionSicknessReduction = (bool)value;
                    break;
            }
            
            // Invoke callback if registered
            if (settingChangeCallbacks.ContainsKey(key))
                settingChangeCallbacks[key].Invoke(value);
            
            SaveSettings();
        }
        
        public void RegisterSettingChangeCallback(string key, System.Action<object> callback)
        {
            if (!settingChangeCallbacks.ContainsKey(key))
                settingChangeCallbacks[key] = callback;
            else
                settingChangeCallbacks[key] += callback;
        }
    }
}
