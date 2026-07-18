using UnityEngine;

namespace Asteroid.Controllers
{
    /// <summary>
    /// Optimized Movement Controller for VR/Android
    /// - Cache all component lookups
    /// - Reuse Vector3 calculations
    /// - Zero allocations in Update loop
    /// </summary>
    public class MovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float baseMovementSpeed = 5f;
        [SerializeField] private float heightAboveGround = 1.5f;
        [SerializeField] private bool useJoystick = true;
        [SerializeField] private float joystickDeadzone = 0.1f;
        
        private Settings.SettingsManager settingsManager;
        private Vector3 moveDirection = Vector3.zero;
        private Vector3 movement = Vector3.zero;
        private float currentSpeed = 0f;
        private OVRCameraRig cameraRig;
        private Transform targetTransform;
        
        // Cache to avoid repeated allocations
        private Vector3 forward;
        private Vector3 right;
        private Vector2 joystickInput;
        
        private void Start()
        {
            settingsManager = Settings.SettingsManager.Instance;
            cameraRig = FindObjectOfType<OVRCameraRig>();
            
            if (cameraTransform == null && cameraRig != null)
                cameraTransform = cameraRig.centerEyeAnchor;
            
            // Determine target transform once
            targetTransform = playerBody != null ? playerBody : (cameraRig != null ? cameraRig.transform : transform);
        }
        
        private void Update()
        {
            HandleInput();
            UpdateMovement();
        }
        
        private void HandleInput()
        {
            // Reset direction without allocation
            moveDirection.x = 0;
            moveDirection.y = 0;
            moveDirection.z = 0;
            
            // Get settings once per frame
            var settings = settingsManager.GetSettings();
            currentSpeed = baseMovementSpeed * settings.movementSpeed;
            useJoystick = settings.joystickEnabled;
            
            if (useJoystick)
                HandleJoystickInput();
            else
                HandleKeyboardInput();
            }
        
        private void HandleJoystickInput()
        {
            // Get joystick input
            joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            
            // Cache forward/right for this frame
            forward = cameraTransform.forward;
            right = cameraTransform.right;
            
            // Forward/Backward
            if (joystickInput.y > joystickDeadzone)
                moveDirection += forward;
            else if (joystickInput.y < -joystickDeadzone)
                moveDirection -= forward;
            
            // Left/Right
            if (joystickInput.x > joystickDeadzone)
                moveDirection += right;
            else if (joystickInput.x < -joystickDeadzone)
                moveDirection -= right;
            
            // Normalize in-place without allocation
            moveDirection.y = 0;
            float magnitude = moveDirection.magnitude;
            if (magnitude > 0.0001f)
                moveDirection /= magnitude;
        }
        
        private void HandleKeyboardInput()
        {
            // Cache forward/right for this frame
            forward = cameraTransform.forward;
            right = cameraTransform.right;
            
            // WSAD input
            if (Input.GetKey(KeyCode.W))
                moveDirection += forward;
            if (Input.GetKey(KeyCode.S))
                moveDirection -= forward;
            if (Input.GetKey(KeyCode.D))
                moveDirection += right;
            if (Input.GetKey(KeyCode.A))
                moveDirection -= right;
            
            // Normalize
            moveDirection.y = 0;
            float magnitude = moveDirection.magnitude;
            if (magnitude > 0.0001f)
                moveDirection /= magnitude;
        }
        
        private void UpdateMovement()
        {
            // Only update if moving
            float magnitude = moveDirection.magnitude;
            if (magnitude > 0.0001f)
            {
                // Reuse movement vector
                float distance = currentSpeed * Time.deltaTime * magnitude;
                movement = moveDirection * distance;
                
                // Move player
                if (targetTransform != null)
                    targetTransform.position += movement;
            }
        }
        
        public void SetMovementSpeed(float multiplier)
        {
            currentSpeed = baseMovementSpeed * multiplier;
        }
        
        public float GetCurrentSpeed() => currentSpeed;
    }
}
