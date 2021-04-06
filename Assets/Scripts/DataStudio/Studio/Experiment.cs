using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Experiment : ScriptableObject
{
    public enum Gender
    {
        Female,
        Male
    }
    public enum ExpType
    {
        OneLane,
        TwoLane
    }
    [SerializeField]
    private ExpType type;
    [SerializeField]
    private string id;
    [SerializeField]
    private int age;
    [SerializeField]
    private Gender gender;
    [SerializeField]
    private DateTime date;

    public ExpType Type
    {
        get { return type; }
        set { type = value; }
    }
    public string ID
    {
        get { return id; }
        set { id = value; }
    }
    public int Age
    {
        get { return age; }
        set { age = value; }
    }
    public Gender Gen
    {
        get { return gender; }
        set { gender = value; }
    }
    public DateTime Date
    {
        get { return date; }
        set { date = value; }
    }
    public bool IsOneLane
    {
        get { return (this.Type == ExpType.OneLane); }
        set { this.Type = value? ExpType.OneLane : ExpType.TwoLane; }
    }

    public override string ToString()
    {
        return String.Format("Experiment: {0}\nParticipant: {1}\nAge: {2}\nGender: {3}\nDate: {4}",
        type, id, age, gender, date);
    }
}
