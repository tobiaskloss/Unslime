using MLAPI;
using MLAPI.Extensions;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Scriptableobjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : NetworkBehaviour
{
    [SerializeField] private CharacterSettings _characterSettings;
    [SerializeField] private InputAction _jumpAction;
    [SerializeField] private InputAction _moveAction;
    [SerializeField] private InputAction _shootAction;
    [SerializeField] private InputAction _mouseAction;


    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private Slider _healthSlider;

    [Header("Ground Check")]
    [SerializeField] private bool _checkForGrounding;

    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private Transform _groundCheckPosition;
    private bool _grounded;
    

    [Header("Input")]
    private Vector2 wsad;
    private Vector2 mousePosition;


    [Header("Shooting")]
    public GameObject bullet;

    public NetworkVariableBool isFlipped = new NetworkVariableBool(new NetworkVariableSettings()
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
    });

    private int _maxHealth = 10;
    public NetworkVariableInt health = new NetworkVariableInt(new NetworkVariableSettings()
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
    });
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (IsServer)
        {
            health.Value = _maxHealth;
        }
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
        _mouseAction.performed += context =>
        {
            if (IsOwner)
            {
                mousePosition = context.ReadValue<Vector2>();
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
        Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        //Vector3 directionVector = (mousePositionWorld - transform.position).normalized;
        //Vector3 gunEnd = transform.position + directionVector;
        SpawnBulletServerRpc(mousePositionWorld);
    }

    [ServerRpc]
    void SpawnBulletServerRpc(Vector3 gunEnd)
    {
        //Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        Quaternion q = Quaternion.LookRotation(transform.position, gunEnd);
        var bulletController = Instantiate(bullet, gunEnd, q);
        
        //bulletController.transform.LookAt(mousePositionWorld);

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

        _healthSlider.value = health.Value;
    }

    private void OnEnable()
    {
        _jumpAction.Enable();
        _moveAction.Enable();
        _shootAction.Enable();
        _mouseAction.Enable();
    }

    private void OnDisable()
    {
        _jumpAction.Disable();
        _moveAction.Disable();
        _shootAction.Disable();
        _mouseAction.Disable();
    }

    public void Damage()
    {
        if (IsServer)
        {
            Debug.Log("Damage!");
            health.Value = health.Value - 1;
            if (health.Value <= 0)
            {
                Destroy(gameObject);
                KillPlayerClientRpc();
            }
        }
      
    }

    [ClientRpc]
    public void KillPlayerClientRpc()
    {
        /*if (!IsHost)
        {
            FindObjectOfType<NetworkManager>().StopClient();
            var NMH = FindObjectOfType<NetworkManagerHud>();
            NMH.ShowUI();
        }*/
    }
}