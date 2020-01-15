using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;  


    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        // Find the local rotation of the canvas
        m_RelativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        // Every update set the current rotation of the slider wheel to the local rotation.
        // Set the rotation to be static.
        // This reduces the confusion on what the health status is when driving the tank around.
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}
