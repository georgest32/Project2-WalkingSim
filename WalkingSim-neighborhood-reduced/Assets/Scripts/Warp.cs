using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Warp : MonoBehaviour
{
    public Transform destinationCollider;
    Locknload locknload;

    private void Start()
    {
        locknload = GameObject.FindObjectOfType<Locknload>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("warp me up friendo");
            other.GetComponent<CharacterController>().enabled = false;
            other.transform.position = destinationCollider.position;
            other.GetComponent<CharacterController>().enabled = true;

            if(gameObject.name == "HappyHouseEnter")
            {
                locknload.beenHappy = true;
            }
            else if (gameObject.name == "SadHouseEnter")
            {
                locknload.beenSad = true;
            }

            if(locknload.beenHappy == true && locknload.beenSad == true)
            {
                SceneManager.LoadScene("Finale");
            }
        }
    }
}
