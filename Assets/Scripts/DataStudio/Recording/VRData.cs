using UnityEngine;
using Valve.VR;

public struct GazeRay
{
    public Vector3 GazeOriginCombinedLocal;
    public Vector3 GazeDirectionCombinedLocal;
}

[System.Serializable]
public class VRData
{
    public Matrix4x4 head;
    public Matrix4x4 leftController;
    public Matrix4x4 rightController;

    public Vector3 GazeOriginCombinedLocal;
    public Vector3 GazeDirectionCombinedLocal;

    public VRData() { }
    public VRData(Matrix4x4 t, Matrix4x4 l, Matrix4x4 r, GazeRay g)
    {
        head = t;
        leftController = l;
        rightController = r;
        GazeOriginCombinedLocal = g.GazeOriginCombinedLocal;
        GazeDirectionCombinedLocal = g.GazeDirectionCombinedLocal;
    }
}