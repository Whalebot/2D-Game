using UnityEngine;

public class SFXScript : MonoBehaviour
{
    [SerializeField]
    private float randomRange = 0;
    public AudioClip[] clips;
    AudioSource AS;
    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
        AudioManager.Instance.AddSFX(AS);

        if (clips.Length > 0)
        {
            AS.clip = clips[Random.Range(0, clips.Length)];
        }
        AS.pitch = 1 + Random.Range(-randomRange, randomRange);
        AS.Play();
    }


}
