using UnityEngine;
using Valve.VR;

[System.Serializable]
public class VRData
{
    public Matrix4x4 head;
    public Matrix4x4 leftController;
    public Matrix4x4 rightController;

    public VRData() { }
    public VRData(Matrix4x4 t, Matrix4x4 l, Matrix4x4 r)
    {
        head = t;
        leftController = l;
        rightController = r;
    }
}