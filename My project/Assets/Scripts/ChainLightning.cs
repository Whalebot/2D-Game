using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ChainLightning : MonoBehaviour
{
    public int damage;
    public VFX vfx;
    public Status status;
    public LineRenderer lineRenderer;
    public Vector3 offset;
    public float searchSize;
    public LayerMask searchMask;
    public LayerMask blockMask;
    public int chains = 2;
    public int chainCount = 0;
    public int chainDelay = 5;
    int chainDelayCounter;
    public List<Status> enemyList;
    public int lifetime = 10;
    int lifeCounter = 0;
    private void Awake()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
    }

    private void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
    }
    void ExecuteFrame()
    {
        chainDelayCounter++;
        if (chainDelayCounter >= chainDelay)
        {
            if (chainCount < chains)
            {
                chainDelayCounter = 0;
                Vector3 fromPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
                Collider[] col = Physics.OverlapSphere(fromPosition, searchSize * 0.5F, searchMask);
                float closestDistance = 100;

                Status targetStatus = null;
                foreach (Collider item in col)
                {
                    Status tempStatus = item.GetComponentInParent<Status>();
                    if (tempStatus == null || tempStatus == status || tempStatus.alignment != status.alignment || enemyList.Contains(tempStatus)) continue;
                    if (ClearLine(fromPosition, tempStatus.transform))
                    {
                        enemyList.Add(tempStatus);

                        Debug.Log("si");
                        Vector2 v = tempStatus.transform.position - fromPosition;
                        float dist = v.x + (v.y * v.y);


                        if (dist < closestDistance)
                        {
                            targetStatus = tempStatus;

                        }
                    }
                }

                if (targetStatus != null)
                {
                    chainCount++;
                    if (chainCount < chains)
                    {
                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(chainCount, targetStatus.transform.position + offset);
                        DoLightningDamage(targetStatus, fromPosition);
                    }
                }
            }
        }

        lifeCounter++;
        if (lifeCounter >= lifetime)
            Destroy(gameObject);
    }

    public void StartChainLightning(Status tempStatus)
    {
        status = tempStatus;
        ConnectLines();
    }

    [Button]
    public void ConnectLines()
    {
        chainCount = 0;
        enemyList.Clear();
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, status.transform.position + offset);
        DoLightningDamage(status, status.transform.position);
    }

    void DoLightningDamage(Status tempStatus, Vector3 fromPosition)
    {
        if (vfx.prefab != null)
        {
            GameObject VFX = Instantiate(vfx.prefab, tempStatus.transform.position, tempStatus.transform.rotation);
            VFX.transform.localScale = vfx.scale;
        }

        Vector3 dir = (tempStatus.transform.position - fromPosition).normalized;
        tempStatus.Health -= damage;
        //No hitstun
        //tempStatus.TakeHit(damage, Vector3.zero, CombatManager.Instance.lvl1.stun, CombatManager.Instance.lvl1.poiseBreak, Vector3.zero, CombatManager.Instance.lvl1.hitstop, HitState.None, false);
    }
    public bool ClearLine(Vector3 pos, Transform t)
    {

        RaycastHit hit;
        bool clearLine = Physics.Raycast(pos + offset, (t.position - pos).normalized, out hit, 1000, blockMask);

        if (clearLine)
            return hit.transform.IsChildOf(t);
        else
            return false;
    }
}
