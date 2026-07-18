using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Asteroid.Debug
{
    /// <summary>
    /// Advanced Developer Tools
    /// Available only when wireless debugging is connected
    /// </summary>
    public class AdvancedDeveloperTools : MonoBehaviour
    {
        [Header("Performance Profiling")]
        [SerializeField] private bool enableProfiling = true;
        [SerializeField] private float profilingInterval = 1f;
        
        [Header("Network Analysis")]
        [SerializeField] private bool enableNetworkStats = true;
        
        [Header("Memory Analysis")]
        [SerializeField] private bool enableMemoryProfiling = true;
        
        [Header("Input Debugging")]
        [SerializeField] private bool enableInputDebugging = false;
        
        private WirelessDebugManager debugManager;
        private PerformanceProfiler profiler;
        private NetworkAnalyzer networkAnalyzer;
        private MemoryProfiler memoryProfiler;
        private InputDebugger inputDebugger;
        
        private void Start()
        {
            debugManager = WirelessDebugManager.Instance;
            
            if (!debugManager.IsConnected())
            {
                Debug.LogWarning("Advanced Developer Tools: Wireless debugging not connected");
                return;
            }
            
            if (enableProfiling)
                profiler = gameObject.AddComponent<PerformanceProfiler>();
            
            if (enableNetworkStats)
                networkAnalyzer = gameObject.AddComponent<NetworkAnalyzer>();
            
            if (enableMemoryProfiling)
                memoryProfiler = gameObject.AddComponent<MemoryProfiler>();
            
            if (enableInputDebugging)
                inputDebugger = gameObject.AddComponent<InputDebugger>();
        }
    }
    
    /// <summary>
    /// Performance profiling for VR optimization
    /// </summary>
    public class PerformanceProfiler : MonoBehaviour
    {
        private class FrameData
        {
            public float fps;
            public float frameTime;
            public float gpuTime;
            public int drawCalls;
            public int triangles;
            public int vertices;
        }
        
        private List<FrameData> frameHistory = new List<FrameData>(300);
        private StringBuilder reportBuilder = new StringBuilder();
        
        private void Update()
        {
            FrameData frame = new FrameData
            {
                fps = 1f / Time.deltaTime,
                frameTime = Time.deltaTime * 1000f,
            };
            
            frameHistory.Add(frame);
            if (frameHistory.Count > 300)
                frameHistory.RemoveAt(0);
        }
        
        public string GenerateReport()
        {
            if (frameHistory.Count == 0)
                return "No frame data available";
            
            reportBuilder.Clear();
            reportBuilder.AppendLine("=== Performance Report ===");
            
            // Calculate statistics
            float totalFrameTime = 0f;
            float minFPS = float.MaxValue;
            float maxFPS = 0f;
            int droppedFrames = 0;
            
            foreach (var frame in frameHistory)
            {
                totalFrameTime += frame.frameTime;
                minFPS = Mathf.Min(minFPS, frame.fps);
                maxFPS = Mathf.Max(maxFPS, frame.fps);
                
                if (frame.fps < 60f) // Quest 3 target
                    droppedFrames++;
            }
            
            float avgFPS = frameHistory.Count / totalFrameTime * 1000f;
            
            reportBuilder.AppendLine($"Average FPS: {avgFPS:F2}");
            reportBuilder.AppendLine($"Min FPS: {minFPS:F2}");
            reportBuilder.AppendLine($"Max FPS: {maxFPS:F2}");
            reportBuilder.AppendLine($"Dropped Frames: {droppedFrames}");
            reportBuilder.AppendLine($"Total Frames Analyzed: {frameHistory.Count}");
            
            return reportBuilder.ToString();
        }
    }
    
    /// <summary>
    /// Network statistics and bandwidth monitoring
    /// </summary>
    public class NetworkAnalyzer : MonoBehaviour
    {
        private int packetsReceived = 0;
        private int packetsSent = 0;
        private long bytesReceived = 0;
        private long bytesSent = 0;
        private float networkLatency = 0f;
        
        public int GetPacketsReceived() => packetsReceived;
        public int GetPacketsSent() => packetsSent;
        public long GetBytesReceived() => bytesReceived;
        public long GetBytesSent() => bytesSent;
        public float GetNetworkLatency() => networkLatency;
    }
    
    /// <summary>
    /// Memory usage and garbage collection analysis
    /// </summary>
    public class MemoryProfiler : MonoBehaviour
    {
        private long totalMemory = 0;
        private long usedMemory = 0;
        private int gcAllocs = 0;
        private int gcCollections = 0;
        
        private void Update()
        {
            totalMemory = System.GC.GetTotalMemory(false);
            usedMemory = SystemInfo.systemMemorySize;
            
            // Track allocations (simplified)
            gcAllocs = 0; // Would need integration with profiler
        }
        
        public string GetMemoryReport()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== Memory Report ===");
            sb.AppendLine($"Total Memory: {totalMemory / 1024 / 1024}MB");
            sb.AppendLine($"Managed Heap: {usedMemory / 1024 / 1024}MB");
            sb.AppendLine($"GC Allocations: {gcAllocs}");
            return sb.ToString();
        }
    }
    
    /// <summary>
    /// Input debugging and event logging
    /// </summary>
    public class InputDebugger : MonoBehaviour
    {
        private class InputEvent
        {
            public string deviceType;
            public string buttonName;
            public float value;
            public float timestamp;
        }
        
        private List<InputEvent> inputHistory = new List<InputEvent>(500);
        
        private void Update()
        {
            LogTriggerEvents();
            LogButtonEvents();
            LogThumbstickEvents();
        }
        
        private void LogTriggerEvents()
        {
            float leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            float rightTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
            
            if (leftTrigger > 0.1f)
                LogInputEvent("LeftController", "IndexTrigger", leftTrigger);
            if (rightTrigger > 0.1f)
                LogInputEvent("RightController", "IndexTrigger", rightTrigger);
        }
        
        private void LogButtonEvents()
        {
            // Log button presses
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
                LogInputEvent("LeftController", "GripButton", 1f);
            if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
                LogInputEvent("RightController", "GripButton", 1f);
        }
        
        private void LogThumbstickEvents()
        {
            Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            
            if (leftStick.magnitude > 0.5f)
                LogInputEvent("LeftController", "Thumbstick", leftStick.magnitude);
            if (rightStick.magnitude > 0.5f)
                LogInputEvent("RightController", "Thumbstick", rightStick.magnitude);
        }
        
        private void LogInputEvent(string device, string button, float value)
        {
            InputEvent evt = new InputEvent
            {
                deviceType = device,
                buttonName = button,
                value = value,
                timestamp = Time.time
            };
            
            inputHistory.Add(evt);
            if (inputHistory.Count > 500)
                inputHistory.RemoveAt(0);
        }
        
        public List<InputEvent> GetInputHistory() => inputHistory;
    }
}
