using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Trial
{
    public int StartTrialIndex { get; set; } // Start of the maneuver
    public int EndTrialIndex { get; set; } // End of the maneuver
    public float PedEnterRoadwayTime { get; set; }
    public float PedXatEnterRoadway { get; set; }
    public float PedZatEnterRoadway { get; set; }
    public float LeadCarXatEnterRoadway { get; set; }
    public float TailCarXatEnterRoadway { get; set; }
    public int LeadCarLane { get; set; }
    public int TailCarLane { get; set; }
    public float PedClearCarTime { get; set; }
    public float PedXatClearCar { get; set; }
    public float PedZatClearCar { get; set; }
    public double PedAvgSpeed { get; set; }
    public float LeadCarXatClear { get; set; }
    public float TailCarXatClear { get; set; }
    public int HitNum { get; set; }
    public List<float> Gaps { get; set; }
    public int StartNextTrial { get; set; }
    public Trial()
    {
        Gaps = new List<float>();
        HitNum = 0;
    }

    // Copy constructor.
    public Trial(Trial trial)
    {
        StartTrialIndex = trial.StartTrialIndex;
        EndTrialIndex = trial.EndTrialIndex;
        PedEnterRoadwayTime = trial.PedEnterRoadwayTime;
        PedXatEnterRoadway = trial.PedXatEnterRoadway;
        PedZatEnterRoadway = trial.PedZatEnterRoadway;
        LeadCarXatEnterRoadway = trial.LeadCarXatEnterRoadway;
        TailCarXatEnterRoadway = trial.TailCarXatEnterRoadway;
        LeadCarLane = trial.LeadCarLane;
        TailCarLane = trial.TailCarLane;
        PedClearCarTime = trial.PedClearCarTime;
        PedXatClearCar = trial.PedXatClearCar;
        PedZatClearCar = trial.PedZatClearCar;
        PedAvgSpeed = trial.PedAvgSpeed;
        LeadCarXatClear = trial.LeadCarXatClear;
        TailCarXatClear = trial.TailCarXatClear;
        HitNum = trial.HitNum;
        Gaps = trial.Gaps;
        StartNextTrial = trial.StartNextTrial;
    }

    public static string Header()
    {
        return "PedAvgSpeed,GapSeen,GapTaken,HitNum,PedEnterRoadwayTime"
                + ",PedXatEnterRoadway,PedZatEnterRoadway,LeadCarXatEnterRoadway"
                + ",TailCarXatEnterRoadway,LeadCarLane,TailCarLane"
                + ",PedClearCarTime,PedXatClearCar,PedZatClearCar"
                + ",LeadCarXatClear,TailCarXatClear";
    }

    public override string ToString()
    {
        return PedAvgSpeed + "," + Gaps.Count() + "," + Gaps.FirstOrDefault() + "," + HitNum + "," + PedEnterRoadwayTime 
                + "," + PedXatEnterRoadway + "," + PedZatEnterRoadway + "," + LeadCarXatEnterRoadway
                + "," + TailCarXatEnterRoadway + "," + LeadCarLane + "," + TailCarLane + ","
                + PedClearCarTime + "," + PedXatClearCar + "," + PedZatClearCar + ","
                + LeadCarXatClear + "," + TailCarXatClear;
    }
}