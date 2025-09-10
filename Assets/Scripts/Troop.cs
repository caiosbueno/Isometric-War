using System;
using UnityEngine;

public enum Team
{
    Player,
    Enemy
}

public class Troop : MonoBehaviour
{
    public GameManager GameManager;
    
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float attackSpeed = 1f;
    public int damage = 10;
    public int health = 100;

    [Header("Team Settings")]
    public Team team;

    [Header("Animation")]
    public Animator animator;
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    [Header("Ranged Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Transform target;
    private float attackCooldown = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (!GameManager.gameIsRunning)
        {
            return;
        }
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;

        if (target == null)
        {
            FindTarget();
            PlayIdle();
            return;
        }

        Troop targetTroop = target.GetComponent<Troop>();
        if (targetTroop == null || targetTroop.health <= 0)
        {
            target = null;
            PlayIdle();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            Attack(targetTroop);
        }
    }

    void FindTarget()
    {
        Troop[] allTroops = FindObjectsOfType<Troop>();
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Troop t in allTroops)
        {
            if (t == this) continue;
            if (t.health <= 0) continue;
            if (t.team == this.team) continue;

            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = t.transform;
            }
        }

        target = closest;
    }

    void MoveTowardsTarget()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        PlayWalk();
    }

    void Attack(Troop enemy)
    {
        if (attackCooldown <= 0f)
        {
            PlayAttack();

            if (attackRange <= 2f)
            {
                // Melee attack
                enemy.TakeDamage(damage);
            }
            else
            {
                // Ranged attack: spawn projectile
                if (projectilePrefab != null && firePoint != null)
                {
                    GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                    Projectile p = proj.GetComponent<Projectile>();
                    if (p != null)
                        p.Init(enemy, damage);
                }
            }

            attackCooldown = 1f / attackSpeed;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    // --- Animations ---
    void PlayIdle()
    {
        if (animator != null) animator.CrossFade(IdleHash, 0.1f);
    }

    void PlayWalk()
    {
        if (animator != null) animator.CrossFade(WalkHash, 0.1f);
    }

    void PlayAttack()
    {
        if (animator != null)
        {
            animator.speed = attackSpeed;  
            animator.CrossFade(AttackHash, 0.05f);
            Invoke(nameof(ResetAnimatorSpeed), 1f / attackSpeed);
        }
    }

    void ResetAnimatorSpeed()
    {
        if (animator != null)
            animator.speed = 1f;
    }
}
