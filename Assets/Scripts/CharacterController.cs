using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Scriptableobjects;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : NetworkBehaviour
{
    [SerializeField] private CharacterSettings _characterSettings;
    [SerializeField] private InputAction _jumpAction;
    [SerializeField] private InputAction _moveAction;
    [SerializeField] private InputAction _shootAction;

    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;

    [Header("Ground Check")]
    [SerializeField] private bool _checkForGrounding;

    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private Transform _groundCheckPosition;
    private bool _grounded;

    [Header("Input")]
    private Vector2 wsad;

    [Header("Shooting")]
    public GameObject bullet;

    public NetworkVariableBool isFlipped = new NetworkVariableBool(new NetworkVariableSettings()
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
    });
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _jumpAction.performed += context =>
        {
            Jump();
        };

        _moveAction.performed += context =>
        {
            if (IsOwner)
            {
                wsad = context.ReadValue<Vector2>();
                isFlipped.Value = wsad.x < 0;
            }
        };

        _moveAction.canceled += context =>
        {
            if (IsOwner)
            {
                wsad = context.ReadValue<Vector2>(); 
            }
        };

        _shootAction.performed += context =>
        {
            if (IsOwner)
            {
                Shoot();
            }
        };
    }

    void Jump()
    {
        if (IsOwner)
        {
            if (_grounded)
            {
                _rigidbody2D.AddForce(Vector2.up * _characterSettings.jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    void Shoot()
    {
        Vector3 gunEnd;
        if (isFlipped.Value)
        {
            gunEnd = transform.position + new Vector3(-.5f, 0, 0);
        }
        else
        {
            gunEnd = transform.position + new Vector3(.5f, 0, 0);
        }
        SpawnBulletServerRpc(gunEnd);
    }

    [ServerRpc]
    void SpawnBulletServerRpc(Vector3 gunEnd)
    {
        var bulletController = Instantiate(bullet, gunEnd, new Quaternion());
        if (isFlipped.Value)
        {
            bulletController.transform.localEulerAngles = new Vector3(0, 0, 180f);
        }

        var networkObject = bulletController.GetComponent<NetworkObject>();
        networkObject.Spawn();
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
        _spriteRenderer.flipX = !isFlipped.Value;
        _rigidbody2D.AddForce(new Vector2(wsad.x * _characterSettings.walkSpeed, 0), ForceMode2D.Force);
    }

    private void OnEnable()
    {
        _jumpAction.Enable();
        _moveAction.Enable();
        _shootAction.Enable();
    }

    private void OnDisable()
    {
        _jumpAction.Disable();
        _moveAction.Disable();
        _shootAction.Disable();
    }
}