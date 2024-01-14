using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Script : MonoBehaviour
{
    [ShowInInspector]
    public static BGM_Script instance;
    [ShowInInspector]
    AudioClip currentClip;

    private void Awake()
    {
        if(instance != null && GetComponent<AudioSource>().clip == currentClip)
        {
            Destroy(gameObject);
        }
        else
        {
            if(instance != null && GetComponent<AudioSource>().clip != currentClip)
            {
                Destroy(instance.gameObject);
            }
            instance = this;
            currentClip = GetComponent<AudioSource>().clip;
            DontDestroyOnLoad(this.gameObject);
        }
    }

}
