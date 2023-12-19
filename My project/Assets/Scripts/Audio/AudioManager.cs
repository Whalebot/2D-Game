using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public float masterVolume;
    public AudioMixer mixer;
    public AudioMixerGroup mainGroup;
    public AudioSource AS;
    public List<AudioSource> sfx;
    public AudioClip defaultBGM;
    public AudioSource battleMusic;
    public float fadeSpeed;

    public AudioSource SFXPrefab;

    float ASstart;
    float battleStart;

    public enum AudioState
    {
        Default, Night, Battle, Boss
    }
    public AudioState audioState;


    private void Awake()
    {
        Instance = this;
        ASstart = AS.volume;
        GameManager.Instance.advanceGameState += AdvanceFrames;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    void AdvanceFrames()
    {
        mixer.SetFloat("MasterVolume", masterVolume);

        List<AudioSource> temp = new List<AudioSource>();

        foreach (var item in sfx)
        {
            if (!item.isPlaying)
                // if (item.time >= item.clip.length)
                temp.Add(item);
        }

        for (int i = temp.Count - 1; i > 0; i--)
        {
            sfx.Remove(temp[i]);
            Destroy(temp[i].gameObject);
        }
    }

    public void PlaySFX(SFX temp, Vector3 position)
    {
        AudioSource sfx = Instantiate(SFXPrefab, position, Quaternion.identity);

        sfx.pitch = 1 + Random.Range(-temp.pitchRange, temp.pitchRange);

        if (temp.audioClips.Count > 0)
        {
            sfx.clip = temp.audioClips[Random.Range(0, temp.audioClips.Count)];
        }

        else
        {
            Debug.Log(sfx.gameObject + " Missing Audio Clip");
            sfx.clip = null;
        }

        if (sfx.clip != null)
        {
            sfx.Play();

            sfx.volume = temp.volume;

            float length = sfx.clip.length;

            Destroy(sfx.gameObject, length);
        }
    }

    public void AddSFX(AudioSource AS)
    {
        sfx.Add(AS);

    }

    public void FadeOutVolume(AudioSource source)
    {
        StartCoroutine(ReduceVolume(source));
    }

    public void FadeOutVolume()
    {
        StartCoroutine(ReduceVolume(AS));
    }

    IEnumerator ReduceVolume(AudioSource source)
    {
        while (source.volume > 0)
        {
            source.volume -= fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    public void FadeInVolume(AudioSource source)
    {
        StartCoroutine(IncreaseVolume(source));
    }

    public void FadeInVolume()
    {
        StartCoroutine(IncreaseVolume());
    }

    IEnumerator BattleMusic()
    {

        while (AS.volume > 0)
        {
            AS.volume -= fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        battleMusic.gameObject.SetActive(true);
        while (battleMusic.volume < battleStart)
        {
            battleMusic.volume += fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator IncreaseVolume()
    {
        while (AS.volume < ASstart)
        {
            AS.volume += fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator IncreaseVolume(AudioSource source)
    {
        while (source.volume < 1)
        {
            source.volume += fadeSpeed;
            yield return new WaitForEndOfFrame();
        }
    }

    public void GoToState(AudioState state)
    {

    }
}
