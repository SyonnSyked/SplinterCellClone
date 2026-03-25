using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        GameManager.instance.player = GameObject.FindWithTag("Player");
        GameManager.instance.playerShootingRoot = GameObject.FindWithTag("PlayerShootingRoot");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
