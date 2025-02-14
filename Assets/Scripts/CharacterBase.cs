using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterBase : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isRunning = false;
    private bool isDashing = false;
    private float dashTime = 0f;
    private float nextAttackTime = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleDash();
        HandleAttack();
    }

    void HandleMovement()
    {
        if (isDashing) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        animator.SetFloat("Speed", move.magnitude * (isRunning ? 2f : 1f));

        if (!controller.isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -2f;
        }

        controller.Move(velocity * Time.deltaTime);

        // Zýplama
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocity.y = jumpForce;
            animator.SetTrigger("Jump");
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        animator.SetTrigger("Dash"); // Eðer dash animasyonu varsa tetikle

        Vector3 dashDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) dashDirection += transform.forward;  // Ýleri
        if (Input.GetKey(KeyCode.S)) dashDirection -= transform.forward;  // Geri
        if (Input.GetKey(KeyCode.D)) dashDirection += transform.right;    // Saða
        if (Input.GetKey(KeyCode.A)) dashDirection -= transform.right;    // Sola

        if (dashDirection == Vector3.zero) dashDirection = transform.forward; // Eðer yön girilmezse ileri dash atsýn

        dashDirection.Normalize(); // Vektörü normalize et, hýz sabit kalsýn

        float dashTime = 0f;
        float dashSpeedBoost = 20f; // Dash hýzýný artýrmak için eklenen deðer

        // Karakterin yerçekiminden etkilenmesini engelle
        float originalGravity = gravity;
        gravity = 0f;

        while (dashTime < dashDuration)
        {
            controller.Move(dashDirection * (dashSpeed + dashSpeedBoost) * Time.deltaTime);
            dashTime += Time.deltaTime;
            yield return null;
        }

        // Dash bitince yerçekimini geri getir
        gravity = originalGravity;

        isDashing = false;
    }



    void HandleAttack()
    {
        if (Time.time < nextAttackTime) return;

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("LightAttack");
            nextAttackTime = Time.time + attackCooldown;
        }
        else if (Input.GetMouseButtonDown(1)) 
        {
            animator.SetTrigger("HeavyAttack");
            nextAttackTime = Time.time + attackCooldown * 1.5f;
        }
    }
}
