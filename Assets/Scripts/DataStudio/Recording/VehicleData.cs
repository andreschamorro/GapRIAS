using UnityEngine;

[System.Serializable]
public class VehicleData
{
    public enum State
    {
        Creation,
        Moving,
        Termination
    }

    public Vector3 worldPos;
    public Vector3 forward;
    public bool signal;
    public int instanceID;
    public State state;

    public VehicleData() { }
    public VehicleData(GameObject obj, State st)
    {
        instanceID = obj.GetInstanceID();
        state = st;
        worldPos = obj.transform.position;
        forward = obj.transform.forward;

        VehicleRIAS rias = obj.GetComponent<VehicleRIAS>();
        if (rias != null) signal = rias.CanWalk;
    }

    public void DebugDraw(float radius, float rayLength)
    {
        Gizmos.DrawWireSphere(worldPos, radius);

        Vector3 from = worldPos;
        Vector3 to = from + forward * rayLength;
        Gizmos.DrawLine(from, to);
    }
}
