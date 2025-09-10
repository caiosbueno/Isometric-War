using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GameManager : MonoBehaviour
{
    
    public bool gameIsRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Start"))
            gameIsRunning = true;
    }
}
