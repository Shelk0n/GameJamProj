using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PhotonManaager : MonoBehaviourPunCallbacks
{
    [SerializeField] string region;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion(region);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("You connected to " + PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedRoom()
    {
        if(!PhotonNetwork.InLobby)
            PhotonNetwork.LoadLevel("GameModesScene");
    }

}
