using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Asteroid.Settings
{
    /// <summary>
    /// Manages all Asteroid settings
    /// Stores persistent settings and applies them in real-time
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }
        
        [Header("World Settings")]
        [SerializeField] private Slider worldScaleSlider;
        [SerializeField] private Text worldScaleText;
        [SerializeField] private float worldScaleMin = 10f;
        [SerializeField] private float worldScaleMax = 100f;
        
        [Header("Height Settings")]
        [SerializeField] private Slider heightOffsetSlider;
        [SerializeField] private Text heightOffsetText;
        [SerializeField] private float heightOffsetMin = 0f;
        [SerializeField] private float heightOffsetMax = 1f;
        
        [Header("FOV Settings")]
        [SerializeField] private Slider fovSlider;
        [SerializeField] private Text fovText;
        [SerializeField] private float fovMin = 80f;
        [SerializeField] private float fovMax = 120f;
        [SerializeField] private Camera mainCamera;
        
        [Header("Movement Settings")]
        [SerializeField] private Slider movementSpeedSlider;
        [SerializeField] private Text movementSpeedText;
        [SerializeField] private float movementSpeedMin = 1f;
        [SerializeField] private float movementSpeedMax = 10f;
        [SerializeField] private Toggle joystickToggle;
        
        [Header("Battery Settings")]
        [SerializeField] private Toggle batterySaverToggle;
        
        [Header("Arm Settings")]
        [SerializeField] private Slider armOffsetSlider;
        [SerializeField] private Text armOffsetText;
        [SerializeField] private float armOffsetMin = 0f;
        [SerializeField] private float armOffsetMax = 1f;
        
        // Settings data
        private SettingsData currentSettings;
        private const string SETTINGS_KEY = "AsteroidSettings";
        
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
            InitializeUI();
            SetupEventHandlers();
        }
        
        private void LoadSettings()
        {
            string json = PlayerPrefs.GetString(SETTINGS_KEY, "");
            
            if (string.IsNullOrEmpty(json))
            {
                currentSettings = new SettingsData();
            }
            else
            {
                currentSettings = JsonUtility.FromJson<SettingsData>(json);
            }
        }
        
        private void InitializeUI()
        {
            // World Scale
            worldScaleSlider.minValue = worldScaleMin;
            worldScaleSlider.maxValue = worldScaleMax;
            worldScaleSlider.value = currentSettings.worldScale;
            
            // Height Offset
            heightOffsetSlider.minValue = heightOffsetMin;
            heightOffsetSlider.maxValue = heightOffsetMax;
            heightOffsetSlider.value = currentSettings.heightOffset;
            
            // FOV
            fovSlider.minValue = fovMin;
            fovSlider.maxValue = fovMax;
            fovSlider.value = currentSettings.fov;
            
            // Movement Speed
            movementSpeedSlider.minValue = movementSpeedMin;
            movementSpeedSlider.maxValue = movementSpeedMax;
            movementSpeedSlider.value = currentSettings.movementSpeed;
            
            // Joystick
            joystickToggle.isOn = currentSettings.joystickEnabled;
            
            // Battery Saver
            batterySaverToggle.isOn = currentSettings.batterySaver;
            
            // Arm Offset
            armOffsetSlider.minValue = armOffsetMin;
            armOffsetSlider.maxValue = armOffsetMax;
            armOffsetSlider.value = currentSettings.armOffset;
            
            UpdateUIText();
        }
        
        private void SetupEventHandlers()
        {
            worldScaleSlider.onValueChanged.AddListener(OnWorldScaleChanged);
            heightOffsetSlider.onValueChanged.AddListener(OnHeightOffsetChanged);
            fovSlider.onValueChanged.AddListener(OnFOVChanged);
            movementSpeedSlider.onValueChanged.AddListener(OnMovementSpeedChanged);
            joystickToggle.onValueChanged.AddListener(OnJoystickToggled);
            batterySaverToggle.onValueChanged.AddListener(OnBatterySaverToggled);
            armOffsetSlider.onValueChanged.AddListener(OnArmOffsetChanged);
        }
        
        private void OnWorldScaleChanged(float value)
        {
            currentSettings.worldScale = value;
            UpdateUIText();
            ApplySettings();
        }
        
        private void OnHeightOffsetChanged(float value)
        {
            currentSettings.heightOffset = value;
            UpdateUIText();
            ApplySettings();
        }
        
        private void OnFOVChanged(float value)
        {
            currentSettings.fov = value;
            if (mainCamera != null)
                mainCamera.fieldOfView = value;
            UpdateUIText();
            ApplySettings();
        }
        
        private void OnMovementSpeedChanged(float value)
        {
            currentSettings.movementSpeed = value;
            UpdateUIText();
            ApplySettings();
        }
        
        private void OnJoystickToggled(bool value)
        {
            currentSettings.joystickEnabled = value;
            ApplySettings();
        }
        
        private void OnBatterySaverToggled(bool value)
        {
            currentSettings.batterySaver = value;
            ApplyBatterySaver(value);
            ApplySettings();
        }
        
        private void OnArmOffsetChanged(float value)
        {
            currentSettings.armOffset = value;
            UpdateUIText();
            ApplySettings();
        }
        
        private void UpdateUIText()
        {
            worldScaleText.text = $"World Scale: {currentSettings.worldScale:F1}";
            heightOffsetText.text = $"Height Offset: {currentSettings.heightOffset:F2}";
            fovText.text = $"FOV: {currentSettings.fov:F1}";
            movementSpeedText.text = $"Movement Speed: {currentSettings.movementSpeed:F1}x";
            armOffsetText.text = $"Arm Offset: {currentSettings.armOffset:F2}";
        }
        
        private void ApplySettings()
        {
            string json = JsonUtility.ToJson(currentSettings);
            PlayerPrefs.SetString(SETTINGS_KEY, json);
            PlayerPrefs.Save();
        }
        
        private void ApplyBatterySaver(bool enabled)
        {
            if (enabled)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 60;
            }
            else
            {
                Application.targetFrameRate = 72; // Meta Quest 3 native refresh
            }
        }
        
        public SettingsData GetSettings()
        {
            return currentSettings;
        }
    }
    
    [System.Serializable]
    public class SettingsData
    {
        public float worldScale = 10f;
        public float heightOffset = 0f;
        public float fov = 80f;
        public float movementSpeed = 1f;
        public bool joystickEnabled = true;
        public bool batterySaver = false;
        public float armOffset = 0f;
    }
}
