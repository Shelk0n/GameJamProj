using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveAction
{
    public static List<GameObject> DeffendersNC = new List<GameObject>();
    public static List<GameObject> AttackersNC = new List<GameObject>();
    public static List<GameObject> ConnectorsNC = new List<GameObject>();
    public static List<int> SendingUnits = new List<int>();

    public static List<int> IdDeffendersNC = new List<int>();
    public static List<int> IdAttackersNC = new List<int>();

    public static List<GameObject> UpdatedToDef = new List<GameObject>();
    public static List<GameObject> UpdatedToScout = new List<GameObject>();

    public static int money = 100;
    public static int movementPoint;
}
