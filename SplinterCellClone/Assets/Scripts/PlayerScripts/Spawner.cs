using UnityEngine;

public class Spawner: MonoBehaviour
{
    [Header("----SpawnerData----")]
    [SerializeField] Transform spawnPos;
    [SerializeField] int spawnLimit;

    private int enemyCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int GetEnemyCount()
    {
        return GameManager.instance.enemyCount;
    }



}
