using UnityEngine;

public class VFXScript : MonoBehaviour
{
    [HideInInspector] public int ID = 0;
    ParticleSystem[] ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponentsInChildren<ParticleSystem>();
        //if (!GameHandler.Instance.runNormally) ps.Pause();
        foreach (var item in ps)
        {
            VFXManager.Instance.AddParticle(item, ID);
        }
    }
    void FixedUpdate() {
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
