using UnityEngine;
using System.Collections.Generic;

namespace Asteroid.Headset
{
    /// <summary>
    /// Eye Tracking System for Meta Quest 3
    /// Supports eye gaze, pupil tracking, and fixation detection
    /// Requires Meta Quest 3 with eye tracking hardware
    /// </summary>
    public class EyeTrackingSystem : MonoBehaviour
    {
        public static EyeTrackingSystem Instance { get; private set; }
        
        [Header("Eye Tracking Settings")]
        [SerializeField] private bool enableEyeTracking = true;
        [SerializeField] private float gazeConfidenceThreshold = 0.5f;
        [SerializeField] private int gazeHistorySize = 10;
        [SerializeField] private float fixationThreshold = 0.05f; // degrees
        [SerializeField] private float fixationDuration = 0.5f; // seconds
        
        [Header("References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private OVRCameraRig cameraRig;
        
        private bool isEyeTrackingSupported = false;
        private bool isEyeTrackingEnabled = false;
        
        private OVREyeGaze leftEyeGaze;
        private OVREyeGaze rightEyeGaze;
        
        private Vector3 gazeDirection = Vector3.forward;
        private Vector3 gazeOrigin = Vector3.zero;
        private float gazeConfidence = 0f;
        
        private List<Vector3> gazeHistory = new List<Vector3>();
        private float fixationTimer = 0f;
        private Vector3 fixationPoint = Vector3.zero;
        
        private System.Action<Vector3, float> onGazeUpdated;
        private System.Action<Vector3> onFixationDetected;
        
        private Debug.WirelessDebugManager debugManager;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            debugManager = Debug.WirelessDebugManager.Instance;
            
            if (mainCamera == null)
                mainCamera = Camera.main;
            
            if (cameraRig == null)
                cameraRig = FindObjectOfType<OVRCameraRig>();
            
            InitializeEyeTracking();
        }
        
        private void InitializeEyeTracking()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                // Check if eye tracking is available
                isEyeTrackingSupported = OVRPlugin.eyeTrackedObjectsSupported;
                
                if (isEyeTrackingSupported && enableEyeTracking)
                {
                    // Enable eye tracking
                    OVRPlugin.SetEyeTrackingEnabled(true);
                    isEyeTrackingEnabled = true;
                    Debug.Log("Eye Tracking Initialized - Meta Quest 3 Eye Tracking Enabled");
                }
                else
                {
                    Debug.LogWarning("Eye tracking not supported on this device");
                    isEyeTrackingEnabled = false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize eye tracking: {e.Message}");
                isEyeTrackingEnabled = false;
            }
            #endif
        }
        
        private void Update()
        {
            if (!isEyeTrackingEnabled)
                return;
            
            if (!debugManager.CanPerformAction("hand_tracking")) // Uses hand tracking permission
                return;
            
            UpdateEyeGaze();
            UpdateFixationDetection();
        }
        
        private void UpdateEyeGaze()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                // Get eye gaze data
                OVRPlugin.EyeGazeState gazeState = new OVRPlugin.EyeGazeState();
                
                if (OVRPlugin.GetEyeGazeState(OVRPlugin.EyeTrackingBone.Gaze, out gazeState))
                {
                    // Extract gaze direction and position
                    gazeOrigin = gazeState.Position;
                    gazeDirection = gazeState.Rotation * Vector3.forward;
                    gazeConfidence = gazeState.Confidence;
                    
                    // Add to history for smoothing
                    gazeHistory.Add(gazeDirection);
                    if (gazeHistory.Count > gazeHistorySize)
                        gazeHistory.RemoveAt(0);
                    
                    // Smooth gaze direction
                    Vector3 smoothedGaze = Vector3.zero;
                    foreach (var g in gazeHistory)
                        smoothedGaze += g;
                    smoothedGaze /= gazeHistory.Count;
                    gazeDirection = smoothedGaze.normalized;
                    
                    // Invoke callback
                    onGazeUpdated?.Invoke(gazeDirection, gazeConfidence);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error updating eye gaze: {e.Message}");
            }
            #endif
        }
        
        private void UpdateFixationDetection()
        {
            if (gazeConfidence < gazeConfidenceThreshold)
            {
                fixationTimer = 0f;
                return;
            }
            
            // Check if gaze is stable (fixating)
            Vector3 gazeDelta = gazeDirection - fixationPoint;
            float gazeDeltaMagnitude = gazeDelta.magnitude;
            
            if (gazeDeltaMagnitude < fixationThreshold)
            {
                fixationTimer += Time.deltaTime;
                
                if (fixationTimer >= fixationDuration)
                {
                    // Fixation detected
                    onFixationDetected?.Invoke(fixationPoint);
                    fixationTimer = 0f;
                }
            }
            else
            {
                fixationTimer = 0f;
                fixationPoint = gazeDirection;
            }
        }
        
        // Callbacks and accessors
        public void RegisterGazeCallback(System.Action<Vector3, float> callback)
        {
            onGazeUpdated += callback;
        }
        
        public void RegisterFixationCallback(System.Action<Vector3> callback)
        {
            onFixationDetected += callback;
        }
        
        public Vector3 GetGazeDirection() => gazeDirection;
        public Vector3 GetGazeOrigin() => gazeOrigin;
        public float GetGazeConfidence() => gazeConfidence;
        public bool IsEyeTrackingEnabled() => isEyeTrackingEnabled;
        
        public Ray GetGazeRay()
        {
            return new Ray(gazeOrigin, gazeDirection);
        }
        
        public bool TryGetGazePoint(float distance, out Vector3 gazePoint)
        {
            gazePoint = gazeOrigin + gazeDirection * distance;
            return gazeConfidence >= gazeConfidenceThreshold;
        }
        
        // UI interaction support
        public bool IsGazingAt(Collider collider)
        {
            Ray gazeRay = GetGazeRay();
            return collider.bounds.IntersectRay(gazeRay) && gazeConfidence >= gazeConfidenceThreshold;
        }
    }
}
