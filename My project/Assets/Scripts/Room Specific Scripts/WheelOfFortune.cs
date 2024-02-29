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
    public GameObject blockade;
    public AudioSource clickSound;
    // Start is called before the first frame update
    private void Awake()
    {
        dummy.damageEvent += SpinWheel;
    }

    void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        LevelManager.Instance.spawnLevelGates += RemoveBlockade;

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

    void RemoveBlockade()
    {
        blockade.SetActive(false);
        gameObject.SetActive(false);
    }

    [Button]
    void GiveReward()
    {
        Status status = GameManager.Instance.playerStatus;
        int damage = 10;
        switch (reward)
        {
            case WheelRewards.item:
                SkillManager.Instance.RollItem(Rank.B);
                GameManager.Instance.OpenGetSkillWindow(RewardType.Item);
                break;
            case WheelRewards.bad:
                damage = 5;
                status.Health -= damage;
                GameManager.Instance.DamageNumbers(status.transform, damage, false, status.alignment);
                LevelManager.Instance.SpawnLevelGates();
                break;
            case WheelRewards.gold:
                GameManager.Instance.Gold += 150;
                LevelManager.Instance.SpawnLevelGates();
                break;
            case WheelRewards.veryBad:
                damage = 10;
                status.Health -= damage;
                GameManager.Instance.DamageNumbers(status.transform, damage, false, status.alignment);
                LevelManager.Instance.SpawnLevelGates();
                break;
            case WheelRewards.potential:
                SkillManager.Instance.RollActiveSkill(Rank.B);
                GameManager.Instance.OpenGetSkillWindow(RewardType.Skill);
                break;
            case WheelRewards.superBad:
                damage = (int)(status.Health * 0.25F);
                status.Health -= damage;
                GameManager.Instance.DamageNumbers(status.transform, damage, false, status.alignment);
                LevelManager.Instance.SpawnLevelGates();
                break;
            case WheelRewards.skill:
                SkillManager.Instance.RollBlessing(Rank.B);
                GameManager.Instance.OpenGetSkillWindow(RewardType.Blessing);
                break;
            case WheelRewards.ultraBad:
                damage = (int)(status.Health * 0.5F);
                status.Health -= damage;
                GameManager.Instance.DamageNumbers(status.transform, damage, false, status.alignment);
                LevelManager.Instance.SpawnLevelGates();
                break;
            default:
                damage = 5;
                status.Health -= damage;
                GameManager.Instance.DamageNumbers(status.transform, damage, false, status.alignment);
                LevelManager.Instance.SpawnLevelGates();
                break;
        }
    }
}
