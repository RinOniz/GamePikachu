using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCheck
{
    private int[,] idGrid;
    private int totalCols;
    private int totalRows;
    //private int currentNodesExpanded;

    private bool CheckLineCol(int row, int col1, int col2)
    {
        int min = Mathf.Min(col1, col2);
        int max = Mathf.Max(col1, col2);

        for (int c = min + 1; c < max; c++)
        {
            if (idGrid[row, c] != -1) return false;
        }
        return true;
    }

    private bool CheckLineRow(int col, int row1, int row2)
    {
        int min = Mathf.Min(row1, row2);
        int max = Mathf.Max(row1, row2);

        for (int r = min + 1; r < max; r++)
        {
            if (idGrid[r, col] != -1) return false;
        }
        return true;
    }

    public bool CheckConnectionLineCheck(Position p1, Position p2, bool drawPath)
    {
        //currentNodesExpanded = 0;
        List<Position> path = new List<Position>(); 

        if (p1.row == p2.row && CheckLineCol(p1.row, p1.col, p2.col))
        {
            if (drawPath) // DrawPath(new List<Position> { p1, p2 });
            return true;
        }
        if (p1.col == p2.col && CheckLineRow(p1.col, p1.row, p2.row))
        {
            if (drawPath) // DrawPath(new List<Position> { p1, p2 });
            return true;
        }

        if (idGrid[p1.row, p2.col] == -1 &&
            CheckLineCol(p1.row, p1.col, p2.col) &&
            CheckLineRow(p2.col, p1.row, p2.row))
        {
            if (drawPath) // DrawPath(new List<Position> { p1, new Position(p1.row, p2.col), p2 });
            return true;
        }

        if (idGrid[p2.row, p1.col] == -1 &&
            CheckLineRow(p1.col, p1.row, p2.row) &&
            CheckLineCol(p2.row, p1.col, p2.col))
        {
            if (drawPath) // DrawPath(new List<Position> { p1, new Position(p2.row, p1.col), p2 });
            return true;
        }

        for (int c = 0; c < totalCols; c++)
        {
            if (c != p1.col && c != p2.col && idGrid[p1.row, c] == -1 && idGrid[p2.row, c] == -1)
            {
                if (CheckLineCol(p1.row, p1.col, c) &&   
                    CheckLineCol(p2.row, p2.col, c) &&    
                    CheckLineRow(c, p1.row, p2.row))      
                {
                    if (drawPath) // DrawPath(new List<Position> { p1, new Position(p1.row, c), new Position(p2.row, c), p2 });
                    return true;
                }
            }
        }

        for (int r = 0; r < totalRows; r++)
        {
            if (r != p1.row && r != p2.row && idGrid[r, p1.col] == -1 && idGrid[r, p2.col] == -1)
            {
                if (CheckLineRow(p1.col, p1.row, r) &&     
                    CheckLineRow(p2.col, p2.row, r) &&     
                    CheckLineCol(r, p1.col, p2.col))       
                {
                    if (drawPath) // DrawPath(new List<Position> { p1, new Position(r, p1.col), new Position(r, p2.col), p2 });
                    return true;
                }
            }
        }

        return false; 
    }
}
