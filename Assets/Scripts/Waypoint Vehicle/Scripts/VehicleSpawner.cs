using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : SpawnerBase
{
    [SerializeField]
    private WaypointLap[] _waypointLap = default;
    [SerializeField]
    private GameObject[] _prefab = null;
    [SerializeField]
    [HideInInspector]
    private float _minSpawnTime = default;
    [SerializeField]
    [HideInInspector]
    private float _maxSpawnTime = default;
    [Range(0.0f, 100.0f)]
    [SerializeField]
    [HideInInspector]
    private float _minVelocity = 15;
    [Range(0.0f, 100.0f)]
    [SerializeField]
    [HideInInspector]
    private float _maxVelocity = 25;

    private float _spawnTime;
    private IEnumerator _spawnCoroutine;

    public float minVelocity 
    {
        get { return _minVelocity; }
        set { _minVelocity = value; }
    }
    public float maxVelocity 
    {
        get { return _maxVelocity; }
        set { _maxVelocity = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public void StartSpawners()
    {
        Debug.Log("Start Spawner");
        _spawnCoroutine = WaitAndSpawn();
        StartCoroutine(_spawnCoroutine);
    }

    public void StopSpawners()
    {
        StopCoroutine(_spawnCoroutine);
    }

    // Spawn every wait seconds.
    public IEnumerator WaitAndSpawn()
    {
        while (true)
        {
            _spawnTime = Random.Range(_minSpawnTime, _maxSpawnTime);
            yield return new WaitForSeconds(_spawnTime);
            float wtime = Spawn();
            yield return new WaitForSeconds(wtime);
        }
    }

    private float Spawn()
    {

        int k = Random.Range(0, _prefab.Length);
        int w = Random.Range(0, _waypointLap.Length);

        var instance = _prefab[k].Spawn(transform.position, transform.rotation);
        instance.GetComponent<VehicleWaypoint>().WaypointLap = _waypointLap[w];
        instance.GetComponent<VehicleMove>().SpeedMPH = Random.Range(_minVelocity, _maxVelocity);
        instance.GetComponent<VehicleWaypoint>().OnEndPath += Despawn;

        OnSpawn(instance.gameObject);
        activeObject.Add(instance.gameObject);
        
        float vel = instance.GetComponent<VehicleMove>().Speed; // m/s
        float with = Vector3.Scale(instance.GetComponent<Collider>().bounds.size, instance.transform.forward).magnitude;
        return with/vel;
    }

    private void Despawn(GameObject obj)
    {
        var vehicle = obj.GetComponent<VehicleWaypoint>();
        if (vehicle)
        {
            vehicle.isDrive = true;
            vehicle.PointIndex = -1;
            vehicle.GetNextPoint();
        }

        OnDespawn(obj);
        activeObject.Remove(obj);

        obj.Despawn();
    }
}

 public static class TransformExtensions
 {
     public static void SetLayer(this Transform trans, int layer) 
     {
         trans.gameObject.layer = layer;
         foreach(Transform child in trans)
             child.SetLayer( layer);
     }
 }
