using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    // Needed to filter out the tanks from the scene
    public LayerMask m_TankMask;
    // Needed to play the particles effect when the shell explodes
    public ParticleSystem m_ExplosionParticles;
    // For playing the shell explosion audio
    public AudioSource m_ExplosionAudio;
    // Maximum damage for perfect direct hit
    public float m_MaxDamage = 100f;
    // Amount of force from the explosion - The farther a shell falls from the tank, the less damage it will have
    public float m_ExplosionForce = 1000f;
    // Shells to have a lifetime 2 seconds 
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        // Destroy the shell after 2 seconds
        Destroy(gameObject, m_MaxLifeTime);
    }

    // Runs after any intersection of the object other
    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        // Gather a list of all tanks that have overlapping collider with the shell explosion radius at the current position of the shell
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i=0; i<colliders.Length; i++)
        {
            // Get the rigid body of each collider
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            // SAFETY CHECK: If a rigid body is not found from the colliders, continue to the next iteration
            if (!targetRigidbody)
            {
                continue;
            }

            // Apply an explosion force to the tank using the amount and radius of the force at position of the shell explosion.
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Declaring a variable of Tank Health type which is an instance of a Tank Health class written by us.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            // SAFETY CHECK: If a tank health is not found from the target rigid body, continue to the next iteration
            if (!targetHealth)
            {
                continue;
            }

            // Calculate the damage caused using the tank position when explosion happened
            float damage = CalculateDamage(targetRigidbody.position);

            // Make the affected tank to take damage
            targetHealth.TakeDamage(damage);
        }

        // Detach the tank particle system from the tank parent body so that it does not get destroyed with the tank
        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // Destory the explosion particle game object that the particles are on (not the component) after the duration of particle system (how long they last for)
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        // Destory the shell
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target tank should take based on it's position to the shell position.
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        // Shrinkg the distance between 0 and 1. 1 means shell dropped on the target tank.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        // The closer the shell is to the tank, the higher the damage is to the max damage
        float damage = relativeDistance * m_MaxDamage;
        // Set damage to 0 if it is negative.
        // NOTE: There is an edge case where the tank is outside the overlap sphere but its collider is inside. This means relativeDistance and thus damage becomes negative.
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}