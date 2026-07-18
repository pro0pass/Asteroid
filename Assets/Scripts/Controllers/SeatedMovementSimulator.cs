using UnityEngine;

namespace Asteroid.Controllers
{
    /// <summary>
    /// Seated Movement Simulation
    /// Makes headset think user is moving fast while stationary
    /// Perfect for sitting users
    /// </summary>
    public class SeatedMovementSimulator : MonoBehaviour
    {
        [Header("Simulation Settings")]
        [SerializeField] private float simulatedSpeedMultiplier = 3f;
        [SerializeField] private bool enableHeadMovementTracking = true;
        [SerializeField] private float headMovementSensitivity = 1.5f;
        [SerializeField] private bool smoothMovement = true;
        [SerializeField] private float smoothDamping = 0.3f;
        
        [Header("References")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private OVRCameraRig cameraRig;
        
        private Vector3 simulatedVelocity = Vector3.zero;
        private Vector3 smoothedVelocity = Vector3.zero;
        private Vector3 previousHeadPosition = Vector3.zero;
        private bool isSimulationActive = false;
        private Debug.WirelessDebugManager debugManager;
        
        private void Start()
        {
            debugManager = Debug.WirelessDebugManager.Instance;
            
            if (cameraRig == null)
                cameraRig = FindObjectOfType<OVRCameraRig>();
            
            if (playerTransform == null)
                playerTransform = cameraRig.trackingSpace;
            
            previousHeadPosition = cameraRig.centerEyeAnchor.position;
        }
        
        private void Update()
        {
            if (!debugManager.CanPerformAction("movement_simulation"))
                return;
            
            // Detect if user is stationary
            DetectSeatedState();
            
            // Track head movement for simulation
            if (enableHeadMovementTracking && isSimulationActive)
            {
                TrackHeadMovement();
            }
        }
        
        private void DetectSeatedState()
        {
            // Check if controller movement is minimal
            Vector2 leftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            Vector2 rightThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            
            float thumbstickMagnitude = Mathf.Max(leftThumbstick.magnitude, rightThumbstick.magnitude);
            
            // If thumbstick input is significant, disable simulation
            isSimulationActive = thumbstickMagnitude < 0.2f;
        }
        
        private void TrackHeadMovement()
        {
            Vector3 currentHeadPos = cameraRig.centerEyeAnchor.position;
            Vector3 headDelta = currentHeadPos - previousHeadPosition;
            
            // Apply head movement to simulate velocity
            simulatedVelocity = headDelta * simulatedSpeedMultiplier;
            
            // Smooth the velocity
            if (smoothMovement)
                smoothedVelocity = Vector3.Lerp(smoothedVelocity, simulatedVelocity, smoothDamping);
            else
                smoothedVelocity = simulatedVelocity;
            
            // Apply to player
            if (playerTransform != null)
            {
                Vector3 newPosition = playerTransform.position + smoothedVelocity * Time.deltaTime * headMovementSensitivity;
                playerTransform.position = newPosition;
            }
            
            previousHeadPosition = currentHeadPos;
        }
        
        public void SetSimulationActive(bool active)
        {
            isSimulationActive = active;
        }
        
        public void SetSpeedMultiplier(float multiplier)
        {
            simulatedSpeedMultiplier = Mathf.Max(1f, multiplier);
        }
        
        public bool IsSimulationActive() => isSimulationActive;
    }
}
