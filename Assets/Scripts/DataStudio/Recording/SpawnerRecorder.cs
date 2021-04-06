using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerRecorder : RecordAndRepeat.RecorderBase
{
    protected override RecordAndRepeat.RecordingBase CreateInstance()
    {
        return ScriptableObject.CreateInstance<RecordAndRepeat.Recording>();
    }

    public void RecordAsJson(object obj)
    {
        RecordString(JsonUtility.ToJson(obj));
    }

    public void RecordString(string data)
    {
        RecordAndRepeat.DataFrame frame = new RecordAndRepeat.DataFrame();
        frame.Data = data;

        RecordData(frame);
    }
}
