using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolType1 : Gun
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    [Header("Config")]
    private float projectileSpeed = 5.0f;
    private float projectileDuration = 3.0f;
    private int projectileDamage = 10;
    private List<GameObject> projectiles = new List<GameObject>();

    /// <summary>
    /// 
    /// </summary>
    protected override void Shoot()
    {
        base.Shoot();

        GameObject proj = Instantiate(projectilePrefab);
        proj.transform.position = projectileSpawnPoint.position;
        proj.transform.rotation = Quaternion.Euler(projectileSpawnPoint.TransformDirection(projectileSpawnPoint.forward));
        projectiles.Add(proj);
        proj.GetComponent<Projectile>().Initialize(projectileSpeed, projectileDuration, projectileDamage);

        ammoRemaining--;
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void ResetObject()
    {
        base.ResetObject();

        foreach (GameObject g in projectiles)
        {
            if (g != null)
            {
                Destroy(g);
            }
        }

        projectiles.Clear();
    }
}
