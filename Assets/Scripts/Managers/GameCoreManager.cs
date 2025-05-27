using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCoreManager : MonoBehaviour
{

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();    
    }

    private bool isPaused;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {
                Time.timeScale = 0;
                isPaused = true;
            }
            else
            {
                Time.timeScale = 1;
                isPaused = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {   

        }
    }
}
