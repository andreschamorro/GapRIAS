using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TabletOS : MonoBehaviour
{
    #region FIELDS
    //public GameObject cursorPrefab;
    //public float maxCursorDistance = 50;
    public bool simpleFeatures;
    public int numLanes;
    public float visthreshold = Mathf.Infinity;
    public float vehDecPoint = -0.05f;
    public Camera viewCamera;
    public Vector3 forwareview;

    private bool featuresIsActive = false;
    private GameObject cursorInstance;
    static Dictionary<string, Sprite> signalSprite;
    private Transform pedestrianSignal;
    private List<VehicleRIAS> visibleVehicles = new List<VehicleRIAS>();
    #endregion

    #region PROPERTIES
    public bool IsLandscape { get { return transform.rotation.eulerAngles.z >= 89; } }
    #endregion
    #region UNITY_METHODS
    void Awake()
    {
        signalSprite = LoadSpriteDictionary("pedestrian-signals");
    }
    // Start is called before the first frame update
    void Start()
    {
        //cursorInstance = Instantiate(cursorPrefab);
        //viewCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        FindVisibleVehicles();
        Orientation();
        CalculateFeatures();

        if (simpleFeatures)
        {
            SimpleFeatures();
        }
        else
        {
            ComplexFeatures();
        }
    }
    #endregion

    #region PUBLIC_METHODS
    #endregion
    #region PRIVATE_METHODS

    private void FindVisibleVehicles()
    {
        // Retrieve all vehicles in scene
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        // Store only visible vehicles
        visibleVehicles.Clear();
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (IsVisible(vehicles[i]) && (IsForward(vehicles[i])))
            {
                visibleVehicles.Add(vehicles[i].GetComponent<VehicleRIAS>());
            }
        }
        featuresIsActive = true;
        //featuresIsActive = visibleVehicles.Any();
    }

    private void CalculateFeatures()
    {
        forwareview = transform.forward;
        // More closed vehicle by lane
        VehicleRIAS[] minVehs = new VehicleRIAS[numLanes];
        float[] minDistance = new float[] {Mathf.Infinity, Mathf.Infinity};
        foreach (VehicleRIAS veh in visibleVehicles)
        {
            int lane = (veh.transform.position.z >= vehDecPoint )? 0 : 1;
            float dis = Vector3.Distance(veh.transform.position, this.transform.position);
            if (dis < minDistance[lane])
            {
                minVehs[lane] = veh;
                minDistance[lane] = dis;
            }
        }
        
        for (int lane = 0; lane < minVehs.Length; lane++)
        {
            // Signal
            Image imgSig = (lane == 0)?
                pedestrianSignal.transform.Find("Right/SignalImage").GetComponent<Image>() : 
                pedestrianSignal.transform.Find("Left/SignalImage").GetComponent<Image>();

            imgSig.sprite = !(minVehs[lane] == null || minVehs[lane].CanWalk)? 
                signalSprite["wait"] : signalSprite["walk"];

            // Time See
            Text textVal = (lane == 0)?
                pedestrianSignal.transform.Find("Right/Time/See").GetComponent<Text>() : 
                pedestrianSignal.transform.Find("Left/Time/See").GetComponent<Text>();

            textVal.text = (minVehs[lane] == null)? 
                "--" : (minVehs[lane].DistanceFrom(transform)/minVehs[lane].Speed).ToString("F0")+" s";

            // Speed
            textVal = (lane == 0)?
                pedestrianSignal.transform.Find("Right/Speed/See").GetComponent<Text>() : 
                pedestrianSignal.transform.Find("Left/Speed/See").GetComponent<Text>();

            textVal.text = minVehs[lane] == null? 
                "--" : minVehs[lane].SpeedMPH.ToString("F0")+" mph";
            

            // Distance
            textVal = (lane == 0)?
                pedestrianSignal.transform.Find("Right/Distance/See").GetComponent<Text>() : 
                pedestrianSignal.transform.Find("Left/Distance/See").GetComponent<Text>();

            textVal.text = minVehs[lane] == null? 
                "--" : (minVehs[lane].DistanceFrom(transform)*3.28084).ToString("F0")+" ft";
        }
    }

    private void SimpleFeatures()
    {
        pedestrianSignal.transform.Find("Left/Time").gameObject.SetActive(false);
        pedestrianSignal.transform.Find("Left/Speed").gameObject.SetActive(false);
        pedestrianSignal.transform.Find("Left/Distance").gameObject.SetActive(false);

        pedestrianSignal.transform.Find("Right/Time").gameObject.SetActive(false);
        pedestrianSignal.transform.Find("Right/Speed").gameObject.SetActive(false);
        pedestrianSignal.transform.Find("Right/Distance").gameObject.SetActive(false);

        // Center
        if (IsLandscape)
        {
            pedestrianSignal.transform.Find("Left/SignalImage").localPosition = new Vector3 (0.0f, -0.0025f, 0.0f);
            pedestrianSignal.transform.Find("Right/SignalImage").localPosition = new Vector3 (0.0f, -0.0025f, 0.0f);
            pedestrianSignal.transform.Find("Left/SignalImage").localScale = 1.05f*Vector3.one;
            pedestrianSignal.transform.Find("Right/SignalImage").localScale = 1.05f*Vector3.one;
        }
        else
        {
            pedestrianSignal.transform.Find("Left/SignalImage").localPosition = Vector3.zero;
            pedestrianSignal.transform.Find("Left/SignalImage").localScale = 1.5f*Vector3.one;
            pedestrianSignal.transform.Find("Right/SignalImage").localPosition = Vector3.zero;
            pedestrianSignal.transform.Find("Right/SignalImage").localScale = 1.5f*Vector3.one;

            pedestrianSignal.transform.localPosition = new Vector3 (0.0f, -0.0365f, 0.0f);
        }
    }

    private void ComplexFeatures()
    {
        pedestrianSignal.transform.Find("Left/Time").gameObject.SetActive(true);
        pedestrianSignal.transform.Find("Left/Speed").gameObject.SetActive(true);
        pedestrianSignal.transform.Find("Left/Distance").gameObject.SetActive(true);

        pedestrianSignal.transform.Find("Right/Time").gameObject.SetActive(true);
        pedestrianSignal.transform.Find("Right/Speed").gameObject.SetActive(true);
        pedestrianSignal.transform.Find("Right/Distance").gameObject.SetActive(true);

        pedestrianSignal.transform.Find("Left/SignalImage").localScale = Vector3.one;
        pedestrianSignal.transform.Find("Right/SignalImage").localScale = Vector3.one;

        // Best Position
        if (IsLandscape)
        {
            pedestrianSignal.transform.Find("Left/SignalImage").localPosition = new Vector3 (0.016f, -0.0025f, 0.0f);
            pedestrianSignal.transform.Find("Right/SignalImage").localPosition = new Vector3 (-0.016f, -0.0025f, 0.0f);
        }
        else
        {
            pedestrianSignal.transform.Find("Left/SignalImage").localPosition = new Vector3 (0.005f, -0.0095f, 0.0f);
            pedestrianSignal.transform.Find("Right/SignalImage").localPosition = new Vector3 (-0.005f, -0.0095f, 0.0f);

            pedestrianSignal.transform.localPosition = new Vector3 (0.0f, -0.031324f, 0.0f);
        }
    }
    private void Orientation()
    {
        // Tablet rotations
        if (IsLandscape)
        {
            this.transform.Find("Display/Interface portrait").gameObject.SetActive(false);
            pedestrianSignal = this.transform.Find("Display/Interface landscape");
            pedestrianSignal.gameObject.SetActive(featuresIsActive);
        }
        else
        {
            this.transform.Find("Display/Interface landscape").gameObject.SetActive(false);
            pedestrianSignal = this.transform.Find("Display/Interface portrait");
            pedestrianSignal.gameObject.SetActive(featuresIsActive);
        }
    }
    
    // Is the renderer within the camera frustrum?
    private bool IsVisible(GameObject obj) {
        if (obj.transform.position.magnitude > visthreshold)
        {
            return false;
        }
        Renderer renderer = obj.GetComponent<Renderer>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(viewCamera);
        return (GeometryUtility.TestPlanesAABB(planes, renderer.bounds)) ? true : false;
    }
    // Is forward to camera
    private bool IsForward(GameObject obj)
    {
        return (Vector3.Angle(transform.forward, obj.transform.forward) > 150.0f);
    }

    private Dictionary<string, Sprite> LoadSpriteDictionary(string path) {
        Sprite[] spritesData = Resources.LoadAll<Sprite>(path);
        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        for (int i = 0; i < spritesData.Length; i++)
        {
            sprites.Add(spritesData[i].name, spritesData[i]);
        }
        return sprites;
    }

    #endregion
}
