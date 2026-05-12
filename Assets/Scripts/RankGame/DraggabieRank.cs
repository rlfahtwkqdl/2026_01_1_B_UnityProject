using UnityEngine;

public class DraggabieRank : MonoBehaviour
{
    public int rankLevel = 1;
    public float dragSpeed = 30f;
    public float snapBackSpeed = 20f;

    public bool isDragging = false;
    public Vector3 originalPosition;
    public GridCell currentCell;

    public Camera mainCamera;
    public Vector3 dragOffset;
    public SpriteRenderer spriteRenderer;

    public RankGameManager GameManager;

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameManager = FindAnyObjectByType<RankGameManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            Vector3 targetPosition = GetMouseWorldPosition() + dragOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.deltaTime);
        }
        else if (transform.position != originalPosition && currentCell != null) //드래그가 끝났는데 원래 위치로 돌아가야 하는 경우
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, snapBackSpeed * Time.deltaTime);
        }
    }


    private void OnMuseDown()
    {
        StartDragging();
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        StopDragging();
    }

    void StartDragging()
    {
        isDragging = true;
        dragOffset = transform.position - GetMouseWorldPosition();
        spriteRenderer.sortingOrder = 0;
    }

    public void MoveToCell(GridCell targetcell)
    {
        if (currentCell != null)
        {
            currentCell.currentRank = null;

          
        }

        currentCell = targetcell;
        targetcell.currentRank = this;

        originalPosition = new Vector3(targetcell.transform.position.x, targetcell.transform.position.y, 0);
        transform.position = originalPosition;
    }


    public void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
    }

    public void MargeWithCell(GridCell targetCell)
    {
        if(targetCell.currentRank == null || targetCell.currentRank.rankLevel != rankLevel)
        {
            ReturnToOriginalPosition();
            return;
        }

        if (currentCell != null)
        {
            currentCell.currentRank = null;
        }

        GameManager.MergeRanks(this, targetCell.currentRank);
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }


    public void SetRankLevel(int level)
    {
        rankLevel = level;

        if(GameManager != null && GameManager.ranksprites.Length > level - 1)
        {
            spriteRenderer.sprite = GameManager.ranksprites[level - 1];
        }
    }

    void StopDragging()                //드래그 종료
    {
        isDragging = false;
        spriteRenderer.sortingOrder = 1;
        GridCell targetCell = GameManager.FindClosestCell(transform.position);        //가장 가까운 칸 찾기

        if (targetCell != null)
        {
            if (targetCell.currentRank == null) //빈칸인 경우 -> 이동
            {
                MoveToCell(targetCell);
            }
            else if (targetCell.currentRank != this && targetCell.currentRank.rankLevel == rankLevel) //같은 랭크일 경우 머지
            {
                MargeWithCell(targetCell);
            }
            else
            {
                ReturnToOriginalPosition();        //유효한 칸이 없으면 기존 위치로 복귀
            }
        }
        else
        {
            ReturnToOriginalPosition();            //유효한 칸이 없으면 기존 위치로 복귀
        }
    }
}
