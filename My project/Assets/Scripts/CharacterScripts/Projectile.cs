using UnityEngine;
using Sirenix.OdinInspector;
public class Projectile : Hitbox
{
    public enum ProjectileMovement
    {
        Linear, Homing, SpawnOnTarget, Teleport
    }

    [TabGroup("Settings")] public ProjectileMovement projectileMovement;
    [TabGroup("Settings")] public Vector3 targetPosition;
    [TabGroup("Settings")] public Transform target;

    bool isDestroying;
    bool delayDestroy;
    [TabGroup("Settings")] public int life;
    [TabGroup("Settings")] public int lifetime;
    [TabGroup("Settings")] public float velocity;

    [TabGroup("Settings")] public bool willDelayReaim;
    [TabGroup("Settings")] public int projectileDelay;
    [TabGroup("Settings")] public float delayVelocity;

    [TabGroup("Settings")] public float updateVelocity;
    [TabGroup("Settings")] public float rotateSpeed;
    [TabGroup("Settings")] public bool onStartVelocity;
    [TabGroup("Settings")] public bool updateVelocty;

    [TabGroup("Settings")] public LayerMask searchMask;

    [TabGroup("Settings")] public float searchSize;


    [TabGroup("Settings")] public GameObject explosion;
    [TabGroup("Settings")] public bool onlyExplosionDamage;
    [TabGroup("Settings")] public bool destroyOnCollission = true;
    [TabGroup("Settings")] public bool destroyOnProjectileClash = true;
    [TabGroup("Settings")] public bool destroyOnHitboxClash = true;
    [TabGroup("Settings")] public bool destroyOnHit;
    [TabGroup("Components")] public GameObject explosionVFX;
    [TabGroup("Components")] public GameObject explosionSFX;

    [TabGroup("Attraction")] public bool willAttract;
    [TabGroup("Attraction")] public Vector2 attractPower;
    [TabGroup("Attraction")] public int resetTimer = 0;
    int resetCounter;
    [TabGroup("Attraction")] public GameObject col;

    [TabGroup("Teleport")] public LayerMask teleportMask;
    [TabGroup("Teleport")] public bool airTeleport;
    [TabGroup("Teleport")] public float teleportYRange = 5f;
    [TabGroup("Teleport")] public float teleportYOffset = 1f;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool hit;
    RaycastHit teleportHit;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        body = transform;

    }

    private void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        resetCounter = resetTimer;

        if (status.alignment == Alignment.Enemy)
            target = GameManager.Instance.player;
        else
        {
            //Look for enemy
            FindTarget();
        }

        if (onStartVelocity)
            rb.velocity = transform.forward * velocity;

        if (projectileMovement == ProjectileMovement.Teleport)
        {
            Teleport();
        }
    }
    void Teleport()
    {
        Collider[] col = Physics.OverlapSphere(transform.position + Vector3.up * (0.5F), 0.005F, teleportMask);

        int i = 1;

        if (col.Length > 0)
        {
            i++;
        }

        bool foundHit = Physics.Raycast(transform.position + Vector3.up * teleportYOffset * i, Vector3.down, out teleportHit, teleportYRange * i, teleportMask);

        Collider[] col2 = Physics.OverlapSphere(transform.position + Vector3.up * (teleportYOffset * i + 0.2F), 0.005F, teleportMask);

        if (airTeleport && col.Length <= 0)
        {
            status.transform.position = transform.position;
        }
        else if (foundHit && col2.Length <= 0)
        {
            transform.position = teleportHit.point;
            status.transform.position = transform.position;
        }
        else
        {
            if (life > 0)
            {
                life--;
                transform.position -= transform.forward * 0.5F;
                Teleport();
            }
            else
            {
                transform.position = status.transform.position;
                status.transform.position = transform.position;
            }
        }
    }

    public void FindTarget()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, searchSize * 0.5F, searchMask);
        float closestDistance = 100;
        foreach (Collider item in col)
        {
            Status tempStatus = item.GetComponentInParent<Status>();
            if (tempStatus == null || tempStatus == status) continue;

            RaycastHit hit;
            bool clearLine = Physics.Raycast(transform.position, (tempStatus.transform.position - transform.position).normalized, out hit, 1000, searchMask);
            Debug.DrawLine(transform.position, hit.point, Color.yellow);

            if (clearLine)
            {
                float dist = Vector2.Distance(tempStatus.transform.position, transform.position);
                if (dist < closestDistance)
                {
                    target = tempStatus.transform;
                }
            }
        }

        if (target != null)
        {
            if (projectileMovement == ProjectileMovement.SpawnOnTarget)
                transform.position = target.position;

            targetPosition = target.position + Vector3.up * 0.5F;
        }
    }
    public virtual void ExecuteFrame()
    {
        if (lifetime > 0)
        {
            lifetime--;
            if (lifetime <= 0) DestroyProjectile();
        }

        if (resetTimer > 0)
        {
            if (enemyList.Count > 0)
            {
                resetCounter--;
                if (resetCounter <= 0)
                {
                    col.gameObject.SetActive(false);
                    enemyList.Clear();
                    resetCounter = resetTimer;
                }
            }
            else
            {
                col.gameObject.SetActive(true);
            }
        }

        AttractBehaviour();

        if (target != null)
        {
            targetPosition = target.position + Vector3.up * 0.5F;
        }

        if (projectileDelay > 0)
        {
            projectileDelay--;
            if (projectileDelay <= 0 && willDelayReaim)
            {
                if (target == null)
                    FindTarget();

                transform.LookAt(targetPosition);
                rb.velocity = transform.forward * delayVelocity;
            }
            return;
        }

        if (projectileMovement == ProjectileMovement.Homing && target == null)
        {
            FindTarget();
        }
        Movement();
    }

    void AttractBehaviour()
    {
        if (willAttract)
        {
            Collider[] col = Physics.OverlapSphere(transform.position, searchSize * 0.5F, searchMask);
            foreach (Collider item in col)
            {
                Status tempStatus = item.GetComponentInParent<Status>();
                if (tempStatus == null || tempStatus == status) continue;
                Vector3 dir = (transform.position - item.transform.position).normalized * attractPower;
                if (tempStatus.GetComponent<Movement>().ground)
                    dir.y = 0;
                tempStatus.GetComponent<Rigidbody>().velocity += dir;
            }
        }
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
            GO.transform.parent = null;
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
                if (target != null && !updateVelocty)
                {
                    Vector2 direction = (targetPosition - transform.position).normalized;

                    Vector3 rotateAmount = Vector3.Cross(direction, transform.forward) * Vector3.Angle(transform.forward, direction);

                    rb.angularVelocity = -rotateAmount * rotateSpeed;
                    //rb.velocity += transform.forward * updateVelocity;
                    rb.velocity += (targetPosition - transform.position).normalized * updateVelocity;
                }
                else
                    rb.velocity += transform.forward * updateVelocity;
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
        if (!foundTarget)
        {
            // Found Collission
            if (destroyOnCollission)
            {
                DestroyProjectile();
            }
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
