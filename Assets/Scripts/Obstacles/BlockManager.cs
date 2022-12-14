using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private SolidBlock blockPrefab;
    [SerializeField] private PlayerController player;

    private List<SolidBlock> blocks;

    private ObjectPool<SolidBlock> pool;

    private void Start()
    {
        pool = new ObjectPool<SolidBlock>(
            () =>
            {
                return Instantiate(blockPrefab);
            },
            block =>
            {
                block.gameObject.SetActive(true);
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
    }

    void Update()
    {
        CheckForColor();
    }

    private void CheckForColor()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i].ActiveColor == player.ActiveColor) return;

            blocks[i].ToggleCollider();
        }
    }
}
