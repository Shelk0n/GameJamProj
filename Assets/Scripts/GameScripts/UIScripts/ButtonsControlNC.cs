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

    public string side;

    [SerializeField] GameObject readyHackerDetector;
    [SerializeField] GameObject readyAdminDetector;
    [SerializeField] GameObject waitingForAnotherPlayer;

    [SerializeField] private int currtntUnits;
    private List<GameObject> connections;
    private List<GameObject> connectors;
    private GameObject currentNC;

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
            if (isMove && Convert.ToInt32(inputUnitsSend.text) <= currtntUnits)
            {
                SaveAction.SendingUnits.Add(Convert.ToInt32(inputUnitsSend.text));
                SaveAction.AttackersNC.Add(currentNC);
                SaveAction.DeffendersNC.Add(attackedObject);
                SaveAction.ConnectorsNC.Add(connectors[connections.IndexOf(attackedObject)]);
                connectors[connections.IndexOf(attackedObject)].GetComponent<SpriteRenderer>().color = Color.blue;
                currentNC.GetComponent<NetworkComponentController>().ChangeUnits(-Convert.ToInt32(inputUnitsSend.text), side);
            } 
            else if(isSummon)
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
        }
    }
    public void OnNextMove()
    {
        List<GameObject> myComponents = new List<GameObject>();
        for(int i = 0; i < SaveAction.DeffendersNC.Count; i++)
        {
            SaveAction.IdAttackersNC.Add(SaveAction.AttackersNC[i].GetComponent<NetworkComponentController>().ID);
            SaveAction.IdDeffendersNC.Add(SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ID);
        }
        object[] IdAttackers = SaveAction.IdAttackersNC.Cast<object>().ToArray();
        object[] IdDeffenders = SaveAction.IdAttackersNC.Cast<object>().ToArray();
        object[] SendingUnit = SaveAction.IdAttackersNC.Cast<object>().ToArray();
        view.RPC("AddAnotherPlayerMovement", RpcTarget.Others, IdAttackers, IdDeffenders, SendingUnit);
        StartCoroutine(WaitingData());
    }
    IEnumerator WaitingData()
    {
        yield return new WaitUntil(() => isDataSent == true);
        ShowChanges();
    }
    public void ShowChanges()
    {
        for (int i = 0; i < SaveAction.AttackersNC.Count; i++)
        {
            string sideDef = "";
            if (SaveAction.ConnectorsNC.Count < i)
            {
                if (side == "Hacker")
                    sideDef = "Admin";
                else
                    sideDef = "Hacker";
            }
            else
                sideDef = side;
                
            if(SaveAction.ConnectorsNC.Count < i)
            {
                if (sideDef != SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().Ownership)
                {
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(-SaveAction.SendingUnits[i], "Neutral");
                    if (SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().units < 0)
                    {
                        SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeSide(sideDef);
                        SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().units *= -1;
                        SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(0, "Neutral");
                    }
                }
                else
                {
                    SaveAction.DeffendersNC[i].GetComponent<NetworkComponentController>().ChangeUnits(SaveAction.SendingUnits[i], "Neutral");
                }
            }
        SaveAction.DeffendersNC.Clear();
        SaveAction.UpdatedToDef.Clear();
        SaveAction.AttackersNC.Clear();
        SaveAction.UpdatedToScout.Clear();
        SaveAction.SendingUnits.Clear();
        SaveAction.ConnectorsNC.Clear();
        CancelPick();
    }
    [PunRPC]
    private void AddAnotherPlayerMovement(object[] AttackersA, object[] DeffendersA, object[] SendingUnitsA)
    {
        List<int> AttackersAP = AttackersA.Cast<int>().ToList();
        List<int> DeffendersAP = DeffendersA.Cast<int>().ToList();
        List<int> SendingUnitsAP = SendingUnitsA.Cast<int>().ToList();
        Debug.Log(123);
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
                        if (NetwokComponent.GetComponent<NetworkComponentController>().ID == DeffendersAP[i])
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
            NetwokComponent.GetComponent<NetworkComponentController>().ID = id;
            id++;
        }
    }
}
