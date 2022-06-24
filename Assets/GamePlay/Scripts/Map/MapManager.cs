using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Range(2, 6)]
    public int numberPlayer;

    public Material[] matLists;

    #region Singleton
    public static MapManager ins;
    public void Awake()
    {
        ins = this;
    }
    #endregion

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
