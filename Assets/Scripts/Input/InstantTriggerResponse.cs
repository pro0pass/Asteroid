using UnityEngine;

namespace Asteroid.Input
{
    /// <summary>
    /// Instant Trigger Response System
    /// Responds to minimal button press (slight pressure)
    /// Works with trigger and grip buttons
    /// </summary>
    public class InstantTriggerResponse : MonoBehaviour
    {
        [Header("Trigger Settings")]
        [SerializeField] private float triggerDeadzone = 0.05f; // Very responsive
        [SerializeField] private float gripDeadzone = 0.05f;
        [SerializeField] private float vibrationIntensity = 0.5f;
        [SerializeField] private bool enableHapticFeedback = true;
        
        [Header("Response Timing")]
        [SerializeField] private float responseDelay = 0.001f; // 1ms response
        [SerializeField] private float triggerSensitivity = 1f;
        
        private float leftTriggerPrevious = 0f;
        private float rightTriggerPrevious = 0f;
        private float leftGripPrevious = 0f;
        private float rightGripPrevious = 0f;
        
        private Debug.WirelessDebugManager debugManager;
        private System.Action<OVRInput.Controller, float> onTriggerPressed;
        private System.Action<OVRInput.Controller, float> onGripPressed;
        
        private void Start()
        {
            debugManager = Debug.WirelessDebugManager.Instance;
        }
        
        private void Update()
        {
            if (!debugManager.CanPerformAction("controller_input"))
                return;
            
            CheckTriggers();
            CheckGrips();
        }
        
        private void CheckTriggers()
        {
            // Left trigger
            float leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            if (leftTrigger > triggerDeadzone && leftTriggerPrevious <= triggerDeadzone)
            {
                OnTriggerPressed(OVRInput.Controller.LTouch, leftTrigger);
            }
            leftTriggerPrevious = leftTrigger;
            
            // Right trigger
            float rightTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
            if (rightTrigger > triggerDeadzone && rightTriggerPrevious <= triggerDeadzone)
            {
                OnTriggerPressed(OVRInput.Controller.RTouch, rightTrigger);
            }
            rightTriggerPrevious = rightTrigger;
        }
        
        private void CheckGrips()
        {
            // Left grip
            float leftGrip = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
            if (leftGrip > gripDeadzone && leftGripPrevious <= gripDeadzone)
            {
                OnGripPressed(OVRInput.Controller.LTouch, leftGrip);
            }
            leftGripPrevious = leftGrip;
            
            // Right grip
            float rightGrip = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
            if (rightGrip > gripDeadzone && rightGripPrevious <= gripDeadzone)
            {
                OnGripPressed(OVRInput.Controller.RTouch, rightGrip);
            }
            rightGripPrevious = rightGrip;
        }
        
        private void OnTriggerPressed(OVRInput.Controller controller, float pressure)
        {
            onTriggerPressed?.Invoke(controller, pressure);
            
            if (enableHapticFeedback && debugManager.HasPermissions())
            {
                // Haptic feedback
                OVRInput.SetControllerVibration(1f, vibrationIntensity, controller);
            }
        }
        
        private void OnGripPressed(OVRInput.Controller controller, float pressure)
        {
            onGripPressed?.Invoke(controller, pressure);
            
            if (enableHapticFeedback && debugManager.HasPermissions())
            {
                // Stronger haptic for grip
                OVRInput.SetControllerVibration(1f, vibrationIntensity * 1.2f, controller);
            }
        }
        
        public void RegisterTriggerCallback(System.Action<OVRInput.Controller, float> callback)
        {
            onTriggerPressed += callback;
        }
        
        public void RegisterGripCallback(System.Action<OVRInput.Controller, float> callback)
        {
            onGripPressed += callback;
        }
        
        public void SetTriggerDeadzone(float deadzone)
        {
            triggerDeadzone = Mathf.Clamp(deadzone, 0f, 0.5f);
        }
        
        public void SetGripDeadzone(float deadzone)
        {
            gripDeadzone = Mathf.Clamp(deadzone, 0f, 0.5f);
        }
    }
}
