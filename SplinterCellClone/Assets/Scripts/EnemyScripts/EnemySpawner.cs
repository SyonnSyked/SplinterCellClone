using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("----SpawnerData----")]
    [SerializeField] Transform spawnPos;
    [SerializeField] GameObject spawnPrefab;
    [SerializeField] int spawnLimit;

    float spawnX;
    float spawnY;
    float spawnZ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SpawnEnemy()
    {
        ZeroRotation();
        Instantiate(spawnPrefab, spawnPos.position, Quaternion.Euler(spawnX, spawnY, spawnZ));
    }

    int GetEnemyCount()
    {
        return GameManager.instance.enemyCount;
    }

    public Transform GetSpawnPos()
    {
        return spawnPos;
    }

    private void ZeroRotation()
    {
        spawnX = 0;
        spawnY = 0;
        spawnZ = 0;
    }
}
