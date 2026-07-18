using UnityEngine;

namespace Asteroid.Controllers
{
    /// <summary>
    /// PSA Forward Drift System
    /// Allows stationary users to sprint forward by holding A button
    /// Makes headset think user is running like a cheetah
    /// Controller hands move to middle torso for immersion
    /// </summary>
    public class PSAForwardDrift : MonoBehaviour
    {
        [Header("Drift Settings")]
        [SerializeField] private float driftSpeed = 15f; // Cheetah-like speed
        [SerializeField] private float maxDriftSpeed = 25f;
        [SerializeField] private float driftAcceleration = 10f;
        [SerializeField] private float driftDeceleration = 8f;
        [SerializeField] private KeyCode driftButton = KeyCode.A;
        [SerializeField] private OVRInput.Button ovrDriftButton = OVRInput.Button.Start; // Alt: secondary menu
        
        [Header("Hand Position")]
        [SerializeField] private float handTorsoOffsetY = -0.5f; // Move hands down to torso
        [SerializeField] private float handTorsoOffsetZ = 0.3f; // Move hands slightly forward
        [SerializeField] private float handTransitionSpeed = 10f;
        
        [Header("References")]
        [SerializeField] private Transform playerBody;
        [SerializeField] private OVRCameraRig cameraRig;
        [SerializeField] private Transform leftController;
        [SerializeField] private Transform rightController;
        
        private float currentDriftVelocity = 0f;
        private bool isDrifting = false;
        private Vector3 leftControllerOriginalPos;
        private Vector3 rightControllerOriginalPos;
        private Vector3 leftControllerTorsoPos;
        private Vector3 rightControllerTorsoPos;
        private bool isInTorsoPosition = false;
        
        private Debug.WirelessDebugManager debugManager;
        private Input.InstantTriggerResponse triggerResponse;
        
        private void Start()
        {
            debugManager = Debug.WirelessDebugManager.Instance;
            triggerResponse = GetComponent<Input.InstantTriggerResponse>();
            
            if (cameraRig == null)
                cameraRig = FindObjectOfType<OVRCameraRig>();
            
            if (playerBody == null)
                playerBody = cameraRig.trackingSpace;
            
            // Find or create controller references
            if (leftController == null)
                leftController = cameraRig.leftHandAnchor;
            if (rightController == null)
                rightController = cameraRig.rightHandAnchor;
            
            // Store original positions
            if (leftController != null)
                leftControllerOriginalPos = leftController.localPosition;
            if (rightController != null)
                rightControllerOriginalPos = rightController.localPosition;
        }
        
        private void Update()
        {
            if (!debugManager.CanPerformAction("movement_simulation"))
                return;
            
            // Check for drift activation (A button or OVR button)
            bool driftInput = Input.GetKey(driftButton) || OVRInput.Get(ovrDriftButton);
            
            if (driftInput && !isDrifting)
                StartDrift();
            else if (!driftInput && isDrifting)
                StopDrift();
            
            if (isDrifting)
            {
                UpdateDrift();
                UpdateControllerPosition();
            }
            else
            {
                ReturnControllerPosition();
            }
        }
        
        private void StartDrift()
        {
            isDrifting = true;
            currentDriftVelocity = 0f;
        }
        
        private void StopDrift()
        {
            isDrifting = false;
        }
        
        private void UpdateDrift()
        {
            // Accelerate drift
            currentDriftVelocity = Mathf.Min(
                currentDriftVelocity + driftAcceleration * Time.deltaTime,
                maxDriftSpeed
            );
            
            // Apply forward movement
            Vector3 forwardDirection = cameraRig.centerEyeAnchor.forward;
            forwardDirection.y = 0; // Keep horizontal
            forwardDirection.Normalize();
            
            if (playerBody != null)
            {
                playerBody.position += forwardDirection * currentDriftVelocity * Time.deltaTime;
            }
            
            // Haptic feedback - simulate running
            if (triggerResponse != null && debugManager.HasPermissions())
            {
                float hapticStrength = Mathf.Clamp01(currentDriftVelocity / maxDriftSpeed);
                OVRInput.SetControllerVibration(hapticStrength, 0.6f, OVRInput.Controller.LTouch);
                OVRInput.SetControllerVibration(hapticStrength, 0.6f, OVRInput.Controller.RTouch);
            }
        }
        
        private void UpdateControllerPosition()
        {
            // Calculate torso position for controllers
            Vector3 torsoCenter = cameraRig.centerEyeAnchor.position + 
                                 Vector3.down * 0.8f + 
                                 cameraRig.centerEyeAnchor.forward * handTorsoOffsetZ;
            
            // Left hand slightly to left
            leftControllerTorsoPos = torsoCenter + Vector3.left * 0.2f;
            // Right hand slightly to right
            rightControllerTorsoPos = torsoCenter + Vector3.right * 0.2f;
            
            // Smoothly transition to torso position
            if (leftController != null)
            {
                leftController.position = Vector3.Lerp(
                    leftController.position,
                    leftControllerTorsoPos,
                    handTransitionSpeed * Time.deltaTime
                );
            }
            
            if (rightController != null)
            {
                rightController.position = Vector3.Lerp(
                    rightController.position,
                    rightControllerTorsoPos,
                    handTransitionSpeed * Time.deltaTime
                );
            }
            
            isInTorsoPosition = true;
        }
        
        private void ReturnControllerPosition()
        {
            if (!isInTorsoPosition)
                return;
            
            // Smoothly return to original positions
            if (leftController != null)
            {
                leftController.localPosition = Vector3.Lerp(
                    leftController.localPosition,
                    leftControllerOriginalPos,
                    handTransitionSpeed * Time.deltaTime
                );
            }
            
            if (rightController != null)
            {
                rightController.localPosition = Vector3.Lerp(
                    rightController.localPosition,
                    rightControllerOriginalPos,
                    handTransitionSpeed * Time.deltaTime
                );
            }
            
            // Stop haptics
            if (debugManager.HasPermissions())
            {
                OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.LTouch);
                OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
            }
            
            isInTorsoPosition = false;
        }
        
        public float GetCurrentSpeed() => currentDriftVelocity;
        public bool IsDrifting() => isDrifting;
        
        public void SetDriftSpeed(float speed)
        {
            driftSpeed = speed;
        }
    }
}
