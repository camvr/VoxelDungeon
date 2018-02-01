using System.Collections;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    void Awake()
    {
        // Instantiate GameManager prefab if it doesn't already exist
        if (GameManager.instance == null)
            Instantiate(gameManager);
    }
}
