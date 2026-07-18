using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Asteroid.AppManager
{
    /// <summary>
    /// Manages APK installation, signing, and app management
    /// </summary>
    public class APKManager : MonoBehaviour
    {
        public static APKManager Instance { get; private set; }
        
        [SerializeField] private string apksDirectory = "/sdcard/Android/data/com.asteroid/cache/apks/";
        [SerializeField] private string signingKeystorePath = "/sdcard/Android/data/com.asteroid/cache/keystore.jks";
        
        private List<InstalledApp> installedApps = new List<InstalledApp>();
        
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
            ScanInstalledApps();
        }
        
        public void ScanInstalledApps()
        {
            installedApps.Clear();
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
                
                // Get list of installed packages
                AndroidJavaObject packages = packageManager.Call<AndroidJavaObject>("getInstalledPackages", 0);
                
                int count = packages.Call<int>("size");
                for (int i = 0; i < count; i++)
                {
                    AndroidJavaObject packageInfo = packages.Call<AndroidJavaObject>("get", i);
                    string packageName = packageInfo.Get<string>("packageName");
                    
                    InstalledApp app = new InstalledApp
                    {
                        packageName = packageName,
                        versionCode = packageInfo.Get<int>("versionCode"),
                        versionName = packageInfo.Get<string>("versionName")
                    };
                    
                    installedApps.Add(app);
                }
                
                Debug.Log($"Scanned {installedApps.Count} installed apps");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error scanning installed apps: {e.Message}");
            }
            #endif
        }
        
        public void InstallAPK(string apkPath)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
                
                intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
                
                // Set package installer
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("fromFile", new AndroidJavaObject("java.io.File", apkPath));
                
                intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/vnd.android.package-archive");
                intent.Call<AndroidJavaObject>("addFlags", 268435456); // FLAG_ACTIVITY_NEW_TASK
                
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intent);
                
                Debug.Log($"Installing APK: {apkPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error installing APK: {e.Message}");
            }
            #endif
        }
        
        public void SignAPK(string apkPath, string keystorePath, string keystorePassword, string keyAlias, string keyPassword)
        {
            #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            try
            {
                string command = $"jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore \"{keystorePath}\" -storepass {keystorePassword} -keypass {keyPassword} \"{apkPath}\" {keyAlias}";
                
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                
                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    Debug.Log($"APK Signed: {output}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error signing APK: {e.Message}");
            }
            #endif
        }
        
        public List<InstalledApp> GetInstalledApps()
        {
            return installedApps;
        }
        
        [System.Serializable]
        public class InstalledApp
        {
            public string packageName;
            public int versionCode;
            public string versionName;
            public string appName;
            public Sprite icon;
        }
    }
}
