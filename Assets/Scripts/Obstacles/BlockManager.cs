using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private SolidBlock blockPrefab;
    [SerializeField] private PlayerController player;

    private List<SolidBlock> blocks = new List<SolidBlock>();
    private ObjectPool<SolidBlock> pool;
    private int spawnX;
    private int spawnY;
    private float camSize { get { return Camera.main.orthographicSize; } }
    private int blocksToSpawn { get { return (int)(camSize * 1.15f); } }

    private void OnEnable()
    {
        player.e_ColorChanged += ToggleAllBlocks;
    }

    private void Start()
    {
        pool = new ObjectPool<SolidBlock>(
            () =>
            {
                return CreateNewBlock();
            },
            block =>
            {
                ReactivateBlock(block);
            },
            block =>
            {
                block.gameObject.SetActive(false);
            },
            block =>
            {
                Destroy(block);
            }
        );

        for (int i = 0; i < blocksToSpawn; i++)
        {
            spawnX = (int)Random.Range(-camSize, camSize);
            spawnY = (int)this.transform.position.y + (int)Random.Range(-camSize, camSize);

            pool.Get();
        }
    }

    void Update()
    {
        SpawnNewBlocksOnBorder();

        ReleaseOutOfBoundsBlocks();
    }

    private void SpawnNewBlocksOnBorder()
    {
        if (blocksToSpawn > pool.CountActive)
        {
            for (int i = 0; i < blocksToSpawn - pool.CountActive; i++)
            {
                spawnX = (int)Random.Range(-camSize, camSize);
                spawnY = (int)this.transform.position.y + (int)camSize + (int)Random.Range(.5f, 2f);

                if (BlockOnSameLevel(spawnY)) continue;

                pool.Get();
            }
        }
    }

    private void ReleaseOutOfBoundsBlocks()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (!blocks[i].isActiveAndEnabled) continue;

            if (blocks[i].transform.position.y <= this.transform.position.y - camSize - 1)
            {
                ReleaseBlock(blocks[i]);
                continue;
            }
        }
    }

    private bool BlockOnSameLevel(int _yPos)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i].transform.position.y == _yPos)
                return true;
        }

        return false;
    }

    private SolidBlock CreateNewBlock()
    {
        SolidBlock block = Instantiate(blockPrefab);
        block.Init(ColorManager.Instance.GetRandomColor());
        block.e_BlockDestroyed += DestroyBlock;
        block.transform.position = new Vector3(spawnX, spawnY);

        blocks.Add(block);

        ToggleBlock(block);
        return block;
    }
    public void ReleaseBlock(SolidBlock _block)
    {
        pool.Release(_block);
    }
    private void ReactivateBlock(SolidBlock _block)
    {
        _block.gameObject.SetActive(true);

        _block.transform.position = new Vector2(spawnX, spawnY);

        _block.SetColor(ColorManager.Instance.GetRandomColor());

        ToggleBlock(_block);
    }

    private void ToggleAllBlocks()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            ToggleBlock(blocks[i]);
        }
    }
    private void ToggleBlock(SolidBlock _block)
    {
        if (player.ActiveColor == null) return;

        if (_block.ActiveColor.ColorID == player.ActiveColor.ColorID)
            _block.ActivateBlock();
        else
            _block.DeactivateBlock();
    }

    private void DestroyBlock(SolidBlock _block)
    {
        ReleaseBlock(_block);
    }

    private void OnDisable()
    {
        player.e_ColorChanged -= ToggleAllBlocks;
    }
}
