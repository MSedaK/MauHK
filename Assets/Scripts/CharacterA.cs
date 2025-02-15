using System.Collections;
using UnityEngine;

public class CharacterA : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public int baseAttackDamage = 20;
    public int extraDamageToPreferredEnemies = 10;
    public int extraDamageFromWeaknessEnemies = 5;
    public LayerMask enemyLayer;
    public float attackCooldown = 0.5f;
    private bool canAttack = true;
    private Animator anim;
    private int enemyKillCount = 0;
    public int upgradeThreshold = 10;
    public int upgradeDamageBoost = 5;
    public string[] preferredEnemies;
    public string[] weaknessEnemies;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        yield return new WaitForSeconds(0.2f);

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * attackRange, 1f, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            HandleEnemyDamage(enemy.gameObject, baseAttackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        Vector3 dashDirection = transform.forward;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            DealDashDamage();  
            yield return null;
        }

        isDashing = false;
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void DealDashDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 1.5f, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            HandleEnemyDamage(enemy.gameObject, 50); 
        }
    }

    void HandleEnemyDamage(GameObject enemyObject, int damage)
    {
        Enemy1 enemy1 = enemyObject.GetComponent<Enemy1>();
        if (enemy1 != null)
        {
            enemy1.TakeDamage(damage);
            RegisterKill();
            return;
        }

        Enemy2 enemy2 = enemyObject.GetComponent<Enemy2>();
        if (enemy2 != null)
        {
            enemy2.TakeDamage(damage);
            RegisterKill();
        }
    }

    void RegisterKill()
    {
        enemyKillCount++;
        if (enemyKillCount >= upgradeThreshold)
        {
            UpgradeAttack();
            enemyKillCount = 0;
        }
    }

    void UpgradeAttack()
    {
        baseAttackDamage += upgradeDamageBoost;
        extraDamageToPreferredEnemies += upgradeDamageBoost;
        Debug.Log("Character A upgraded!");
    }
}
