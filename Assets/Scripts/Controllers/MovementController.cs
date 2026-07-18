using UnityEngine;

namespace Asteroid.Controllers
{
    /// <summary>
    /// Handles WSAD/Joystick-based movement
    /// Moves player in front (near torso), not touching ground
    /// </summary>
    public class MovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float baseMovementSpeed = 5f;
        [SerializeField] private float heightAboveGround = 1.5f;
        [SerializeField] private bool useJoystick = true;
        
        private Settings.SettingsManager settingsManager;
        private Vector3 moveDirection;
        private float currentSpeed;
        private OVRCameraRig cameraRig;
        
        private void Start()
        {
            settingsManager = Settings.SettingsManager.Instance;
            cameraRig = FindObjectOfType<OVRCameraRig>();
            
            if (cameraTransform == null && cameraRig != null)
                cameraTransform = cameraRig.centerEyeAnchor;
        }
        
        private void Update()
        {
            HandleInput();
            UpdateMovement();
        }
        
        private void HandleInput()
        {
            moveDirection = Vector3.zero;
            
            // Get settings
            var settings = settingsManager.GetSettings();
            currentSpeed = baseMovementSpeed * settings.movementSpeed;
            useJoystick = settings.joystickEnabled;
            
            if (useJoystick)
            {
                HandleJoystickInput();
            }
            else
            {
                HandleKeyboardInput();
            }
        }
        
        private void HandleJoystickInput()
        {
            // Get left controller joystick input
            Vector2 joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            
            // Forward/Backward
            if (joystickInput.y > 0.1f)
                moveDirection += cameraTransform.forward;
            else if (joystickInput.y < -0.1f)
                moveDirection -= cameraTransform.forward;
            
            // Left/Right
            if (joystickInput.x > 0.1f)
                moveDirection += cameraTransform.right;
            else if (joystickInput.x < -0.1f)
                moveDirection -= cameraTransform.right;
            
            moveDirection.y = 0; // Keep movement horizontal
            moveDirection = moveDirection.normalized;
        }
        
        private void HandleKeyboardInput()
        {
            // WSAD input
            if (Input.GetKey(KeyCode.W))
                moveDirection += cameraTransform.forward;
            if (Input.GetKey(KeyCode.S))
                moveDirection -= cameraTransform.forward;
            if (Input.GetKey(KeyCode.D))
                moveDirection += cameraTransform.right;
            if (Input.GetKey(KeyCode.A))
                moveDirection -= cameraTransform.right;
            
            moveDirection.y = 0;
            moveDirection = moveDirection.normalized;
        }
        
        private void UpdateMovement()
        {
            if (moveDirection.magnitude > 0)
            {
                Vector3 movement = moveDirection * currentSpeed * Time.deltaTime;
                
                // Move player near torso, not touching ground
                if (playerBody != null)
                {
                    playerBody.position += movement;
                }
                else if (cameraRig != null)
                {
                    cameraRig.transform.position += movement;
                }
            }
        }
        
        public void SetMovementSpeed(float multiplier)
        {
            currentSpeed = baseMovementSpeed * multiplier;
        }
    }
}
