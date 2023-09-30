using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField roomName;
    [SerializeField] RoomListItem itemPrefab;
    [SerializeField] Transform content;

    List<RoomInfo> allRoomsInfo = new List<RoomInfo>();
    List<RoomListItem> allListItem = new List<RoomListItem>();
    public void CreateRoomButton()
    {
        if(!PhotonNetwork.IsConnected)
            return;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomName.text, roomOptions, TypedLobby.Default);
        PhotonNetwork.LoadLevel("GameModesScene");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList) 
        {
            for (int i = 0; i < allRoomsInfo.Count; i++)
            {
                if (info.Name == allRoomsInfo[i].Name)
                {
                    allListItem[i].textPlayerCount.text = info.PlayerCount + "/" + info.MaxPlayers;
                    if (info.PlayerCount == 0)
                    {
                        Destroy(allListItem[i].gameObject);
                        allListItem.RemoveAt(i);
                        allRoomsInfo.RemoveAt(i);
                    }
                    return;
                }
            }
            RoomListItem listItem = Instantiate(itemPrefab, content);
            if (listItem != null)
            {
                listItem.SetInfo(info);
                allListItem.Add(listItem);
                allRoomsInfo.Add(info);
            }
        }
    }
    public void JoinRandomRoomButton()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void JoinButton()
    {
        PhotonNetwork.JoinRoom(roomName.text);
    }
}
