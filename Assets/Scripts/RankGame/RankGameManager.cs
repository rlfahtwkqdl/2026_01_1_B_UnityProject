using UnityEngine;
using System.Collections.Generic;

public class RankGameManager : MonoBehaviour
{
    public int gridWidth = 7;
    public int gridHeight = 7;
    public float CellSize = 1.3f;
    public GameObject cellPrefabs;
    public Transform gridContainer;

    public GameObject rankPrefabs;
    public Sprite[] ranksprites;
    public int maxRankLevel = 7;

    public GridCell[,] grid;


    void InitializeGrid()
    {
        grid = new GridCell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(
                    x * CellSize - (gridWidth * CellSize / 2) + CellSize / 2,
                    y * CellSize - (gridWidth * CellSize / 2) + CellSize / 2,
                    1f
                    );

                GameObject cellObj = Instantiate(cellPrefabs, position, Quaternion.identity, gridContainer);
                GridCell cell = cellObj.AddComponent<GridCell>();
                cell.initialize(x, y);

                grid[x, y] = cell;
            }
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeGrid();

        for (int i = 0; i < 4; i++)
        {
            SpawnNewRank();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            SpawnNewRank();
        }
    }

    public DraggabieRank CreateRankInCell(GridCell cell , int level)
    {
        if (cell == null || !cell.isEmpty()) return null;

        level = Mathf.Clamp(level, 1, maxRankLevel);

        Vector3 rankPosition = new Vector3(cell.transform.position.x, cell.transform.position.y, 0f);

        GameObject rankObj = Instantiate(rankPrefabs, rankPosition, Quaternion.identity, gridContainer);
        rankObj.name = "Rank_Level_" + level;

        DraggabieRank rank = rankObj.AddComponent<DraggabieRank>();

        rank.SetRankLevel(level);

        cell.SetRank(rank);

        return rank;
    }

    private GridCell FineEmptyCell()
    {
        List<GridCell> emptyCells = new List<GridCell>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].isEmpty())
                {
                    emptyCells.Add(grid[x, y]);
                }
            }
        }

        if (emptyCells.Count == 0)
        {
            return null;
        }

        return emptyCells[Random.Range(0, emptyCells.Count)];
    }

    public bool SpawnNewRank()
    {
        GridCell emptyCell = FineEmptyCell();
        if (emptyCell == null) return false;

        int rankLevel = Random.Range(0, 100) < 80 ? 1 : 2;

        CreateRankInCell(emptyCell, rankLevel);

        return true;
    }

    public GridCell FindClosestCell(Vector3 position) // 가장 가까운 칸 찾기
    {
        for (int x = 0; x < gridWidth; x++) // 1. 먼저 위치가 포함된 칸 확인
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].ContainsPosition(position))
                {
                    return grid[x, y];
                }
            }
        }

        GridCell closestCell = null; // 2. 없다면 가장 가까운 칸 찾기
        float closestDistance = float.MaxValue;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float distance = Vector3.Distance(position, grid[x, y].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCell = grid[x, y];
                }
            }
        }

        if (closestDistance > CellSize * 2) // 3. 너무 멀면 null 반환
        {
            return null;
        }

        return closestCell;
    }

    public void RemoveRank(DraggabieRank rank)    //계급장 제거
    {
        if (rank == null) return;

        if (rank.currentCell != null)             //칸 데이터에서 제거
        {
            rank.currentCell.currentRank = null;
        }

        Destroy(rank.gameObject);                 //게임 오브젝트 제거
    }

    public void MergeRanks(DraggabieRank draggableRank, DraggabieRank targetRank)
    {
        if (draggableRank == null || targetRank == null || draggableRank.rankLevel != targetRank.rankLevel)
        {
            //같은 레벨이 아니면 머지가 되지 않는다.
            if (draggableRank != null) draggableRank.ReturnToOriginalPosition();
            return;
        }

        int newLevel = targetRank.rankLevel + 1;
        if (newLevel > maxRankLevel)
        {
            RemoveRank(draggableRank);
            return;
        }

        targetRank.SetRankLevel(newLevel);
        RemoveRank(draggableRank);
    }
}
