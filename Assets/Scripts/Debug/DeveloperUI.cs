using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace Asteroid.Debug
{
    /// <summary>
    /// Developer UI for monitoring and debugging
    /// Only visible when wireless debugging is connected and permissions granted
    /// </summary>
    public class DeveloperUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas debugCanvas;
        [SerializeField] private Text debugText;
        [SerializeField] private Button toggleButton;
        [SerializeField] private bool showByDefault = false;
        
        [Header("Display Settings")]
        [SerializeField] private float updateRate = 0.5f;
        [SerializeField] private int maxDisplayLines = 30;
        
        private WirelessDebugManager debugManager;
        private Headset.AdvancedHeadsetFeatures headsetFeatures;
        private PerformanceMetrics perfMetrics;
        private StringBuilder displayBuffer = new StringBuilder();
        private float updateTimer = 0f;
        private bool isVisible = false;
        
        private void Start()
        {
            debugManager = WirelessDebugManager.Instance;
            headsetFeatures = Headset.AdvancedHeadsetFeatures.Instance;
            
            if (debugManager == null || !debugManager.IsConnected())
            {
                gameObject.SetActive(false);
                return;
            }
            
            // Only show if debugging is enabled and connected
            isVisible = showByDefault && debugManager.HasPermissions();
            debugCanvas.enabled = isVisible;
            
            if (toggleButton != null)
                toggleButton.onClick.AddListener(ToggleDebugUI);
        }
        
        private void Update()
        {
            if (!debugManager.IsConnected() || !debugManager.HasPermissions())
                return;
            
            updateTimer += Time.deltaTime;
            if (updateTimer >= updateRate)
            {
                UpdateDebugDisplay();
                updateTimer = 0f;
            }
        }
        
        private void UpdateDebugDisplay()
        {
            displayBuffer.Clear();
            
            // Connection Status
            displayBuffer.AppendLine($"[Wireless Debug] Connected: {debugManager.IsConnected()}");
            displayBuffer.AppendLine($"[Permissions] Granted: {debugManager.HasPermissions()}");
            displayBuffer.AppendLine($"[Device IP] {debugManager.GetDeviceIP()}");
            displayBuffer.AppendLine("");
            
            // Performance
            displayBuffer.AppendLine($"FPS: {1f / Time.deltaTime:F1}");
            displayBuffer.AppendLine($"Frame Time: {Time.deltaTime * 1000f:F2}ms");
            displayBuffer.AppendLine($"Delta Time: {Time.deltaTime:F4}s");
            displayBuffer.AppendLine("");
            
            // Battery & Thermal
            displayBuffer.AppendLine($"Battery: {SystemInfo.batteryLevel * 100f:F1}%");
            displayBuffer.AppendLine($"Graphics Device: {SystemInfo.graphicsDeviceName}");
            displayBuffer.AppendLine($"Processor: {SystemInfo.processorType}");
            displayBuffer.AppendLine("");
            
            // Memory
            long totalMem = System.GC.GetTotalMemory(false);
            displayBuffer.AppendLine($"Managed Heap: {totalMem / 1024f / 1024f:F2}MB");
            displayBuffer.AppendLine("");
            
            // Input
            Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            displayBuffer.AppendLine($"L Stick: {leftStick:F2}");
            displayBuffer.AppendLine($"R Stick: {rightStick:F2}");
            
            debugText.text = displayBuffer.ToString();
        }
        
        private void ToggleDebugUI()
        {
            isVisible = !isVisible;
            debugCanvas.enabled = isVisible;
        }
        
        private void OnDestroy()
        {
            if (toggleButton != null)
                toggleButton.onClick.RemoveListener(ToggleDebugUI);
        }
    }
}
