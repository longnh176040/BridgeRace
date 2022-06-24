using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public Transform[] pos;
    private bool[] isTaken;
    private List<Vector3> spawnPos = new List<Vector3>();
    private List<Vector3> canSpawnPos = new List<Vector3>();
    private int numBrickEachPlayer;


    private ObjectPooling objectPooling;
    private MapManager mapManager;

    void Start()
    {
        objectPooling = ObjectPooling.ins;
        mapManager = MapManager.ins;

        isTaken = new bool[pos.Length];
        for (int i = 0; i < pos.Length; i++)
        {
            spawnPos.Add(pos[i].position);
            canSpawnPos.Add(pos[i].position);
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

        int rand = -1;
        for(int i = 0; i < numPlayer;i++)
        {
            for(int j = 0; j < numBrickEachPlayer; j++)
            {
                if (canSpawnPos.Count > 1)
                {
                    rand = Random.Range(0, canSpawnPos.Count);
                }
                else
                {
                    rand = 0;
                }
                Brick br = objectPooling.SpawnFromPool(Constant.BRICK_TAG).GetComponent<Brick>();
                br.trans.position = canSpawnPos[rand];
                br.InitBrick(i, rand);

                canSpawnPos.RemoveAt(rand);
            }
        }
    }
}
