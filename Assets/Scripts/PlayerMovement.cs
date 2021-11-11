using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //camera tilting help from https://www.youtube.com/watch?v=USWcQF3KKo8

    public CharacterController controller;

    public static PlayerMovement instance;

    private Rigidbody rb;

    public float defaultSpeed = 5f;
    public float speed = 5f;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHight = 1.5f;

    public float time;
    public float maxTime;
    public bool pressedJump;

    [SerializeField] private float dashForce = 50f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float timeBetweenDashes = 1f;
    private bool canDash = true;

    Vector3 velocity;
    public bool isGrounded;


    //for crouching
    float originalHeight;
    public float reducedHeight;
    public bool isCrouching;

    //random sprint values
    public bool isSprinting;

    //health stuff
    public int playerHealth;
    public TextMeshProUGUI healthText;

    //camera tilt
    public Camera cam;
    public float camTilt;
    public float camTiltTime;
    public float tilt { get; private set; }

    //sidestepping code
    float mass = 3.0F; // defines the character mass
    Vector3 impact = Vector3.zero;
    public bool canSideStep;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalHeight = controller.height;
        isCrouching = false;
        isSprinting = false;
        canSideStep = true;
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

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        //camera tilting script
        if (x > 0)
        {
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        }
        else if (x < 0)
        {
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
        }

        if (x == 0)
        {
            tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
        }

        //sidestepping
        if (Input.GetKeyDown(KeyCode.LeftAlt) && canSideStep == true)
        {
            AddImpact(move, 200f);
            canSideStep = false;
            StartCoroutine(enableSide());
        }


        //Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
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

        healthText.text = "Health: " + playerHealth;

        //crouching
        if (Input.GetKeyDown(KeyCode.LeftControl) && isSprinting == false)
        {
            crouch();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) && isSprinting == false)
        {
            GetUp();
        }

        //sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift) && isCrouching == false)
        {
            speed = 5;
            isSprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && isCrouching == false)
        {
            speed = 3;
            isSprinting = false;
        }

        //sidestepping logic
        if (impact.magnitude > 0.2F) controller.Move(impact * Time.deltaTime);
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }


    void crouch()
    {
        controller.height = reducedHeight;
        isCrouching = true;
    }

    void GetUp()
    {
        controller.height = originalHeight;
        isCrouching = false;
    }

    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    public IEnumerator enableSide(){
        yield return new WaitForSeconds(2);
        canSideStep = true;
    }
}
