using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeManagerScript : MonoBehaviour
{
    [SerializeField] Text masterUsername;
    [SerializeField] Text secondUsername;

    [SerializeField] GameObject waitingForAnotherPalyer;

    private void Start()
    {
        SetNickName();
    }
    public void SetNickName()
    {
        /*if (masterUsername.text == "")
            masterUsername.text = PlayerPrefs.GetString("username");
        if (view.IsMine)
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        view.RPC("SendSecond", RpcTarget.AllBuffered, PlayerPrefs.GetString("username"));*/
    }
    private void SendSecond(string username)
    {
        /*secondUsername.text = username;
        if(masterUsername.text == PhotonNetwork.NickName)
            view.RPC("SendFirst", RpcTarget.Others, PlayerPrefs.GetString("username"));*/
    }
    private void SendFirst(string username)
    {
        masterUsername.text = username;
        waitingForAnotherPalyer.SetActive(false);
    }
    public void OnBasicClick()
    {
        /*if (secondUsername.text != "" && masterUsername.text == PhotonNetwork.NickName)
            view.RPC("LoadBasic", RpcTarget.All);*/
    }
    private void LoadBasic()
    {
        //PhotonNetwork.LoadLevel("BasicField");
    }
}
