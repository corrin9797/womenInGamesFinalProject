using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameControllerScript : MonoBehaviour {
    //Variables for testing purposes. If you're reading this, that means I forgot to edit it out.
    public bool testBool = false;
    public bool once = false;

    //Which phase of the game is active - "setup," "action," and "resolve"
    public static string phase = "setup";
    int turnsTaken = 0;

    //[x,y] x==0 at left and y==0 at bottom
    public static GameObject[,] tileArray = new GameObject[8, 5];
    //Cube construction stuff.
    public Vector3 cubePosition;
    public GameObject cubePrefab;
    GameObject currentCube;

    public float score = 0f;
    public int combo = 1;

    //Pushers
    GameObject pusherOne;
    GameObject pusherTwo;
    GameObject[] pushers = new GameObject[2];

    //Loot
    GameObject lootOne=null;
    GameObject lootTwo=null;


    //The game grid
    GameObject[,] grid = new GameObject[8, 5];
    //Array denoting which tiles need to be removed upon resolution. Used for removeMatches function
    bool[,] markedForDestruction = new bool[8, 5];
    Color[] validColors = new Color[6] { Color.black, Color.blue, Color.green, Color.red, Color.gray, Color.white,};
    //Color[] validColors = new Color[2] { Color.black, Color.white };


    GameObject MakeCube(int xPos, int yPos)
    {
        cubePosition = new Vector3(xPos, yPos, 0);

        //                                    Quaternion, as of right now, is a magic word.
        currentCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
        currentCube.GetComponent<Renderer>().material.color = validColors[Random.Range(0,validColors.Length)];
        currentCube.AddComponent<cubeScript>();
        



        return currentCube;
    }
    int[] GetPusherCoordinates(string direction, int distanceFromZero)
    {
        int xPos = 0;
        int yPos = 0;
        if (direction == "right")
        {
            xPos = -1;
            yPos = distanceFromZero;
        }
        else if (direction == "left")
        {
            xPos = 8;
            yPos = distanceFromZero;
        }
        else if (direction == "up")
        {
            xPos = distanceFromZero;
            yPos = -1;
        }
        else if (direction == "down")
        {
            xPos = distanceFromZero;
            yPos = 5;
        }
        else
        {
            print("MAKEPUSHER THINKS IT DOESN'T HAVE A DIRECTION!");
        }
        int[] answer = new int[2];
        answer[0]= xPos;
        answer[1]= yPos;
        return answer;
    }
    GameObject MakePusher(string direction, int distanceFromZero)
    {

        int[] coords = new int[2];
        coords = GetPusherCoordinates(direction, distanceFromZero);
        int xPos = coords[0];
        int yPos = coords[1];
        cubePosition = new Vector3(xPos, yPos, 0);

        //                                    Quaternion, as of right now, is a magic word.
        currentCube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
        currentCube.GetComponent<Renderer>().material.color = validColors[Random.Range(0, validColors.Length)];
        currentCube.AddComponent<pusherScript>();
        currentCube.GetComponent<pusherScript>().coordinateDistanceFromZero = distanceFromZero;
        currentCube.GetComponent<pusherScript>().direction = direction;
        currentCube.GetComponent<pusherScript>().color = currentCube.GetComponent<Renderer>().material.color;
       
        return currentCube;
    }





    //Direction is clockwise (true) or counterclockwise (false)
    public void MovePusher(GameObject pusherObject, bool clockwise)
    {
        int distanceFromZero = pusherObject.GetComponent<pusherScript>().coordinateDistanceFromZero;
        string pusherDirection = pusherObject.GetComponent<pusherScript>().direction;
        Color pusherColor = pusherObject.GetComponent<pusherScript>().color;

        if (pusherDirection == "right")
        {
            if (clockwise)
            {
                distanceFromZero++;
                if (distanceFromZero > 4)
                {
                    distanceFromZero = 0;
                    pusherDirection = "down";
                }
            }
            else
            {
                distanceFromZero--;
                if (distanceFromZero < 0){
                    distanceFromZero = 0;
                    pusherDirection = "up";
                }
            }
        }
        else if (pusherDirection == "down")
        {
            if (clockwise)
            {
                distanceFromZero++;
                if (distanceFromZero > 7)
                {
                    distanceFromZero = 4;
                    pusherDirection = "left";
                }
            }
            else
            {
                distanceFromZero--;
                if (distanceFromZero < 0)
                {
                    distanceFromZero = 4;
                    pusherDirection = "right";
                }
            }
        }
        else if (pusherDirection == "left")
        {
            if (!clockwise)
            {
                distanceFromZero++;
                if (distanceFromZero > 4)
                {
                    distanceFromZero = 7;
                    pusherDirection = "down";
                }
            }
            else
            {
                distanceFromZero--;
                if (distanceFromZero < 0)
                {
                    distanceFromZero = 7;
                    pusherDirection = "up";
                }
            }
        }
        else if (pusherDirection == "up")
        {
            if (!clockwise)
            {
                distanceFromZero++;
                if (distanceFromZero > 7)
                {
                    distanceFromZero = 0;
                    pusherDirection = "left";
                }
            }
            else
            {
                distanceFromZero--;
                if (distanceFromZero < 0)
                {
                    distanceFromZero = 0;
                    pusherDirection = "right";
                }
            }
        }
        int[] coords = new int[2];
        
        coords = GetPusherCoordinates(pusherDirection, distanceFromZero);
        pusherObject.transform.position = new Vector3(coords[0], coords[1], 0);
        pusherObject.GetComponent<pusherScript>().coordinateDistanceFromZero= distanceFromZero;
        pusherObject.GetComponent<pusherScript>().direction= pusherDirection;
        pusherObject.GetComponent<pusherScript>().color= pusherColor;

        //Sets to true when the pusher finds itself on the list of pushers. If that happens twice, that mean two pushers are in the same spot.
        bool foundItself = false;
        foreach (GameObject pusher in pushers){
            //Checks if the new location is the same as another pusher's location. If it is, move again in the same direction.
            if (pusher.GetComponent<pusherScript>().direction==pusherDirection && pusher.GetComponent<pusherScript>().coordinateDistanceFromZero == distanceFromZero)
            {
                if (foundItself)
                {
                    MovePusher(pusherObject, clockwise);
                }
                else
                {
                    foundItself = true;
                }
            }
        }



    }

    





    //Return y coordinate where match ends. If no match, return y coordinate.
    public int MatchCheckUp(int x, int y)
    {
        if (grid[x, y] != null)
        {

            Color startColor = grid[x, y].GetComponent<Renderer>().material.color;

            try
            {
                while (startColor == grid[x, y].GetComponent<Renderer>().material.color)
                {

                    y++;
                }
            }
            catch (System.Exception)
            {

            }
            return y - 1;
        }
        return y;
    }
    public int MatchCheckRight(int x, int y)
    {

        if (grid[x, y] != null)
        {
            Color startColor = grid[x, y].GetComponent<Renderer>().material.color;

            try
            {
                while (startColor == grid[x, y].GetComponent<Renderer>().material.color)
                {

                    x++;
                }
            }
            catch (System.Exception)
            {

            }
            return x - 1;
        }
        return x;
    }


    //Returns scroe from removed cubes.
    public float removeMatches()
    {
        float score = 0.0f;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                //Checks for matches of 3 or greater, then destroys them.
                if (MatchCheckUp(x, y) - 1 > y)
                {
                    int tempY = MatchCheckUp(x, y);
                    score += ((tempY - 1) - y);
                    while (tempY >= y)
                    {

                        markedForDestruction[x, tempY] = true;
                        tempY--;
                    }
                    y = MatchCheckUp(x, y);
                }
            }

        }
        
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                //Checks for matches of 3 or greater, then destroys them.
                if (MatchCheckRight(x, y) - 1 > x)
                {
                    int tempX = MatchCheckRight(x, y);
                    score += ((tempX - 1) - x);
                    while (tempX >= x)
                    {

                        markedForDestruction[tempX, y] = true;
                        tempX--;
                    }
                    x = MatchCheckRight(x, y);
                }
            }

        }
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (markedForDestruction[x, y])
                {
                    grid[x, y].GetComponent<cubeScript>().isDestroyed = true;
                    Destroy(grid[x, y]);
                    markedForDestruction[x, y] = false;
                    
                }
            }
        }
        return score*10;
    }

    //Fall triggers gravity once. 
    public void Fall()
    {
        
            //Starts at one because there cannot be an empty space below the bottom row.
        for (int y=1; y<5; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (grid[x, y - 1] == null)
                {
                    if (grid[x, y] != null)
                    {
                        grid[x, y - 1] = MakeCube(x, y - 1);
                        grid[x, y - 1].GetComponent<Renderer>().material.color = grid[x, y].GetComponent<Renderer>().material.color;
                        Destroy(grid[x, y]);
                    }
                }
                else if (grid[x,y-1].GetComponent<cubeScript>().isDestroyed && grid[x,y].GetComponent<cubeScript>().isDestroyed!=true)
                {

                    grid[x, y - 1] = MakeCube(x, y - 1);
                    grid[x, y - 1].GetComponent<Renderer>().material.color = grid[x, y].GetComponent<Renderer>().material.color;
                    Destroy(grid[x, y]);
                    grid[x, y - 1].GetComponent<cubeScript>().isDestroyed = false;
                    grid[x, y].GetComponent<cubeScript>().isDestroyed = true;
                    //print(x + "," + y);

                }
                
            }
        }
    }
    //Fill puts cubes into the top row of the grid where there are null spaces.
    public void Fill()
    {
        for (int x=0; x<8; x++)
        {
            if (grid[x, 4] == null)
            {
                grid[x, 4] = MakeCube(x, 4);
            }
            else if (grid[x, 4].GetComponent<cubeScript>().isDestroyed)
            {
                grid[x, 4] = MakeCube(x, 4);
            }
            
            
            
            
        }
    }


    public void push(GameObject pusherObject)
    {
        if (pusherObject != null)
        {
            pusherScript pusher = pusherObject.GetComponent<pusherScript>();
            Color currentColor = pusher.color;
            bool reachedEndOfGrid = true;
            if (pusher.direction == "right")
            {
                for (int x = 0; x < 8; x++)
                {
                    if (grid[x, pusher.coordinateDistanceFromZero] == null)
                    {
                        //Breaks out of the loop.
                        grid[x, pusher.coordinateDistanceFromZero] = MakeCube(x, pusher.coordinateDistanceFromZero);
                        grid[x, pusher.coordinateDistanceFromZero].GetComponent<Renderer>().material.color = currentColor;

                        reachedEndOfGrid = false;
                        break;
                    }
                    else
                    {
                        Color tempColor = grid[x, pusher.coordinateDistanceFromZero].GetComponent<Renderer>().material.color;
                        grid[x, pusher.coordinateDistanceFromZero].GetComponent<Renderer>().material.color = currentColor;
                        currentColor = tempColor;
                    }
                }
                if (reachedEndOfGrid)
                {
                    //Loot is reset to clear after each scoring. (TODO)
                    lootTwo = lootOne;
                    lootOne = MakeCube(8, pusher.coordinateDistanceFromZero);
                    lootOne.GetComponent<Renderer>().material.color = currentColor;
                }

            }
            else if (pusher.direction == "left")
            {
                for (int x = 7; x >= 0; x--)
                {
                    if (grid[x, pusher.coordinateDistanceFromZero] == null)
                    {
                        //Breaks out of the loop.
                        grid[x, pusher.coordinateDistanceFromZero] = MakeCube(x, pusher.coordinateDistanceFromZero);
                        grid[x, pusher.coordinateDistanceFromZero].GetComponent<Renderer>().material.color = currentColor;

                        reachedEndOfGrid = false;
                        break;
                    }
                    else
                    {
                        Color tempColor = grid[x, pusher.coordinateDistanceFromZero].GetComponent<Renderer>().material.color;
                        grid[x, pusher.coordinateDistanceFromZero].GetComponent<Renderer>().material.color = currentColor;
                        currentColor = tempColor;
                    }
                }
                if (reachedEndOfGrid)
                {
                    //Loot is reset to clear after each scoring. (TODO)
                    lootTwo = lootOne;
                    lootOne = MakeCube(-1, pusher.coordinateDistanceFromZero);
                    lootOne.GetComponent<Renderer>().material.color = currentColor;
                }
            }
            else if (pusher.direction == "up")
            {
                for (int y = 0; y < 5; y++)
                {
                    if (grid[pusher.coordinateDistanceFromZero, y] == null)
                    {
                        //Breaks out of the loop.
                        grid[pusher.coordinateDistanceFromZero, y] = MakeCube(pusher.coordinateDistanceFromZero, y);
                        grid[pusher.coordinateDistanceFromZero, y].GetComponent<Renderer>().material.color = currentColor;

                        reachedEndOfGrid = false;
                        break;
                    }
                    else
                    {
                        Color tempColor = grid[pusher.coordinateDistanceFromZero, y].GetComponent<Renderer>().material.color;
                        grid[pusher.coordinateDistanceFromZero, y].GetComponent<Renderer>().material.color = currentColor;
                        currentColor = tempColor;
                    }
                }
                if (reachedEndOfGrid)
                {
                    //Loot is reset to clear after each scoring. (TODO)
                    lootTwo = lootOne;
                    lootOne = MakeCube(pusher.coordinateDistanceFromZero, 5);
                    lootOne.GetComponent<Renderer>().material.color = currentColor;

                }
            }
            else if (pusher.direction == "down")
            {
                for (int y = 4; y >= 0; y--)
                {
                    if (grid[pusher.coordinateDistanceFromZero, y] == null)
                    {
                        //Breaks out of the loop.
                        grid[pusher.coordinateDistanceFromZero, y] = MakeCube(pusher.coordinateDistanceFromZero, y);
                        grid[pusher.coordinateDistanceFromZero, y].GetComponent<Renderer>().material.color = currentColor;

                        reachedEndOfGrid = false;
                        break;
                    }
                    else
                    {
                        Color tempColor = grid[pusher.coordinateDistanceFromZero, y].GetComponent<Renderer>().material.color;
                        grid[pusher.coordinateDistanceFromZero, y].GetComponent<Renderer>().material.color = currentColor;
                        currentColor = tempColor;
                    }
                }
                if (reachedEndOfGrid)
                {
                    //Loot is reset to clear after each scoring. (TODO)
                    lootTwo = lootOne;
                    lootOne = MakeCube(pusher.coordinateDistanceFromZero, -1);
                    lootOne.GetComponent<Renderer>().material.color = currentColor;
                }
            }
            else
            {
                print("PROBLEM! PUSHER DIDN'T THINK IT HAD A DIRECTION!");
            }
            Destroy(pusherObject);
        }
    }
    // Use this for initialization
    void Start () {
        for (int x = 0; x<8; x++)
        {
            for (int y = 0; y < 5; y++) {
                grid[x,y]=MakeCube(x, y);
                markedForDestruction[x, y] = false;
                grid[x,y].GetComponent<cubeScript>().isDestroyed = false;

            }
            
        }

        while (removeMatches() > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                Fall();
                Fill();
            }
        }

        pusherOne = MakePusher("right", 0);
        pusherTwo = MakePusher("up", 0);
        pushers[0] = pusherOne;
        pushers[1] = pusherTwo;


    }
    //Because you can't pause in a normal function, but you can pause in a coroutine thata function is running is running.
    public void finalizePushers()
    {
        if (phase =="setup")
        {
            StartCoroutine(TurnCoroutine());
        }
    }
    IEnumerator TurnCoroutine()
    {
        phase = "destroy";
        yield return new WaitForSeconds(4);
        phase = "resolve";
        push(pusherOne);
        yield return new WaitForSeconds(1);
        push(pusherTwo);
        yield return new WaitForSeconds(1);
        float lastScore = 0f;
        lastScore = removeMatches();
        while (lastScore > 0 || combo==1)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.5f);
                Fall();
                
                Fill();
                
            }
            lastScore = lastScore * combo;
            score += lastScore;
            combo++;
            yield return new WaitForSeconds(0.5f);
            lastScore = removeMatches();
            
        }
        combo = 1;
        Destroy(lootOne);
        Destroy(lootTwo);
        pusherOne = MakePusher("right", 0);
        pusherTwo = MakePusher("up", 0);
        pushers[0] = pusherOne;
        pushers[1] = pusherTwo;
        turnsTaken++;
        phase = "setup";
        print(score);

    }

    

    // Update is called once per frame
    void Update () {
        if (phase == "setup")
        {
            //These four if statements handle pusher movement.
            if (Input.GetKeyDown("s"))
            {
                MovePusher(pusherOne, true);
            }
            if (Input.GetKeyDown("a"))
            {
                MovePusher(pusherOne, false);
            }
            if (Input.GetKeyDown("x"))
            {
                MovePusher(pusherTwo, true);
            }
            if (Input.GetKeyDown("z"))
            {
                MovePusher(pusherTwo, false);
            }

        }


        if (testBool&& !once)
        {
            
            //push(pusherOne);
            StartCoroutine(TurnCoroutine());
            //push(pusherTwo);
            once = true;
            
        }
    }
}
