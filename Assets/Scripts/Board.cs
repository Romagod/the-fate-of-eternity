using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public List<Cell> Cells;
    // Start is called before the first frame update
    void Start()
    {
        Cells = new List<Cell>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int AddCell(Cell cell)
    {
        int count = Cells.Count;
        Cells.Add(cell);
        return count;
    }
}
