using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class titleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
    public void titleButton()
    {
        gameControllerScript.score = 0;
        SceneManager.LoadScene("mainScene");
    }
    
    void OnGUI()
    {
        if (gameControllerScript.score > 0)
        {
            GUI.Box(new Rect(250, 150, 270, 25), "Game Over! You scored: " + gameControllerScript.score);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
