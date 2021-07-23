using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public UnityEngine.CharacterController controller;

    public static PlayerMovement instance;

    private Rigidbody rb;

    public float defaultSpeed = 12f;
    public float speed = 12f;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHight = 3f;

    public float time;
    public float maxTime;
    public bool pressedJump;

    [SerializeField] private float dashForce = 50f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float timeBetweenDashes = 1f;
    private bool canDash = true;

    Vector3 velocity;
    public bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x * speed + transform.forward * z * speed + transform.up * velocity.y;

        controller.Move(move * Time.deltaTime);
  

        //Jump
        if (Input.GetButtonDown("Jump") && isGrounded && pressedJump == false)
        {
            velocity.y = Mathf.Sqrt(jumpHight * -2f * gravity);
            pressedJump = true;
        }

        if (pressedJump == false && isGrounded == false)
        {
            time += Time.deltaTime;
            if (time <= maxTime)
            {
                if (Input.GetButtonDown("Jump") && pressedJump == false)
                {
                    velocity.y = Mathf.Sqrt(jumpHight * -2f * gravity);
                    pressedJump = true;
                    time = 0;
                }
                else if (time >= maxTime)
                {
                    pressedJump = true;
                    time = 0;
                }
            }
        }

        controller.Move(velocity * Time.deltaTime);

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash == true && x > 0f || Input.GetKeyDown(KeyCode.LeftShift) && canDash == true && x < 0f || Input.GetKeyDown(KeyCode.LeftShift) && canDash == true && z > 0f || Input.GetKeyDown(KeyCode.LeftShift) && canDash == true && z < 0f)
        {
            canDash = false;
            StartCoroutine(Dash());
            StartCoroutine(DashRecharge());
        }
    }

    public IEnumerator Dash()
    {
        //rb.AddForce(Camera.main.transform.forward * dashForce, ForceMode.VelocityChange);

        //rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        velocity.y = 0f;

        speed = dashForce;

        yield return new WaitForSeconds(dashDuration);

        speed = defaultSpeed;

        velocity.y = 0f;
        //rb.velocity = Vector3.zero;
    }

    public IEnumerator DashRecharge()
    {
        yield return new WaitForSeconds(timeBetweenDashes);
        canDash = true;
    }
}
