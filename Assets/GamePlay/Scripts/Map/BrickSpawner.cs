using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public Transform[] pos;
    private bool[] isTaken;
    private List<Vector3> spawnPos = new List<Vector3>();       //List những vị trí có thể spawn
    private int numBrickEachPlayer;
    private int[] brickCountEachPlayer;

    private ObjectPooling objectPooling;
    private MapManager mapManager;

    void Start()
    {
        objectPooling = ObjectPooling.ins;
        mapManager = MapManager.ins;

        BaseActor.CollectBrick += RespawnBrick;

        isTaken = new bool[pos.Length];
        for (int i = 0; i < pos.Length; i++)
        {
            spawnPos.Add(pos[i].position);
        }

        brickCountEachPlayer = new int[mapManager.numberPlayer];
        for (int i = 0; i < brickCountEachPlayer.Length; i++)
        {
            brickCountEachPlayer[i] = Constant.TOTAL_BRICK_PER_PLAYER;
        }

        Invoke(nameof(WaitToInitMap), 3f);
    }

    private void WaitToInitMap()
    {
        InitMap(mapManager.numberPlayer);
    }

    public void InitMap(int numPlayer)
    {
        numBrickEachPlayer = pos.Length / numPlayer;
        List<int> randPosArr = new List<int>();

        for (int i = 0; i < spawnPos.Count; i++)
        {
            randPosArr.Add(i);
        }

        int rand = -1;
        for(int i = 0; i < numPlayer; i++)
        {
            for(int j = 0; j < numBrickEachPlayer; j++)
            {
                if (randPosArr.Count > 1)
                {
                    rand = Random.Range(0, randPosArr.Count);
                }
                else
                {
                    rand = 0;
                }
                SpawnBrick(i, randPosArr[rand]);

                brickCountEachPlayer[i]--;
                randPosArr.RemoveAt(rand);
            }
        }
    }

    private void SpawnBrick(int brickId, int brickOrder)
    {
        Brick br = objectPooling.SpawnFromPool(Constant.BRICK_TAG).GetComponent<Brick>();
        br.trans.position = spawnPos[brickOrder];
        br.InitBrick(brickId, brickOrder);
    }

    private void RespawnBrick(int brickId, int brickOrder)
    {
        if (brickCountEachPlayer[brickId] > 0)
        {
            StartCoroutine(IE_RespawnBrick(brickId, brickOrder));
        }
    }

    private IEnumerator IE_RespawnBrick(int brickId, int brickOrder)
    {
        yield return Yielders.Get(Constant.RESPAWN_BRICK_TIME);
        SpawnBrick(brickId, brickOrder);
        brickCountEachPlayer[brickId]--;
    } 
}
