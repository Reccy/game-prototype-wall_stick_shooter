using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour {

    public PlayerMovement.State initState;
    public GameObject playerPrefab;
    private GameObject player;
    private ParticleSystem particleSystemIn, particleSystemOut;
    
    void Start()
    {
        particleSystemIn = transform.Find("IntroEffects").GetComponent<ParticleSystem>();
        particleSystemOut = transform.Find("EndEffects").GetComponent<ParticleSystem>();
        StartCoroutine(SpawnPlayerRoutine());
    }

    public void SpawnPlayer()
    {
        StartCoroutine(SpawnPlayerRoutine());
    }

    IEnumerator SpawnPlayerRoutine()
    {
        particleSystemIn.Play();
        yield return new WaitForSeconds(1);
        particleSystemIn.Stop();
        particleSystemOut.Play();
        QuickSpawnPlayer();
    }

    public void QuickSpawnPlayer()
    {
        player = (GameObject)Instantiate(playerPrefab, transform.position, transform.rotation);
        player.GetComponent<PlayerMovement>().state = initState;
    }
}
