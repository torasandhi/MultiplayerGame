using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController _controller;
    private Controls _controls;

    private Vector2 _moveInput;
    private Vector3 _velocity;
    private float _playerSpeed = 5f;
    private float _rotationSpeed = 720f;
    private float _gravity = -9.81f;
    private float _groundCheckDistance = 0.2f;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        // Get the controls instance from your singleton
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController singleton not found! Make sure it's in your scene.");
            return;
        }

        _controls = PlayerController.Instance.GetCurrentInput();

        if (_controls != null)
        {
            _controls.Player.Move.performed += OnMovePerformed;
            _controls.Player.Move.canceled += OnMoveCanceled;
        }
        else
        {
            Debug.LogError("Controls object is null! Check PlayerController.cs");
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    void Update()
    {

        Vector3 moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        Vector3 horizontalVelocity = moveDirection * _playerSpeed;

        if (_controller.isGrounded)
        {
            if (_velocity.y < 0f)
                _velocity.y = -2f; 
        }
        else
        {
            _velocity.y += _gravity * Time.deltaTime;
        }

        Vector3 finalVelocity = horizontalVelocity + _velocity;

        _controller.Move(finalVelocity * Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (_controls != null)
        {
            _controls.Player.Move.performed -= OnMovePerformed;
            _controls.Player.Move.canceled -= OnMoveCanceled;
        }
    }
}
