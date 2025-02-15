using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody rb;
    private Animator anim;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector3 moveDirection;
    private Vector3 lastMouseDirection = Vector3.forward;

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
        RotateTowardsMouse();

        if (!isDashing)
        {
            HandleMovement();
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }

        HandleAttack();
    }

    void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y;

            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction.magnitude > 0.1f)
            {
                lastMouseDirection = direction;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forwardDirection = lastMouseDirection;

        moveDirection = (forwardDirection * moveZ + Vector3.Cross(Vector3.up, forwardDirection) * moveX).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            rb.velocity = moveDirection * speed + new Vector3(0, rb.velocity.y, 0);
            anim.SetBool("isMoving", true);
            anim.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift));
        }
        else
        {
            anim.SetBool("isMoving", false);
            anim.SetBool("isRunning", false);
        }
    }

    IEnumerator Dash()
    {
        if (!canDash) yield break;

        canDash = false;
        isDashing = true;

        Vector3 dashDirection = moveDirection.magnitude > 0 ? moveDirection : lastMouseDirection;

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        float decelerationTime = 0.1f;  
        float t = 0;
        Vector3 initialVelocity = rb.velocity;

        while (t < 1)
        {
            t += Time.deltaTime / decelerationTime;
            rb.velocity = Vector3.Lerp(initialVelocity, moveDirection * walkSpeed, t);
            yield return null;
        }

        isDashing = false;

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

    public void ResetState()
    {
        isDashing = false;
        canDash = true;
        moveDirection = Vector3.zero;
        lastMouseDirection = Vector3.forward;

        // Animator ve hareket deðiþkenlerini de resetle
        if (anim != null)
        {
            anim.SetBool("isMoving", false);
            anim.SetBool("isRunning", false);
        }

        // Rigidbody hýzýný sýfýrla
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

}