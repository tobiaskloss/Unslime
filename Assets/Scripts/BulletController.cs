using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float bulletForce = 5000f;
    public float lifeTime = 1f;
    public float lifeTimeAfterCollision = .1f;

    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, lifeTimeAfterCollision);
    }


}
