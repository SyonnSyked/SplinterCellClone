using Unity.Cinemachine;
using UnityEngine;

public class Spawner: MonoBehaviour
{
    [Header("----SpawnerData----")]
    [SerializeField] Transform spawnPos;
    [SerializeField] GameObject spawnPrefab;
    [SerializeField] int spawnLimit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!GameObject.FindWithTag("PlayerSpawnFlag"))
            SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SpawnPlayer()
    {
        Instantiate(spawnPrefab, spawnPos.position, spawnPos.rotation);
    }

    int GetEnemyCount()
    {
        return GameManager.instance.enemyCount;
    }



}
