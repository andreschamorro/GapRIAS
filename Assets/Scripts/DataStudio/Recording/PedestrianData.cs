using UnityEngine;

[System.Serializable]
public class PedestrianData
{
    public Vector3 worldPos;
    public Quaternion worldRot;
    public Vector3 forward;

    public PedestrianData() { }
    public PedestrianData(Transform t)
    {
        worldPos = t.position;
        worldRot = t.rotation;
        forward = t.forward;
    }

    public void DebugDraw(float radius, float rayLength)
    {
        Gizmos.DrawWireSphere(worldPos, radius);

        Vector3 from = worldPos;
        Vector3 to = from + forward * rayLength;
        Gizmos.DrawLine(from, to);
    }
}