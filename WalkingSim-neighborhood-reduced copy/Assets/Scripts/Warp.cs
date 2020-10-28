using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Warp : MonoBehaviour
{
    Locknload locknload;
    public GameObject panel;

    private void Start()
    {
        locknload = FindObjectOfType<Locknload>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("warp me up friendo");

            if(gameObject.name == "HappyHouseEnter")
            {
                SceneManager.LoadScene("HappyHouse");
                locknload.beenHappy = true;
            }
            else if (gameObject.name == "SadHouseEnter")
            {
                SceneManager.LoadScene("SadHouse");
                locknload.beenSad = true;
            }
            else if (gameObject.name == "HappyHouseExit")
            {
                if (locknload.beenHappy == true && locknload.beenSad == true)
                {
                    SceneManager.LoadScene("Finale");
                }
                else SceneManager.LoadScene("backup1");
            }
            else if (gameObject.name == "SadHouseExit")
            {
                if (locknload.beenHappy == true && locknload.beenSad == true)
                {
                    SceneManager.LoadScene("Finale");
                }
                else SceneManager.LoadScene("backup1");
            }
            else if (gameObject.name == "FinaleHouseEnter")
            {
                panel.SetActive(true);
                Debug.Log("finale");
            }
        }
    }
}
