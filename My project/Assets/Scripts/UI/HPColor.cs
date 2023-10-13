using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPColor : MonoBehaviour
{

    public Gradient gradient;

    Image img;


    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

        // img.color = Color.Lerp(hpMin, hpMax, transform.localScale.x);

        img.color = gradient.Evaluate(1 - transform.localScale.x);

    }
}
