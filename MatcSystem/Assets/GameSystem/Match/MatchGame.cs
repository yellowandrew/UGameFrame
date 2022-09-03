using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MatchGame;

public class MatchGame : MonoBehaviour
{
    public enum SpecialBlock
    {
        MATCHED_4 = 1,
        MATCHED_5,
        MATCHED_CROSS,
        COUNT,
    }
    public enum BlockType {
        one,two,three,four,five,six,

        count
    }
    public enum InGameState
    {
        GAME_ENTRY,
        GAME_WORK,
        GAME_WORKING,
        GAME_READY,
        GAME_PLAYING,
        GAME_ATTACKING,
        GAME_SELECT_BLOCK,
        GAME_SWAP_BLOCK,
    }
    public GameObject _hintPrefab;
    public GameObject _panel;
    public GameObject _hintPanel;
    public GameObject _board;
    public GameObject _blocksPrefab;
    public Sprite[] blockSprites;
    public Sprite[] SpecicalblockSprites;
    public GameObject[] _enemyPrefab;

    public Vector2Int _tilesNum;
    public Vector2Int _tileSize;

    public Camera _camera;

    public Vector2 _boardPadding;
    public Vector2 _tilesMargin;

    public int _maxStage;

    public float _hintTime = 5f;

    protected Block[,] _tiles;
    protected InGameState gameState;
    protected Enemy _currentMonster = null;
    protected int _currentCombo = 0;
    protected int _currentStage = 0;

    protected float _fShakingScreen = 0;

    protected bool _gameEnd = false;
    protected List<Block> _hints = new List<Block>();
    protected List<GameObject> _hintSpr = new List<GameObject>();
    private bool _hintDirty = true;
    private float _hintDt = 0;
    protected Vector2Int _selectedBlock;

    protected List<List<Block>> destroyVertical = new List<List<Block>>();
    protected List<List<Block>> destroyHorizontal = new List<List<Block>>();

    protected List<Block> store = new List<Block>();

    void Start()
    {
        _tiles = new Block[_tilesNum.x, _tilesNum.y];
        gameState = InGameState.GAME_ENTRY;
        
    }
    public virtual void FindHint() { }
    IEnumerator GameOver()
    {
        _gameEnd = true;
        yield return new WaitForSeconds(2f);
    }
    // Update is called once per frame
    public virtual void Update()
    {
        updateShakingScreen();
        updateWalking();

        updateEmptyFill();
        updateHint();
    }
    public virtual Vector2 tilePos(int x, int y)
    {
        return new Vector2(x,y);
    }
    private void updateHint()
    {
        if (isBlocksMoveToAnim()) return;
        if (_hintDirty)
        {
            _hints.Clear();
            FindHint();

            if (_hints.Count == 0)
            {
                // TODO :: RE-POSITIONING BLOCKS
            }
            else
            {
                foreach (GameObject go in _hintSpr)
                {
                    Destroy(go);
                }
                _hintSpr.Clear();

                foreach (Block _b in _hints)
                {
                    GameObject obj = (GameObject)Instantiate(_hintPrefab);
                    obj.transform.parent = _hintPanel.transform;
                    obj.transform.localPosition = new Vector3(_b.transform.localPosition.x, _b.transform.localPosition.y, 10000);
                    obj.transform.localScale = new Vector3(0, 0, 1);

                    _hintSpr.Add(obj);
                }
            }
            _hintDirty = false;
            _hintDt = 0;
        }
        _hintDt += Time.deltaTime;
        if (_hintDt > _hintTime)
        {
            foreach (GameObject go in _hintSpr)
            {
				go.transform.localScale = new Vector3(1,1,1);
				//go.GetComponent<UISprite>().width  = (int)_tileSize.x;
				//go.GetComponent<UISprite>().height = (int)_tileSize.y;

            }
        }
    }

    private void updateEmptyFill()
    {
        for (int i = 0; i < _tilesNum.x; i++)
        {
            for (int j = 0; j < _tilesNum.y; j++)
            {
                if (_tiles[i, j] == null)
                {
                    Block block = null;
                    if (j == 0)
                    {
                        //block = pushNewItem(UnityEngine.Random.Range(0,4),i);
                        block = pushNewItem(UnityEngine.Random.Range(0, (int)BlockType.count), i);
                    } else  {
                        block = _tiles[i, j - 1];
                        _tiles[i, j - 1] = null;
                    }
                    if (block)
                    {
                        block.moveToY(tilePos(i, j).y);
                        _tiles[i, j] = block;
                        _tiles[i, j].posInBoard = new Vector2Int(i, j);
                    }
                }
            }
        }
    }
    protected Block pushNewItem(int r, int x) 
    {
        if (r < 0 || r >= (int)BlockType.count) r = 0;
        if (_tiles[x, 0] != null) return null;

        GameObject newBlock = Instantiate(_blocksPrefab);
        newBlock.transform.parent = _panel.transform;
        newBlock.transform.localPosition = new Vector3(tilePos(x, 0).x, _tilesNum.y+2);

        newBlock.transform.localScale = new Vector2(_tileSize.x, _tileSize.y);


        Block b = newBlock.GetComponent<Block>();
        b.type = r;

        b.setSprite(blockSprites[r],blockSprites[r+ (int)BlockType.count]);

        _tiles[x, 0] = b;

        _hintDirty = true;
        foreach (GameObject go in _hintSpr)
        {
            Destroy(go);
        }
        _hintSpr.Clear();
        return b;
    }
    private void updateWalking()  {  }

    private void updateShakingScreen()
    {
        if (_fShakingScreen > 0)
        {
            _fShakingScreen -= 0.1f;
            _panel.transform.localPosition = new Vector3(
                    UnityEngine.Random.Range(0, 8) - 4,
                    UnityEngine.Random.Range(0, 8) - 4,
                    0
                );
            if (_fShakingScreen < 0)
            {
                _panel.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    protected bool isBlocksMoveToAnim()
    {
        for (int i = 0; i < _tilesNum.x; i++)
        {
            for (int j = 0; j < _tilesNum.y; j++)
            {
                if (_tiles[i, j] == null) return true;
                if (_tiles[i, j].isAnimation)
                {
                    return true;
                }
            }
        }
        return false;
    }

   
    protected Vector3 ClickPoint()=> _camera.ScreenToWorldPoint(Input.mousePosition);
    protected Vector2Int findClickBlock()
    {
        Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int indx = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        return indx;
    }
    protected bool intersectNodeToPoint(GameObject node, Vector3 pos) {

        return false;
    }

   protected void createSpecialBlock(SpecialBlock t, Block b)
    {
        GameObject _block = Instantiate(_blocksPrefab); ;
        _block.transform.parent = _panel.transform;
        _block.transform.localPosition = b.transform.localPosition;
       

        Block _b = _block.GetComponent<Block>();
        _b.setSprite(SpecicalblockSprites[(int)t - 1], SpecicalblockSprites[(int)SpecialBlock.COUNT+ (int)t-1]);
        _b.posInBoard = b.posInBoard;
        _b.type = -(int)t;
        _tiles[_b.posInBoard.x, _b.posInBoard.y] = _b;

        Destroy(b.gameObject);
    }

        public void OnFinishedWorking()
        {
            gameState = InGameState.GAME_PLAYING;
        }
    }
