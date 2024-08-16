using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    float destroyTimer = 5;
    NetworkVariable<float> projectileSpeed = new NetworkVariable<float>();

    void Start()
    {
        if (IsServer)
        {
            projectileSpeed.Value = 1f; //Used to give error "Client is not allowed to write to this network variable.", until I check for IsServer, which runs it only on the server instead of the client (and server).
        }
    }

    void Update() //Update happens on both client and server if you don't specifically check for server or client authority
    {
        if(IsServer)    //so this should only run on the server
        {
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0)
            {
                DestroyProjectile();
            }
        }
        transform.position += transform.up * projectileSpeed.Value * 5 * Time.deltaTime; //so this is run on both the client and the server, if I've understood correcly.
    }

    private void DestroyProjectile()
    {
        GetComponent<NetworkObject>().Despawn(); //If I understand this correctly, Despawn destroys this game object and then sends a message to destroy this game object on all other clients connected? Is this server authoritative?
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(IsServer && IsSpawned) 
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<Player>().TakeDamage();  //Used to give error "Client is not allowed to write to this network variable.", until I check for IsServer, which runs it only on the server instead of the client (and server).
                DestroyProjectile();
            }
        }
    }
}
