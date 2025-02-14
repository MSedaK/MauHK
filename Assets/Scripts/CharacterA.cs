using System.Collections;
using UnityEngine;

public class CharacterA: MonoBehaviour
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

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.2f);

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * attackRange, 1f, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                int damage = baseAttackDamage;
                if (System.Array.Exists(preferredEnemies, tag => enemy.CompareTag(tag)))
                {
                    damage += extraDamageToPreferredEnemies;
                }
                if (System.Array.Exists(weaknessEnemies, tag => enemy.CompareTag(tag)))
                {
                    damage -= extraDamageFromWeaknessEnemies;
                }
                enemyScript.TakeDamage(damage);
                enemyKillCount++;
                if (enemyKillCount >= upgradeThreshold)
                {
                    UpgradeAttack();
                    enemyKillCount = 0;
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void UpgradeAttack()
    {
        baseAttackDamage += upgradeDamageBoost;
        extraDamageToPreferredEnemies += upgradeDamageBoost;
        Debug.Log("Character A upgraded!");
    }
}