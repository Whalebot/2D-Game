using UnityEngine;

public class Projectile : Hitbox
{
    public GameObject explosionVFX;
    public GameObject explosionSFX;

    public bool destroyOnBlock;
    public bool destroyOnHit;
    bool isDestroying;
    bool delayDestroy;
    public int life;
    public int lifetime;
    public float velocity;
    public bool onStartVelocity;
    public bool updateVelocty;

    public bool destroyOnCollission = true;
    public bool destroyOnProjectileClash = true;
    public bool destroyOnHitboxClash = true;

    public float opacity = 0.1F;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool hit;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        body = transform;

    }

    private void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        if (onStartVelocity)
            rb.velocity = transform.forward * velocity;
    }


    public virtual void ExecuteFrame()
    {
        if (lifetime > 0)
        {
            lifetime--;
            if (lifetime <= 0) DestroyProjectile();
        }

        Movement();
    }

    public void DestroyProjectile()
    {
        if (!delayDestroy)
            GameManager.Instance.advanceGameState += FramePassed;

        delayDestroy = true;
        //Hit FX
        //if (explosionVFX != null)
        //    Instantiate(explosionVFX, transform.position, transform.rotation);
        //else
        //    Instantiate(VFXManager.Instance.defaultProjectileVFX, transform.position, transform.rotation);

        //if (explosionSFX != null)
        //    Instantiate(explosionSFX, transform.position, transform.rotation);
        //else
        //    Instantiate(VFXManager.Instance.defaultProjectileSFX, transform.position, transform.rotation);
        //Hit FX
        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, transform.rotation);
        else
            Instantiate(VFXManager.Instance.defaultProjectileVFX, transform.position, transform.rotation);

        if (explosionSFX != null)
            Instantiate(explosionSFX, transform.position, transform.rotation);
        else
            Instantiate(VFXManager.Instance.defaultProjectileSFX, transform.position, transform.rotation);
    }

    void FramePassed()
    {
        if (isDestroying)
            Destroy(gameObject);
        isDestroying = true;

    }

    private void OnEnable()
    {

    }

    protected void OnDestroy()
    {
        GameManager.Instance.advanceGameState -= FramePassed;
        GameManager.Instance.advanceGameState -= ExecuteFrame;
    }

    public virtual void Movement()
    {
        if (updateVelocty)
            rb.velocity = transform.forward * velocity;
    }

    new void OnTriggerEnter(Collider other)
    {
        if (delayDestroy) return;
        colPos = other.gameObject.transform;
        Projectile proj = other.GetComponentInParent<Projectile>();
        if (proj != null && destroyOnProjectileClash && proj.status != status)
        {
            life--;

            DestroyProjectile();
            return;
        }

        Hitbox hitbox = other.GetComponent<Hitbox>();
        if (hitbox != null && destroyOnHitboxClash && hitbox.status != status)
        {
            life--;
            DestroyProjectile();
            return;
        }


        Status enemyStatus = other.GetComponentInParent<Status>();

        if (enemyStatus != null && hitbox == null)
        {
            if (status == enemyStatus) return;

            if (!enemyList.Contains(enemyStatus))
            {
                if (enemyStatus.invincible) return;
                else if (enemyStatus.projectileInvul) return;

                enemyList.Add(enemyStatus);
                DoDamage(enemyStatus, 1);
                return;
            }
        }

        if (destroyOnCollission)
        {

            DestroyProjectile();
        }
    }

    public override void DoDamage(Status other, float dmgMod)
    {
        if (!hit)
            base.DoDamage(other, dmgMod);
        hit = true;
        DestroyProjectile();
    }
}
