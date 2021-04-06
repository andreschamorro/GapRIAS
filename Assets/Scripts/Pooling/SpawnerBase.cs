using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBase : MonoBehaviour
{
    [HideInInspector] public List<GameObject> activeObject  = new List<GameObject>();
    public event Action<GameObject> SpawnEvent;
    public event Action<GameObject> DespawnEvent;
    protected virtual void OnSpawn( GameObject e )
    {
        Action<GameObject> handler = SpawnEvent;
        if( handler != null )
        {
            handler( e );
        }
    }
    protected virtual void OnDespawn( GameObject e )
    {
        Action<GameObject> handler = DespawnEvent;
        if( handler != null )
        {
            handler( e );
        }
    }
}