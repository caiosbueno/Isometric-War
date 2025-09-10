using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private int damage;
    private Troop target;

    public void Init(Troop targetTroop, int dmg)
    {
        target = targetTroop;
        damage = dmg;
    }

    void Update()
    {
        if (target == null || target.health <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards target
        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Check if close enough to hit
        if (Vector3.Distance(transform.position, target.transform.position) < 0.3f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
