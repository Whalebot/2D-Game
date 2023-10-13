using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyScript : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 2;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Delete");
    }

    IEnumerator Delete() {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
