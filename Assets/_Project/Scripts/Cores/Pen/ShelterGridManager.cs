using UnityEngine;

public class ShelterGridManager : MonoBehaviour
{
    [Header("Grid Layout")]
    [SerializeField] private int rows = 2;
    [SerializeField] private int columns = 3;
    [SerializeField] private Vector2 cellSize = new Vector2(180f, 150f);  // en píxeles UI
    [SerializeField] private Vector2 spacing = new Vector2(10f, 10f);     // espacio entre pens

    [Header("Referencias")]
    [SerializeField] private GameObject penPrefab;
    [SerializeField] private RectTransform gridContainer; // el GridOrigin como RectTransform

    private Pen[,] _gridMatrix;

    private void Awake()
    {
        _gridMatrix = new Pen[rows, columns];
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        if (gridContainer == null)
        {
            Debug.LogError("ShelterGridManager: falta el Grid Container (RectTransform).");
            return;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                // Posición en píxeles dentro del Canvas
                float posX = c * (cellSize.x + spacing.x);
                float posY = -r * (cellSize.y + spacing.y); // negativo porque Y baja en UI

                GameObject penObj = Instantiate(penPrefab, gridContainer);
                RectTransform penRect = penObj.GetComponent<RectTransform>();

                // Anclar arriba-izquierda del container y posicionar desde ahí
                penRect.anchorMin = new Vector2(0f, 1f);
                penRect.anchorMax = new Vector2(0f, 1f);
                penRect.pivot = new Vector2(0f, 1f);
                penRect.anchoredPosition = new Vector2(posX, posY);
                penRect.sizeDelta = cellSize;

                Pen pen = penObj.GetComponent<Pen>();

                PenState initialState = PenState.Locked;

                pen.Init(r, c, initialState, this);
                _gridMatrix[r, c] = pen;
            }
        }
    }

    /// <summary>
    /// Habilita el primer pen disponible para comprar.
    /// Llamado por CapacityManager.EnableExpansion().
    /// </summary>
    public void ShowNextAvailable()
    {
        if (_gridMatrix == null) return;

        // Busca el primer pen que esté Locked y lo marca Available
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Pen pen = _gridMatrix[r, c];
                if (pen != null && pen.CurrentState == PenState.Locked)
                {
                    pen.SetAvailable();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Desbloquea el siguiente vecino disponible.
    /// Llamado por CapacityManager después de un desbloqueo exitoso.
    /// </summary>
    public void UnlockNeighbors(int row, int column)
    {
        if (column + 1 < columns)
            _gridMatrix[row, column + 1].SetAvailable();
        else if (row + 1 < rows)
            _gridMatrix[row + 1, 0].SetAvailable();
    }

    public Pen GetFirstEmptyPen()
    {
        if (_gridMatrix == null) return null;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Pen pen = _gridMatrix[r, c];
                if (pen != null && pen.CurrentState == PenState.Empty)
                    return pen;
            }
        }

        return null;
    }

    /// <summary>
    /// Desbloquea secuencialmente una cantidad de corrales iniciales poniéndolos en estado vacío/disponible.
    /// No afecta dinero ni misiones.
    /// </summary>
    public void UnlockInitialPens(int count)
    {
        if (_gridMatrix == null || count <= 0) return;

        int unlockedCount = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Pen pen = _gridMatrix[r, c];
                if (pen == null) continue;

                if (unlockedCount < count)
                {
                    // Forzamos al corral a reinicializarse en estado vacío (Empty)
                    pen.Init(r, c, PenState.Empty, this);
                    unlockedCount++;
                }
                else
                {
                    // Al primer corral bloqueado que le sigue a los gratuitos, 
                    // lo dejamos en 'Available' para que el jugador pueda clickearlo y comprarlo.
                    if (pen.CurrentState == PenState.Locked)
                    {
                        pen.SetAvailable();
                    }
                    return; // Ya terminamos de procesar los iniciales y el siguiente disponible.
                }
            }
        }
    }
}