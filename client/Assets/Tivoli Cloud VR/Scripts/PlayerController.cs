using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputActions _inputActions;

    private Rigidbody _rigidbody;
    public Transform cameraBoom;

    private bool _mouseLocked;

    public void Awake()
    {
        _inputActions = new InputActions();
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public void Start()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Move.Enable();
        _inputActions.Player.Look.Enable();
        _inputActions.Player.Look.performed += OnLook;
        _inputActions.Player.ActivateLook.Enable();
    }

    public void OnDestroy()
    {
        _inputActions.Disable();
    }

    public void FixedUpdate()
    {
        var moveXy = _inputActions.Player.Move.ReadValue<Vector2>();
        var positionOffset = Quaternion.Euler(0, transform.localEulerAngles.y, 0) *
                             new Vector3(moveXy.x, 0, moveXy.y);
        _rigidbody.MovePosition(transform.position + positionOffset * 0.1f);
    }

    private void LockMouse()
    {
        if (_mouseLocked) return;
        Cursor.lockState = CursorLockMode.Locked;
        _mouseLocked = true;
    }

    private void UnlockMouse()
    {
        if (!_mouseLocked) return;
        Cursor.lockState = CursorLockMode.None;
        _mouseLocked = false;
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        var activateLook = _inputActions.Player.ActivateLook.ReadValue<float>() > 0.5f;
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

        var lookDelta = _inputActions.Player.Look.ReadValue<Vector2>();
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