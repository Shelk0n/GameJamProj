using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using Photon.Pun;
using System.Linq;

public class ButtonsControlNC : MonoBehaviour
{
    private static NetworkComponentController nc;
    private static ButtonsControlNC instance;

    [SerializeField] GameObject moveMenu;
    [SerializeField] InputField inputUnitsSend;
    [SerializeField] Text moneyText;
    [SerializeField] Text movementPoints;

    public string side;
    public string unSide;

    private List<GameObject> myNertworkComponents = new List<GameObject>();
    private List<int> removes = new List<int>();

    [SerializeField] GameObject readyHackerDetector;
    [SerializeField] GameObject readyAdminDetector;
    [SerializeField] GameObject waitingForAnotherPlayer;

    [SerializeField] private int currtntUnits;
    private List<GameObject> connections;
    private List<GameObject> connectors;
    private GameObject currentNC;

    [SerializeField] string stepTurn = "Hacker";

    private bool isSummon = false;
    private bool isMove = false;
    private bool isDataSent = false;

    private GameObject attackedObject;

    private PhotonView view;
    public static void InitializeNC(NetworkComponentController ncController)
    {
        nc = ncController;
        nc.onNetworkComponentClickEvent += instance.OnNCClick;
        nc.onUnitsSendingToThisEvent += instance.OnSendUnits;
    }
    
    private void OnNCClick(int currentUnit, List<GameObject> connection, List<GameObject> connector, GameObject thisNetworkComponent, string sideNC)
    {
        if (side == sideNC)
        {
            currtntUnits = currentUnit;
            if (connections != null)
                CancelPick();
            connections = connection;
            connectors = connector;
            if (currentNC != null)
                CancelPick();
            currentNC = thisNetworkComponent;
            currentNC.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    public void CancelPick()
    {
        currentNC.GetComponent<NetworkComponentController>().OnChangeOwnership();
        for (int i = 0; i < connections.Count; i++)
        {
            connections[i].GetComponent<NetworkComponentController>().OnChangeOwnership();
            if (!SaveAction.ConnectorsNC.Contains(connectors[i]))
            {
                connectors[i].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
    public void OnSendUnits(int units, GameObject attacked)
    {
        if(currentNC.GetComponent<NetworkComponentController>().units > 0)
        {
            attackedObject = attacked;
            moveMenu.SetActive(true);
            isSummon = false;
            isMove = true;
        }
    }
    public void OnSummonClick()
    {
        if(currentNC.GetComponent<NetworkComponentController>().summonableUnits > 0)
        {
            moveMenu.SetActive(true);
            isMove = false;
            isSummon = true;
        }
    }
    public void OnSendClick()
    {
        if (int.TryParse(inputUnitsSend.text, out var number))
        {
            if (isMove && Convert.ToInt32(inputUnitsSend.text) <= currtntUnits && SaveAction.movementPoint >= 1)
            {
                SaveAction.SendingUnits.Add(Convert.ToInt32(inputUnitsSend.text));
                SaveAction.AttackersNC.Add(currentNC);
                SaveAction.DeffendersNC.Add(attackedObject);
                SaveAction.ConnectorsNC.Add(connectors[connections.IndexOf(attackedObject)]);
                connectors[connections.IndexOf(attackedObject)].GetComponent<SpriteRenderer>().color = Color.blue;
                currentNC.GetComponent<NetworkComponentController>().ChangeUnits(-Convert.ToInt32(inputUnitsSend.text), side);
            } 
            else if(isSummon && SaveAction.movementPoint >= 0.8f)
            {
                Summon(Convert.ToInt32(inputUnitsSend.text));
            }
            inputUnitsSend.text = "";
            isMove = false;
            isSummon = false;
            moveMenu.SetActive(false);
            CancelPick();
        }
    }
    public void Summon(int sumNumber)
    {
        if(SaveAction.money >= sumNumber && sumNumber <= currentNC.GetComponent<NetworkComponentController>().summonableUnits)
        {
            currentNC.GetComponent<NetworkComponentController>().summonableUnits-=sumNumber;
            SaveAction.money -= sumNumber;
            currentNC.GetComponent<NetworkComponentController>().ChangeUnits(sumNumber, side);
            moneyText.text = SaveAction.money.ToString();
        }
    }
    public void OnCancelSendClick()
    {
        inputUnitsSend.text = "";
        moveMenu.SetActive(false);
        CancelPick();
    }
    public void OnMoveClick()
    {
        if(connections != null)
        {
            foreach (GameObject connection in connections)
            {
                connection.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            foreach (GameObject connector in connectors)
            {
                connector.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
    }
    public void OnReadyToNextMoveButton()
    {
        waitingForAnotherPlayer.SetActive(true);
        if (side == "Hacker")
            view.RPC("HackerReady", RpcTarget.All);
        else
            view.RPC("AdminReady", RpcTarget.All);
    }
    [PunRPC]
    private void HackerReady()
    {
        readyHackerDetector.SetActive(true);
        if(readyAdminDetector.activeSelf == true)
        {
            waitingForAnotherPlayer.SetActive(false);
            OnNextMove();
            readyAdminDetector.SetActive(false);
            readyHackerDetector.SetActive(false);
        }
    }
    [PunRPC]
    private void AdminReady()
    {
        readyAdminDetector.SetActive(true);
        if (readyHackerDetector.activeSelf == true)
        {
            waitingForAnotherPlayer.SetActive(false);
            OnNextMove();
            readyAdminDetector.SetActive(false);
            readyHackerDetector.SetActive(false);
        }
    }
    private void UpdatingOwnershipList()
    {
        myNertworkComponents.Clear();
        foreach (var NetwokComponent in GameObject.FindGameObjectsWithTag("NetworkComponent"))
        {
            if (NetwokComponent.GetComponent<NetworkComponentController>().Ownership == side)
            {
                myNertworkComponents.Add(NetwokComponent);
            }
        }
    }
    public void OnNextMove()
    {
        for(int i = 0; i < SaveAction.DeffendersNC.Count; i++)
        {
            SaveAction.IdAttackersNC.Add(SaveAction.AttackersNC[i].GetComponent<NetworkComponentController>().ID);
            SaveAction.IdDeffendersNC.Add(SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ID);
        }

        object[] IdAttackers = null;
        object[] IdDeffenders = null;
        object[] SendingUnit = null;
        IdAttackers = SaveAction.IdAttackersNC.Cast<object>().ToArray();
        IdDeffenders = SaveAction.IdDeffendersNC.Cast<object>().ToArray();
        SendingUnit = SaveAction.SendingUnits.Cast<object>().ToArray();

        UpdatingOwnershipList();
        List<int> myNCid = new List<int>();
        List<int> myNCUnits = new List<int>();
        foreach(var NetworkComponent in myNertworkComponents)
        {
            myNCid.Add(NetworkComponent.GetComponent<NetworkComponentController>().ID);
            myNCUnits.Add(NetworkComponent.GetComponent<NetworkComponentController>().units);
        }
        object[] myNCidObj = myNCid.Cast<object>().ToArray();
        object[] myNCUnitsObj = myNCUnits.Cast<object>().ToArray();
        isDataSent = false;
        view.RPC("AddAnotherPlayerMovement", RpcTarget.Others, IdAttackers, IdDeffenders, SendingUnit, myNCidObj, myNCUnitsObj);
        StartCoroutine(WaitingData());
    }
    IEnumerator WaitingData()
    {
        yield return new WaitUntil(() => isDataSent == true);
        ShowChanges();
    }
    public void ShowChanges()
    {
        if (side == "Hacker")
            unSide = "Admin";
        else
            unSide = "Hacker";
        removes = new List<int>();
        if(side == stepTurn)
        {
            for (int i = 0; i < SaveAction.AttackersNC.Count; i++)
            {
                ShowChangesFunc(i);
            }
            stepTurn = unSide;
        }
        else
        {
            for (int i = SaveAction.AttackersNC.Count-1; i >= 0; i--)
            {
                ShowChangesFunc(i);
            }
            stepTurn = side;
        }
        UpdatingOwnershipList();

        SaveAction.movementPoint = myNertworkComponents.Count * 0.1f + (SaveAction.movementPoint * 3) / myNertworkComponents.Count;
        movementPoints.text = SaveAction.movementPoint.ToString();

        if(currentNC != null)
            CancelPick();
        SaveAction.DeffendersNC = new List<GameObject>();
        SaveAction.UpdatedToDef = new List<GameObject>();
        SaveAction.AttackersNC = new List<GameObject>();
        SaveAction.UpdatedToScout = new List<GameObject>();
        SaveAction.SendingUnits = new List<int>();
        SaveAction.ConnectorsNC = new List<GameObject>();
        SaveAction.IdAttackersNC = new List<int>();
        SaveAction.IdDeffendersNC = new List<int>();
        connections = null;
        connectors = null;
        currentNC = null;
    }
    public void ShowChangesFunc(int i)
    {
        if(!removes.Contains(i))
        {
            if (side != SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().Ownership && SaveAction.AttackersNC[i].GetComponent<NetworkComponentController>().Ownership == side &&
                    unSide != SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().Ownership)
            {
                SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(-SaveAction.SendingUnits[i], "Neutral");
                if (SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().units < 0)
                {
                    for (int j = 0; j < SaveAction.DeffendersNC.Count; j++)
                    {
                        if (SaveAction.AttackersNC[j].GetComponent<NetworkComponentController>().ID ==
                            SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ID)
                        {
                            removes.Add(j);
                        }
                    }
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeSide(side);
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().units *= -1;
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(0, "Neutral");
                }
            }
            else if (side != SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().Ownership && SaveAction.AttackersNC[i].GetComponent<NetworkComponentController>().Ownership != side)
            {
                SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(-SaveAction.SendingUnits[i], unSide);
                if (SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().units < 0)
                {
                    for (int j = 0; j < SaveAction.DeffendersNC.Count; j++)
                    {
                        if (SaveAction.AttackersNC[j].GetComponent<NetworkComponentController>().ID ==
                            SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ID)
                        {
                            removes.Add(j);
                        }
                    }
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeSide(unSide);
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().units *= -1;
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(0, unSide);
                }
            }
            else if ((SaveAction.AttackersNC[i].GetComponent<NetworkComponentController>().Ownership == side && SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().Ownership == unSide) ||
                (SaveAction.AttackersNC[i].GetComponent<NetworkComponentController>().Ownership == unSide && SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().Ownership == side))
            {
                SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(-SaveAction.SendingUnits[i], "Neutral");
                if (SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().units < 0)
                {
                    for (int j = 0; j < SaveAction.AttackersNC.Count; j++)
                    {
                        if (SaveAction.AttackersNC[j].GetComponent<NetworkComponentController>().ID ==
                            SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ID)
                        {
                            removes.Add(j);
                        }
                    }
                    for(int j = 0;j < SaveAction.DeffendersNC.Count; j++)
                    {
                        Debug.Log(i);
                        Debug.Log(SaveAction.DeffendersNC[i]);
                    }
                    if(SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().Ownership == "Hacker")
                        SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeSide("Admin");
                    else
                        SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeSide("Hacker");
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().units *= -1;
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(0, "Neutral");
                }
            }
            else
            {
                SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(SaveAction.SendingUnits[i], "Neutral");
            }
        }
    }
    [PunRPC]
    private void AddAnotherPlayerMovement(object[] AttackersA, object[] DeffendersA, object[] SendingUnitsA, object[] UpdatingUnitsId, object[] UpdatingUnits)
    {
        List<int> AttackersAP = AttackersA.Cast<int>().ToList();
        List<int> DeffendersAP = DeffendersA.Cast<int>().ToList();
        List<int> SendingUnitsAP = SendingUnitsA.Cast<int>().ToList();

        List<int> UpdateUnitsId = UpdatingUnitsId.Cast<int>().ToList();
        List<int> UpdateUnits = UpdatingUnits.Cast<int>().ToList();

        for (int i = 0; i < UpdateUnitsId.Count; i++)
        {
            foreach (var NetwokComponent in GameObject.FindGameObjectsWithTag("NetworkComponent"))
            {
                if (NetwokComponent.GetComponent<NetworkComponentController>().ID == UpdateUnitsId[i])
                {
                    NetwokComponent.GetComponent<NetworkComponentController>().units = UpdateUnits[i];
                    NetwokComponent.GetComponent<NetworkComponentController>().ChangeUnits(0, "Neutral");
                }
            }
        }

        for (int i = 0; i < AttackersAP.Count; i++)
        {
            SaveAction.IdAttackersNC.Add(AttackersAP[i]);
            SaveAction.IdDeffendersNC.Add(DeffendersAP[i]);
            SaveAction.SendingUnits.Add(SendingUnitsAP[i]);
        }
        
        for (int i = 0;i < AttackersAP.Count;i++)
        {
            foreach (var NetwokComponent in GameObject.FindGameObjectsWithTag("NetworkComponent"))
            {
                if(NetwokComponent.GetComponent<NetworkComponentController>().ID  == AttackersAP[i])
                {
                    SaveAction.AttackersNC.Add(NetwokComponent);
                    foreach (var NetwokComponent1 in GameObject.FindGameObjectsWithTag("NetworkComponent"))
                    {
                        if (NetwokComponent1.GetComponent<NetworkComponentController>().ID == DeffendersAP[i])
                        {
                            SaveAction.DeffendersNC.Add(NetwokComponent1);
                            SaveAction.SendingUnits.Add(SendingUnitsAP[i]);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        AttackersAP = null;
        DeffendersAP = null;
        SendingUnitsAP = null;
        UpdateUnits = null;
        UpdateUnitsId = null;
        isDataSent = true;
    }

    private void OnDestroy()
    {
        nc.onNetworkComponentClickEvent -= instance.OnNCClick;
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        moneyText.text = SaveAction.money.ToString();
        view = GetComponent<PhotonView>();
        int id = 0;
        foreach(var NetwokComponent in GameObject.FindGameObjectsWithTag("NetworkComponent"))
        {
            NetwokComponent.GetComponent<NetworkComponentController>().ChangeUnits(20, "Neutral");
            NetwokComponent.GetComponent<NetworkComponentController>().ID = id;
            id++;
        }
    }
}
