using PurrNet;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class Player : NetworkBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private Transform firePos;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private NetworkAnimator animator;
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private Material allyMaterial;
    [SerializeField] private Material enemyMaterial;

    [Header("Player Settings")]
    [SerializeField] private SyncVar<float> health = new(100);
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private int selfLayer, otherLayer;


    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    private ProjectileShooter _shooter;
    private CharacterController _controller;
    private Controls _controls;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector3 _velocity;
    private float _xRotation = 0f;
    private bool _jumpInput;

    public Action<Player> OnDeath_Server;

    public float Health => health.value;
    public Camera PlayerCamera => playerCamera;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _shooter = GetComponent<ProjectileShooter>();

    }


    protected override void OnSpawned()
    {
        base.OnSpawned();
        enabled = isOwner;

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(isOwner);

        // Lock cursor for FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InitializeControls();
        InitializeRenderer();

        health.onChanged += OnHealthChange;

        if (!isOwner) return;
        OnHealthChange(health.value);
    }

    private void InitializeControls()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController singleton not found!");
            return;
        }

        _controls = PlayerController.Instance.GetCurrentInput();

        if (_controls != null)
        {
            _controls.Player.Move.performed += OnMovePerformed;
            _controls.Player.Move.canceled += OnMoveCanceled;
            _controls.Player.Attack.performed += OnAttackPerformed;
            _controls.Player.Look.performed += OnLookPerformed;
            _controls.Player.Look.canceled += OnLookCanceled;
            _controls.Player.Jump.performed += OnJumpPerformed;
            _controls.Player.Jump.canceled += OnJumpCanceled;
        }
    }

    private void InitializeRenderer()
    {
        foreach (var renderer in renderers)
        {
            if (isOwner)
            {
                renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                renderer.material = allyMaterial;
            }
            else
            {

                renderer.material = enemyMaterial;
            }
        }

        //set layer
        var actualLayer = isOwner ? selfLayer : otherLayer;
        SetLayer(gameObject, actualLayer);
    }

    private void OnHealthChange(float newHealth)
    {
        if (!isOwner) return;

        Transform ui = UIManager.Instance.GetUI("ui-gameplay");
        Image healthbar = ui.Find("healthbar").gameObject.GetComponent<Image>();

        healthbar.fillAmount = newHealth / 100;
    }

    private void SetLayer(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }

    #region Input

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        _lookInput = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        _jumpInput = context.performed;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        _jumpInput = false;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed || !isOwner)
            return;

        _shooter.Shoot();
    }

    #endregion

    private void Update()
    {
        if (!isOwner)
            return;

        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        Vector2 lookDelta = _lookInput * mouseSensitivity;

        _xRotation -= lookDelta.y;
        _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);

        cameraHolder.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookDelta.x);
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        Vector3 horizontalVelocity = move * playerSpeed;

        // Jumping and gravity
        if (_controller.isGrounded)
        {
            if (_velocity.y < 0f)
                _velocity.y = -2f;

            if (_jumpInput)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime;
        }

        Vector3 finalVelocity = horizontalVelocity + _velocity;
        _controller.Move(finalVelocity * Time.deltaTime);

        Vector3 localVelocity = transform.worldToLocalMatrix.MultiplyVector(finalVelocity);
        // handle animations
        animator.SetFloat("Forward", localVelocity.z);
        animator.SetFloat("Sideways", localVelocity.x);
    }


    #region IDamageable

    [ServerRpc(requireOwnership: false)]
    public void IChangeHealth(int amount)
    {
        if (!isServer) return;
        Mathf.Clamp(health.value += amount, 0, 100);

        if (health.value <= 0)
        {
            OnDeath_Server?.Invoke(this);
            Despawn();
        }
    }

    public void IDespawn() { }
    public void IRespawn() { }

    #endregion

    protected override void OnDestroy()
    {
        if (_controls != null)
        {
            _controls.Player.Move.performed -= OnMovePerformed;
            _controls.Player.Move.canceled -= OnMoveCanceled;
            _controls.Player.Attack.performed -= OnAttackPerformed;
            _controls.Player.Look.performed -= OnLookPerformed;
            _controls.Player.Look.canceled -= OnLookCanceled;
            _controls.Player.Jump.performed -= OnJumpPerformed;
            _controls.Player.Jump.canceled -= OnJumpCanceled;
        }

        if (!isOwner) return;
        health.onChanged -= OnHealthChange;
    }
}