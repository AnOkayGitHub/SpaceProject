using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int roomSize = 7;
    [SerializeField] private int gridSize = 10;
    private Dictionary<Vector2, bool> grid = new Dictionary<Vector2, bool>();
    private List<Vector2> occupiedCells = new List<Vector2>();
    private List<Vector2> totalOpenNeighbors = new List<Vector2>();
    private List<GameObject> rooms;
    private int roomsSpawned = 5;
    private int itemRooms;

    [SerializeField] private int maxRooms = 15;
    [SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] private GameObject[] itemRoomPrefabs;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private Transform roomHolder;
    [SerializeField] private float timeBetweenRoomCreation;
    [SerializeField] private float timeBeforeGeneration = 2f;
    [SerializeField] private bool waitForGenToHide = false;
    [SerializeField] private bool doIntroMovie = true;
    [SerializeField] private Animator scanner;
    [SerializeField] private Animator introAnimator;
    [SerializeField] private float timeBetweenScans = 4f;
    [SerializeField] private float introMovieTime;
    [SerializeField] private UpdateUI uiUpdater;

    private void Start()
    {
        World.waitForGeneration = waitForGenToHide;
        rooms = new List<GameObject>();
        World.mainCam = Camera.main.gameObject;

        itemRooms = Mathf.RoundToInt(Random.Range(1, (World.level / 2) + 1));

        for(int i = 0; i < 4; i++)
        {
            World.discoveredRooms.Add(transform.GetChild(0).gameObject);
        }
        

        // Create a 10x10 grid of empty cells, 7x7 units in size
        StartCoroutine("WaitForGeneration");
        StartCoroutine("Scan");
    }

    private IEnumerator WaitForGeneration()
    {
        uiUpdater.UpdateLevel();
        yield return new WaitForSeconds(timeBeforeGeneration);
        StartGeneration();
    }

    public Dictionary<Vector2, bool> GetGrid()
    {
        return grid;
    }

    private void StartGeneration()
    {
        grid = new Dictionary<Vector2, bool>();
        occupiedCells = new List<Vector2>();

        // Start at the top left corner and make all cells empty
        int halfway = (gridSize / 2) * roomSize;

        for (int x = -halfway; x < halfway; x += roomSize)
        {
            for (int y = -halfway; y < halfway; y += roomSize)
            {
                grid[new Vector2(x, y)] = false;
            }
        }

        // Set default cells to occupied
        grid[new Vector2(0, 0)] = true;
        grid[new Vector2(roomSize, 0)] = true;
        grid[new Vector2(-roomSize, 0)] = true;
        grid[new Vector2(0, roomSize)] = true;
        grid[new Vector2(0, -roomSize)] = true;

        // Add the occupied cells to the occupied cells list
        foreach (KeyValuePair<Vector2, bool> keyValuePair in grid)
        {
            if (keyValuePair.Value)
            {
                occupiedCells.Add(keyValuePair.Key);
            }
        }

        StartCoroutine("SpawnRooms");
    }

    private void UpdateAvailableNeighbors()
    {
        
        totalOpenNeighbors = new List<Vector2>();

        // Get the open neighbors of all occupied cells
        foreach (Vector2 pos in occupiedCells)
        {

            foreach (Vector2 neighbor in CheckNeighbors(pos))
            {
                totalOpenNeighbors.Add(neighbor);
            }
        }
    }

    private IEnumerator SpawnRooms()
    {
        for (int i = roomsSpawned; i < maxRooms - itemRooms; i++)
        {
            
            UpdateAvailableNeighbors();

            Vector2 newRoomPos = totalOpenNeighbors[Random.Range(0, totalOpenNeighbors.Count)];
            GameObject newRoom;

            if (i != maxRooms - itemRooms - 1)
            {
                newRoom = (GameObject)Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length - 1)]);
            } 
            else
            {
                newRoom = (GameObject)Instantiate(bossRoomPrefab);
            }

            newRoom.transform.position = newRoomPos;
            newRoom.transform.parent = roomHolder;
            rooms.Add(newRoom);

            occupiedCells.Add(newRoomPos);
            grid[newRoomPos] = true;
            totalOpenNeighbors.Remove(newRoomPos);

            foreach (Vector2 neighbor in CheckNeighbors(newRoomPos))
            {
                totalOpenNeighbors.Add(neighbor);
            }

            roomsSpawned = i;
            yield return new WaitForSeconds(timeBetweenRoomCreation);
        }

        for (int i = 0; i < itemRooms; i++)
        {

            UpdateAvailableNeighbors();

            Vector2 newRoomPos = totalOpenNeighbors[Random.Range(0, totalOpenNeighbors.Count)];

            GameObject newRoom = (GameObject)Instantiate(itemRoomPrefabs[Random.Range(0, itemRoomPrefabs.Length - 1)]);
            
            newRoom.transform.position = newRoomPos;
            newRoom.transform.parent = roomHolder;
            rooms.Add(newRoom);

            occupiedCells.Add(newRoomPos);
            grid[newRoomPos] = true;
            totalOpenNeighbors.Remove(newRoomPos);

            foreach (Vector2 neighbor in CheckNeighbors(newRoomPos))
            {
                totalOpenNeighbors.Add(neighbor);
            }

            roomsSpawned = i;
            yield return new WaitForSeconds(timeBetweenRoomCreation);
        }

        World.canHideRooms = true;
        StartCoroutine("WaitForPlay");
    }

    private List<Vector2> CheckNeighbors(Vector2 currentPos)
    {
        // Check up, down, left, right for neighbors and add them to a list, then return that list
        List<Vector2> openNeighbors = new List<Vector2>();

        for (int x = (int) currentPos.x - roomSize; x <= currentPos.x + roomSize; x += roomSize)
        {
            for (int y = (int) currentPos.y - roomSize; y <= currentPos.y + roomSize; y += roomSize)
            {
                if ((x == currentPos.x || y == currentPos.y))
                {
                    Vector2 pos = new Vector2(x, y);
                    if(pos != currentPos && !occupiedCells.Contains(pos))
                    {
                        if(pos.x < (gridSize / 2) * roomSize && pos.x > (-gridSize / 2) * roomSize && pos.y < (gridSize / 2) * roomSize && pos.y > (-gridSize / 2) * roomSize)
                        {
                            if (!grid[pos])
                            {
                                openNeighbors.Add(pos);
                            }
                        }
                        
                    }
                    
                }
            }
        }

        return openNeighbors;
    }

    private IEnumerator Scan()
    {
        yield return new WaitForSeconds(timeBetweenScans);

        scanner.Play("Scan", -1, 0f);

        StartCoroutine("Scan");
    }

    private IEnumerator WaitForPlay()
    {
        if(doIntroMovie)
        {
            introAnimator.Play("HUD Init", -1, 0f);
            yield return new WaitForSeconds(introMovieTime);
        }
        
        World.readyToPlay = true;
    }

}
