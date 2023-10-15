using Cinemachine;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [TabGroup("Components")] public Camera overlayCamera;
    [TabGroup("Components")] public CinemachineVirtualCamera gameplayCamera;
     [TabGroup("Components")] public CinemachineConfiner confiner;
    [TabGroup("Components")] public CinemachineVirtualCamera[] cameras;
    [TabGroup("Components")] public Movement mov;

    [TabGroup("Components")] CinemachineBasicMultiChannelPerlin[] noises;
    [TabGroup("Settings")] public LayerMask defaultLayerMask;
    [TabGroup("Settings")] public LayerMask cutsceneLayerMask;

    [TabGroup("Settings")] [SerializeField] private float shakeTimer;
    [TabGroup("Settings")] private float startTimer;
    [TabGroup("Settings")] private float startIntensity;
    [TabGroup("Settings")] public bool toggle;
    [TabGroup("Settings")] public float xMult, yMult;
    [TabGroup("Settings")] public float camLerp;
    [TabGroup("Components")] public Transform defaultTarget;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        noises = new CinemachineBasicMultiChannelPerlin[cameras.Length];
        for (int i = 0; i < noises.Length; i++)
        {
            noises[i] = cameras[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }


        ShakeCamera(0, 0.1F);
    }

    IEnumerator DelayCameraInheritPosition()
    {
        yield return new WaitForFixedUpdate();
        gameplayCamera.m_Transitions.m_InheritPosition = true;
    }

    void Noise(float amplitude)
    {
        gameplayCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
    }


    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            {
                for (int i = 0; i < noises.Length; i++)
                {
                    noises[i].m_AmplitudeGain = Mathf.Lerp(startIntensity, 0f, (1 - (shakeTimer / startTimer)));

                }
                Noise(Mathf.Lerp(startIntensity, 0f, (1 - (shakeTimer / startTimer))));

            }
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        startIntensity = intensity;
        shakeTimer = time * 0.16667F;
        startTimer = time * 0.16667F;
    }
}