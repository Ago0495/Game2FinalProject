using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

public class NetworkPlayerController : NetworkComponent
{
    [SerializeField] private Rigidbody MyRig;
    private GameObject camera;
    private Transform camearHolderPos;

    [SerializeField] private PlayerInput MyInput;
    [SerializeField] private InputActionAsset MyMap;

    [SerializeField] private float speed;
    [SerializeField] private bool canJump = true;
    public float lookSpeed = 2f;
    private Vector2 lastInput;
    private Vector2 lookInput;
    private float yaw;
    private float pitch;
    private Vector3 movingPlatform;

    public Vector2 Vector2FromString(string s)
    {
        //"(X,Y)"
        string[] args = s.Trim().Trim('(').Trim(')').Split(',');

        return new Vector2(
            float.Parse(args[0]),
            float.Parse(args[1])
            );
    }

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "MOVE")
        {
            if (IsServer)
            {
                lastInput = Vector2FromString(value);
            }
        }
        if (flag == "JUMP")
        {
            if (IsServer && canJump)
            {
                canJump = false;
                MyRig.linearVelocity += new Vector3(0, 10, 0);
            }
        }
        if (flag == "ROTATE")
        {
            if (IsServer)
            {
                yaw = float.Parse(value);
                transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }
    }

    public override void NetworkedStart()
    {
        if (IsLocalPlayer)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera");
            camearHolderPos = transform.GetChild(0).transform;
        }
    }

    public void OnDirectionChanged(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            if (context.started || context.performed)
            {
                SendCommand("MOVE", context.ReadValue<Vector2>().ToString());
            }
            if (context.canceled)
            {
                SendCommand("MOVE", Vector2.zero.ToString());
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SendCommand("JUMP", context.ReadValue<float>().ToString());
        }
    }

    public void OnLook(InputAction.CallbackContext lk)
    {
        lookInput = lk.ReadValue<Vector2>();
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            MyRig.linearVelocity = transform.forward * speed * lastInput.y + transform.right * speed * lastInput.x + new Vector3(0, MyRig.linearVelocity.y, 0) + movingPlatform;
        }

        if (IsLocalPlayer && camearHolderPos != null && camera != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            camera.transform.position = camearHolderPos.transform.position;
            RotateView();
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (IsServer)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (collision.contacts[i].point.y < transform.position.y)
                {
                    canJump = true;

                    if (collision.contacts[i].otherCollider.GetComponent<Rigidbody>() != null)
                    {
                        Rigidbody TempRB = collision.contacts[i].otherCollider.GetComponent<Rigidbody>();
                        movingPlatform = TempRB.linearVelocity;
                    }
                }
            }
        }
    }
    private void RotateView()
    {
        yaw += lookInput.x * lookSpeed;
        pitch -= lookInput.y * lookSpeed;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        //transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        SendCommand("ROTATE", yaw.ToString());
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
