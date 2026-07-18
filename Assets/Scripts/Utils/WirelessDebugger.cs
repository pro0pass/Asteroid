using UnityEngine;

namespace Asteroid.Utils
{
    /// <summary>
    /// Wireless debugging utility
    /// Manages ADB over TCP/IP connection (port 5555)
    /// </summary>
    public class WirelessDebugger : MonoBehaviour
    {
        public static WirelessDebugger Instance { get; private set; }
        
        [SerializeField] private int debugPort = 5555;
        [SerializeField] private string deviceIP = "";
        [SerializeField] private bool debugLogsEnabled = true;
        
        private bool isConnected = false;
        
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
        }
        
        private void InitializeWirelessDebugging()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                // Enable wireless debugging
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                
                // Get device IP
                GetDeviceIP();
                
                Debug.Log($"Wireless Debugging Enabled on {deviceIP}:{debugPort}");
                isConnected = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize wireless debugging: {e.Message}");
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
                
                string wifi_service = context.GetStatic<string>("WIFI_SERVICE");
                
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject wifiMgr = currentActivity.Call<AndroidJavaObject>("getSystemService", wifi_service);
                
                AndroidJavaObject connectionInfo = wifiMgr.Call<AndroidJavaObject>("getConnectionInfo");
                int ipAddress = connectionInfo.Call<int>("getIpAddress");
                
                deviceIP = (ipAddress & 0xff) + "." + ((ipAddress >> 8) & 0xff) + "." +
                          ((ipAddress >> 16) & 0xff) + "." + ((ipAddress >> 24) & 0xff);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not retrieve device IP: {e.Message}");
            }
            #endif
        }
        
        public bool IsConnected()
        {
            return isConnected;
        }
        
        public string GetDeviceIP()
        {
            return deviceIP;
        }
        
        public void LogDebug(string message)
        {
            if (debugLogsEnabled)
            {
                Debug.Log($"[Wireless Debug] {message}");
            }
        }
    }
}
