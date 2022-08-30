using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnUsernameChanged))]
    private string username;
    
    public Transform nametag;
    
    private Rigidbody rigidbody;
    public Transform cameraBoom;
    
    private InputActions inputActions;

    private bool mouseLocked;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        inputActions = new InputActions();
        inputActions.Player.Enable();
        inputActions.Player.Move.Enable();
        inputActions.Player.Look.Enable();
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.ActivateLook.Enable();

        Camera.main.transform.parent = cameraBoom;
        Camera.main.transform.localPosition = new Vector3(0f, 0f, -2f);
        cameraBoom.transform.eulerAngles = new Vector3(10f, 0f, 0f);
        
        CmdSetupPlayer(SystemInfo.deviceName);
    }

    public override void OnStopLocalPlayer()
    {
        inputActions.Disable();
        
        Camera.main.transform.parent = null;
    }

    [Command]
    private void CmdSetupPlayer(string _username)
    {
        username = _username;
    }
    
    private void OnUsernameChanged(string _old, string _new)
    {
        if (isLocalPlayer) return;
        nametag.GetComponentInChildren<TextMeshPro>().text = _new;
    }
    
    private void Update()
    {
        if (!isLocalPlayer)
        {
            nametag.transform.LookAt(Camera.main.transform);
        }
    }
    
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        
        var moveXy = inputActions.Player.Move.ReadValue<Vector2>();
        var positionOffset = Quaternion.Euler(0, transform.localEulerAngles.y, 0) *
                             new Vector3(moveXy.x, 0, moveXy.y);
        rigidbody.MovePosition(transform.position + positionOffset * 0.1f);
    }
    
    private void LockMouse()
    {
        if (mouseLocked) return;
        Cursor.lockState = CursorLockMode.Locked;
        mouseLocked = true;
    }
    
    private void UnlockMouse()
    {
        if (!mouseLocked) return;
        Cursor.lockState = CursorLockMode.None;
        mouseLocked = false;
    }
    
    private void OnLook(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        
        var activateLook = inputActions.Player.ActivateLook.ReadValue<float>() > 0.5f;
        if (activateLook)
        {
            LockMouse();
        }
        else
        {
            UnlockMouse();
            return;
        }
        
        const float sensitivity = 0.25f;
    
        var lookDelta = inputActions.Player.Look.ReadValue<Vector2>();
        transform.localEulerAngles += new Vector3(0f, lookDelta.x * sensitivity, 0f);
    
        var newCameraBoom = cameraBoom.localEulerAngles + new Vector3(-lookDelta.y * sensitivity, 0f, 0f);
    
        // 90deg
        // --.
        //    | 0deg
        //    | 360deg
        // --`
        // 270deg
        if (newCameraBoom.x is > 90 and < 180) newCameraBoom.x = 90;
        if (newCameraBoom.x is < 270 and > 180) newCameraBoom.x = 270;
    
        cameraBoom.localEulerAngles = newCameraBoom;
    }
}