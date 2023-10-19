using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    GameObject camcam;
    public Camera m_Camera;
    public bool ignoreY;
    Status status;
    private void Start()
    {
        status = GetComponentInParent<Status>();
        camcam = Camera.main.gameObject;
        if (camcam != null)

            m_Camera = camcam.GetComponent<Camera>();

    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void FixedUpdate()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -Mathf.Sign(status.transform.localScale.x), transform.localScale.y, transform.localScale.z);
        //if (m_Camera == null)
        //{
        //    camcam = Camera.main.gameObject;
        //    m_Camera = camcam.GetComponent<Camera>();
        //}
        //if (m_Camera != null)
        //{
        //    transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
        //    if (ignoreY) transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        //}

    }
}