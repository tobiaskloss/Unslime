using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private float jumpForce = 10;
    
    
    [SerializeField]
    private InputAction _jumpAction;

    private Rigidbody2D _rigidbody2D;
    
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
   
    void Start()
    {
        _jumpAction.performed += context =>
        {
            _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        };
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
