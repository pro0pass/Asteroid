using UnityEngine;
using UnityEngine.UI;

namespace Asteroid.Menu
{
    /// <summary>
    /// Individual app icon UI component
    /// Displays app logo, name, and handles interactions
    /// </summary>
    public class AppIconUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Text appNameText;
        [SerializeField] private Button appButton;
        
        private string appId;
        private string appName;
        
        private void Start()
        {
            if (appButton != null)
                appButton.onClick.AddListener(OnAppClicked);
        }
        
        public void SetApp(string name, Sprite icon)
        {
            appName = name;
            appId = name.ToLower().Replace(" ", "_");
            
            if (iconImage != null)
                iconImage.sprite = icon;
            
            if (appNameText != null)
                appNameText.text = name;
        }
        
        private void OnAppClicked()
        {
            Debug.Log($"Launching app: {appName}");
            LaunchApp(appId);
        }
        
        private void LaunchApp(string appId)
        {
            // Launch the application
            // Implementation depends on app type
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
                
                // Set package name and launch
                AndroidJavaClass pmClass = new AndroidJavaClass("android.content.pm.PackageManager");
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                
                // Launch intent
                currentActivity.Call("startActivity", intent);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to launch app {appId}: {e.Message}");
            }
            #endif
        }
    }
}
