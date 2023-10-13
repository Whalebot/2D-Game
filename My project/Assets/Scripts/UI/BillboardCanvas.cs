using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    GameObject camcam;
    public Camera m_Camera;
    public bool ignoreY;
    private void Start()
    {

        {
            camcam = Camera.main.gameObject;
            if (camcam != null)

                m_Camera = camcam.GetComponent<Camera>();
        }

    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void FixedUpdate()
    {
        if (m_Camera == null)
        {
            camcam = Camera.main.gameObject;
            m_Camera = camcam.GetComponent<Camera>();
        }
        if (m_Camera != null)
        {
            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
            if (ignoreY) transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

    }
}