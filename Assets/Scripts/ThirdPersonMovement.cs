using JetBrains.Annotations;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float gravity = -9.82f * 2;
    public float jumpHeight = 3f; 

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 fallVelocity;

    float turnSmoothVelocity;

    public Animator animator;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            if (fallVelocity.y < 0)
            {
                fallVelocity.y = -2f;
            }

            if (isGrounded && Input.GetButton("Jump"))
            {
                fallVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // jump
            }
        }
        else
        {
            fallVelocity.y += gravity * Time.deltaTime; // apply gravity
        }

        // Horizontal Movement //
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(horizontal != 0 || vertical != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

            Vector3 moveVelocity = Vector3.zero;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            moveVelocity = moveDir.normalized * speed;

        }

        moveVelocity.y = fallVelocity.y;

        // Apply the combined horizontal and vertical movement in a single call
        controller.Move(moveVelocity * Time.deltaTime);

        Debug.Log("Grounded: " + isGrounded + " | FallVelY: " + fallVelocity.y);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
