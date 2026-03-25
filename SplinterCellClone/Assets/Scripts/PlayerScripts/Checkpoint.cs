using UnityEngine;
using System.Collections;
public class Checkpoint : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerSpawner.GetComponentInChildren<Spawner>().GetSpawnPos().position = GetComponentInChildren<Transform>().position;
            StartCoroutine(ShowPopup());
        }
    }


    IEnumerator ShowPopup()
    {
        GameManager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        GameManager.instance.checkpointPopup.SetActive(false);
    }
}
