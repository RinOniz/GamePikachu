using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [SerializeField] public GameObject boardGameObject;
    [SerializeField] private GameObject tilePrefab;

    [SerializeField] private Sprite[] classicSprites;
    [SerializeField] private Sprite[] funnySprites;
    [SerializeField] private Sprite[] botSprites;

    private Sprite[] tileSprites;

    private GameObject[,] tileObjects;
    private GameObject hintTileOne;
    private GameObject hintTileTwo;

    private const int totalCols = 18;
    private const int totalRows = 10;
    public static int totalTiles = 8 * 16;

    private float startX = -9.2f;
    private float startY = 4.5f;
    private int[,] idGrid;

    private List<int> remainingPairs;

    public int currNodesExpanded = 0;
    public long totalNodesBot = 0; 
    public float totalTimeBot = 0f; 
    public int botMoveCount = 0; 

    private void Start()
    {
        int mode = PlayerPrefs.GetInt("GameMode", 0);

        if (mode == 0)
        {
            classicSprites = Resources.LoadAll<Sprite>("Items_0");
            tileSprites = classicSprites;
        }
        else if (mode == 1)
        {
            funnySprites = Resources.LoadAll<Sprite>("Items_1");
            tileSprites = funnySprites;
        }
        else if (mode == 2)
        {
            botSprites = Resources.LoadAll<Sprite>("Items_0");
            tileSprites = botSprites;

            Random.InitState(12345);
        }

        PickRandomPairs();
        CreateBoard(ShuffleTiles());
        ClearHintTiles();

        if (mode == 2)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
        }
    }

    public int GetTotalTiles()
    {
        return totalTiles;
    }

    private void PickRandomPairs()
    {
        remainingPairs = new List<int>();

        for (int i = 0; i < totalTiles / 2; i++)
        {
            int pairID = Random.Range(0, tileSprites.Length);

            remainingPairs.Add(pairID);
        }
    }

    private List<int> ShuffleTiles()
    {
        List<int> duplicatedList = new List<int>();

        for (int i = 0; i < remainingPairs.Count; i++)
        {
            duplicatedList.AddRange(new List<int> { remainingPairs[i], remainingPairs[i] });    
        }

        // Thuat toan Fisher-Yates Shuffle  
        for (int i = 0; i < duplicatedList.Count; i++)
        {
            int temp = duplicatedList[i];
            int randomID = Random.Range(i, duplicatedList.Count);

            duplicatedList[i] = duplicatedList[randomID];
            duplicatedList[randomID] = temp;
        }

        return duplicatedList;
    }

    private void CreateBoard(List<int> duplicatedID)
    {
        tileObjects = new GameObject[totalRows, totalCols];
        idGrid = new int[totalRows, totalCols];

        int i = 0;

        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalCols; col++)
            {
                if (col == 0 || col == totalCols - 1 || row == 0 || row == totalRows - 1)
                {
                    tileObjects[row, col] = null;
                    idGrid[row, col] = -1;  
                }
                else
                {
                    int valueID = duplicatedID[i];

                    Vector3 position = new Vector3(startX + col, startY - row, 0.0f);
                    GameObject newTileObj = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
                    newTileObj.name = "Tile[" + row + ", " + col + "]";

                    Tile tileData = newTileObj.GetComponent<Tile>();
                    tileData.row = row;
                    tileData.col = col;
                    tileData.id = valueID;

                    SpriteRenderer spriteRender = newTileObj.GetComponent<SpriteRenderer>();
                    spriteRender.sortingOrder = 2;
                    spriteRender.sprite = tileSprites[tileData.id];

                    newTileObj.transform.localScale = new Vector3(0.76f, 0.76f, 1f);
                    newTileObj.transform.SetParent(boardGameObject.transform);

                    tileObjects[row, col] = newTileObj;
                    idGrid[row, col] = valueID;

                    i++;
                }
            }
        }
    }

    public bool CheckConnection(Position startPos, Position endPos, bool drawPath)
    {
        if (PlayerPrefs.GetInt("GameMode", 0) != 2)
        {
            return CheckConnectionBFS(startPos, endPos, drawPath);
        }

        string algo = PlayerPrefs.GetString("AI_Algorithm", "BFS");

        switch (algo)
        {
            case "DFS": 
                return CheckConnectionDFS(startPos, endPos, drawPath);
            case "ASTAR": 
                return CheckConnectionAStar(startPos, endPos, drawPath);
            default: 
                return CheckConnectionBFS(startPos, endPos, drawPath);
        }
    }

    public bool CheckConnectionBFS(Position startPos, Position endPos, bool drawPath)
    {
        //Debug.Log("Dang chay BFS");

        currNodesExpanded = 0;

        // neu vi tri dich bi chan boi 4 o xung quanh thi khong the di den duoc
        if (IsEndPositionBlocked(startPos, endPos))
        {
            return false;
        }

        Queue<Node> nodes = new Queue<Node>();
        Node start = new Node(startPos.row, startPos.col);

        start.pathHistory = "";   // lich su di chuyen
        start.positions = new List<Position>(); // danh sach duong di 
        start.positions.Add(startPos);
        start.turnCount = 0;   // so lan re 
        start.isStartNode = true;   // danh dau (flag) node bat dau

        nodes.Enqueue(start);   // them node start vao queue

        while (nodes.Count != 0)
        {
            Node currNode = nodes.Dequeue(); // lay node o dau hang doi

            currNodesExpanded++;
            
            if (currNode.turnCount > 2) // toi da 2 lan re
            {
                continue;   // bo qua node hien tai
            }

            if (currNode.row == endPos.row && currNode.col == endPos.col) // win condition, neu node hien tai la vi tri dich
            {
                List<Position> path = currNode.positions;

                if (drawPath) 
                {
                    if (idGrid[startPos.row, startPos.col] == idGrid[endPos.row, endPos.col])
                    {
                        GameObject.Find("Path").GetComponent<Path>().DrawPath(path);
                    }
                }

                return true;   
            }

            // vet dau loang ra 4 huong
            int[,] directions = new int[4, 2] {
                { 0, 1 },   // phai
                { 0, -1 },  // trai
                { 1, 0 },   // xuong
                { -1, 0 }   // len
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int row = currNode.row + directions[i, 0];
                int col = currNode.col + directions[i, 1];

                if (!IsValid(row, col))  // check co nam trong idGrid khong
                {
                    continue;
                }

                int currNodeValue = idGrid[currNode.row, currNode.col];  // gia tri hien tai

                // node start la node dac biet, ngoai le de BFS bat dau loang, tu node thu 2 tro di chi dc di qua o trong (-1)
                if (currNode.isStartNode || currNodeValue == -1)  // tu node start di dc moi huong, cac node sau chi di qua o trong (-1)
                {
                    bool skip = false;

                    List<Position> positions = new List<Position>();

                    foreach (Position position in currNode.positions)
                    {
                        if (position.row == row && position.col == col)
                        {
                            skip = true;
                        }

                        positions.Add(position);
                    }

                    if (skip)
                    {
                        continue;
                    }

                    Node node = new Node(row, col);

                    node.turnCount = currNode.turnCount ;
                    node.pathHistory = (string)currNode.pathHistory.Clone();
                    node.positions = positions;

                    node.AddPosition(new Position(row, col));
                    nodes.Enqueue(node);
                }
            }
        }

        return false;   // nodes.Count == 0 -> khong tim thay dich

        /* 
        - Loang 4 huong tu start
        - Chi di qua o trong (-1) neu khong phai o start   
        - Gioi han so lan re <= 2
        - Luu duong di
        - Gap end -> ve duong di -> return true
        */
    }

    public bool CheckConnectionDFS(Position startPos, Position endPos, bool drawPath)
    {
        //Debug.Log("Dang chay DFS");

        currNodesExpanded = 0;

        if (IsEndPositionBlocked(startPos, endPos))
        {
            return false;
        }

        // Dung Stack thay vi Queue giong BFS
        Stack<Node> nodes = new Stack<Node>();
        Node start = new Node(startPos.row, startPos.col);

        start.pathHistory = "";
        start.positions = new List<Position>();
        start.positions.Add(startPos);
        start.turnCount = 0;
        start.isStartNode = true;

        nodes.Push(start); // Push thay vi Enqueue

        while (nodes.Count != 0)
        {
            Node currNode = nodes.Pop(); // Pop thay vi Dequeue

            currNodesExpanded++;

            if (currNode.turnCount > 2)
            {
                continue;
            }

            if (currNode.row == endPos.row && currNode.col == endPos.col)
            {
                if (drawPath)
                {
                    if (idGrid[startPos.row, startPos.col] == idGrid[endPos.row, endPos.col])
                    {
                        GameObject.Find("Path").GetComponent<Path>().DrawPath(currNode.positions);
                    }
                }
                return true;
            }

            int[,] directions = new int[4, 2] {
                { 0, 1 },
                { 0, -1 },
                { 1, 0 },
                { -1, 0 } 
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int row = currNode.row + directions[i, 0];
                int col = currNode.col + directions[i, 1];

                if (!IsValid(row, col))
                {
                    continue;
                }

                int currNodeValue = idGrid[currNode.row, currNode.col];

                if (currNode.isStartNode || currNodeValue == -1)
                {
                    bool skip = false;

                    List<Position> positions = new List<Position>();

                    foreach (Position position in currNode.positions)
                    {
                        if (position.row == row && position.col == col)
                        {
                            skip = true;
                        }

                        positions.Add(position);
                    }

                    if (skip)
                    {
                        continue;
                    }

                    Node node = new Node(row, col);

                    node.turnCount = currNode.turnCount;
                    node.pathHistory = (string)currNode.pathHistory.Clone();
                    node.positions = positions;
                    node.AddPosition(new Position(row, col));

                    nodes.Push(node); // Push thay vi Enqueue
                }
            }
        }
        return false;
    }

    public bool CheckConnectionAStar(Position startPos, Position endPos, bool drawPath)
    {
        //Debug.Log("Dang chay A*");

        currNodesExpanded = 0;

        if (IsEndPositionBlocked(startPos, endPos))
        {
            return false;
        }

        // A* dung List de chua cac node dang cho duyet (Open Set)
        List<Node> openSet = new List<Node>();

        Node start = new Node(startPos.row, startPos.col);

        start.pathHistory = "";
        start.positions = new List<Position>();
        start.positions.Add(startPos);
        start.turnCount = 0;
        start.isStartNode = true;

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            // Di tim node co chi phi F thap nhat de duyet truoc
            int lowestF = int.MaxValue;
            int bestIndex = 0;

            for (int i = 0; i < openSet.Count; i++)
            {
                // g(n): quang duong da di (so o trong positions)
                int g = openSet[i].positions.Count;

                // H(n): heuristic - khoang cach manhattan den dich (khong tinh den re) = |x1 - x2| + |y1 - y2|
                int h = Mathf.Abs(endPos.row - openSet[i].row) + Mathf.Abs(endPos.col - openSet[i].col);

                // Max ban co la ~ 30 nen co the dat 40 la max ping, an toan thi 50
                int turnPenalty = openSet[i].turnCount * 50;

                // f(n) = g(n) + h(n) + tien phat be lai
                int f = g + h + turnPenalty;

                if (f < lowestF)
                {
                    lowestF = f;
                    bestIndex = i;
                }
            }

            // Lay node tot nhat ra khỏi open set de duyet
            Node currNode = openSet[bestIndex];
            openSet.RemoveAt(bestIndex);

            currNodesExpanded++;

            if (currNode.turnCount > 2)
            {
                continue;
            }

            if (currNode.row == endPos.row && currNode.col == endPos.col)
            {
                if (drawPath)
                {
                    if (idGrid[startPos.row, startPos.col] == idGrid[endPos.row, endPos.col])
                    {
                        GameObject.Find("Path").GetComponent<Path>().DrawPath(currNode.positions);
                    }
                }

                return true;
            }

            int[,] directions = new int[4, 2] {
                { 0, 1 },
                { 0, -1 },
                { 1, 0 },
                { -1, 0 }
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int row = currNode.row + directions[i, 0];
                int col = currNode.col + directions[i, 1];

                if (!IsValid(row, col))
                {
                    continue;
                }

                int currNodeValue = idGrid[currNode.row, currNode.col];

                if (currNode.isStartNode || currNodeValue == -1)
                {
                    bool skip = false;

                    List<Position> positions = new List<Position>();

                    foreach (Position position in currNode.positions)
                    {
                        if (position.row == row && position.col == col)
                        {
                            skip = true;
                        }

                        positions.Add(position);
                    }

                    if (skip)
                    {
                        continue;
                    }

                    Node node = new Node(row, col);

                    node.turnCount = currNode.turnCount;
                    node.pathHistory = (string)currNode.pathHistory.Clone();
                    node.positions = positions;
                    node.AddPosition(new Position(row, col));

                    openSet.Add(node); // Them vao open set de duyet sau
                }
            }
        }

        return false;
    }
    
    // Tim kiem dua tren kinh nghiem choi
    public Position[] FindValidPairs()
    {
        // Cap lien ke nhau 
        Position[] adjacentPair = FindAdjacentPairs();
        if (adjacentPair != null)
        {
            Debug.Log("Lien ke");
            return adjacentPair;
        }

        // Cap o mep
        Position[] edgePair = FindEdgePairs();
        if (edgePair != null)
        {
            Debug.Log("Mep");
            return edgePair;
        }

        // Vet can
        Debug.Log("Vet can");
        return FindNormalPairs();
    }

    private Position[] FindAdjacentPairs()
    {
        for (int r = 1; r < totalRows - 1; r++)
        {
            for (int c = 1; c < totalCols - 1; c++)
            {
                int id = idGrid[r, c];

                if (id == -1) 
                {
                    continue;
                }

                // Check con ben phai
                if (c + 1 < totalCols - 1 && idGrid[r, c + 1] == id)
                {
                    if (CheckConnection(new Position(r, c), new Position(r, c + 1), false)) 
                    {
                        return new Position[] { new Position(r, c), new Position(r, c + 1) };
                    }
                }

                // Check con ben duoi
                if (r + 1 < totalRows - 1 && idGrid[r + 1, c] == id)
                {
                    if (CheckConnection(new Position(r, c), new Position(r + 1, c), false))
                    {
                        return new Position[] { new Position(r, c), new Position(r + 1, c) };
                    }
                }
            }
        }

        return null;
    }

    private Position[] FindEdgePairs()
    {
        List<Position> edgeTiles = new List<Position>();

        for (int r = 1; r < totalRows - 1; r++)
        {
            for (int c = 1; c < totalCols - 1; c++)
            {
                if (idGrid[r, c] != -1)
                {
                    int depth = GetLayerDepth(r, c);

                    // chi lay nhung o trong 2 lop mep ngoai
                    if (depth <= 2)
                    {
                        edgeTiles.Add(new Position(r, c));
                    }
                }
            }
        }

        // lop mep ngoai duoc uu tien check truoc 
        edgeTiles.Sort((p1, p2) =>
        {
            int depth1 = GetLayerDepth(p1.row, p1.col);
            int depth2 = GetLayerDepth(p2.row, p2.col);

            return depth1.CompareTo(depth2);
        });

        for (int i = 0; i < edgeTiles.Count; i++)
        {
            for (int j = i + 1; j < edgeTiles.Count; j++)
            {
                Position p1 = edgeTiles[i];
                Position p2 = edgeTiles[j];

                if (idGrid[p1.row, p1.col] == idGrid[p2.row, p2.col])
                {
                    if (CheckConnection(p1, p2, false))
                    {
                        return new Position[] { p1, p2 };
                    }
                }
            }
        }

        return null;
    }

    private Position[] FindNormalPairs()
    {
        for (int r1 = 0; r1 < totalRows; r1++)
        {
            for (int c1 = 0; c1 < totalCols; c1++)
            {
                int id1 = idGrid[r1, c1];

                if (id1 == -1)
                {
                    continue;
                }

                for (int r2 = r1; r2 < totalRows; r2++)
                {
                    int startCol = (r1 == r2) ? c1 + 1 : 0; // Neu cung hang thi chi check con ben phai, neu khac hang thi check tu dau hang

                    for (int c2 = startCol; c2 < totalCols; c2++)
                    {
                        int id2 = idGrid[r2, c2];

                        if (id1 == id2)
                        {
                            if (CheckConnection(new Position(r1, c1), new Position(r2, c2), false))
                            {
                                return new Position[] { new Position(r1, c1), new Position(r2, c2) };
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    public void Clear(Position firstPos, Position secondPos)
    {
        int valueID = idGrid[firstPos.row, firstPos.col];

        idGrid[firstPos.row, firstPos.col] = -1;
        idGrid[secondPos.row, secondPos.col] = -1;

        tileObjects[firstPos.row, firstPos.col] = null;
        tileObjects[secondPos.row, secondPos.col] = null;

        ClearHintTiles();
        RestoreHintTilesColor();

        remainingPairs.Remove(valueID);

        int currentLevel = PlayerPrefs.GetInt("GameLevel", 1);

        if (currentLevel == 2)
        {
            ShiftDown(); 
        }
        else if (currentLevel == 3)
        {
            ShiftUp(); 
        }

        if (remainingPairs.Count > 0 && HasAnyMoves() == false)
        {
            Debug.Log("Het duong, tu dong Change");

            Change();
        }
    }

    public void Change()
    {
        List<int> newBoardLayout = ShuffleTiles();

        int indexID = 0;

        for (int row = 0; row < tileObjects.GetLength(0); row++)
        {
            for (int col = 0; col < tileObjects.GetLength(1); col++)
            {
                GameObject tile = tileObjects[row, col];

                if (tile == null)
                {
                    continue;
                }

                int id = newBoardLayout[indexID];

                Tile tileData = tile.GetComponent<Tile>();
                tileData.id = id;

                SpriteRenderer spriteRender = tile.GetComponent<SpriteRenderer>();
                spriteRender.sprite = tileSprites[tileData.id];

                idGrid[row, col] = id;
                indexID++;
            }
        }

        ClearHintTiles();
    }

    public bool GetHint()
    {
        Position[] pair = FindValidPairs();

        if (pair != null)
        {
            hintTileOne = tileObjects[pair[0].row, pair[0].col];
            hintTileTwo = tileObjects[pair[1].row, pair[1].col];

            Color color = new Color(113f / 255f, 204f / 255f, 86f / 255f, 1.0f);
            hintTileOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
            hintTileTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

            return true;
        }

        return false;
    }

    private int GetLayerDepth(int r, int c)
    {
        int distTop = r;
        int distBottom = (totalRows - 1) - r;
        int distLeft = c;
        int distRight = (totalCols - 1) - c;

        return Mathf.Min(distTop, distBottom, distLeft, distRight);
    }

    // Ham di chuyen vien gach tu vi tri cu (fromRow, col) den vi tri moi (toRow, col)
    private void MoveTile(int fromRow, int col, int toRow)
    {
        // chuyen du lieu logic
        idGrid[toRow, col] = idGrid[fromRow, col];
        idGrid[fromRow, col] = -1; // o cu thanh o trong (-1)

        tileObjects[toRow, col] = tileObjects[fromRow, col];
        tileObjects[fromRow, col] = null;

        // update thong tin tile
        GameObject tileObj = tileObjects[toRow, col];

        Tile tileData = tileObj.GetComponent<Tile>();
        tileData.row = toRow;

        tileObj.transform.position = new Vector3(startX + col, startY - toRow, 0.0f);
        tileObj.name = "Tile[" + toRow + ", " + col + "]";
    }

    private void ShiftDown()
    {
        // Duyet tung cot, bo 2 cot hai ben mep
        for (int c = 1; c < totalCols - 1; c++)
        {
            // Duyet tu duoi len tren, bo hang mep duoi va tren
            for (int r = totalRows - 2; r >= 1; r--)
            {
                if (idGrid[r, c] == -1)
                {
                    // Di tim vien gach dau tien nam ben tren no
                    for (int k = r - 1; k >= 1; k--)
                    {
                        if (idGrid[k, c] != -1) // Tim thay
                        {
                            MoveTile(k, c, r); // Chuyen xuong vi tri r

                            break;
                        }
                    }
                }
            }
        }
    }

    private void ShiftUp()
    {
        for (int c = 1; c < totalCols - 1; c++)
        {
            // Duyet tu tren xuong duoi, bo hang mep tren va duoi
            for (int r = 1; r < totalRows - 1; r++)
            {
                if (idGrid[r, c] == -1)
                {
                    // Di tim vien gach dau tien nam ben duoi no
                    for (int k = r + 1; k < totalRows - 1; k++)
                    {
                        if (idGrid[k, c] != -1)
                        {
                            MoveTile(k, c, r); // Chuyen no len vi tri r

                            break;
                        }
                    }
                }
            }
        }
    }

    public bool HasAnyMoves()
    {
        Position[] pair = FindValidPairs();

        return pair != null; // true neu con duong, false neu het duong
    }

    // Ham kiem tra xem vi tri (row, col) co nam trong idGrid hay khong
    private bool IsValid(int row, int col)
    {
        if (row < 0 || col < 0 || row >= totalRows || col >= totalCols)
        {
            return false;
        }

        return true;
    }

    // Kiem tra xem o dich co bi chan khong, neu khong thi true luon khong can tim duong di
    private bool IsEndPositionBlocked(Position startPos, Position endPos)
    {
        int[,] directions = new int[4, 2] { 
            { 0, 1 },
            { 0, -1 },
            { 1, 0 },
            { -1, 0 }
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int row = endPos.row + directions[i, 0];
            int col = endPos.col + directions[i, 1];

            if (row == startPos.row && col == startPos.col)
            {
                return false;
            }
            else if (idGrid[row, col] == -1)
            {
                return false;
            }
        }

        return true;
    }

    private void RestoreHintTilesColor()
    {
        if (hintTileOne != null && hintTileTwo != null)
        {
            Color cleanColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            hintTileOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
            hintTileTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
        }
    }

    private void ClearHintTiles()
    {
        if (hintTileOne != null && hintTileTwo != null)
        {
            Color cleanColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            hintTileOne.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
            hintTileTwo.transform.GetChild(0).GetComponent<SpriteRenderer>().color = cleanColor;
        }

        hintTileOne = null;
        hintTileTwo = null;
    }

    // Giup Bot lay duoc gameobject de "gia vo click" vao tile do
    public GameObject GetTileObject(int row, int col)
    {
        return tileObjects[row, col];
    }
}
