using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 vel;
    Rigidbody rb;
    public Vector3 offset;
    public LayerMask interactMask;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameManager.Instance.advanceGameState += ExecuteFrame;
    }

    private void ExecuteFrame()
    {
        rb.velocity = (vel);
        Collider[] col = Physics.OverlapBox(transform.position + offset, Vector3.one * 2, transform.rotation, interactMask);
        foreach (Collider item in col)
        {
            Status status = item.GetComponent<Status>();
            if (status != null)
            {
                Debug.Log(status.transform);
                Rigidbody tempRb = status.GetComponent<Rigidbody>();
                tempRb.velocity += rb.velocity;
            }
        }
    }
}
