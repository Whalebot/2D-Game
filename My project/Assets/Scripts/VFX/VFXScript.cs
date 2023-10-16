using UnityEngine;

public class VFXScript : MonoBehaviour
{
    [HideInInspector] public int ID = 0;
    ParticleSystem[] ps;
    bool added = false;
    // Start is called before the first frame update
    void Start()
    {

        if (!added)
        {
            ps = GetComponentsInChildren<ParticleSystem>();
            foreach (var item in ps)
            {
                VFXManager.Instance.AddParticle(item, ID);
            }
        }


    }
    public void SetupVFX(Status status = null)
    {
        ps = GetComponentsInChildren<ParticleSystem>();
        added = true;
        foreach (var item in ps)
        {
            VFXManager.Instance.AddParticle(item, ID, status);
        }
    }

    void FixedUpdate()
    {
        bool found = false;
        foreach (var item in ps)
        {
            if (item != null)
                found = true;
        }

        if (!found)
            Destroy(gameObject);
    }
}
