using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public int row, col;

    public Position(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public string GetMove(Position previousPosition)
    {
        int deltaRow = row - previousPosition.row;
        int deltaCol = col - previousPosition.col;

        if (deltaCol == 0)
        {
            if (deltaRow == 1)
            {
                return "D";
            }
            else if (deltaRow == -1)
            {
                return "U";
            }
        }
        else if (deltaRow == 0)
        {
            if (deltaCol == 1)
            {
                return "R";
            }
            else if (deltaCol == -1)
            {
                return "L";
            }
        }

        return "";
    }
}
