using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    public Tile tilePrefab;
    public static int tilesPerRow;
    public static int tilesPerColumn;
    public int numberOfTiles;  // allow you to set the number of tiles to be created
    public float distanceBetweenTiles = 1f;
    public static int numberOfMines;

    public static List<Tile> tilesAll = new List<Tile>();
    public static List<Tile> tilesMined = new List<Tile>();
    public static List<Tile> tilesUnmined = new List<Tile>();

    public static int minesMarkedCorrectly = 0;
    public static int tilesUncovered = 0;
    public static int minesRemaining = 0;

    public static string state = "inGame";  // inGame gameOver  gameWon

    public static bool isFirstClick = true;  // check the first click on tile to assign mines

    public static int life = 1;  // check if you have life to follow the game


    public void CreateTiles()
    {
        numberOfTiles = tilesPerRow * tilesPerColumn;
        float xOffset = 0f;
        float yOffset = -1f;

        for(int tilesCreated = 0; tilesCreated < numberOfTiles; tilesCreated++)
        {
            xOffset += distanceBetweenTiles;

            if(tilesCreated % tilesPerRow == 0)
            {
                yOffset += distanceBetweenTiles;
                xOffset = 0;
            }

            Tile newTile = (Tile) Instantiate(tilePrefab, new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z), transform.rotation);
            newTile.transform.parent = this.transform;  // the grid is the parent of the tiles
            newTile.ID = tilesCreated;
            newTile.tilesPerRow = tilesPerRow;

            tilesAll.Add(newTile);  // when a new tile is created is added to this
        }
        // AssignMines();
    }

    // Assign the mines - used in the tile.cs after first click
    public void AssignMines(Tile clickedTile)
    {
        lookAllAdjacent(clickedTile);  // look for the adjacent tiles of the first click tile
       
        List<Tile> tilesUnmined = new List<Tile>(tilesAll); // All tiles are  added to tilesUnmined at first

        tilesUnmined.Remove(clickedTile);  // remove clicked tile, dont begin with bomb

        foreach (Tile adjacentTile in clickedTile.adjacentTiles)  // removes adjacent tiles from tilesUnmined, to disable to add mines to them
        {
            tilesUnmined.Remove(adjacentTile);
        }

        for (int minesAssigned = 0; minesAssigned < numberOfMines; minesAssigned++)
        {
            Tile currentTile = tilesUnmined[Random.Range(0, (tilesUnmined.Count - 1))];

            currentTile.isMined = true;

            tilesMined.Add(currentTile);
            tilesUnmined.Remove(currentTile);
        }
    }


    // Each tile looks for its adjacent, and mine/empty/number happens - used in the tile.cs after first click
    public void setAllAdjacent()
    {
        foreach (Tile tile in tilesAll) // Loop through List with foreach
        {
            lookAllAdjacent(tile);
            tile.CountMines();
        }
    }


    // look up adjacent tiles and add them to list adjacentTiles
    public void lookAllAdjacent(Tile tile)
    {
        if (tile.inBounds(Grid.tilesAll, tile.ID + tilesPerRow)) tile.tileUpper = Grid.tilesAll[tile.ID + tilesPerRow];
        if (tile.inBounds(Grid.tilesAll, tile.ID - tilesPerRow)) tile.tileLower = Grid.tilesAll[tile.ID - tilesPerRow];
        if (tile.inBounds(Grid.tilesAll, tile.ID - 1) && tile.ID % tilesPerRow != 0) tile.tileLeft = Grid.tilesAll[tile.ID - 1];
        if (tile.inBounds(Grid.tilesAll, tile.ID + 1) && (tile.ID + 1) % tilesPerRow != 0) tile.tileRight = Grid.tilesAll[tile.ID + 1];

        if (tile.inBounds(Grid.tilesAll, tile.ID + tilesPerRow + 1) && (tile.ID + 1) % tilesPerRow != 0) tile.tileUpperRight = Grid.tilesAll[tile.ID + tilesPerRow + 1];
        if (tile.inBounds(Grid.tilesAll, tile.ID + tilesPerRow - 1) && tile.ID % tilesPerRow != 0) tile.tileUpperLeft = Grid.tilesAll[tile.ID + tilesPerRow - 1];
        if (tile.inBounds(Grid.tilesAll, tile.ID - tilesPerRow + 1) && (tile.ID + 1) % tilesPerRow != 0) tile.tileLowerRight = Grid.tilesAll[tile.ID - tilesPerRow + 1];
        if (tile.inBounds(Grid.tilesAll, tile.ID - tilesPerRow - 1) && tile.ID % tilesPerRow != 0) tile.tileLowerLeft = Grid.tilesAll[tile.ID - tilesPerRow - 1];

        if (tile.tileUpper) { tile.adjacentTiles.Add(tile.tileUpper); }
        if (tile.tileLower) { tile.adjacentTiles.Add(tile.tileLower); }
        if (tile.tileLeft) { tile.adjacentTiles.Add(tile.tileLeft); }
        if (tile.tileRight) { tile.adjacentTiles.Add(tile.tileRight); }

        if (tile.tileUpperLeft) { tile.adjacentTiles.Add(tile.tileUpperLeft); }
        if (tile.tileUpperRight) { tile.adjacentTiles.Add(tile.tileUpperRight); }
        if (tile.tileLowerLeft) { tile.adjacentTiles.Add(tile.tileLowerLeft); }
        if (tile.tileLowerRight) { tile.adjacentTiles.Add(tile.tileLowerRight); }
    }


    void finishGame()
    {
        state = "gameWon";
        //uncovers remaining fields if all nodes have been placed
        foreach (Tile currentTile in tilesAll)
        {
            if (currentTile.state == "idle" && !currentTile.isMined)
            {
                currentTile.uncoverTileExternal();
            }
        }

        //marks remaining mines if all nodes except the mines have been uncovered
        foreach (Tile currentTile in tilesAll)
        {
            if (currentTile.state != "flagged")
            {
                currentTile.SetFlag();
            }
        }
    }


    // Use this for initialization
    void Start ()
    {

        // RESET THE GRID
        tilesAll.Clear();
        tilesMined.Clear();
        tilesUnmined.Clear();
        minesMarkedCorrectly = 0;
        tilesUncovered = 0;
        minesRemaining = 0;

        isFirstClick = true;

        life = 1;

        // read the grid settings from global
        tilesPerRow = GlobalControl.Instance.rows;
        tilesPerColumn = GlobalControl.Instance.columns;
        numberOfMines = GlobalControl.Instance.mines;

        CreateTiles();


        state = "inGame";
    }


    // Update is called once per frame
    void Update()
    {
        if (state == "inGame")
        {
            if (/*(minesRemaining == 0 && minesMarkedCorrectly == numberOfMines) ||*/ (tilesUncovered == numberOfTiles - numberOfMines))
            {
                finishGame();  // you won
            }
        }
    }

}
