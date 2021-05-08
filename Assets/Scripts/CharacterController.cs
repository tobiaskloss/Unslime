using Scriptableobjects;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] private CharacterSettings _characterSettings;
    [SerializeField] private InputAction _jumpAction;
    [SerializeField] private InputAction _moveAction;

    private Rigidbody2D _rigidbody2D;

    [Header("Ground Check")] [SerializeField]
    private bool _checkForGrounding;

    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private Transform _groundCheckPosition;
    private bool _grounded;

    private Vector2 wsad;

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

        _moveAction.performed += context =>
        {
            wsad = context.ReadValue<Vector2>();            
        };
        _moveAction.canceled += context =>
        {
            wsad = context.ReadValue<Vector2>();            
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
        _rigidbody2D.AddForce(new Vector2(wsad.x * _characterSettings.walkSpeed, 0), ForceMode2D.Force);
    }

    private void OnEnable()
    {
        _jumpAction.Enable();
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _jumpAction.Disable();
        _moveAction.Disable();
    }
}