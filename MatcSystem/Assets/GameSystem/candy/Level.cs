using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public bool canSwipe = true;
    private List<Swap> possibleSwaps;

    public GameObject noText;
    
    void Start()
    {
        tileTransform = GameObject.Find("tiles").transform;
        jewels = new Jewel[numColumns, numRows];
        tiles = new Tile[numColumns, numRows];
        possibleSwaps = new List<Swap>();

        LoadMap("map_1");
        var js = Shuffle();

    }


    public void Reload() {
        Shuffle();
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
        noText.SetActive(false);
        List<Jewel> list;

        int c = 0;
        do
        {
            c++;
            list = CreateInitialCookies();
            DetectPossibleSwaps();

        } while (possibleSwaps.Count == 0);
        Debug.Log("shuffle <" + c + "> time");
        return list;
    }


    void ReLoad()
    {
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                if (jewels[col, row] != null)
                {
                    Destroy(jewels[col, row].gameObject);//use pool
                    jewels[col, row] = null;
                }
            }
        }
    }
    List<Jewel> CreateInitialCookies()
    {
        ReLoad();
        List<Jewel> list = new List<Jewel>();
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                if (tiles[col, row].IsNull) continue;

                int maxRoll = 0;
                int jType = 0;
                do
                {
                    maxRoll++;
                    jType = UnityEngine.Random.Range(0, (int)JewelType.COUNT);
                    // Debug.Log(maxRoll);
                } while (hasMathesAt(col, row, (JewelType)jType) && maxRoll < 99);

                // GameObject jObj = Instantiate(jewelPrefabs[jType]);
                // Jewel jewel = new Jewel(col, row, (JewelType)jType, jObj, transform);
                // GameObject tGo = Instantiate(tilePrefab);
                //  tGo.transform.parent = tileTransform;
                // tGo.transform.localPosition = new Vector2(col, row);
                Jewel jewel = CreateJewel(col, row, (JewelType)jType);
                jewels[col, row] = jewel;
                list.Add(jewel);
            }
        }

        return list;
    }

    Jewel CreateJewel(int col,int row, JewelType type) {
        GameObject jObj = Instantiate(jewelPrefabs[(int)type]);
        Jewel jewel = new Jewel(col, row, type, jObj, transform);
        GameObject tGo = Instantiate(tilePrefab);
        tGo.transform.parent = tileTransform;
        tGo.transform.localPosition = new Vector2(col, row);

        return jewel;
    }

    bool hasMathesAt(int col, int row, JewelType type)
    {
        bool b_col = false;
        bool b_row = false;
        if (col >= 2)
        {
            if (jewels[col - 1, row] != null && jewels[col - 2, row] != null)
                b_col = jewels[col - 1, row].type == type && jewels[col - 2, row].type == type;

        }
        if (row >= 2)
        {
            if (jewels[col, row - 1] != null && jewels[col, row - 2] != null)
                b_row = jewels[col, row - 1].type == type && jewels[col, row - 2].type == type;
        }

        return b_col || b_row;
    }
    public Jewel JewelAt(int col, int row) => jewels[col, row];
    public Tile TileAt(int col, int row) => tiles[col, row];
    List<Chain> detectHorizontalMatches()
    {
        List<Chain> chains = new List<Chain>();
        for (int row = 0; row < numRows; row++)
        {
            var column = 0;
            while (column < numColumns - 2)
            {
                // 3
                if (jewels[column, row] != null)
                {
                    var matchType = jewels[column, row].type;
                    // 4
                    if (jewels[column + 1, row]!=null&&jewels[column + 1, row]?.type == matchType &&
                      jewels[column + 2, row]!=null&&jewels[column + 2, row]?.type == matchType)
                    {
                        // 5
                        var chain = new Chain(ChainType.horizontal);
                        do
                        {
                            chain.Add(jewels[column, row]);
                            column += 1;
                        } while (column < numColumns && jewels[column, row]?.type == matchType);


                        chains.Add(chain);
                        continue;
                    }
                }
                // 6
                column += 1;
            }
        }

        return chains;
    }
    List<Chain> detectVerticalMatches()
    {
        List<Chain> chains = new List<Chain>();
        for (int column = 0; column < numColumns ; column++)
        {
            var row = 0;
            while (row < numRows - 2)
            {
                // 3
                if (jewels[column, row] != null)
                {
                    var matchType = jewels[column, row].type;
                    // 4
                    if (jewels[column, row+1] != null && jewels[column, row + 1]?.type == matchType &&
                      jewels[column, row + 2] != null && jewels[column , row + 2]?.type == matchType)
                    {
                        // 5
                        var chain = new Chain(ChainType.vertical);
                        do
                        {
                            chain.Add(jewels[column, row]);
                            row += 1;
                        } while (row < numRows && jewels[column, row]?.type == matchType);


                        chains.Add(chain);
                        continue;
                    }
                }
                // 6
                row += 1;
            }
        }

        return chains;
    }

    List<Chain> RemoveMatches() {

        List<Chain> list = new List<Chain>();
        var horizontalChains = detectHorizontalMatches();
        var verticalChains = detectVerticalMatches();
        RemoveJewel(horizontalChains);
        RemoveJewel(verticalChains);
        foreach (var item in horizontalChains)   list.Add(item);
        foreach (var item in verticalChains) list.Add(item);
  
        return list;
    }
    void RemoveJewel(List<Chain> chains)
    {
        foreach (var ch in chains)
        {
            foreach (var jw in ch.jewels)
            {
                if (jewels[jw.column, jw.row] != null)
                {
                    Destroy(jewels[jw.column, jw.row].gameObject);
                    jewels[jw.column, jw.row] = null;
                }
                
             }
        }
    }
    List<List<Jewel>> TopUpJewels() {
        List<List<Jewel>> columns = new List<List<Jewel>>();
        JewelType jType= JewelType.COUNT;
        for (int column = 0; column < numColumns; column++) {
            List<Jewel> array = new List<Jewel>();
            var row = numRows - 1;
            while (row >= 0 && jewels[column, row] == null){
                if (!tiles[column, row].IsNull) {
                    JewelType newType;
                    do
                    {
                        newType = (JewelType)UnityEngine.Random.Range(0, (int)JewelType.COUNT);
                    } while (newType== jType);
                    jType = newType;

                    Jewel jewel = CreateJewel(column, row, jType);
                    jewels[column, row] = jewel;
                    array.Add(jewel);

                }
                row--;
            }

            if (array.Count > 0)
            {
                columns.Add(array);
            }
        }

        return columns;
    }
    List<List<Jewel>> FillHoles() {
        List<List<Jewel>> columns = new List<List<Jewel>>();
        for (int column = 0; column < numColumns; column++)
        {
            List<Jewel> array = new List<Jewel>();
            for (int row = 0; row < numRows; row++)
            {
                if (!tiles[column,row].IsNull&&jewels[column, row]==null)
                {
                    for (int lookup = row+1; lookup < numRows; lookup++)
                    {
                        if (jewels[column,lookup]!=null)
                        {
                            Jewel jewel = jewels[column, lookup];
                            jewels[column, row] = jewel;
                            jewel.row = row;
                            array.Add(jewel);
                            jewels[column, lookup] = null;
                            break;
                        }
                    }
                }
            }

            if (array.Count>0)
            {
                columns.Add(array);
            }
        }

        return columns;
    }
    IEnumerator AnimateNewJewels(List<List<Jewel>> columns,Action callback=null) {
        float longDelay = 0.1f;
        foreach (var list in columns) {
            var startRow = list[0].row + 1;
            for (int i = 0; i < list.Count; i++)
            {
                var delay = 0.05f + 0.1f * (list.Count - i - 1);
                longDelay = Mathf.Max(longDelay, delay);
                var startPos = new Vector3(list[i].column, startRow, 0);
                var toPos = new Vector3(list[i].column, list[i].row, 0);
                AnimationUtil.MoveTo(list[i].gameObject, startPos,toPos, delay);
            }
        }

         yield return new WaitForSeconds(longDelay);
        callback?.Invoke();
    }
    IEnumerator AnimateFallingJewels(List<List<Jewel>> columns,Action callback= null) {
        float longDelay = 0.1f;
        foreach (var list in columns)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var delay = 0.05f + 0.1f * i;
                longDelay = Mathf.Max(longDelay, delay);
                AnimationUtil.MoveTo(list[i].gameObject, new Vector3(list[i].column, list[i].row,0), delay);
            }
        }
        yield return new WaitForSeconds(longDelay);
        callback?.Invoke();
    }
    bool HasChainAt(int col, int row)
    {
        if (jewels[col, row] != null)
        {
            // Left
            JewelType type = jewels[col, row].type;
            // Horizontal chain check
            var horizontalLength = 1;
            var i = col - 1;
            while (i >= 0 && jewels[i, row] != null && jewels[i, row]?.type == type)
            {
                i -= 1;
                horizontalLength += 1;
            }
            // Right
            i = col + 1;
            while (i < numColumns && jewels[i, row] != null && jewels[i, row]?.type == type)
            {
                i += 1;
                horizontalLength += 1;
            }

            if (horizontalLength >= 3) return true;
            // Vertical chain check
            var verticalLength = 1;

            // Down
            i = row - 1;
            while (i >= 0 && jewels[col, i] != null && jewels[col, i]?.type == type)
            {
                i -= 1;
                verticalLength += 1;
            }

            // Up
            i = row + 1;
            while (i < numRows && jewels[col, i] != null && jewels[col, i]?.type == type)
            {
                i += 1;
                verticalLength += 1;
            }
            return verticalLength >= 3;

        }
        return false;
    }
    void DetectPossibleSwaps()
    {
        List<Swap> list = new List<Swap>();
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                var jewel = jewels[col, row];
                if (jewel != null)
                {
                    if (col < numColumns - 1)
                    {
                        var other = jewels[col + 1, row];
                        if (other != null)
                        {
                            // Swap
                            jewels[col, row] = other;
                            jewels[col + 1, row] = jewel;

                            if (HasChainAt(col, row) || HasChainAt(col + 1, row))
                            {
                                list.Add(new Swap(jewel, other));
                            }
                            // Swap  back
                            jewels[col, row] = jewel;
                            jewels[col + 1, row] = other;
                        }
                    }

                    if (row < numRows - 1)
                    {
                        var other = jewels[col, row + 1];
                        if (other != null)
                        {
                            jewels[col, row] = other;
                            jewels[col, row + 1] = jewel;

                            // Swap
                            jewels[col, row] = other;
                            jewels[col, row + 1] = jewel;

                            if (HasChainAt(col, row) || HasChainAt(col, row + 1))
                            {
                                list.Add(new Swap(jewel, other));
                            }
                            // Swap  back
                            jewels[col, row] = jewel;
                            jewels[col, row + 1] = other;

                        }

                    }

                    else if (col == numColumns - 1)
                    {
                        if (row < numRows - 1)
                        {
                            var other = jewels[col, row + 1];
                            jewels[col, row] = other;
                            jewels[col, row + 1] = jewel;

                            if (HasChainAt(col, row) || HasChainAt(col, row + 1))
                            {
                                list.Add(new Swap(jewel, other));
                            }

                            // Swap  back
                            jewels[col, row] = jewel;
                            jewels[col, row + 1] = other;
                        }
                    }
                }
            }
        }

        if (list.Count==0)
        {
            Debug.LogError("No More Move!!!!");
            noText.SetActive(true);
        }
        possibleSwaps = list;
    }
    Vector2Int TouchBoradPosition()
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
            Vector2Int pos = TouchBoradPosition();
            if (IsInBorad(pos))
            {
                swipeFromColumn = pos.x;
                swipeFromRow = pos.y;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!canSwipe) return;

            Vector2Int pos = TouchBoradPosition();
            if (IsInBorad(pos))
            {

                var horizontalDelta = 0;
                var verticalDelta = 0;
                if (pos.x < swipeFromColumn)
                {          // swipe left
                    horizontalDelta = -1;
                    //  Debug.Log("left");
                }
                else if (pos.x > swipeFromColumn)
                {   // swipe right
                    horizontalDelta = 1;
                    // Debug.Log("right");
                }
                else if (pos.y < swipeFromRow)
                {         // swipe down
                    verticalDelta = -1;
                    /// Debug.Log("down");
                }
                else if (pos.y > swipeFromRow)
                {         // swipe up
                    verticalDelta = 1;
                    // Debug.Log("up");
                }

                if (horizontalDelta != 0 || verticalDelta != 0)
                {
                  
                    TrySwap(horizontalDelta, verticalDelta);
                }

            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            swipeFromColumn = -1;
            swipeFromRow = -1;
            canSwipe = true;
        }
    }

    bool IsPossibleSwap(Swap swap)
    {

        var ls = possibleSwaps.Where(s => (s.JewelA == swap.JewelA && s.JewelB == swap.JewelB) ||
       (s.JewelB == swap.JewelA && s.JewelA == swap.JewelB)).ToList();

        return ls.Count > 0;

    }

    void TrySwap(int x, int y)
    {
       // Debug.Log($"TrySwap<{x},{y}>");
        canSwipe = false;
        int toCol = swipeFromColumn + x;
        int toRow = swipeFromRow + y;

        var toJewel = JewelAt(toCol, toRow);
        var fromJewel = JewelAt(swipeFromColumn, swipeFromRow);
        if (toJewel != null && fromJewel != null)
        {
            Swap swap = new Swap(fromJewel, toJewel);
            if (IsPossibleSwap(swap))
            {
                StartCoroutine(AnimaSwap(swap, 0.25f));
            }
            else
            {
                StartCoroutine(AnimateInvalidSwap(swap, 0.15f));
            }
        }
    }
    IEnumerator AnimateInvalidSwap(Swap swap, float t)
    {
        canSwipe = false;
        var columnA = swap.JewelA.column;
        var rowA = swap.JewelA.row;

        var columnB = swap.JewelB.column;
        var rowB = swap.JewelB.row;
        AnimationUtil.MoveTo(swap.JewelA.gameObject, new Vector3(columnB, rowB, 0), t);
        AnimationUtil.MoveTo(swap.JewelB.gameObject, new Vector3(columnA, rowA, 0), t);
        yield return new WaitForSeconds(t);
        AnimationUtil.MoveTo(swap.JewelA.gameObject, new Vector3(columnA, rowA, 0), t);
        AnimationUtil.MoveTo(swap.JewelB.gameObject, new Vector3(columnB, rowB, 0), t);
        yield return new WaitForSeconds(t);
        
    }

    IEnumerator AnimaSwap(Swap swap, float t)
    {
        
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
        AnimationUtil.MoveTo(swap.JewelA.gameObject, new Vector3(columnB, rowB, 0), t);
        AnimationUtil.MoveTo(swap.JewelB.gameObject, new Vector3(columnA, rowA, 0), t);
        yield return new WaitForSeconds(t);
        //Debug.Log("Swap Over");
        
        HandleMatches();
    }


    void HandleMatches() {
        var chains = RemoveMatches();
        if (chains.Count==0)
        {
            DetectPossibleSwaps();
            canSwipe = true;
            return;
        }
        var cols = FillHoles();
        StartCoroutine(AnimateFallingJewels(cols,()=> {
            cols = TopUpJewels();
            StartCoroutine(AnimateNewJewels(cols, () => {
                HandleMatches();
            }));
        }));
        
    }
}
