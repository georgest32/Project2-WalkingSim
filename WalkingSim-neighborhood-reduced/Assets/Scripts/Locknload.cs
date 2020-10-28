using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Locknload : MonoBehaviour
{
    public bool beenHappy;
    public bool beenSad;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {
        if (beenHappy && SceneManager.GetActiveScene().name == "backup1")
        {
            GameObject.FindWithTag("Happy").SetActive(false);
        }
        if (beenSad && SceneManager.GetActiveScene().name == "backup1")
        {
            GameObject.FindWithTag("Sad").SetActive(false);
        }
    }
}