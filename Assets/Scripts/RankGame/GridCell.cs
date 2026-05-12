using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int x, y;                           
    public DraggabieRank currentRank;
    public SpriteRenderer cellRenderers;


    private void Awake()
    {
        cellRenderers = GetComponent<SpriteRenderer>();
    }

    public void initialize(int gridX, int gridY)
    {
        x = gridX;
        y = gridY;
        name = "Cell_" + x + "," + y;
    }


    public bool isEmpty()
    {
        return currentRank == null;
    }

    public bool ContainsPosition(Vector3 position)
    {
        Bounds bounds = cellRenderers.bounds;
        return bounds.Contains(position);
    }


    public void SetRank(DraggabieRank rank)
    {
        currentRank = rank;

        if (rank != null)
        {
            rank.currentCell = this;
        }

        rank.originalPosition = new Vector3(transform.position.x, transform.position.y, 0);
        rank.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
