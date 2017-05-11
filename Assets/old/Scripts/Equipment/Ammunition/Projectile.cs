using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float duration;
    private float speed;
    private int damage;

    public void Initialize(float projectileSpeed, float projectileDuration, int projectileDamage)
    {
        speed = projectileSpeed;
        duration = projectileDuration;
        damage = projectileDamage;
    }

    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
            KillSelf();

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void KillSelf()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IReceivesProjectileHits>() != null)
        {
            other.GetComponent<IReceivesProjectileHits>().ReceiveHit(damage);
            KillSelf();
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }
}
