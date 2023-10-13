using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Vector3 dir;
    public Gradient dmgGradient;
    public int minimumValue;
    public int maximumValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupNumber(int dmg) {
        text.text = "" + dmg;

        float val = dmg;
        val = (dmg - minimumValue)/(maximumValue - minimumValue);
        if (dmg < minimumValue) val = 0;
        if (dmg > maximumValue) val = 1;
        text.color = dmgGradient.Evaluate(val);
    }

    private void FixedUpdate()
    {
        transform.Translate(dir);
    }
}
