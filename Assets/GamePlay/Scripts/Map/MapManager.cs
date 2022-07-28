using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Range(2, 6)]
    public int numberPlayer;

    public Material[] matLists;

    public static Dictionary<int, Stage> stageDict = new Dictionary<int, Stage>();

    #region Singleton
    public static MapManager ins;
    public void Awake()
    {
        ins = this;
    }
    #endregion


    public void SetupStageDictionary(Stage stage)
    {
        if (stageDict == null)
        {
            stageDict = new Dictionary<int, Stage>();
        }

        if (!stageDict.ContainsKey(stage.id))
        {
            stageDict.Add(stage.id, stage);
        }
    }
}
