using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [SerializeField] private float _gridSize;
    private float _fallTime;
    [SerializeField] private int _puyoComboCount = 4;
    private CellElement[,] _puyoGrid;
    private bool[,] _checkedChainedPuyos;

    private Vector2Int _playablePuyoPosition;

    [Header("Assets")]
    [SerializeField] private GameParams _gameParams;
    [SerializeField] private GameObject _backgroundTilePrefab;

    [SerializeField] private Transform _backgroundTilesParent;
    [SerializeField] private Transform _puyosParent;

    private void Start()
    {
        InitGameStats();
        CreateGrid();
        CreateNewPuyoOnTop();
    }

    private void InitGameStats()
    {
        _fallTime = _gameParams.GameFallSpeed;
    }

    private void CreateNewPuyoOnTop()
    {
        int xPuyoStart = Mathf.FloorToInt(_gridWidth / 2);
        int yPuyoStart = _gridHeight - 1;
        CreateNewPuyo(xPuyoStart, yPuyoStart);
    }

    private void CreateGrid()
    {
        _puyoGrid = new CellElement[_gridWidth, _gridHeight];
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                Instantiate(_backgroundTilePrefab, CellToWorldPosition(x, y), Quaternion.identity, _backgroundTilesParent);
                _puyoGrid[x, y] = new CellElement(CellElement.Type.Empty);
            }
        }
    }

    private void CreateNewPuyo(int xGrid, int yGrid, Puyo existingPuyo = null)
    {
        if (existingPuyo == null)
            existingPuyo = GetRandomPuyo();

        Puyo newPuyo = Instantiate(existingPuyo, CellToWorldPosition(xGrid, yGrid), Quaternion.identity, _puyosParent);
        _playablePuyoPosition = new Vector2Int(xGrid, yGrid);
        _puyoGrid[xGrid, yGrid] = new CellElement(CellElement.Type.FallingPuyo);
        _puyoGrid[xGrid, yGrid].CurrentPuyo = newPuyo;
        CheckPuyoGround(xGrid, yGrid);
        if (_puyoGrid[xGrid, yGrid].CellType == CellElement.Type.GroundedPuyo)
        {
            _checkedChainedPuyos = new bool[_gridWidth, _gridHeight];
            Puyo.ColorType colorToSearch = _puyoGrid[xGrid, yGrid].CurrentPuyo.Color;
            CheckForChain(xGrid, yGrid, colorToSearch);
            CountChainedPuyos();
            CreateNewPuyoOnTop();
        }
    }

    private Puyo GetRandomPuyo()
    {
        int randomIndex = Random.Range(0, _gameParams.GamePuyos.Count);
        Puyo randomPuyo = _gameParams.GamePuyos[randomIndex];

        return randomPuyo;
    }

    private void CheckPuyoGround(int xGrid, int yGrid)
    {
        if (yGrid <= 0)
        {
            _puyoGrid[xGrid, yGrid].CellType = CellElement.Type.GroundedPuyo;
            return;
        }
        if (_puyoGrid[xGrid, yGrid - 1].CellType != CellElement.Type.Empty)
        {
            _puyoGrid[xGrid, yGrid].CellType = CellElement.Type.GroundedPuyo;
            return;
        }

        StartCoroutine(WaitForPuyoFall(xGrid, yGrid, xGrid, yGrid - 1));
    }

    private void UpdatePuyoPosition(int currentX, int currentY, int targetX, int targetY)
    {
        Puyo currentPuyo = DestroyPuyo(currentX, currentY);

        CreateNewPuyo(targetX, targetY, currentPuyo);
    }

    private Puyo DestroyPuyo(int currentX, int currentY)
    {
        CellElement currentPuyo = _puyoGrid[currentX, currentY];
        Destroy(currentPuyo.CurrentPuyo.gameObject);
        _puyoGrid[currentX, currentY] = new CellElement(CellElement.Type.Empty);

        return currentPuyo.CurrentPuyo;
    }

    private IEnumerator WaitForPuyoFall(int currentX, int currentY, int targetX, int targetY)
    {
        yield return new WaitForSeconds(_fallTime);

        UpdatePuyoPosition(currentX, currentY, targetX, targetY);
    }

    #region Chains
    private void CheckForChain(int xGrid, int yGrid, Puyo.ColorType color)
    {
        if (xGrid < 0 || xGrid >= _gridWidth || yGrid < 0 || yGrid >= _gridHeight) return;
        if (_puyoGrid[xGrid, yGrid].CellType == CellElement.Type.Empty) return;
        if (_checkedChainedPuyos[xGrid, yGrid] == true) return;
        if (_puyoGrid[xGrid, yGrid].CurrentPuyo.Color != color) return;


        _checkedChainedPuyos[xGrid, yGrid] = true;

        CheckForChain(xGrid + 1, yGrid, color);
        CheckForChain(xGrid - 1, yGrid, color);
        CheckForChain(xGrid, yGrid + 1, color);
        CheckForChain(xGrid, yGrid - 1, color);
    }

    private void CountChainedPuyos()
    {
        List<Vector2Int> _chainedPuyos = new List<Vector2Int>();
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                if (_checkedChainedPuyos[x,y] == true)
                {
                    _chainedPuyos.Add(new Vector2Int(x, y));
                }
            }
        }
        float _puyosCount = _chainedPuyos.Count;

        HashSet<Vector2Int> _puyosThatWillFall = new HashSet<Vector2Int>();
        if (_puyosCount >= _puyoComboCount)
        {
            foreach (Vector2Int puyo in _chainedPuyos)
            {
                DestroyPuyo(puyo.x, puyo.y);
                //for (int y = puyo.y; y < _gridHeight-1; y++)
                //{
                //    Debug.Log(y);
                //}
            }
        }
    }
    #endregion

    #region Player Horizontal Movements
    private void Update()
    {
        PlayerMovements();
    }

    private void PlayerMovements()
    {
        if (_playablePuyoPosition == null) return;
        if (_puyoGrid[_playablePuyoPosition.x, _playablePuyoPosition.y].CellType == CellElement.Type.GroundedPuyo) return;

        int leftAxis = Input.GetKeyDown(KeyCode.LeftArrow) ? 1 : 0;
        int rightAxis = Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0;
        if (rightAxis - leftAxis == 0) return;

        Vector2Int targetPosition = new Vector2Int(_playablePuyoPosition.x + rightAxis - leftAxis, _playablePuyoPosition.y);

        if (targetPosition.x < 0 || targetPosition.x > _gridWidth - 1) return;

        UpdatePuyoPosition(_playablePuyoPosition.x, _playablePuyoPosition.y, targetPosition.x, targetPosition.y);
    }
    #endregion

    private Vector2 CellToWorldPosition(int xGrid, int yGrid)
    {
        return new Vector2(xGrid, yGrid) * _gridSize;
    }
}
