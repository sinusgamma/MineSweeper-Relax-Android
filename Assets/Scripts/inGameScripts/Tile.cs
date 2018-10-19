using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Tile : MonoBehaviour {

    public bool isMined = false;
    public TextMesh displayText;
    public GameObject displayFlag;
    public Color colorIdle;
    public Color colorReset;
    public Sprite spriteUncovered;
    public Sprite spriteDetonated;
    public int ID;
    public int tilesPerRow;  // get from Grid.cs

    public Tile tileUpper;
    public Tile tileLower;
    public Tile tileLeft;
    public Tile tileRight;
    public Tile tileUpperRight;
    public Tile tileUpperLeft;
    public Tile tileLowerRight;
    public Tile tileLowerLeft;

    public List<Tile> adjacentTiles = new List<Tile>();
    public int adjacentMines = 0;

    public string state = "idle";  // idle, uncovered, flagged

    private GameObject UIingame;
    public GiveResponse giveResponse;


    // **************************************************


    public void getInput(bool isLong)  // called in inputMouse or inputTouch manager
    {
        if (Grid.state == "inGame")
        {
            if (state == "idle")
            {
                if (isLong == true)  // if long click
                    SetFlag();

                if (isLong == false)  // short click
                {
                    // after the first click in grid happens the setting of mines and numbers !!!
                    if(Grid.isFirstClick == true)  // if this is the first click on board, then assign mines
                    {
                        transform.parent.GetComponent<Grid>().AssignMines(this);  // assign the mines on grid when this tile is clicked, and make a minefree square around the first click
                        transform.parent.GetComponent<Grid>().setAllAdjacent();  // look up for each tiles adjacent
                        Grid.isFirstClick = false;  // no more tiles can assign miles
                    }
                    uncoverTile();
                }
            }
            else if (state == "flagged")
            {
                if (isLong == true)  // if long click
                    SetFlag();
            }
        }
    }


    public bool inBounds(List<Tile> inputArray, int targetID)  // checks if the id exists in list
    {
        if (targetID < 0 || targetID >= inputArray.Count)
            return false;
        else
            return true;
    }


    public void CountMines()
    {
        adjacentMines = 0;
        foreach (Tile currentTile in adjacentTiles)
        {
            if (currentTile.isMined)
                adjacentMines += 1;

            displayText.text = adjacentMines.ToString();
            if (adjacentMines <= 0)
                displayText.text = "";
        }
    }


    public void SetFlag()
    {
        if (state == "idle")
        {
            state = "flagged";
            displayFlag.GetComponent<SpriteRenderer>().enabled = true;
            Grid.minesMarkedCorrectly += 1;
        }
        else if (state == "flagged")
        {
            state = "idle";
            displayFlag.GetComponent<SpriteRenderer>().enabled = false;
            Grid.minesMarkedCorrectly -= 1;
        }

        giveResponse.startBlinking();
    }


    public void uncoverTile()  // after click on covered tile come here
    {
        if (!isMined)
        {
            gameObject.GetComponent<SpriteRenderer>().color = colorReset; // reset color of uncovered tiles
            state = "uncovered";
            displayText.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = spriteUncovered;

            Grid.tilesUncovered += 1;

            if (adjacentMines == 0)
            {
                uncoverAdjacentTiles();
            }
        }
        else  // if you clicked a mine
        {
            if(Grid.life == 1)  // if you have life
            {
                Grid.state = "pending";  // during pending time from popup can choos continueGame() - ingameManager from popupPending's button
                Grid.life = 0;  // no more life
            }
            else  // if you don't have life, then Game Over
            {
                Explode();
            }
            
        }
    }


    public void uncoverTileExternal()
    {
        gameObject.GetComponent<SpriteRenderer>().color = colorReset; // reset color of uncovered tiles
        state = "uncovered";
        displayText.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = spriteUncovered;
        Grid.tilesUncovered += 1;
    }


    private void uncoverAdjacentTiles()
    {
        foreach (Tile currentTile in adjacentTiles)
        {
            //uncover all adjacent tiles with 0 mines
            if (!currentTile.isMined && currentTile.state == "idle" && currentTile.adjacentMines == 0)
            {
                currentTile.uncoverTile();
            }
            //uncover all adjacent tiles with more than 1 adjacent mine, then stop uncovering
            else if (!currentTile.isMined && currentTile.state == "idle" && currentTile.adjacentMines > 0)
            {
                currentTile.uncoverTileExternal();
            }
        }
    }


    public void Explode()
    {
        Grid.state = "gameOver";
        state = "detonated";
        gameObject.GetComponent<SpriteRenderer>().sprite = spriteDetonated;

        foreach (Tile currentTile in Grid.tilesMined)
        {
            currentTile.ExplodeExternal();
        }
    }


    public void ExplodeExternal()
    {
        state = "detonated";
        gameObject.GetComponent<SpriteRenderer>().color = colorReset; // reset color of uncovered tiles
        gameObject.GetComponent<SpriteRenderer>().sprite = spriteDetonated;
    }


    // add color variance for the tile
    public void addColorVariance()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 1)
        {
            colorIdle = new Color(Random.Range(0.0F, 1.0F), Random.Range(0.0F, 1.0F), 1.0F, 1.0F);
        }
        else if (sceneIndex == 2)
        {
            colorIdle = new Color(Random.Range(0.5F, 0.85F), Random.Range(0.0F, 0.3F), 0.0F, 1.0F);
        }
        else if (sceneIndex == 3)
        {
            colorIdle = new Color(Random.Range(0.8F, 1.0F), Random.Range(1.0F, 1.0F), Random.Range(1.0F, 1.0F), 1.0F);
        }
        
        gameObject.GetComponent<SpriteRenderer>().color = colorIdle;
    }


    // Use this for initialization **************************
    void Start ()
    {
        addColorVariance();

        displayFlag.GetComponent<SpriteRenderer>().enabled = false;  // hide flag at start
        displayText.GetComponent<MeshRenderer>().enabled = false;  // hide text at start

        UIingame = GameObject.FindGameObjectWithTag("inGameUI");
        giveResponse = UIingame.GetComponent<GiveResponse>();  // get the response class
    }
}
