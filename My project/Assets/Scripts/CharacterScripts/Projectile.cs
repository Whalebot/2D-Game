using UnityEngine;
using Sirenix.OdinInspector;
public class Projectile : Hitbox
{
    public enum ProjectileMovement
    {
        Linear, Homing, SpawnOnTarget, Teleport
    }



    [TabGroup("Debug")] public Transform target;

    [TabGroup("Debug")] public Vector3 targetPosition;
    bool isDestroying;
    bool delayDestroy;
    [TabGroup("Settings")] public int life;
    [TabGroup("Settings")] public int lifetime;

    [Header("Movement")]
    [TabGroup("Settings")] public ProjectileMovement projectileMovement;
    [TabGroup("Settings")] public float velocity;
    [TabGroup("Settings")] public float maxVelocity = 50f;
    [TabGroup("Settings")] public Vector3 rotationVector;
    [TabGroup("Settings")] public bool onStartVelocity;
    [TabGroup("Settings")] public bool updateVelocty;
    [TabGroup("Settings")] public float updateVelocity;

    [Header("Homing")]
    [TabGroup("Settings")] public float rotateSpeed;
    [TabGroup("Settings")] public bool waitForTarget;

    [Header("Re-Aim")]
    [TabGroup("Unique")] public bool willDelayReaim;
    [TabGroup("Unique")] public int projectileDelay;
    [TabGroup("Unique")] public float delayVelocity;

    [Header("Projectiles")]
    [TabGroup("Unique")] public GameObject projectile;
    [TabGroup("Unique")] public int projectileTimer = 15;
    [TabGroup("Unique")] public int maxProjectiles = 0;
    int projectileCounter;
    int projectilesShot;

    [TabGroup("Settings")] public LayerMask searchMask;
    [TabGroup("Settings")] public float searchSize;

    [TabGroup("Unique")]
    [TabGroup("Unique")] public Vector3 startOffset;
    [TabGroup("Unique")] public bool followCaster;
    [TabGroup("Unique")] public bool keepParent;
    [TabGroup("Unique")] public float minimumFollowDistance;
    [Header("Explosion")] public GameObject explosion;
    [TabGroup("Unique")] public bool onlyExplosionDamage;

    [TabGroup("Unique")] public GameObject explosionVFX;
    [TabGroup("Unique")] public GameObject explosionSFX;

    [Header("Collision")]
    [TabGroup("Settings")] public bool destroyOnCollission = true;
    [TabGroup("Settings")] public bool destroyOnProjectileClash = true;
    [TabGroup("Settings")] public bool destroyOnHitboxClash = true;
    [TabGroup("Settings")] public bool destroyOnHit;



    [TabGroup("Unique")] public bool willAttract;
    [TabGroup("Unique")] public Vector2 attractPower;


    [HideIf("@projectileMovement != ProjectileMovement.Teleport")] public LayerMask teleportMask;
    [HideIf("@projectileMovement != ProjectileMovement.Teleport")] public bool airTeleport;
    [HideIf("@projectileMovement != ProjectileMovement.Teleport")] public float teleportYRange = 5f;
    [HideIf("@projectileMovement != ProjectileMovement.Teleport")] public float teleportYOffset = 1f;
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

        if (status != null)
        {
            if (status.alignment == Alignment.Enemy && projectileMovement != ProjectileMovement.Homing)
                target = GameManager.Instance.player;
            else  //Look for enemy
                FindTarget();
        }
        else
        {
            //Look for enemy
            FindTarget();
        }

        startOffset = transform.position - status.transform.position;

        if (onStartVelocity)
            rb.velocity = transform.forward * velocity;

        if (projectileMovement == ProjectileMovement.Teleport)
        {
            Teleport();
        }
    }
    protected void OnDestroy()
    {
        GameManager.Instance.advanceGameState -= FramePassed;
        GameManager.Instance.advanceGameState -= ExecuteFrame;
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
    public bool ClearLine(Transform t)
    {
        RaycastHit hit;
        bool clearLine = Physics.Raycast(transform.position, (t.position - transform.position).normalized, out hit, 1000, searchMask);
        Debug.DrawLine(transform.position, hit.point, Color.yellow);

        if (clearLine)
            return hit.transform.IsChildOf(t);
        else
            return false;
    }
    public void FindTarget()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, searchSize * 0.5F, searchMask);
        float closestDistance = 100;
        foreach (Collider item in col)
        {
            Status tempStatus = item.GetComponentInParent<Status>();
            if (tempStatus == null || tempStatus == status) continue;
            if (tempStatus.alignment == status.alignment) continue;
            if (ClearLine(tempStatus.transform))
            {
                Vector2 v = tempStatus.transform.position - transform.position;
                float dist = v.x + (v.y * v.y);
                if (dist < closestDistance)
                {
                    target = tempStatus.transform;
                }
            }
        }

        if (target != null)
        {
            if (keepParent)
                transform.SetParent(null);

            if (projectileMovement == ProjectileMovement.SpawnOnTarget)
                transform.position = target.position;

            targetPosition = target.position + Vector3.up * 0.5F;

            if (followCaster)
                rb.velocity = Vector3.zero;
        }
    }
    public virtual void ExecuteFrame()
    {

        if (lifetime > 0)
        {
            lifetime--;
            if (lifetime <= 0) DestroyProjectile();
        }


        MultiHitProperty();
        ShootProjectiles();
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

    void ShootProjectiles()
    {
        if (projectile == null) return;

        projectileCounter--;
        if (projectileCounter <= 0 && projectilesShot < maxProjectiles)
        {
            projectilesShot++;
            projectileCounter = projectileTimer;
            GameObject proj = Instantiate(projectile, transform.position, transform.rotation, transform);
            Hitbox hitbox = proj.GetComponent<Hitbox>();

            hitbox.SetupHitbox(hitboxID, attack, status, move);

            proj.transform.localPosition = projectile.transform.localPosition;
            proj.transform.localRotation = projectile.transform.rotation;
            proj.transform.SetParent(null);
        }
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
                Rigidbody rb = tempStatus.GetComponent<Rigidbody>();
                rb.velocity += dir / rb.mass;
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

    public virtual void Movement()
    {
        transform.Rotate(rotationVector, Space.Self);

        switch (projectileMovement)
        {
            case ProjectileMovement.Linear:
                if (updateVelocty)
                {
                    rb.velocity += transform.forward * updateVelocity;
                }
                else
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
                else if (followCaster)
                {
                    float dist = Vector3.Distance(status.transform.position + startOffset, transform.position);
                    //if (dist > minimumFollowDistance)
                    rb.velocity = ((status.transform.position + startOffset) - transform.position).normalized * velocity * dist;

                }
                else if (!waitForTarget)
                    rb.velocity += transform.forward * updateVelocity;
                break;
            default:
                break;
        }

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
    }

    new void OnTriggerEnter(Collider other)
    {
        if (delayDestroy) return;
        bool foundTarget = false;
        bool ignoreCollision = false;
        colPos = other.gameObject.transform;
        Projectile proj = other.GetComponentInParent<Projectile>();
        Hitbox hitbox = other.GetComponent<Hitbox>();
        Hazard hazard = other.GetComponentInParent<Hazard>();

        if (proj != null || hitbox != null)
        {
            foundTarget = true;
        }

        if (hazard != null)
        {
            ignoreCollision = true;

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
            if (willDelayReaim && projectileDelay > 0) return;
            if (status == enemyStatus) return;
            if (status.alignment == enemyStatus.alignment) return;

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
            if (destroyOnCollission && !ignoreCollision)
            {
                Debug.Log(ignoreCollision);
                DestroyProjectile();
            }
        }
    }

    public override void DoDamage(Status other, float dmgMod)
    {
        if (!hit && !onlyExplosionDamage)
        {
            base.DoDamage(other, dmgMod);
            //CheckAttack(other,)
            //      hit = true;
        }

        if (destroyOnHit)
            DestroyProjectile();
    }
}