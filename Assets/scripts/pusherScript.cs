using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pusherScript : MonoBehaviour {

    public string direction;
    public Color color;

    //The distance of the pusher from x=0 for "up" and "down" or y=0 for "left" and "right".
    //For instance, a "right" pusher on the bottom row would have this at zero. The same is true for "left".
    //An "up" or "down" pusher would measure distance from the leftmost column.
    public int coordinateDistanceFromZero;
    public bool isActive;
	// Use this for initialization
	void Start () {
		
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
