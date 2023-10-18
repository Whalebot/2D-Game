using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance
    {
        get; private set;
    }
    public GameObject defaultHitVFX;
    public GameObject defaultBlockVFX;
    public GameObject counterHitVFX;
    public GameObject defaultHitSFX;
    public GameObject defaultBlockSFX;
    public GameObject counterHitSFX;
    public GameObject exMoveVFX;

    public GameObject defaultProjectileVFX;
    public GameObject defaultProjectileSFX;

    public GameObject recoveryFX;
    public GameObject wakeupFX;

    public GameObject throwFX;
    public GameObject throwSFX;

    public GameObject throwBreakVFX;
    public GameObject throwBreakSFX;

    public List<ParticleObject> particles;
    public List<ParticleObject> deletedParticles;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.advanceGameState += AdvanceParticles;
    }
    [Button]
    public void AdvanceParticles()
    {
        deletedParticles.Clear();
        foreach (var item in particles)
        {
            if (item.ps == null)
            {
                deletedParticles.Add(item);
                continue;
                //Delete shit
            }

            float tempTime = 0;
            tempTime = (GameManager.Instance.gameFrameCount - item.startFrame) * Time.fixedDeltaTime;
            item.ps.Simulate(Time.fixedDeltaTime, true, false, true);
            int currentFrame = (int)((item.ps.main.duration + item.ps.main.startLifetimeMultiplier) / Time.fixedDeltaTime);
            //Debug.Log($"{item.ps.time} {item.ps.main.duration + item.ps.main.startLifetimeMultiplier}");
            if (GameManager.Instance.gameFrameCount >= item.startFrame + currentFrame)
            {
                deletedParticles.Add(item);
                //Delete shit
            }
            //if (item.ps.time >= item.ps.main.duration + item.ps.main.startLifetimeMultiplier)
            //{
            //    deletedParticles.Add(item);
            //    //Delete shit
            //}

        }
        for (int i = 0; i < deletedParticles.Count; i++)
        {
            particles.Remove(deletedParticles[deletedParticles.Count - i - 1]);
            if (deletedParticles[deletedParticles.Count - i - 1].ps != null)
            {


                foreach (Transform item in deletedParticles[deletedParticles.Count - i - 1].ps.transform)
                {
                    item.SetParent(null);
                }
                Destroy(deletedParticles[deletedParticles.Count - i - 1].ps.gameObject);
            }
        }
    }

    public void DestroyParticles(Status status)
    {
        deletedParticles.Clear();
        foreach (var item in particles)
        {
            if (item.status == status)
                deletedParticles.Add(item);
        }

        for (int i = 0; i < deletedParticles.Count; i++)
        {
            particles.Remove(deletedParticles[deletedParticles.Count - i - 1]);
            if (deletedParticles[deletedParticles.Count - i - 1].ps != null)
            {


                foreach (Transform item in deletedParticles[deletedParticles.Count - i - 1].ps.transform)
                {
                    item.SetParent(null);
                }
                Destroy(deletedParticles[deletedParticles.Count - i - 1].ps.gameObject);
            }
        }
    }

    public void RevertParticles()
    {
        foreach (var item in particles)
        {

            float tempTime = 0;
            tempTime = (GameManager.Instance.gameFrameCount - item.startFrame) * Time.fixedDeltaTime;
            item.ps.Simulate(tempTime, true, true, true);
        }
    }

    public void AddParticle(ParticleSystem ps, int ID, Status status = null)
    {

        ParticleObject p = new ParticleObject(ps, GameManager.Instance.gameFrameCount, status);
        ps.Simulate(Time.fixedDeltaTime, true, false, true);
        ps.Pause();
        particles.Add(p);
    }
}

[System.Serializable]
public class ParticleObject
{
    public ParticleObject(ParticleSystem p, int i, Status s = null)
    {
        ps = p;
        startFrame = i;
        status = s;
    }
    public ParticleSystem ps;
    public int startFrame;
    public Status status = null;
}