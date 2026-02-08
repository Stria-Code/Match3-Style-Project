using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Playables;


[System.Serializable]
public class CandyData
{
    public int candyIndex;
    public float posX;
    public float posY;
}

[System.Serializable]
public class GameData
{
    public List <CandyData> candiesData;
    public int score;
    public int moves;
}
public class Grid : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject tilesPrefab;
    [SerializeField] private GameObject[] candies;
    [SerializeField] private GameObject[,] gridCandies;
    [SerializeField] private GameObject[,] gridTiles;
    [SerializeField] AudioClip movePieces;

    private GameObject tile;
    private Material previousMaterial;
    private GameObject firstSelectedCandy;
    private GameObject secondSelectedCandy;
    private List<Vector2> positions;

    void Start()
    {
        previousMaterial = null;
        gridCandies = new GameObject[width, height];
        gridTiles = new GameObject[width, height];

        FillGrid();
        //LoadGame();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrid();

        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }

        // Handles the object reference error
        if (firstSelectedCandy != null || secondSelectedCandy != null)
        {
            // Loop through the grid vertically
            CheckForMatchVertically();

            //Randomly fill the empty spaces in the grid
            RePopulateGrid();
        }



        return;
    }
    private void FillGrid()
    {
        //Checks for a save file
        if (File.Exists(Application.persistentDataPath + "/GameData.dat"))
        {
           //Loads from save file
           LoadGame();
        }
        else
        {
            //If no save file, creates new random grid
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //stores the current position
                    Vector2 tempPosition = new Vector2(i, j);

                    //creates a tile game object at each position, from the tilesPrefab game object, with no rotation
                    tile = Instantiate(tilesPrefab, tempPosition, Quaternion.identity);

                    gridTiles[i, j] = tile;

                    //stores a number between 0 and the length of the candies array, for a random candy spawn
                    int useCandy = Random.Range(0, candies.Length);

                    //creates a candy game object at each position, from the candies array, at the position of each tile, with no rotation
                    gridCandies[i, j] = Instantiate(candies[useCandy], tile.transform.position, Quaternion.identity);
                }
            }
        }
    }
    private void RePopulateGrid()
    {
        //Fills empty spaces in the grid if there are any
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gridCandies[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j);
                    int useCandy = Random.Range(0, candies.Length);

                    gridCandies[i, j] = Instantiate(candies[useCandy], tempPosition, Quaternion.identity);
                }
            }
        }
    }
    private void MovePiecesUp(GameObject obj1, GameObject obj2, GameObject obj3)
    {
        //Checks to see if the 3 objects that need moving up are null and returns
        if (obj1 == null || obj2 == null || obj3 == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //checks all objects x position is the same
                if (obj1.transform.position.x == x && obj2.transform.position.x == x && obj3.transform.position.x == x)
                {
                    //checks y position does not go outside grid
                    if (y + 1 >= height || y - 2 < 0) continue;

                    //moves each object vertically by one, until can't be moved
                    if (obj1.transform.position.y == y)
                    {
                        SwapPieces(obj1, gridCandies[x, y + 1]);  
                    }

                    if(obj2.transform.position.y == y - 1)
                    {
                        SwapPieces(obj2, gridCandies[x, y + 1]);
                    }

                    if (obj3.transform.position.y == y - 2)
                    {
                        SwapPieces(obj3, gridCandies[x, y + 1]);
                    }
                }

                //checks all objects y position is the same
                if (obj1.transform.position.y == y && obj2.transform.position.y == y && obj3.transform.position.y == y)
                {
                    //Same process as above
                    if (y + 1 >= height || y - 2 < 0) continue;

                    if (obj1.transform.position.x == x)
                    {
                        SwapPieces(obj1, gridCandies[x, y + 1]);
                    }
                    if (obj2.transform.position.x == x)
                    {
                        SwapPieces(obj2, gridCandies[x, y + 1]);
                    }
                    if (obj3.transform.position.x == x)
                    {
                        SwapPieces(obj3, gridCandies[x, y + 1]);
                    }
                }
            }
        }
    }
    private void UpdateGrid()
    {
        foreach (GameObject candy in FindObjectsOfType<GameObject>())
        {
            if (candy.CompareTag("Candy"))
            {
                Vector2 gridPosition = candy.transform.position;
                if (gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height)
                {
                    gridCandies[(int)gridPosition.x, (int)gridPosition.y] = candy;
                }
            }
        }
    }

    //Horizontal matches work to an extent but break the grid when trying to delete them and repopulate, therefore were left out.
    private void CheckForMatchHorizontally()
    {
        for (int y = 0; y < height; y++)
        {
            int h = 1;

            for (int x = 1; x < width; x++)
            {
                if (gridCandies[x, y] == null || gridCandies[x - 1, y] == null)
                {
                    h = 1;
                    continue;
                }

                SpriteRenderer pieceRender = gridCandies[x, y].GetComponent<SpriteRenderer>();
                Material material = pieceRender.material;

                SpriteRenderer previousPieceRender = gridCandies[x - 1, y].GetComponent<SpriteRenderer>();
                previousMaterial = previousPieceRender.material;

                if (material.color == previousMaterial.color)
                {
                    h++;
                }
                else
                {
                    h = 1;
                }

                if (h == 3 && gridCandies[x - 2, y] != null)
                {
                    MovePiecesUp(gridCandies[x, y], gridCandies[x - 1, y], gridCandies[x - 2, y]);
                    DestroyMatch(gridCandies[x, y], gridCandies[x - 1, y], gridCandies[x - 2, y]);
                    ScoreManager.score += 100;
                    SoundManager.instance.PlaySound(movePieces);
                }

            }
        }
    }   
    private void CheckForMatchVertically()
    {
        for (int x = 0; x < width; x++)
        {
            //Default variable h to 1 which shows amount of candies of same colour currently matched

            int h = 1;

            for (int y = 1; y < height; y++)
            {
                //Checks the current candy and the one underneath it to skip to the next piece if these are null
                if (gridCandies[x, y] == null || gridCandies[x, y - 1] == null)
                {
                    h = 1;
                    continue;
                }

                //Gets the renderer and then material of current candy
                SpriteRenderer pieceRender = gridCandies[x, y].GetComponent<SpriteRenderer>();
                Material material = pieceRender.material;

                //Gets the renderer and then material of candy underneath current candy
                SpriteRenderer previousPieceRender = gridCandies[x, y - 1].GetComponent<SpriteRenderer>();
                previousMaterial = previousPieceRender.material;

                //Checks if their material is the same, if yes increase h
                if (material.color == previousMaterial.color)
                {
                    h++;
                }
                else
                {
                    h = 1;
                }

                //When h is 3, and all 3 candies are not null, a match has been detected
                if (h == 3 && gridCandies[x, y - 2] != null)
                {
                    //Pieces get moved to the top of the grid and then deleted, leaving empty spaces
                    MovePiecesUp(gridCandies[x, y], gridCandies[x, y - 1], gridCandies[x, y - 2]);
                    DestroyMatch(gridCandies[x, y], gridCandies[x, y - 1], gridCandies[x, y - 2]);

                    //Adds to score
                    ScoreManager.score += 100;

                    //Plays sound
                    SoundManager.instance.PlaySound(movePieces);
                }
            }
        }
    }
    private void OnMouseDown()
    {
        Vector2 firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(firstTouch);
        if (hit.CompareTag("Candy"))
        {
            firstSelectedCandy = hit.gameObject;
        }
        else
        {
            return;
        }

    }
    private void OnMouseUp()
    {
        Vector2 secondTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(secondTouch);
        if (hit.CompareTag("Candy"))
        {
            secondSelectedCandy = hit.gameObject;
        }
        else
        {
            return;
        }

        if (IsNext())
        {
            SwapPieces(firstSelectedCandy, secondSelectedCandy);
            MovesManager.moves -= 1;
        }
    }
    private bool IsNext()
    {
        float x_difference = Mathf.Abs( secondSelectedCandy.transform.position.x - firstSelectedCandy.transform.position.x );
        float y_difference = Mathf.Abs( secondSelectedCandy.transform.position.y - firstSelectedCandy.transform.position.y);
        if (x_difference > 1 || y_difference > 1) return false;
        if (x_difference == 1 && y_difference == 1) return false;
        return true;
    }
    private void SwapPieces(GameObject piece1, GameObject piece2)
    {
        if (piece1 == null || piece2 == null) return;

        Vector2 tempPos = piece1.transform.position;
        piece1.transform.position = piece2.transform.position;
        piece2.transform.position = tempPos;
    }
    private void DestroyMatch(GameObject obj1, GameObject obj2, GameObject obj3)
    {
        if (obj1 == null || obj2 == null || obj3 == null) return;

        Vector2 pos1 = obj1.transform.position;
        Vector2 pos2 = obj2.transform.position;
        Vector2 pos3 = obj3.transform.position;

        gridCandies[(int)pos1.x, (int)pos1.y] = null;
        gridCandies[(int)pos2.x, (int)pos2.y] = null;
        gridCandies[(int)pos3.x, (int)pos3.y] = null;

        Destroy(obj1);
        Destroy(obj2);
        Destroy(obj3);
    }
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = File.Create(Application.persistentDataPath + "/GameData.dat");

        List<CandyData> candiesData = new List<CandyData>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gridCandies[x, y] != null)
                {
                    string candyName = gridCandies[x, y].name.Replace("(Clone)", "").Trim();

                    for (int i = 0; i < candies.Length; i++)
                    {
                        if (candies[i].name == candyName)
                        {
                            candiesData.Add(new CandyData {candyIndex = i, posX = gridCandies[x, y].transform.position.x, posY = gridCandies[x, y].transform.position.y});
                        }
                    }
                }
            }
        }

        GameData details = new()
        {
            score = ScoreManager.score,
            moves = MovesManager.moves,
            candiesData = candiesData
        };

        bf.Serialize(stream, details);
        stream.Close();
    }
    private void LoadGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/GameData.dat", FileMode.Open);

        GameData data = (GameData)bf.Deserialize(file);
        file.Close();

        ScoreManager.score = data.score;
        MovesManager.moves = data.moves;


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                foreach (CandyData candyData in data.candiesData)
                {
                    if (candyData.posX == x && candyData.posY == y)
                    {
                        Vector2 tempPosition = new Vector2(x, y);
                        tile = Instantiate(tilesPrefab, tempPosition, Quaternion.identity);
                        gridTiles[x, y] = tile;

                        gridCandies[x, y] = Instantiate(candies[candyData.candyIndex], tile.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }
}
