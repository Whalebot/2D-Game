using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffProperty", menuName = "ScriptableObjects/BuffProperty")]
public class BuffMoveProperty : UniqueSkillProperty
{
    public bool onSelf;
    public bool onAllies;
    public List<StatusEffect> appliedEffects;
    public override void OnStartupFrame(AttackScript atk, int frame, SkillSO skill = null)
    {
        base.OnStartupFrame(atk, frame);

        if (onAllies)
        {
            List<Status> allyStatuses = new List<Status>();
            foreach (var item in AIManager.Instance.allEnemies)
            {
                if (item.TryGetComponent(out Status tempStatus))
                {
                    if (tempStatus != atk.GetComponent<Status>())
                        allyStatuses.Add(tempStatus);
                }
            }
            foreach (var allyStatus in allyStatuses)
            {
                foreach (var item in appliedEffects)
                {
                    StatusEffect clone = Instantiate(item);
                    if (skill == null)
                        allyStatus.ApplyStatusEffect(clone, null, Rank.D);
                    else
                        allyStatus.ApplyStatusEffect(clone, null, skill.skillRank);
                }
            }
        }
        else
            foreach (var item in appliedEffects)
            {
                SkillHandler skillHandler = atk.GetComponent<SkillHandler>();
                StatusEffect clone = Instantiate(item);
                Status status = atk.GetComponent<Status>();
                clone = skillHandler.ModifyStatusEffect(clone);
                if (skill == null)
                    status.ApplyStatusEffect(clone, null, Rank.D);
                else
                    status.ApplyStatusEffect(clone, null, skill.skillRank);
            }
    }
}
