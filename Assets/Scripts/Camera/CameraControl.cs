using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Approximate time for the camera to move to position - Smoothes camera pan
    public float m_DampTime = 0.2f;
    // Tanks to always remain on the screen (Padding for the level size)
    public float m_ScreenEdgeBuffer = 4f;
    // Prevent camera from zooming in too much
    public float m_MinSize = 6.5f;
    // Set tanks to be followed by the camera. Hide this variable from Unity UI
    // NOTE: Set to public so that other scripts can set targets variable
    // Transform[] means array of transforms (all of the tanks called Targets)
    [HideInInspector] public Transform[] m_Targets; 

    // Access the camera to change its size variable and change it
    private Camera m_Camera;
    // Dampening the camera zoom and movement
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;
    // The position the camera is trying to reach (=Avg pos of all tanks)
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        // Get the main camera from the CameraRig object. 
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        // Methods to move and zoom the camera
        Move();
        Zoom();
    }

    
    private void Move()
    {
        // Find avg pos of all tanks
        FindAveragePosition();

        // Smoothly move the camera from the current position to the desired position
        // NOTE: ref means to write back to the variable
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        // Create a blank new Vector3
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        // Targets is the list of tanks
        for (int i = 0; i < m_Targets.Length; i++)
        {
            // Continue looping to the next item if tank object is not active. Don't zoom on deactivated tanks
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            // Grab the tanks position and add to the average position of all tanks
            averagePos += m_Targets[i].position;
            numTargets++;
        }

        // Get the avg pos of all tanks depending on how many tanks there are
        if (numTargets > 0)
            averagePos /= numTargets;

        // Set the y-cord of avg pos to the rig pos. Don't need to change the y-cordinates
        // NOTE: to makes sure camera is not moving up and down
        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        // orthographicSize is size settings of the main camera in orthographic mode
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        // Transform camera position from world space to local space
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        // Calculate the size for all tanks and set the size to the biggest value
        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Set the y value to whatever is bigger, the current size or the new calculated size
            // NOTE: Camera's size in y direction equals the distance y from local pos
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            // NOTE: Camera's size in x direction is x-coordinates of desired pos divided by current aspect ratio
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }

        // Add padding to camera size so tanks never get too close to camera screen border
        size += m_ScreenEdgeBuffer;

        // Size to not go below Minsize otherwise the zoom becomes too small
        size = Mathf.Max(size, m_MinSize);

        return size;
    }

    // Public functions can be called from outside of the script
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}