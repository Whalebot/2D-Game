using UnityEngine;

public class CharacterSFX : MonoBehaviour
{
    public Status status;
    Movement mov;

    [HeaderAttribute("Attack Sounds")]
    public GameObject[] attackSFX;
    [HeaderAttribute("Hurt Sounds")]
    public GameObject hurtSFX;

    [HeaderAttribute("Death Sounds")]
    public GameObject deathSFX;
    [HeaderAttribute("Jump")]
    public GameObject jump;
    public GameObject land;
    [HeaderAttribute("Rolling")]
    public GameObject roll;
    [HeaderAttribute("Footsteps")]
    public GameObject footstep;
    [HeaderAttribute("Roar")]
    public GameObject roarSFX;

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponentInParent<Status>();
        mov = GetComponentInParent<Movement>();
        status.hitstunEvent += HurtSFX;
        status.deathEvent += Death;
        if (mov != null)
            mov.landEvent += Land;
    }

    void Land()
    {
        if (land != null) Instantiate(land, transform.position, Quaternion.identity);
    }

    void Death()
    {
        if (deathSFX != null) Instantiate(deathSFX, transform.position, Quaternion.identity);
    }


    public void AttackSFX(int ID)
    {
        if (attackSFX.Length <= 0) return;
        if (attackSFX.Length > ID)
        {
            if (attackSFX[ID] != null)
            {
                GameObject GO = Instantiate(attackSFX[ID], transform.position, Quaternion.identity);
            }
            else if (attackSFX[0] != null)
            {
                GameObject GO = Instantiate(attackSFX[0], transform.position, Quaternion.identity);
            }
        }

        else if (attackSFX[0] != null)
        {
            GameObject GO = Instantiate(attackSFX[0], transform.position, Quaternion.identity);
        }
    }

    public void HurtSFX()
    {
        if (hurtSFX != null)
            Instantiate(hurtSFX, transform.position, Quaternion.identity);
    }



    public void Step(AnimationEvent evt)
    {
        Instantiate(footstep, transform.position, Quaternion.identity);
    }

    public void Roll()
    {
        Instantiate(roll, transform.position, Quaternion.identity);
    }


    void Roar(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5F)
        {
            Instantiate(roarSFX, transform.position, Quaternion.identity);

        }
    }
}
