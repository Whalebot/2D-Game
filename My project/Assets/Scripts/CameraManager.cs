using Cinemachine;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [TabGroup("Components")] public Camera overlayCamera;
    [TabGroup("Components")] public CinemachineConfiner confiner;
    [TabGroup("Components")] public CinemachineVirtualCamera[] cameras;
    [TabGroup("Components")] public Movement mov;
    [TabGroup("Components")] public CinemachineFreeLook freeLookCam;

    [TabGroup("Components")] CinemachineBasicMultiChannelPerlin[] noises;
    [TabGroup("Settings")] public LayerMask defaultLayerMask;
    [TabGroup("Settings")] public LayerMask cutsceneLayerMask;

    [TabGroup("Settings")] [SerializeField] private float shakeTimer;
    [TabGroup("Settings")] private float startTimer;
    [TabGroup("Settings")] private float startIntensity;
    [TabGroup("Settings")] public bool toggle;
    [TabGroup("Settings")] public float xMult, yMult;
    [TabGroup("Settings")] public float camLerp;
    [TabGroup("Components")] public CinemachineVirtualCamera lockOnCam;
    [TabGroup("Components")] public CinemachineVirtualCamera lockOnCam2;


    [TabGroup("Components")] public CinemachineTargetGroup targetGroup;
    [TabGroup("Components")] public CinemachineTargetGroup targetGroup2;
    [TabGroup("Components")] public CinemachineVirtualCamera groupCamera;

    [TabGroup("Components")] public Transform defaultTarget;

    [TabGroup("Components")] CinemachineComposer topComposer, middleComposer, bottomComposer;

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


        freeLookCam.m_XAxis.Value = 0;
        freeLookCam.m_YAxis.Value = 0.5f;

        //freeLookCam.m_YAxisRecentering.RecenterNow();
        //freeLookCam.m_RecenterToTargetHeading.RecenterNow();

        ShakeCamera(0, 0.1F);


        topComposer = freeLookCam.GetRig(0).GetCinemachineComponent<CinemachineComposer>();
        middleComposer = freeLookCam.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        bottomComposer = freeLookCam.GetRig(2).GetCinemachineComponent<CinemachineComposer>();
    }

    IEnumerator DelayCameraInheritPosition()
    {
        yield return new WaitForFixedUpdate();
        freeLookCam.m_Transitions.m_InheritPosition = true;
    }

    void Noise(float amplitude)
    {

        freeLookCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        freeLookCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        freeLookCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
    }

    public void SetLockOnTarget(Transform temp)
    {
        toggle = !toggle;
        StartCoroutine(DelayCamSwitch(temp));
    }

    IEnumerator DelayCamSwitch(Transform temp)
    {

        lockOnCam.LookAt = targetGroup.transform;
        lockOnCam2.LookAt = targetGroup2.transform;


        //lockOnCam.Priority = 15;
        //targetGroup.AddMember(temp, 0.5F, 3F);
        //if (targetGroup.m_Targets.Length > 2)
        //{
        //    Transform oldTarget = targetGroup.m_Targets[1].target;
        //    if (oldTarget != null)
        //    {
        //        while (targetGroup.m_Targets[1].weight > 0)
        //        {
        //            targetGroup.m_Targets[1].weight -= 0.01F;
        //            yield return new WaitForFixedUpdate();
        //        }
        //        targetGroup.RemoveMember(oldTarget);
        //    }
        //}

        SetGroupTarget(temp);

        if (toggle)
        {
            targetGroup.m_Targets[1].target = temp;
            yield return new WaitForFixedUpdate();
            lockOnCam.Priority = 15;
            lockOnCam2.Priority = 14;
        }
        else
        {
            targetGroup2.m_Targets[1].target = temp;
            yield return new WaitForFixedUpdate();
            lockOnCam.Priority = 14;
            lockOnCam2.Priority = 15;
        }
    }

    public void DisableLockOn()
    {
        lockOnCam.Priority = 10;
        lockOnCam2.Priority = 10;
        targetGroup.m_Targets[1].target = null;
        targetGroup2.m_Targets[1].target = null;
    }

    public void SetGroupTarget(Transform temp)
    {

        shakeTimer = 0;
        for (int i = 0; i < noises.Length; i++)
            noises[i].m_AmplitudeGain = 0;
    }

    public void SetGroupTarget()
    {
        groupCamera.gameObject.SetActive(true);
        shakeTimer = 0;
        for (int i = 0; i < noises.Length; i++)
            noises[i].m_AmplitudeGain = 0;
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


        if (GameManager.isPaused || GameManager.menuOpen)
        {
            freeLookCam.m_XAxis.m_InputAxisValue = 0;
            freeLookCam.m_YAxis.m_InputAxisValue = 0;
            LoadData();
            return;

        }
        freeLookCam.m_XAxis.m_InputAxisValue = InputManager.Instance.lookDirection.x * xMult;
        freeLookCam.m_YAxis.m_InputAxisValue = InputManager.Instance.lookDirection.y * yMult;

        if (!mov.ground)
        {
            topComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.55f, camLerp);
            middleComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.45F, camLerp);
            bottomComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.50f, camLerp);
            freeLookCam.m_Orbits[0].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[0].m_Radius, 4, camLerp);
            freeLookCam.m_Orbits[1].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[1].m_Radius, 6, camLerp);
            freeLookCam.m_Orbits[2].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[2].m_Radius, 4, camLerp);
        }
        else
        {
            topComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.65f, camLerp);
            middleComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.55F, camLerp);
            bottomComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.60f, camLerp);
            freeLookCam.m_Orbits[0].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[0].m_Radius, 3, camLerp);
            freeLookCam.m_Orbits[1].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[1].m_Radius, 5, camLerp);
            freeLookCam.m_Orbits[2].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[2].m_Radius, 3, camLerp);
        }
    }

    void LoadData()
    {
        //xMult = DataManager.Instance.currentSaveData.settings.cameraX / (float)50;
        //yMult = DataManager.Instance.currentSaveData.settings.cameraY / (float)50;
    }

    public void ShakeCamera(float intensity, float time)
    {
        startIntensity = intensity;
        shakeTimer = time * 0.16667F;
        startTimer = time * 0.16667F;
    }
}