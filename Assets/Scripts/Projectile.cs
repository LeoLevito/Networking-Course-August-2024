using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    float destroyTimer = 5;
    NetworkVariable<float> projectileSpeed = new NetworkVariable<float>();

    // Start is called before the first frame update
    void Start()
    {
        projectileSpeed.Value = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0)
        {
            DestroyProjectile();
        }

        transform.position += transform.up * projectileSpeed.Value * 5 * Time.deltaTime;
    }

    //[Rpc(SendTo.Server)]
    private void DestroyProjectile()
    {
        GetComponent<NetworkObject>().Despawn(); //If I understand this correctly, Despawn destroys this game object and then sends a message to destroy this game object on all other clients connected? Is this server authoritative?
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(); //I don't think this is server authoritative right now.
            DestroyProjectile();
        }
    }
}
