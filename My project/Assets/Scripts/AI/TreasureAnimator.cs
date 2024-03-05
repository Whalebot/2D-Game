using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureAnimator : MonoBehaviour
{
    Animator anim;
    Treasure treasure;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        treasure = GetComponentInParent<Treasure>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Open", treasure.open);
    }
}
