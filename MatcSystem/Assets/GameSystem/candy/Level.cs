using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int numColumns = 7;
    public int numRows = 8;
    public GameObject tilePrefab;
    public GameObject[] jewelPrefabs;
    public Jewel[,] jewels;
    public Tile[,] tiles;
    Transform tileTransform;

    public int swipeFromColumn;
    public int swipeFromRow;

    public int state = 0;
    void Start()
    {
        tileTransform = GameObject.Find("tiles").transform;
        jewels = new Jewel[numColumns, numRows];
        tiles = new Tile[numColumns, numRows];

        LoadMap("map_1");
        var js = Shuffle();
    }

    public void LoadMap(string map)
    {
        TextAsset ta = Resources.Load<TextAsset>("maps/" + map);
        string[] lines = ta.text.Split('\n');
        int rows = lines.Length;
        for (int r = 0; r < rows; r++)
        {
            string[] li = lines[r].Split(',');
            for (int c = 0; c < li.Length; c++)
            {
                int.TryParse(li[c], out int id);
                Tile t = new Tile(id);
                tiles[c, rows - 1 - r] = t;
            }
        }
    }

    public List<Jewel> Shuffle()
    {
        List<Jewel> list = new List<Jewel>();
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                if (tiles[col, row].IsEmpty) continue;

                int maxRoll = 0;
                int jType = 0;
                do
                {
                    maxRoll++;
                    jType = Random.Range(0, (int)JewelType.COUNT);
                } while ((col >= 2 &&
                      jewels[col - 1, row].type == (JewelType)jType &&
                      jewels[col - 2, row].type == (JewelType)jType)
                      || (row >= 2 &&
                        jewels[col, row - 1].type == (JewelType)jType &&
                        jewels[col, row - 2].type == (JewelType)jType)&&maxRoll<99);

                GameObject jObj = Instantiate(jewelPrefabs[jType]);
                Jewel jewel = new Jewel(col, row, (JewelType)jType, jObj, transform);
                jewels[col, row] = jewel;
                list.Add(jewel);

                GameObject tGo = Instantiate(tilePrefab);
                tGo.transform.parent = tileTransform;
                tGo.transform.localPosition = new Vector2(col, row);
            }
        }

        return list;
    }

    public Jewel JewelAt(int col, int row) => jewels[col, row];
    public Tile TileAt(int col, int row) => tiles[col, row];


    Vector2Int BoradPosition()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }
    bool IsInBorad(Vector2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x <= numColumns && pos.y < numRows;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int pos = BoradPosition();
            if (IsInBorad(pos))
            {
                swipeFromColumn = pos.x;
                swipeFromRow = pos.y;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (state == 1) return;
        
            Vector2Int pos = BoradPosition();
            if (IsInBorad(pos))
            {

                var horizontalDelta = 0;
                var verticalDelta = 0;
                if (pos.x < swipeFromColumn)
                {          // swipe left
                    horizontalDelta = -1;
                    Debug.Log("left");
                }
                else if (pos.x > swipeFromColumn)
                {   // swipe right
                    horizontalDelta = 1;
                    Debug.Log("right");
                }
                else if (pos.y < swipeFromRow)
                {         // swipe down
                    verticalDelta = -1;
                    Debug.Log("down");
                }
                else if (pos.y > swipeFromRow)
                {         // swipe up
                    verticalDelta = 1;
                    Debug.Log("up");
                }

                if (horizontalDelta != 0 || verticalDelta != 0)
                {
                    trySwap(horizontalDelta, verticalDelta);
                }

            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            swipeFromColumn = -1;
            swipeFromRow = -1;
            state = 0;
        }
    }

    void trySwap(int x, int y) {
        state = 1;
        int toCol = swipeFromColumn + x;
        int toRow = swipeFromRow + y;

        var toJewel = JewelAt(toCol, toRow);
        var fromJewel = JewelAt(swipeFromColumn, swipeFromRow);
        if (toJewel!=null && fromJewel!=null)
        {
            HandleSwap(new Swap(fromJewel, toJewel));
        }
    }

    void HandleSwap(Swap swap) {
       
        var columnA = swap.JewelA.column;
        var rowA = swap.JewelA.row;

        var columnB = swap.JewelB.column;
        var rowB = swap.JewelB.row;

        jewels[columnA, rowA] = swap.JewelB;
        swap.JewelB.column = columnA;
        swap.JewelB.row = rowA;

        jewels[columnB, rowB] = swap.JewelA;
        swap.JewelA.column = columnB;
        swap.JewelA.row = rowB;
        AnimationUtil.MoveTo(swap.JewelA.gameObject, new Vector3(columnB,rowB,0), 0.25f);
        AnimationUtil.MoveTo(swap.JewelB.gameObject, new Vector3(columnA, rowA, 0), 0.25f);
      //  swap.JewelA.UpdatePosition();
     //   swap.JewelB.UpdatePosition();

      
        // state = 0;
    }
}
