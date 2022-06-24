using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Dictionary<int, BaseActor> actorDictionary;

    #region Singleton
    public static LevelManager ins;
    public void Awake()
    {
        ins = this;
    }
    #endregion

    void Start()
    {
        
    }

    public void SetupActorDictionary(BaseActor actor)
    {
        if (actorDictionary == null)
        {
            actorDictionary = new Dictionary<int, BaseActor>();
        }

        if (!actorDictionary.ContainsKey(actor.id))
        {
            actorDictionary.Add(actor.id, actor);
        }
    }

    public BaseActor GetActor(int id)
    {
        if (actorDictionary.ContainsKey(id))
        {
            return actorDictionary[id];
        }
        return null;
    }
}
