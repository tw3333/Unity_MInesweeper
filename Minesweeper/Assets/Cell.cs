using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;


public enum CellState
{
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,


    Mine = -1,
    Flag = -2,
    Unopen = -3,
}

public enum CoverCellState
{
    None = 0,
    Flag = 1,

    End,

}

public class Cell : MonoBehaviour
{
 
    [SerializeField]
    private TextMeshProUGUI _view = null;


    [SerializeField]
    private CellState _cellState = CellState.None;

    [SerializeField]
    private CoverCellState _coverCellState = CoverCellState.None;


    [SerializeField]
    private Color _coverColor = Color.blue;


    [SerializeField]
    //private Image Image = red;


   

    private int _row;
    public int row { get { return _row; } set { _row = value; } }

    private int _col;
    public int col { get { return _col; } set { _col = value; } }

    private bool _isOpen = false;
    public bool isOpen { get { return _isOpen; } set { _isOpen = value; } }




    public CellState cellState
    {
        get => _cellState;
        set
        {
            _cellState = value;


            OnCellStateChanged();
        }
    }

    public CoverCellState coverCellState
    {
        get => _coverCellState;

        set
        {
            _coverCellState = value;

            OnCellStateChanged();
        }

        
    }

    



    // Start is called before the first frame update
    private void OnValidate()
    {
        OnCellStateChanged();

    }

    // Update is called once per frame
    void Update()
    {

        OnCellStateChanged();

    }



    private void OnCellStateChanged()
    {
        if ( _view == null) { return; }


        //CoverCellState
        if (!_isOpen)
        {

            //_img.color = _cover;
            this.GetComponent<Image>().color = new Color(0, 103, 192);
                
            if (_coverCellState == CoverCellState.None)
            {
                _view.text = "";
                    

            }
            
            else if (_coverCellState == CoverCellState.Flag)
            {

                _view.text = "F";
                _view.color = Color.yellow;


                this.GetComponent<Image>().color = new Color(255, 0, 0);


            }

            return;
        }



     
        if (_isOpen)
        {

            this.GetComponent<Image>().color = new Color(255, 255, 255);


            if (_cellState == CellState.None)
            {
                _view.text = "";
                //if(Img == null) { Debug.Log("NULL"); return; }

            }
            else if (_cellState == CellState.Mine)
            {
                _view.text = "X";
                _view.color = Color.red;

            }
            else
            {
                _view.text = ((int)_cellState).ToString();
                _view.color = Color.blue;

            }

            

        }


    }
}
