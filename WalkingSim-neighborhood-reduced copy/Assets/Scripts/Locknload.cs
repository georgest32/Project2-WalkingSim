using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locknload : MonoBehaviour
{
    public bool beenHappy;
    public bool beenSad;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}