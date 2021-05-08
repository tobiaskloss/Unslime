using System.Collections;
using System.Collections.Generic;
using MLAPI;
using Scriptableobjects;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    public Rigidbody2D rb;
    public float bulletForce = 5000f;
    public float lifeTime = 1f;
    public float lifeTimeAfterCollision = .1f;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            rb.AddForce(transform.forward * bulletForce, ForceMode2D.Impulse);
            Destroy(gameObject, lifeTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                var characterController = collision.gameObject.GetComponent<CharacterController>();
                characterController.Damage();
            }
            Destroy(gameObject);
        }
    }


}
