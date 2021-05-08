using System;
using System.Collections;
using System.Collections.Generic;
using Scriptableobjects;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] private CharacterSettings _characterSettings;

    [SerializeField] private InputAction _jumpAction;

    private Rigidbody2D _rigidbody2D;

    [Header("Ground Check")] [SerializeField]
    private bool _checkForGrounding;

    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private Transform _groundCheckPosition;
    private bool _grounded;


    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _jumpAction.performed += context =>
        {
            if (_grounded)
            {
                _rigidbody2D.AddForce(Vector2.up * _characterSettings.jumpForce, ForceMode2D.Impulse);
            }
        };
    }

    private void FixedUpdate()
    {
        _grounded = false;

        var colliders = Physics2D.OverlapCircleAll(_groundCheckPosition.position, 0.2f, _groundLayers);

        foreach (var collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                _grounded = true;
            }
        }
    }

    private void OnEnable()
    {
        _jumpAction.Enable();
    }

    private void OnDisable()
    {
        _jumpAction.Disable();
    }
}