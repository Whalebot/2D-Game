using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonProperty", menuName = "ScriptableObjects/SkillProperty/HitboxProperty")]
public class HitboxMoveProperty : UniqueSkillProperty
{
    public Move move;
    public Vector3 offset;
    public MoveGroup affectedMoves;
    public bool destroyOnRecovery;
    List<GameObject> hitboxes = new List<GameObject>();
    public override void OnStartupFrame(AttackScript atk, int frame, SkillSO skill = null)
    {
        base.OnStartupFrame(atk, frame, skill);

        if (affectedMoves != null)
            if (!affectedMoves.moves.Contains(atk.activeMove))
                return;

        Status status = atk.GetComponent<Status>();
        SpawnProjectile(atk.transform.position + atk.transform.forward * offset.z + atk.transform.right * offset.x + atk.transform.up * offset.y, atk.transform.rotation, atk, status, atk.hitboxContainer);
    }

    public override void OnTimer(SkillHandler handler)
    {
        AttackScript atk = handler.GetComponent<AttackScript>();
        Status status = handler.GetComponent<Status>();
        SpawnProjectile(handler.transform.position + handler.transform.forward * offset.z + handler.transform.right * offset.x + handler.transform.up * offset.y, handler.transform.rotation, atk, status, atk.hitboxContainer);
    }

    public override void OnRecoveryBehaviour()
    {
        base.OnRecoveryBehaviour();

        for (int i = 0; i < hitboxes.Count; i++) {
            Destroy(hitboxes[i]);
        }
        hitboxes.Clear();
    }

    void SpawnProjectile(Vector3 position, Quaternion rotation, AttackScript attack, Status status, Transform container)
    {
       

        for (int i = 0; i < move.attacks.Length; i++)
        {

            if (move.attacks[i].hitbox != null)
            {

            
                hitboxes.Add(Instantiate(move.attacks[i].hitbox, position, rotation, container));
                hitboxes[i].transform.localPosition = move.attacks[i].hitbox.transform.localPosition;
                hitboxes[i].transform.localRotation = move.attacks[i].hitbox.transform.rotation;

                Hitbox hitbox = hitboxes[i].GetComponentInParent<Hitbox>();

                if (move.attacks[i].attackType == AttackType.Projectile)
                {
                    Projectile projectile = hitboxes[i].GetComponentInParent<Projectile>();

                    if (!projectile.keepParent)
                        hitboxes[i].transform.SetParent(null);

                    if (projectile != null) projectile.SetupHitbox(i, attack, status, move);
                }
                else
                if (hitbox != null) hitbox.SetupHitbox(i, attack, status, move);
            }
        }

    }
}

