using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class WheelOfFortune : MonoBehaviour
{
    public GameObject wheel;
    public bool hasSpun = false;
    public float spinSpeed;
    public float currentSpeed;
    public float slowdownSpeed;
    public float stoppingSpeed;
    public float currentValue;
    public enum WheelRewards { item, bad, gold, veryBad, potential, superBad, skill, ultraBad }
    public WheelRewards reward;
    public Status dummy;

    public AudioSource clickSound;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        dummy.damageEvent += SpinWheel;
    }

    void ExecuteFrame()
    {

        if (currentSpeed > stoppingSpeed)
        {
            currentSpeed *= slowdownSpeed;
            wheel.transform.Rotate(transform.forward, -currentSpeed);
            if (currentSpeed <= stoppingSpeed)
            {
                GiveReward();
            }
        }
        currentValue = wheel.transform.rotation.eulerAngles.z;
        ChangeReward((WheelRewards)((int)(currentValue / 45f) % 8));
    }

    void ChangeReward(WheelRewards value)
    {
        if (value == reward) return;
        reward = value;
        clickSound.Play();
    }

    [Button]
    void SpinWheel(int value)
    {
        if (!hasSpun)
        {
            hasSpun = true;
            spinSpeed = value;
            currentSpeed = spinSpeed;
        }
    }

    [Button]
    void GiveReward()
    {

    }
}
