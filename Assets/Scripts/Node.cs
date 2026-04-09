using System;
using System.Collections.Generic;

// Luu tru lich su, tinh so lan re khi them vi tri moi
public class Node
{
    public int row, col, turnCount;
    public string pathHistory;
    public List<Position> positions;
    public bool isStartNode;

    public Node(int _row, int _col)
    {
        row = _row;
        col = _col;
    }

    // Them mot vi tri vao danh sach vi tri
    public void AddPosition(Position position)
    {
        if (positions.Count == 0) 
        {
            pathHistory = "";
        }
        else  
        {
            Position lastPosition = positions[positions.Count - 1];

            string move = position.GetMove(lastPosition);

            if (pathHistory != "")
            {
                string lastMove = pathHistory.Substring(pathHistory.Length - 1);

                if (lastMove != move)
                {
                    turnCount++;
                }
            }

            pathHistory += move;
        }

        positions.Add(position);
    }
}
