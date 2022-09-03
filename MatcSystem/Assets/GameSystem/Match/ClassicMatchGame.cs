using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MatchGame;

public class ClassicMatchGame : MatchGame
{
    private Vector2Int _swapingBlock1;
    private Vector2Int _swapingBlock2;
    void updateThreeMatching() {
        if (isBlocksMoveToAnim()) return;

        destroyVertical.Clear();
        destroyHorizontal.Clear();
        store.Clear();


        for (int i = 0; i < _tilesNum.x; i++)
        {
            int type = 0;
            store.Clear();
            for (int j = 0; j < _tilesNum.y; j++)
            {
                type = checkDestroyBlock(type, store, destroyVertical, i, j);
            }
            if (store.Count >= 3) destroyVertical.Add(new List<Block>(store));
        }

        for (int j = 0; j < _tilesNum.y; j++)
        {
            int type = 0;
            store.Clear();
            for (int i = 0; i < _tilesNum.x; i++)
            {
                type = checkDestroyBlock(type, store, destroyHorizontal, i, j);
            }
            if (store.Count >= 3) destroyHorizontal.Add(new List<Block>(store));
        }

        List<Block> crossVertical = null;
        List<Block> crossHorizontal = null;
        foreach (List<Block> _list in destroyVertical)
        {
            foreach (Block _b in _list)
            {
                List<Block> ret = findDestroyBlockInList(destroyHorizontal, _b);
                if (ret != null)
                {
                    crossVertical = _list;
                    crossHorizontal = ret;
                    break;
                }
            }
            if (crossVertical != null) break;
        }

        int destroyNum = 0;
        bool anyBlockDestroyed = true;
        if (crossVertical != null)
        {
            Block cross = findCrossBlock(crossVertical, crossHorizontal);
            destroyNum += destroyBlocks(crossVertical, cross, cross);
            destroyNum += destroyBlocks(crossHorizontal, cross, cross);

            createSpecialBlock(SpecialBlock.MATCHED_CROSS, cross);

        }
        else if (destroyVertical.Count > 0)
        {
            destroyNum += destroyBlocks(destroyVertical[0], null);
        }
        else if (destroyHorizontal.Count > 0)
        {
            destroyNum += destroyBlocks(destroyHorizontal[0], null);
        }
        else
        {
            anyBlockDestroyed = false;
            if (gameState == InGameState.GAME_ENTRY)
            {
                gameState = InGameState.GAME_WORK;
                Invoke("OnFinishedWorking", 2);
            }
        }


        if (gameState == InGameState.GAME_SWAP_BLOCK)
        {
            if (anyBlockDestroyed == false)
            {
                if (_swapingBlock1.x != -1)
                {
                    swapBlock(_swapingBlock1.x,_swapingBlock1.y,
                              _swapingBlock2.x, _swapingBlock2.y);
                }
            }
            else
            {
                //decreaseTurn();
            }
            gameState = InGameState.GAME_PLAYING;
        }

    }
    Block findCrossBlock(List<Block> v, List<Block> h)
    {
        foreach (Block _vb in v)
        {
            foreach (Block _hb in h)
            {
                if (_vb == _hb)
                {
                    return _vb;
                }
            }
        }
        return null;
    }
    List<Block> findDestroyBlockInList(List<List<Block>> ori, Block b)
    {
        foreach (List<Block> _list in ori)
        {
            foreach (Block _b in _list)
            {
                if (_b == b) return _list;
            }
        }
        return null;
    }
    int checkDestroyBlock(int type, List<Block> store, List<List<Block>> ori, int i, int j)
    {
        if (_tiles[i, j].type != type)
        {
            if (store.Count >= 3)
            {
                ori.Add(new List<Block>(store));
            }
            type = _tiles[i, j].type;
            store.Clear();
        }
        store.Add(_tiles[i, j]);
        return type;
    }
    void updateTouchBoard() {
        if (isBlocksMoveToAnim()) return;
        if (Input.GetMouseButton(0)) {
            Vector2Int idx = findClickBlock();
            //Debug.Log(idx);
            if (gameState == InGameState.GAME_PLAYING)
            {
                if (idx.x >= 0)
                {
                    gameState = InGameState.GAME_SELECT_BLOCK;
                    _selectedBlock = idx;

                    Block _b = _tiles[idx.x, idx.y];
                    _b.touchDown();
                }
            } else if (gameState == InGameState.GAME_SELECT_BLOCK) {
                int i = _selectedBlock.x;
                int j = _selectedBlock.y;
                Block b = _tiles[i, j];

                if (idx.x != i|| idx.y !=j)
                {
                    float dx = idx.x - i;
                    float dy = idx.y - j;

                    b.touchUp();
                    int ti = i;
                    int tj = j;
                    if (System.Math.Abs(dx) > System.Math.Abs(dy))   {
                        if (dx > 0)   ti += 1;
                        else  ti -= 1;
                    } else  {
                        if (dy > 0)  tj += 1;
                        else  tj -= 1;
                    }

                    bool isNotBlock = (ti >= 0 && ti < _tilesNum.x && tj >= 0 && tj < _tilesNum.y);
                    if (b.type >= 0)
                    {
                        _swapingBlock1 = new Vector2Int(-1, -1);
                        _swapingBlock2 = new Vector2Int(-1, -1);
                        if (isNotBlock)
                        {
                            swapBlock(i, j, ti, tj);
                        }
                        gameState = InGameState.GAME_SWAP_BLOCK;
                    }
                    else
                    {
                        SpecialBlock type = (SpecialBlock)System.Math.Abs(b.type);
                        if (isNotBlock && type == SpecialBlock.MATCHED_5)
                        {
                            b.move(_tiles[ti, tj].transform.localPosition);
                            b.dieAfterAnim = true;

                            _tiles[i, j] = null;

                            List<Block> f = findBlocksWithType(_tiles[ti, tj].type);
                            destroyBlocks(f, null, null, false);
                            
                        }
                        
                    }
                }

            }

        } else {
            if (gameState == InGameState.GAME_SELECT_BLOCK)
            {
                Block b = _tiles[_selectedBlock.x, _selectedBlock.y];
                b.touchUp();
                
                if (b.type < 0)
                {
                    SpecialBlock type = (SpecialBlock)System.Math.Abs(b.type);
                    if (type == SpecialBlock.MATCHED_4)
                    {
                        List<Block> blocks = findBlockLine(_selectedBlock.x, false);
                        destroyBlocks(blocks, null, null, false);
                    }
                    else if (type == SpecialBlock.MATCHED_CROSS)
                    {
                        List<Block> blocks1 = findBlockLine(_selectedBlock.x, false);
                        List<Block> blocks2 = findBlockLine(_selectedBlock.y, true);

                        destroyBlocks(blocks1, b, null, false);
                        destroyBlocks(blocks2, null, null, false);
                    }
                }
                
                gameState = InGameState.GAME_PLAYING;
            }
        }
    }
    private int destroyBlocks(List<Block> v, Block except = null, Block focusTo = null, bool useMoreMatch = true) {
        int count = 0;
        if (useMoreMatch)
        {
            if (v.Count > 3 && focusTo == null)
            {
                focusTo = v[v.Count - 1];
            }
        }

        foreach (Block _b in v)
        {
            if (except == _b) continue;

            _tiles[_b.posInBoard.x, _b.posInBoard.y] = null;
            if (focusTo == null)
            {
                /*
                GameObject _p = MakeBlockDestroyParticle();
                _p.transform.parent = _panel.transform;
                _p.SendMessage("generate", _b);

                if (_currentMonster != null)
                {
                    PlayerAttackParticle __ = (MakePlayerAttackParticle()).GetComponent<PlayerAttackParticle>();
                    __.generate(_panel, _b, _b.transform.localPosition, new Vector2(0, 300));
                    __.Finish = OnFinishedPlayerAttack;
                }
                */
                Destroy(_b.gameObject);
            }
            else
            {
                _b.move(focusTo.transform.localPosition);
                _b.dieAfterAnim = true;
            }
            count++;
        }

        if (focusTo && except != focusTo)
        {
            if (v.Count == 4)
            {
                createSpecialBlock(SpecialBlock.MATCHED_4, focusTo);
            }
            else if (v.Count >= 5)
            {
                createSpecialBlock(SpecialBlock.MATCHED_5, focusTo);
            }
        }

        return count;
    }
    // second argument is axis. ori == true => x-axis, false => y-axis 
    List<Block> findBlockLine(int line, bool ori)
    {
        List<Block> blocks = new List<Block>();
        if (ori)
        {
            for (int x = 0; x < _tilesNum.x; x++)
            {
                blocks.Add(_tiles[x, line]);
            }
        }
        else
        {
            for (int y = 0; y < _tilesNum.y; y++)
            {
                blocks.Add(_tiles[line, y]);
            }
        }
        return blocks;
    }
    protected List<Block> findBlocksWithType(int type)
    {
        List<Block> blocks = new List<Block>();

        for (int i = 0; i < _tilesNum.x; i++)
        {
            for (int j = 0; j < _tilesNum.y; j++)
            {
                if (_tiles[i, j] != null && _tiles[i, j].type == type)
                {
                    blocks.Add(_tiles[i, j]);
                }
            }
        }
        return blocks;
    }
    void swapBlock(int i, int j, int ti, int tj)
{
    Block tb = _tiles[ti, tj];
    _tiles[ti, tj] = _tiles[i, j];
    _tiles[i, j] = tb;

    _tiles[ti, tj].posInBoard = new Vector2Int(ti, tj);
    _tiles[i, j].posInBoard = new Vector2Int(i, j);

    _tiles[ti, tj].move(tilePos(ti, tj));
    _tiles[i, j].move(tilePos(i, j));

    _swapingBlock1 = new Vector2Int(ti, tj);
    _swapingBlock2 = new Vector2Int(i, j);
}
public override void Update()
    {
        base.Update();
        updateThreeMatching();
        updateTouchBoard();
    }

}
