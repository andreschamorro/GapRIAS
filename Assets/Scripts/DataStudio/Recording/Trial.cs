using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Trial
{
    public int StartTrialIndex { get; set; } // Start of the maneuver
    public int EndTrialIndex { get; set; } // End of the maneuver
    public float PetEnterRoadwayTime { get; set; }
    public float PetXatEnterRoadway { get; set; }
    public float PetZatEnterRoadway { get; set; }
    public float LeadCarXatEnterRoadway { get; set; }
    public float TailCarXatEnterRoadway { get; set; }
    public int LeadCarLane { get; set; }
    public int TailCarLane { get; set; }
    public float PetClearCarTime { get; set; }
    public float PetXatClearCar { get; set; }
    public float PetZatClearCar { get; set; }
    public double PetAvgSpeed { get; set; }
    public float LeadCarXatClear { get; set; }
    public float TailCarXatClear { get; set; }
    public List<float> Gaps { get; set; }
    public int StartNextTrial { get; set; }
    public Trial()
    {
        Gaps = new List<float>();
    }

    public string Header()
    {
        return "PetAvgSpeed,GapSeen,GapTaken,PetEnterRoadwayTime"
                + ",PetXatEnterRoadway,PetZatEnterRoadway,LeadCarXatEnterRoadway"
                + ",TailCarXatEnterRoadway,LeadCarLane,TailCarLane"
                + ",PetClearCarTime,PetXatClearCar,PetZatClearCar"
                + ",LeadCarXatClear,TailCarXatClear";
    }

    public override string ToString()
    {
        return PetAvgSpeed + "," + Gaps.Count() + "," + Gaps.FirstOrDefault() + "," + PetEnterRoadwayTime 
                + "," + PetXatEnterRoadway + "," + PetZatEnterRoadway + "," + LeadCarXatEnterRoadway
                + "," + TailCarXatEnterRoadway + "," + LeadCarLane + "," + TailCarLane + ","
                + PetClearCarTime + "," + PetXatClearCar + "," + PetZatClearCar + ","
                + LeadCarXatClear + "," + TailCarXatClear;
    }
}