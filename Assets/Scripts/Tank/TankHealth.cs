using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        // Get the particle system of the instantiated explosion prefab after creating it
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        // Get the audio source of the explosion particle system grabbed from the instantiated explosion prefab
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        // Set the explosion particle system to inactive
        // NOTE: When instantiating a new particle system, it is better to set it to inactive than destory and creating them.
        // Destroying objects may invoke the garbage collector
        m_ExplosionParticles.gameObject.SetActive(false);
    }

    // When tank turns back on
    private void OnEnable()
    {
        // Set health to starting health
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount;

        SetHealthUI();

        // Check if tank health has run out and is not dead
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            // Kill the tank
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider on every frame.
        m_Slider.value = m_CurrentHealth;
        // Interpolate between colours for the slider
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;
        // Set the particle 
        m_ExplosionParticles.transform.position = transform.position;
        // Turn the particle back on
        m_ExplosionParticles.gameObject.SetActive(true);
        // Play the particle effect and explosion audio
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();
        // Turn off the tank and remove it from the scene
        gameObject.SetActive(false);
    }
}