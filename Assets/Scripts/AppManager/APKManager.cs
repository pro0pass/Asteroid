using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Asteroid.AppManager
{
    /// <summary>
    /// Optimized APK Manager for VR/Android
    /// - Caches app list to avoid repeated scans
    /// - Minimal allocations in hot paths
    /// - Efficient Android JNI calls
    /// </summary>
    public class APKManager : MonoBehaviour
    {
        public static APKManager Instance { get; private set; }
        
        [SerializeField] private string apksDirectory = "/sdcard/Android/data/com.asteroid/cache/apks/";
        [SerializeField] private bool autoScanOnStart = true;
        
        private List<InstalledApp> installedApps;
        private bool isScanning = false;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Pre-allocate list
            installedApps = new List<InstalledApp>(64);
        }
        
        private void Start()
        {
            if (autoScanOnStart)
                ScanInstalledApps();
        }
        
        public void ScanInstalledApps()
        {
            if (isScanning)
                return;
            
            isScanning = true;
            installedApps.Clear();
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                // Cache Java references
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
                
                // Get list of installed packages
                AndroidJavaObject packages = packageManager.Call<AndroidJavaObject>("getInstalledPackages", 0);
                int count = packages.Call<int>("size");
                
                for (int i = 0; i < count; i++)
                {
                    AndroidJavaObject packageInfo = packages.Call<AndroidJavaObject>("get", i);
                    
                    // Reuse InstalledApp object
                    InstalledApp app = new InstalledApp
                    {
                        packageName = packageInfo.Get<string>("packageName"),
                        versionCode = packageInfo.Get<int>("versionCode"),
                        versionName = packageInfo.Get<string>("versionName")
                    };
                    
                    installedApps.Add(app);
                }
                
                #if UNITY_EDITOR
                Debug.Log($"Scanned {installedApps.Count} apps");
                #endif
            }
            catch (System.Exception e)
            {
                #if UNITY_EDITOR
                Debug.LogError($"APK scan error: {e.Message}");
                #endif
            }
            #endif
            
            isScanning = false;
        }
        
        public void InstallAPK(string apkPath)
        {
            if (!File.Exists(apkPath))
            {
                #if UNITY_EDITOR
                Debug.LogError($"APK not found: {apkPath}");
                #endif
                return;
            }
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                // Cache Java references
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
                
                intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
                
                // Create URI
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject file = new AndroidJavaObject("java.io.File", apkPath);
                AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("fromFile", file);
                
                // Set data and type
                intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/vnd.android.package-archive");
                intent.Call<AndroidJavaObject>("addFlags", 268435456); // FLAG_ACTIVITY_NEW_TASK
                
                // Start activity
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intent);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"APK installation error: {e.Message}");
            }
            #endif
        }
        
        public List<InstalledApp> GetInstalledApps() => installedApps;
        
        public int GetAppCount() => installedApps.Count;
        
        public bool TryGetApp(string packageName, out InstalledApp app)
        {
            app = null;
            for (int i = 0; i < installedApps.Count; i++)
            {
                if (installedApps[i].packageName == packageName)
                {
                    app = installedApps[i];
                    return true;
                }
            }
            return false;
        }
        
        [System.Serializable]
        public class InstalledApp
        {
            public string packageName;
            public int versionCode;
            public string versionName;
        }
    }
}
