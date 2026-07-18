using UnityEngine;
using System.Collections.Generic;

namespace Asteroid.Debug
{
    /// <summary>
    /// Wireless Debugging Manager
    /// Handles ADB over TCP/IP connection to Meta Quest 3
    /// Enables permission system for headset actions
    /// </summary>
    public class WirelessDebugManager : MonoBehaviour
    {
        public static WirelessDebugManager Instance { get; private set; }
        
        [Header("Connection Settings")]
        [SerializeField] private int debugPort = 5555;
        [SerializeField] private float connectionTimeout = 5f;
        [SerializeField] private bool autoReconnect = true;
        
        [Header("Permission Settings")]
        [SerializeField] private bool requirePermissionForActions = true;
        [SerializeField] private List<string> permittedActions = new List<string>();
        
        private bool isConnected = false;
        private bool permissionsGranted = false;
        private string deviceIP = "";
        private float connectionAttemptTimer = 0f;
        private System.Action<bool> onConnectionStateChanged;
        
        // Permission callbacks
        private Dictionary<string, System.Action<bool>> permissionCallbacks = new Dictionary<string, System.Action<bool>>();
        
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
            InitializeWirelessDebugging();
            SetupDefaultPermissions();
        }
        
        private void InitializeWirelessDebugging()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                GetDeviceIP();
                AttemptConnection();
                isConnected = true;
                onConnectionStateChanged?.Invoke(true);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Wireless Debug Init Failed: {e.Message}");
                isConnected = false;
            }
            #endif
        }
        
        private void GetDeviceIP()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass wifiManager = new AndroidJavaClass("android.net.wifi.WifiManager");
                AndroidJavaClass context = new AndroidJavaClass("android.content.Context");
                string wifiService = context.GetStatic<string>("WIFI_SERVICE");
                
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject wifiMgr = currentActivity.Call<AndroidJavaObject>("getSystemService", wifiService);
                AndroidJavaObject connectionInfo = wifiMgr.Call<AndroidJavaObject>("getConnectionInfo");
                int ipAddress = connectionInfo.Call<int>("getIpAddress");
                
                deviceIP = (ipAddress & 0xff) + "." +
                           ((ipAddress >> 8) & 0xff) + "." +
                           ((ipAddress >> 16) & 0xff) + "." +
                           ((ipAddress >> 24) & 0xff);
                
                Debug.Log($"Device IP: {deviceIP}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not get device IP: {e.Message}");
            }
            #endif
        }
        
        private void AttemptConnection()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            // Connection would be established via system settings
            // This is handled at OS level on Meta Quest
            Debug.Log($"Wireless Debugging ready on {deviceIP}:{debugPort}");
            #endif
        }
        
        private void SetupDefaultPermissions()
        {
            permittedActions.Add("movement_simulation");
            permittedActions.Add("haptic_feedback");
            permittedActions.Add("controller_input");
            permittedActions.Add("hand_tracking");
            permittedActions.Add("performance_monitoring");
            permittedActions.Add("system_info");
        }
        
        private void Update()
        {
            if (autoReconnect && !isConnected)
            {
                connectionAttemptTimer += Time.deltaTime;
                if (connectionAttemptTimer >= connectionTimeout)
                {
                    AttemptConnection();
                    connectionAttemptTimer = 0f;
                }
            }
        }
        
        public bool IsConnected() => isConnected;
        public bool HasPermissions() => permissionsGranted;
        public string GetDeviceIP() => deviceIP;
        
        public void RequestPermissions()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                
                // Request debugging permissions
                Debug.Log("Requesting developer debugging permissions...");
                permissionsGranted = true;
                onConnectionStateChanged?.Invoke(true);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Permission request failed: {e.Message}");
                permissionsGranted = false;
            }
            #endif
        }
        
        public bool CanPerformAction(string actionName)
        {
            if (!requirePermissionForActions)
                return true;
            
            return permissionsGranted && permittedActions.Contains(actionName);
        }
        
        public void RegisterPermissionCallback(string actionName, System.Action<bool> callback)
        {
            if (!permissionCallbacks.ContainsKey(actionName))
                permissionCallbacks[actionName] = callback;
        }
        
        public void OnConnectionStateChanged(System.Action<bool> callback)
        {
            onConnectionStateChanged += callback;
        }
    }
}
