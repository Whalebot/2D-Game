using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : Projectile
{
    public GameObject projectileToSpawn;
    public int projectilesSpawned = 1;
    public float projectileSpreadAngle = 15f;
    public float projectileSpacing = 1f;

    private void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
        SpawnProjectiles();
    }

    public void SpawnProjectiles()
    {
        for (int i = 0; i < projectilesSpawned; i++)
        {
            Vector3 startRotation = transform.rotation.eulerAngles;
            Quaternion projectileRotation;
            float spawnOffset = 0;

            if (projectilesSpawned % 2 == 1)
            {
                float offset = i * projectileSpreadAngle - (int)(projectilesSpawned / 2f) * projectileSpreadAngle;
                spawnOffset = i * projectileSpacing - (int)(projectilesSpawned / 2f) * projectileSpacing;
                startRotation.z -= offset;
                projectileRotation = Quaternion.AngleAxis(-offset, Vector3.forward);
            }
            else
            {
                float offset = i * projectileSpreadAngle - (int)(projectilesSpawned / 2f) * projectileSpreadAngle + 0.5f * projectileSpreadAngle;
                spawnOffset = i * projectileSpacing - (int)(projectilesSpawned / 2f) * projectileSpacing + 0.5f * projectileSpacing;
                startRotation.z -= offset;
                projectileRotation = Quaternion.AngleAxis(-offset, transform.right);
            }

            //  GameObject go = Instantiate(projectileToSpawn, transform.position + Quaternion.Euler(startRotation) * transform.forward * projectileSpacing, Quaternion.Euler(startRotation));
            Vector3 spawnPosition = transform.position + projectileRotation * transform.forward * projectileSpacing;
            GameObject go = Instantiate(projectileToSpawn, spawnPosition, Quaternion.identity);
            go.transform.forward = (spawnPosition - transform.position).normalized;
            //go.transform.parent = null;

            Projectile proj = go.GetComponent<Projectile>();
            proj.SetupHitbox(hitboxID, attack, status, move);
            //go.transform.localScale = Vector3.one ;
        }
    }
}
