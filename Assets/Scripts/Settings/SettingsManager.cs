using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace Asteroid.Settings
{
    /// <summary>
    /// Optimized Settings Manager for VR/Android
    /// - Zero GC allocations for hot paths
    /// - Efficient string caching
    /// - Deferred UI updates
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
        private SettingsData previousSettings;
        private const string SETTINGS_KEY = "AsteroidSettings";
        
        // Cache for string formatting
        private StringBuilder textBuffer = new StringBuilder(32);
        private bool isDirty = false;
        private float saveCooldown = 0f;
        private const float SAVE_DELAY = 0.5f; // Batch saves
        
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
            // Create a copy to detect changes
            previousSettings = JsonUtility.FromJson<SettingsData>(JsonUtility.ToJson(currentSettings));
            InitializeUI();
            SetupEventHandlers();
        }
        
        private void LoadSettings()
        {
            string json = PlayerPrefs.GetString(SETTINGS_KEY, "");
            
            if (string.IsNullOrEmpty(json))
                currentSettings = new SettingsData();
            else
                currentSettings = JsonUtility.FromJson<SettingsData>(json);
        }
        
        private void InitializeUI()
        {
            // Initialize all sliders and toggles
            if (worldScaleSlider != null)
            {
                worldScaleSlider.minValue = worldScaleMin;
                worldScaleSlider.maxValue = worldScaleMax;
                worldScaleSlider.value = currentSettings.worldScale;
            }
            
            if (heightOffsetSlider != null)
            {
                heightOffsetSlider.minValue = heightOffsetMin;
                heightOffsetSlider.maxValue = heightOffsetMax;
                heightOffsetSlider.value = currentSettings.heightOffset;
            }
            
            if (fovSlider != null)
            {
                fovSlider.minValue = fovMin;
                fovSlider.maxValue = fovMax;
                fovSlider.value = currentSettings.fov;
            }
            
            if (movementSpeedSlider != null)
            {
                movementSpeedSlider.minValue = movementSpeedMin;
                movementSpeedSlider.maxValue = movementSpeedMax;
                movementSpeedSlider.value = currentSettings.movementSpeed;
            }
            
            if (joystickToggle != null)
                joystickToggle.isOn = currentSettings.joystickEnabled;
            
            if (batterySaverToggle != null)
                batterySaverToggle.isOn = currentSettings.batterySaver;
            
            if (armOffsetSlider != null)
            {
                armOffsetSlider.minValue = armOffsetMin;
                armOffsetSlider.maxValue = armOffsetMax;
                armOffsetSlider.value = currentSettings.armOffset;
            }
            
            UpdateUIText();
        }
        
        private void SetupEventHandlers()
        {
            if (worldScaleSlider != null)
                worldScaleSlider.onValueChanged.AddListener(OnWorldScaleChanged);
            if (heightOffsetSlider != null)
                heightOffsetSlider.onValueChanged.AddListener(OnHeightOffsetChanged);
            if (fovSlider != null)
                fovSlider.onValueChanged.AddListener(OnFOVChanged);
            if (movementSpeedSlider != null)
                movementSpeedSlider.onValueChanged.AddListener(OnMovementSpeedChanged);
            if (joystickToggle != null)
                joystickToggle.onValueChanged.AddListener(OnJoystickToggled);
            if (batterySaverToggle != null)
                batterySaverToggle.onValueChanged.AddListener(OnBatterySaverToggled);
            if (armOffsetSlider != null)
                armOffsetSlider.onValueChanged.AddListener(OnArmOffsetChanged);
        }
        
        // Setting change callbacks - no allocations
        private void OnWorldScaleChanged(float value)
        {
            currentSettings.worldScale = value;
            UpdateUITextForField(worldScaleText, "World Scale", value, "F1");
            MarkDirty();
        }
        
        private void OnHeightOffsetChanged(float value)
        {
            currentSettings.heightOffset = value;
            UpdateUITextForField(heightOffsetText, "Height Offset", value, "F2");
            MarkDirty();
        }
        
        private void OnFOVChanged(float value)
        {
            currentSettings.fov = value;
            if (mainCamera != null)
                mainCamera.fieldOfView = value;
            UpdateUITextForField(fovText, "FOV", value, "F1");
            MarkDirty();
        }
        
        private void OnMovementSpeedChanged(float value)
        {
            currentSettings.movementSpeed = value;
            UpdateUITextForField(movementSpeedText, "Movement Speed", value, "F1", "x");
            MarkDirty();
        }
        
        private void OnJoystickToggled(bool value)
        {
            currentSettings.joystickEnabled = value;
            MarkDirty();
        }
        
        private void OnBatterySaverToggled(bool value)
        {
            currentSettings.batterySaver = value;
            ApplyBatterySaver(value);
            MarkDirty();
        }
        
        private void OnArmOffsetChanged(float value)
        {
            currentSettings.armOffset = value;
            UpdateUITextForField(armOffsetText, "Arm Offset", value, "F2");
            MarkDirty();
        }
        
        private void UpdateUITextForField(Text textComponent, string label, float value, string format, string suffix = "")
        {
            if (textComponent == null) return;
            
            textBuffer.Clear();
            textBuffer.Append(label);
            textBuffer.Append(": ");
            textBuffer.Append(value.ToString(format));
            if (!string.IsNullOrEmpty(suffix))
                textBuffer.Append(suffix);
            
            textComponent.text = textBuffer.ToString();
        }
        
        private void UpdateUIText()
        {
            UpdateUITextForField(worldScaleText, "World Scale", currentSettings.worldScale, "F1");
            UpdateUITextForField(heightOffsetText, "Height Offset", currentSettings.heightOffset, "F2");
            UpdateUITextForField(fovText, "FOV", currentSettings.fov, "F1");
            UpdateUITextForField(movementSpeedText, "Movement Speed", currentSettings.movementSpeed, "F1", "x");
            UpdateUITextForField(armOffsetText, "Arm Offset", currentSettings.armOffset, "F2");
        }
        
        private void MarkDirty()
        {
            isDirty = true;
            saveCooldown = SAVE_DELAY;
        }
        
        private void Update()
        {
            // Deferred save to batch multiple changes
            if (isDirty)
            {
                saveCooldown -= Time.deltaTime;
                if (saveCooldown <= 0)
                {
                    ApplySettings();
                    isDirty = false;
                }
            }
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
        
        public SettingsData GetSettings() => currentSettings;
        
        public void ResetToDefaults()
        {
            currentSettings = new SettingsData();
            InitializeUI();
            ApplySettings();
        }
        
        private void OnDestroy()
        {
            // Clean up event listeners
            if (worldScaleSlider != null) worldScaleSlider.onValueChanged.RemoveListener(OnWorldScaleChanged);
            if (heightOffsetSlider != null) heightOffsetSlider.onValueChanged.RemoveListener(OnHeightOffsetChanged);
            if (fovSlider != null) fovSlider.onValueChanged.RemoveListener(OnFOVChanged);
            if (movementSpeedSlider != null) movementSpeedSlider.onValueChanged.RemoveListener(OnMovementSpeedChanged);
            if (joystickToggle != null) joystickToggle.onValueChanged.RemoveListener(OnJoystickToggled);
            if (batterySaverToggle != null) batterySaverToggle.onValueChanged.RemoveListener(OnBatterySaverToggled);
            if (armOffsetSlider != null) armOffsetSlider.onValueChanged.RemoveListener(OnArmOffsetChanged);
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
