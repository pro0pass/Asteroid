using UnityEngine;

namespace Asteroid.Utils
{
    /// <summary>
    /// Generates and manages falling cosmic shower particle effects
    /// Used as background animation
    /// </summary>
    public class CosmicShowerBackground : MonoBehaviour
    {
        [Header("Particle Settings")]
        [SerializeField] private ParticleSystem cosmicParticles;
        [SerializeField] private int particleCount = 500;
        [SerializeField] private float particleSpeed = 5f;
        [SerializeField] private float particleSize = 0.1f;
        [SerializeField] private Gradient particleColorGradient;
        [SerializeField] private float emissionRate = 50f;
        
        [Header("Canvas Settings")]
        [SerializeField] private Canvas targetCanvas;
        [SerializeField] private float distanceFromCamera = 100f;
        
        private ParticleSystem.EmissionModule emissionModule;
        private ParticleSystem.VelocityOverLifetimeModule velocityModule;
        
        private void Start()
        {
            SetupCosmicShower();
        }
        
        private void SetupCosmicShower()
        {
            if (cosmicParticles == null)
            {
                Debug.LogWarning("CosmicShowerBackground: No ParticleSystem assigned");
                return;
            }
            
            // Setup emission
            emissionModule = cosmicParticles.emission;
            emissionModule.rateOverTime = emissionRate;
            
            // Setup velocity
            velocityModule = cosmicParticles.velocityOverLifetime;
            velocityModule.y = new ParticleSystem.MinMaxCurve(-particleSpeed, -particleSpeed * 1.5f);
            
            // Setup main module
            ParticleSystem.MainModule mainModule = cosmicParticles.main;
            mainModule.maxParticles = particleCount;
            mainModule.startSize = new ParticleSystem.MinMaxCurve(particleSize * 0.5f, particleSize * 1.5f);
            mainModule.startColor = new ParticleSystem.MinMaxGradient(particleColorGradient);
            
            cosmicParticles.Play();
        }
        
        public void SetEmissionRate(float rate)
        {
            emissionModule.rateOverTime = rate;
        }
        
        public void SetParticleSpeed(float speed)
        {
            velocityModule.y = new ParticleSystem.MinMaxCurve(-speed, -speed * 1.5f);
        }
        
        private void Update()
        {
            // Keep particles behind the canvas
            if (targetCanvas != null && cosmicParticles != null)
            {
                cosmicParticles.transform.position = targetCanvas.transform.position + targetCanvas.transform.forward * distanceFromCamera;
            }
        }
    }
}
