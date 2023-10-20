using UnityEngine;
using Sirenix.OdinInspector;
public class Projectile : Hitbox
{
    public enum ProjectileMovement
    {
        Linear, Homing
    }

    public ProjectileMovement projectileMovement;
    public Transform target;

    bool isDestroying;
    bool delayDestroy;
    [TabGroup("Settings")] public int life;
    [TabGroup("Settings")] public int lifetime;
    [TabGroup("Settings")] public float velocity;
    [TabGroup("Settings")] public float updateVelocity;
    [TabGroup("Settings")] public float rotateSpeed;
    [TabGroup("Settings")] public bool onStartVelocity;
    [TabGroup("Settings")] public bool updateVelocty;

    [TabGroup("Settings")] public GameObject explosion;
    [TabGroup("Settings")] public bool onlyExplosionDamage;
    [TabGroup("Settings")] public bool destroyOnCollission = true;
    [TabGroup("Settings")] public bool destroyOnProjectileClash = true;
    [TabGroup("Settings")] public bool destroyOnHitboxClash = true;
    [TabGroup("Settings")] public bool destroyOnHit;
    [TabGroup("Components")] public GameObject explosionVFX;
    [TabGroup("Components")] public GameObject explosionSFX;

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

        if (status.alignment == Alignment.Enemy)
            target = GameManager.Instance.player;

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

        if (explosion != null)
        {
            GameObject GO = Instantiate(explosion, transform.position, transform.rotation, transform);
            GO.transform.localPosition = explosion.transform.localPosition;
            Projectile proj = GO.GetComponent<Projectile>();
            proj.status = status;
            proj.attack = attack;
            proj.move = move;
            proj.hitboxID = hitboxID;
        }

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
        switch (projectileMovement)
        {
            case ProjectileMovement.Linear:
                if (updateVelocty)
                    rb.velocity = transform.forward * velocity;
                break;
            case ProjectileMovement.Homing:
                if (target != null)
                {
                    Vector2 direction = (target.position - transform.position).normalized;

                    Vector3 rotateAmount = Vector3.Cross(direction, transform.forward) * Vector3.Angle(transform.forward, direction);

                    rb.angularVelocity = -rotateAmount * rotateSpeed;

                    //rb.velocity += transform.forward * updateVelocity;
                    rb.velocity += (target.position - transform.position).normalized * updateVelocity;
                }
                break;
            default:
                break;
        }

    }

    new void OnTriggerEnter(Collider other)
    {
        if (delayDestroy) return;
        bool foundTarget = false;
        colPos = other.gameObject.transform;
        Projectile proj = other.GetComponentInParent<Projectile>();
        Hitbox hitbox = other.GetComponent<Hitbox>();

        if (proj != null || hitbox != null)
        {
            foundTarget = true;
        }

        if (proj != null && destroyOnProjectileClash && proj.status != status)
        {
            life--;
         
            DestroyProjectile();
            return;
        }


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
                foundTarget = true;
                enemyList.Add(enemyStatus);
                DoDamage(enemyStatus, 1);
                return;
            }
        }

        if (destroyOnCollission && !foundTarget)
        {
            DestroyProjectile();
        }
    }

    public override void DoDamage(Status other, float dmgMod)
    {
        if (!hit && !onlyExplosionDamage)
            base.DoDamage(other, dmgMod);
        //      hit = true;
        if (destroyOnHit)
            DestroyProjectile();
    }
}
