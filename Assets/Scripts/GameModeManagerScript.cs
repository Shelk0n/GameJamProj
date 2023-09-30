using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class GameModeManagerScript : MonoBehaviour
{
    [SerializeField] Text masterUsername;
    [SerializeField] Text secondUsername;

    [SerializeField] GameObject waitingForAnotherPalyer;

    private PhotonView view;
    private void Start()
    {
        view = GetComponent<PhotonView>();
        SetNickName();
    }
    public void SetNickName()
    {
        if (masterUsername.text == "")
            masterUsername.text = PlayerPrefs.GetString("username");
        if (view.IsMine)
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        view.RPC("SendSecond", RpcTarget.AllBuffered, PlayerPrefs.GetString("username"));
    }
    [PunRPC]
    private void SendSecond(string username)
    {
        secondUsername.text = username;
        if(masterUsername.text == PhotonNetwork.NickName)
            view.RPC("SendFirst", RpcTarget.Others, PlayerPrefs.GetString("username"));
    }
    [PunRPC]
    private void SendFirst(string username)
    {
        masterUsername.text = username;
        waitingForAnotherPalyer.SetActive(false);
    }
    public void OnBasicClick()
    {
        if (secondUsername.text != "" && masterUsername.text == PhotonNetwork.NickName)
            view.RPC("LoadBasic", RpcTarget.All);
    }
    [PunRPC]
    private void LoadBasic()
    {
        PhotonNetwork.LoadLevel("BasicField");
    }
}
