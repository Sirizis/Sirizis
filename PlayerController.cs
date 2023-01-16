using UnityEngine;
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]
    float verticalVelocity;
    public float playerSpeed = 2.0f;
    public float playerSpeedAtWalk = 2.0f;
    public float playerSpeedAtSprint = 5.0f;

    [SerializeField] private float gravity = 0.7f;
    [SerializeField] private float cameraSpeed = 10f;
    [SerializeField] private float jumpForce;

    [Header("Required Components")]
    [SerializeField] private CharacterController cc;

    [Header("Scripts")]
    [SerializeField] private Stamina stamina;

    [HideInInspector] public Vector3 euler;
    [HideInInspector] public Vector3 movin;

    private bool ifCanGetUp;

    void Start()
    {
        euler = transform.localEulerAngles;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Movement();
    }

    private void LateUpdate()
    {
        Camlook();
    }

    public void Camlook()
    {
        euler.x -= Input.GetAxis("Mouse Y") * cameraSpeed;
        euler.x = Mathf.Clamp(euler.x, -80.0f, 70.0f);
        euler.y += Input.GetAxis("Mouse X") * cameraSpeed;
        transform.localEulerAngles = euler;
    }

    public void Movement()
    {
        //-----Movement-----
        movin = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movin = transform.TransformDirection(movin);
        movin *= playerSpeed;
        cc.Move(movin * Time.deltaTime);
        cc.Move(Vector3.down * gravity);
        //-----Duck-----
        if (Input.GetKey(KeyCode.LeftControl))
        {
            cc.height = 1.5f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) | ifCanGetUp)
        {
            Ray ray = new Ray(transform.position, Vector3.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.distance <= 1.25f)
                {
                    ifCanGetUp = true;
                }
                else
                {
                    ifCanGetUp = false;
                    cc.height = 2.0f;
                }
            }
            else
            {
                ifCanGetUp = false;
                cc.height = 2.0f;
            }
        }
        //-----Sprint-----
        if (Input.GetKey(KeyCode.LeftShift) && !stamina.isFullRegening)
        {
            if (movin.x == 0.0f || movin.y == 0.0f)
                return;
            stamina.Sprint(stamina.barSpeedAtStandart);
            stamina.isStopSprinting = false;
            playerSpeed = playerSpeedAtSprint;
        }
        //-----Jump-----
        if (cc.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = jumpForce;
            }
            else
            {
                verticalVelocity = 0;
            }
        }
        else
        {
            verticalVelocity = 0;
        }

        Vector3 jumpVector = new Vector3(0, verticalVelocity, 0);
        cc.Move(jumpVector * Time.deltaTime);
    }
}