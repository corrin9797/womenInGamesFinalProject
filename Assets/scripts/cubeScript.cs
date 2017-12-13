using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeScript : MonoBehaviour {

    public bool isDestroyed;

	// Use this for initialization
	void Start () {
        isDestroyed = false;
	}
	
    void OnMouseDown()
    {
        if (gameControllerScript.phase == "action")
        {
            
            Destroy(this.gameObject);
        }
    }
    
	// Update is called once per frame
	void Update () {
		
	}
}
