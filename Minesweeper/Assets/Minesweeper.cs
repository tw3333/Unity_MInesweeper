using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Minesweeper : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int _rows = 1;

    [SerializeField]
    private int _columns = 1;

    [SerializeField]
    private int _mineCount = 1;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField]
    private Cell _cellPrefab = null;


    [SerializeField]
    public GameObject _gameOver;

    [SerializeField]
    public GameObject _gameClear;



    public TextMeshPro _timer;
    public TextMeshPro _flag;



    private Cell[,] _cells; //Cell用の配列
    private bool _isToch = false;
    private bool _is1thToch = false;

    public bool isGameClear = false;
    public bool isGameOver = false;
    
    GameObject _checkCell;
    
    // Start is called before the first frame update
    void Start()
    {


        

        //下地セル完成
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        _cells = new Cell[_rows, _columns];

        var parent = _gridLayoutGroup.gameObject.transform;
        for (var r = 0; r < _rows; r++)
        {
            for(var c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                cell.row = r;
                cell.col = c;
  
                _cells[r, c] = cell;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        //gameoverの時
        if (isGameOver)

        {

            _gameOver.SetActive(true);


            return;
        }

        //gameclearの時
        if (isGameClear)
        {

            _gameClear.SetActive(true);
            return;
        }


        if (!_isToch) { return; } //クリック判定
        _isToch = false;


        var cell = _checkCell;
        var Cellcs = cell.GetComponent<Cell>();
        
        //最初のターンは地雷を設置しない→最初はgameoverにならない
        if (!_is1thToch)
        {
            _is1thToch = true;
            
            //地雷設置
            SetMine(ref _cells, Cellcs.row, Cellcs.col);
            //セルを設置、更新
            SetCell(ref _cells);

        }

        Cellcs.isOpen = true;

        //Game over or clear 判定
        isGameOver = CheckGameOver(ref Cellcs);
        isGameClear = CheckGameClear(ref _cells);

        //セル自動展開
        AutoOpenCells(ref _cells, Cellcs.row, Cellcs.col);

    }

    private CellState GetMineCount(ref Cell[,] cells, int row, int column)
    {
        //var cell = _cells[row, column];
        if(cells[row, column] == null) { Debug.Log("Error"); }
        if (cells[row, column].cellState == CellState.Mine) { return CellState.Mine; }

        // 周囲のセルの地雷の数を数える
        int count = 0;
        for(int dr = -1; dr <= 1; dr++)
        {
            for(int dc = -1; dc <= 1; dc++)
            {

                int temp_r = row + dr;
                int temp_c = column + dc;

                if(temp_r < 0 || temp_r >= cells.GetLength(0)) { continue; }
                if(temp_c < 0 || temp_c >= cells.GetLength(1)) { continue; }

                var cell = cells[temp_r, temp_c];

                if(cell.cellState == CellState.Mine) { count++; }



            }
        }

        return (CellState)count;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //var cell = eventDate.pointerCurrentRaycast.gameObject;
        //var image = cell.GetComponent<Image>();
        //image.color = Color.blue;
        ////cell.CellState == CellState.Flag;

        //左クリックで起きるイベント処理
        if(eventData.button == PointerEventData.InputButton.Left)
        {

            var cell = eventData.pointerCurrentRaycast.gameObject;
       
            if(cell.GetComponent<Cell>() == null) { return; } //セル以外は処理しない

            var Cellcs = cell.GetComponent<Cell>();

            if(Cellcs.isOpen == true) { return; }
            _checkCell = cell;
            _isToch = true;

        }
        


        //右クリック
        else if (eventData.button == PointerEventData.InputButton.Right)
        {

            var cell = eventData.pointerCurrentRaycast.gameObject;
            if(cell.GetComponent<Cell>() == null) { return; }
            var Cellcs = cell.GetComponent<Cell>();
            if(Cellcs.isOpen == true) { return; }

            Cellcs.coverCellState++;    

            if(Cellcs.coverCellState >= CoverCellState.End){ Cellcs.coverCellState = CoverCellState.None;}
            

        }



    }

   
    
    void SetMine(ref Cell[,] cells, int _row, int _col)
    {

        //if(_mineCount >= cells.GetLength(0) * cells.GetLength(1)))  { }

        if (_mineCount > cells.Length)
        {
            Debug.LogError($"地雷数はセル数より少なく設定してください\n" + $"地雷数={_mineCount}, セル数={cells.Length}");
        }

        for(var i = 0; i < _mineCount; i++)
        {
            int r = 0;
            int c = 0;

            while(true)
            {

                r = Random.Range(0,cells.GetLength(0));
                c = Random.Range(0,cells.GetLength(1));
                if(r == _row && c == _col) { continue; }

                var cell = _cells[r,c];

                if(cell.cellState == CellState.Mine ) { continue; }

                cell.cellState = CellState.Mine;
                break;


            }

        }

    }

    void SetCell(ref Cell[,] cells)
    {
        for (var r = 0; r < cells.GetLength(0); r++)
        {

            for (var c = 0; c < cells.GetLength(1); c++)
            {

                cells[r, c].cellState = (CellState)GetMineCount(ref cells, r, c);

            }

        }


    }



    private void AutoOpenCells(ref Cell[,] cells, int row , int col)
    {
        //Noneセル同士の自動展開

        if (1 <= (int)cells[row, col].cellState || 8 >= (int) cells[row, col].cellState )
        {
            cells[row, col].isOpen = true;
        }

        if(cells[row, col].cellState != CellState.None) { return; }
        cells[row, col].isOpen = true;


        //再帰関数　 r = -1,0,1, c = -1, 0 ,1で探索している 
        for(int dr = -1; dr <= 1; dr++)
        {
            for(int dc = -1; dc <= 1; dc++)
            {

                int temp_r = row + dr;
                int temp_c = col + dc;

                if(row == temp_r && col == temp_c) { continue; }
                if( temp_r < 0 || temp_r >= cells.GetLength(0)) { continue; }
                if (temp_c < 0 || temp_r >= cells.GetLength(1)) { continue; }

                if(!cells[temp_r, temp_c].isOpen) { AutoOpenCells(ref cells, temp_r, temp_c); }


            }
        }

        return;

    }


    private bool CheckGameOver(ref Cell cell)
    {
        if(cell.cellState == CellState.Mine) { return true; }

        return false;
    }

    private bool CheckGameClear(ref Cell[,] cells)
    {
        int num = 0;
        for(int r = 0; r < cells.GetLength(0); r++)
        {
            for(int c = 0; c < cells.GetLength(1); c++)
            {

                if(cells[r,c].isOpen) { num++; }

            }
        }

        if(num == cells.GetLength(0) * cells.GetLength(1) - _mineCount) { return true; }

        return false;
    }


    //ボタン用関数　シーンの再読み込みしてるだけ
    public void  Retry()
    {
        isGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _gameOver.SetActive(false);

    }

    public void MorePlay()
    {
        isGameClear = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _gameOver.SetActive(false);

    }
}



