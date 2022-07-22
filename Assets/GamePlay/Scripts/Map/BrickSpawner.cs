using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public Transform[] posStage1, posStage2, posStage3;
    private List<Vector3> spawnPos1 = new List<Vector3>();       //List những vị trí có thể spawn
    private List<Vector3> spawnPos2 = new List<Vector3>();
    private List<Vector3> spawnPos3 = new List<Vector3>();

    private (Transform[], List<Vector3>) stage1, stage2, stage3;
    private Dictionary<int, (Transform[], List<Vector3>)> stageDict = new Dictionary<int, (Transform[], List<Vector3>)>();

    public List<List<Brick>> brickLists = new List<List<Brick>>(); //List những brick có trên map
    private int numBrickEachPlayer;
    private int[] brickCountEachPlayer;

    private ObjectPooling objectPooling;
    private MapManager mapManager;

    #region Singleton
    public static BrickSpawner ins;
    private void Awake()
    {
        ins = this;
    }
    #endregion

    void Start()
    {
        objectPooling = ObjectPooling.ins;
        mapManager = MapManager.ins;

        BaseActor.CollectBrick += RespawnBrick;

        Init();
    }

    private void Init()
    {
        for (int i = 0; i < posStage1.Length; i++)
        {
            spawnPos1.Add(posStage1[i].position);
        }

        for (int i = 0; i < posStage2.Length; i++)
        {
            spawnPos2.Add(posStage2[i].position);
        }

        for (int i = 0; i < posStage3.Length; i++)
        {
            spawnPos3.Add(posStage3[i].position);
        }

        stage1 = (posStage1, spawnPos1);
        stage2 = (posStage2, spawnPos2);
        stage3 = (posStage3, spawnPos3);
        stageDict.Add(1, stage1);
        stageDict.Add(2, stage2);
        stageDict.Add(3, stage3);

        brickCountEachPlayer = new int[mapManager.numberPlayer];
        for (int i = 0; i < brickCountEachPlayer.Length; i++)
        {
            brickCountEachPlayer[i] = Constant.TOTAL_BRICK_PER_PLAYER;
        }

        for(int i = 0; i < mapManager.numberPlayer; i++)
        {
            List<Brick> brickList = new List<Brick>();
            brickLists.Add(brickList);
        }

        Invoke(nameof(WaitToInitMap), 3f);
    }

    private void WaitToInitMap()
    {
        InitMap(mapManager.numberPlayer, 1);
    }

    public void InitMap(int numPlayer, int stage)
    {
        numBrickEachPlayer = stageDict[stage].Item1.Length / numPlayer;
        List<int> randPosArr = new List<int>();

        for (int i = 0; i < stageDict[stage].Item2.Count; i++)
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
                SpawnBrick(i, randPosArr[rand], stage);

                brickCountEachPlayer[i]--;
                randPosArr.RemoveAt(rand);
            }
        }
    }

    public void InitMapWithId(int playerId, int stage)
    {
        List<int> randPosArr = new List<int>();

        for (int i = 0; i < stageDict[stage].Item2.Count; i++)
        {
            randPosArr.Add(i);
        }

        int rand = -1;
        for (int j = 0; j < numBrickEachPlayer; j++)
        {
            if (randPosArr.Count > 1)
            {
                rand = Random.Range(0, randPosArr.Count);
            }
            else
            {
                rand = 0;
            }
            SpawnBrick(playerId, randPosArr[rand], stage);

            brickCountEachPlayer[playerId]--;
            randPosArr.RemoveAt(rand);
        }
    }

    public void DequeueUnuseBrick(int id, int stage)
    {
        foreach (Brick b in brickLists[id])
        {
            if (b.owner == null && b.id == id && b.stage == stage)
            {
                ObjectPooling.ins.EnQueueObj(Constant.BRICK_TAG, b.gameObject);
            }
        }
    }

    private void SpawnBrick(int brickId, int brickOrder, int stage)
    {
        Brick br = objectPooling.SpawnFromPool(Constant.BRICK_TAG).GetComponent<Brick>();
        br.trans.position = stageDict[stage].Item2[brickOrder];
        br.InitBrick(brickId, brickOrder, stage);

        if (!brickLists[brickId].Contains(br))
        {
            brickLists[brickId].Add(br);
        }
    }

    private void RespawnBrick(int brickId, int brickOrder, int stage)
    {
        if (brickCountEachPlayer[brickId] > 0)
        {
            StartCoroutine(IE_RespawnBrick(brickId, brickOrder, stage));
        }
    }

    private IEnumerator IE_RespawnBrick(int brickId, int brickOrder, int stage)
    {
        yield return Yielders.Get(Constant.RESPAWN_BRICK_TIME);
        SpawnBrick(brickId, brickOrder, stage);
        brickCountEachPlayer[brickId]--;
    } 
}
