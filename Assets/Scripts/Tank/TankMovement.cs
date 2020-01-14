using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         

    // As soon as the scene is awakened
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Use this method to disable tanks
    private void OnEnable ()
    {
        // Tanks to be set inactive. When rigid bodies are set as kinematic, no forces are applied on them.
        // Use when you want physics but no forces being applied to the rigid body. 
        m_Rigidbody.isKinematic = false;
        // To prevent tanks from moving unexpectedly when disabled.
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    // Use this method to enable tanks
    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber; // Vertical1, Vertical2, etc.
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        // Grab tank's original audio pitch - to be used to simulate tank engine sound
        m_OriginalPitch = m_MovementAudio.pitch;
    }
    
    // Update every frame - Where input is calculated
    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            // If the audio clip selected for tank not moving is EngineDriving, set it to EngineIdling
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                // Set the audio pitch to a random value between OP-PR and OP + PR
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                // Whenever you change clips using script, need to call the play() method on it to play the audio again.
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                // NOTE: Changing the pitch prevents phasing when two or more players are playing
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }


    }

    // Run the block below every physics step.
    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        // NOTE: Time.deltaTime ensures the updates are performed per second instead of physics step to ensure smoothness.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        // Move the tank by adding movement to the current position of the tank. 
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        // Unity uses Quaternion data type to store rotation value. Can use a function to convert a Vector3 to Quaternion.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f); // x, y, z
        // Cant add two Quaternions together - will not make any sense. They need to be multiplied instead.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}