using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody rb;
    private Animator anim;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector3 moveDirection;

    [Header("Combat Settings")]
    public float attackCooldown = 0.5f;
    public float strongAttackCooldown = 1.5f;
    private bool canAttack = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isDashing)
        {
            HandleMovement();
        }
        HandleDash();
        HandleAttack();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);

            transform.forward = moveDirection;

            anim.SetBool("isMoving", true);
            anim.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift));
        }
        else
        {
            anim.SetBool("isMoving", false);
            anim.SetBool("isRunning", false);
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rb.velocity = moveDirection * dashSpeed;
        anim.SetTrigger("Dash");

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack("Attack"));
        }
        else if (Input.GetMouseButtonDown(1) && canAttack)
        {
            StartCoroutine(Attack("StrongAttack"));
        }
    }

    IEnumerator Attack(string attackType)
    {
        canAttack = false;
        anim.SetTrigger(attackType);

        yield return new WaitForSeconds(attackType == "Attack" ? attackCooldown : strongAttackCooldown);
        canAttack = true;
    }
}
