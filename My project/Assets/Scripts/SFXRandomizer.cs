using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXRandomizer : MonoBehaviour
{
    [SerializeField]
    AudioClip[] audioClips;
   
    [SerializeField] float pitchRange = 0.1F;
    [SerializeField] [Range(0, 1)] float minVol = 0.8F;
    [SerializeField] [Range(0, 1)] float maxVol = 1F;
    public float delay = 0;
    AudioSource AS;

    // Start is called before the first frame update
    void OnEnable()
    {
        AS = GetComponent<AudioSource>();
        StartCoroutine("DelayStart");
    }


    IEnumerator DelayStart() {
        yield return new WaitForSeconds(delay);

        if (audioClips.Length > 0)
            AS.clip = audioClips[Random.Range(0, audioClips.Length)];

        AS.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
        AS.volume = Random.Range(minVol, maxVol);
        AS.Play();
    }
}
