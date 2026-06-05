using UnityEngine;

public class ShelterGridManager : MonoBehaviour
{
    [Header("Grid Layout")]
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 4;
    [SerializeField] private Vector2 cellSize = new Vector2(4f, 3f);
    [SerializeField] private Transform gridOrigin;

    [Header("Economy")]
    [SerializeField] private int baseCost = 1000;
    [SerializeField] private float costMultiplier = 1.5f;

    [Header("References")]
    [SerializeField] private GameObject penPrefab;

    private Camera mainCamera;
    private Pen[,] gridMatrix;

    void Start()
    {
        mainCamera = Camera.main;
        gridMatrix = new Pen[rows, columns];
        GenerateGrid();
    }

    /*void Update()
    {
        HandleInput();
    }*/

    private void GenerateGrid()
    {
        if (gridOrigin == null)
        {
            Debug.LogError("Grid Origin Transform is missing!");
            return;
        }

        Vector3 originPos = gridOrigin.position;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                // Strict grid layout with zero spacing (vallas compartidas)
                float posX = originPos.x + (c * cellSize.x);
                float posY = originPos.y - (r * cellSize.y); 
                Vector3 spawnPosition = new Vector3(posX, posY, 0f);

                GameObject newPenObj = Instantiate(penPrefab, spawnPosition, Quaternion.identity, gridOrigin);
                Pen newPen = newPenObj.GetComponent<Pen>();

                // Progression rule: First pen [0,0] starts empty, its direct neighbors start available
                PenState initialState = PenState.Locked;
                if (r == 0 && c == 0)
                {
                    initialState = PenState.Empty;
                }
                else if ((r == 0 && c == 1))
                {
                    initialState = PenState.Available;
                }

                // Progressive cost based on matrix distance
                int calculatedCost = Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, r + c));

                newPen.Init(r, c, initialState, calculatedCost, this);
                gridMatrix[r, c] = newPen;
            }
        }
    }

    public void UnlockNeighbors(int currentRow, int currentColumn)
    {
        // Check neighbor to the right
        if (currentColumn + 1 < columns)
        {
            gridMatrix[currentRow, currentColumn + 1].SetAvailable();
        }

        // Check neighbor below
        else if (currentRow + 1 < rows)
        {
            gridMatrix[currentRow + 1, 0].SetAvailable();
        }
    }

   /* private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                Pen clickedPen = hit.collider.GetComponent<Pen>();
                if (clickedPen != null)
                {
                    clickedPen.OnClicked();
                }
            }
        }
    }*/
}