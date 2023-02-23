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



    private Cell[,] _cells; //Cell�p�̔z��
    private bool _isToch = false;
    private bool _is1thToch = false;

    public bool isGameClear = false;
    public bool isGameOver = false;
    
    GameObject _checkCell;
    
    // Start is called before the first frame update
    void Start()
    {


        

        //���n�Z������
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

        //gameover�̎�
        if (isGameOver)

        {

            _gameOver.SetActive(true);


            return;
        }

        //gameclear�̎�
        if (isGameClear)
        {

            _gameClear.SetActive(true);
            return;
        }


        if (!_isToch) { return; } //�N���b�N����
        _isToch = false;


        var cell = _checkCell;
        var Cellcs = cell.GetComponent<Cell>();
        
        //�ŏ��̃^�[���͒n����ݒu���Ȃ����ŏ���gameover�ɂȂ�Ȃ�
        if (!_is1thToch)
        {
            _is1thToch = true;
            
            //�n���ݒu
            SetMine(ref _cells, Cellcs.row, Cellcs.col);
            //�Z����ݒu�A�X�V
            SetCell(ref _cells);

        }

        Cellcs.isOpen = true;

        //Game over or clear ����
        isGameOver = CheckGameOver(ref Cellcs);
        isGameClear = CheckGameClear(ref _cells);

        //�Z�������W�J
        AutoOpenCells(ref _cells, Cellcs.row, Cellcs.col);

    }

    private CellState GetMineCount(ref Cell[,] cells, int row, int column)
    {
        //var cell = _cells[row, column];
        if(cells[row, column] == null) { Debug.Log("Error"); }
        if (cells[row, column].cellState == CellState.Mine) { return CellState.Mine; }

        // ���͂̃Z���̒n���̐��𐔂���
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

        //���N���b�N�ŋN����C�x���g����
        if(eventData.button == PointerEventData.InputButton.Left)
        {

            var cell = eventData.pointerCurrentRaycast.gameObject;
       
            if(cell.GetComponent<Cell>() == null) { return; } //�Z���ȊO�͏������Ȃ�

            var Cellcs = cell.GetComponent<Cell>();

            if(Cellcs.isOpen == true) { return; }
            _checkCell = cell;
            _isToch = true;

        }
        


        //�E�N���b�N
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
            Debug.LogError($"�n�����̓Z������菭�Ȃ��ݒ肵�Ă�������\n" + $"�n����={_mineCount}, �Z����={cells.Length}");
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
        //None�Z�����m�̎����W�J

        if (1 <= (int)cells[row, col].cellState || 8 >= (int) cells[row, col].cellState )
        {
            cells[row, col].isOpen = true;
        }

        if(cells[row, col].cellState != CellState.None) { return; }
        cells[row, col].isOpen = true;


        //�ċA�֐��@ r = -1,0,1, c = -1, 0 ,1�ŒT�����Ă��� 
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


    //�{�^���p�֐��@�V�[���̍ēǂݍ��݂��Ă邾��
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



