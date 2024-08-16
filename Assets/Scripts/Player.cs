using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] InputReader inputReader;

    NetworkVariable<Vector2> moveInput = new NetworkVariable<Vector2>(); //will be replicated across the network. on every copy of an object. writePerm Owner allows owner to change variable.
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Transform objectSpawnPosition;
    [SerializeField] Canvas emoticonCanvas;
    NetworkVariable<float> health = new NetworkVariable<float>(100);
    bool isDead = false;

    private void Start()
    {
        if (inputReader != null && IsLocalPlayer) //localplayer check prevents two clients trying to write to the replicated player. Or something.
        {
            inputReader.MoveEvent += OnMove;
            inputReader.ShootEvent += SpawnRPC;
        }
        emoticonCanvas.transform.SetParent(null, true);
        emoticonCanvas.transform.rotation = Quaternion.identity;
    }

    private void OnMove(Vector2 input)
    {
        MoveRPC(input);
    }

    private void Update()
    {
        if (!isDead)
        {
            if (IsServer)
            {
                transform.position += transform.up * moveInput.Value.y * Time.deltaTime; //not sure if deltaTime can differ between client and server and if that has an effect on latency.
            }
            transform.Rotate(transform.rotation.x, transform.rotation.y, moveInput.Value.x * 100 * -Time.deltaTime); //Wait I'm confused, why does this transfer to the server if it's not in the if-IsServer check? 
            emoticonCanvas.transform.position = objectSpawnPosition.transform.position;                              //Is it because of the moveInput variable? 
        }                                                                                                            //Edit: Nope, it's just because it doesn't have any authoritative checks, so it just runs on both the client and the server.
    }

    [Rpc(SendTo.Server)] //Decorator
    private void SpawnRPC()
    {
        NetworkObject ob = Instantiate(objectToSpawn).GetComponent<NetworkObject>();
        ob.transform.position = objectSpawnPosition.transform.position;
        ob.transform.rotation = transform.rotation;
        ob.Spawn(true);
    }

    [Rpc(SendTo.Server)] //Decorator
    private void MoveRPC(Vector2 data)
    {
        moveInput.Value = data;
    }

    public void TakeDamage()
    {
        health.Value -= 15f;
        Debug.Log("Damage taken");
        if (health.Value <= 0)
        {
            HidePlayerRPC(false);
            isDead = true;
            StartCoroutine(RespawnTimer());
        }
    }

    [Rpc(SendTo.Everyone)] //Decorator
    private void HidePlayerRPC(bool state)
    {
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = state;
        }
        emoticonCanvas.GetComponentInChildren<Text>().enabled = state;
        GetComponent<CapsuleCollider2D>().enabled = state;
    }

    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(5);
        HidePlayerRPC(true);
        health.Value = 100;
        isDead = false;
    }

    new private void OnDestroy() //in the case of a player disconnecting from the server, destroy the loose EmoticonCanvas game object so it doesn't stay the the scene.
    {
        Destroy(emoticonCanvas.gameObject);
    }
}
