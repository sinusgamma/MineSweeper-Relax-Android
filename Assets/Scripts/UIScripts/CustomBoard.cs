using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CustomBoard : MonoBehaviour {

    public Slider sliderCols;
    public Slider sliderRows;
    public Slider sliderMines;

    public Text textCols;
    public Text textRows;
    public Text textMines;

    private int colNumber;
    private int rowNumber;
    private int mineNumber;

    public void setMinMaxMines()  // set the maximum/minimum number of mines for the tabel
    {
        sliderMines.minValue = sliderCols.value * sliderRows.value * 0.15f;
        sliderMines.maxValue = sliderCols.value * sliderRows.value * 0.26f;
    }

    // set the grids from board menu  -  other grids are set in global control
    public void gridSetCustom()
    {
        GlobalControl.Instance.rows = (int)sliderRows.value;
        GlobalControl.Instance.columns = (int)sliderCols.value;
        GlobalControl.Instance.mines = (int)sliderMines.value;
    }



    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        textCols.text = "Rows: " + sliderCols.value.ToString();  // show the col slider values    -   changed the name, why so behaviour????
        textRows.text = "Columns: " + sliderRows.value.ToString();   // show the row slider values

        setMinMaxMines();

        textMines.text = "Mines: " + sliderMines.value.ToString();
    }
}
