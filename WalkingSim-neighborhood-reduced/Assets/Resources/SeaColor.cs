using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SeaColor : MonoBehaviour
{
    // Start is called before the first frame update
    public float seaHeight = 57.8f;
    private Color seaColor = new Color(51, 130, 220);


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < seaHeight)
        {
            RenderSettings.fogColor = seaColor;
            RenderSettings.fogDensity = 0.03f;
        }
    }
}
