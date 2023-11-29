using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    string URL = "https://daun.yatopiacraft.ru:44384";

    [SerializeField] InputField roomName;
    [SerializeField] RoomListItem itemPrefab;
    [SerializeField] Transform content;
    [SerializeField] Toggle hideFullRooms;

    //List<RoomInfo> allRoomsInfo = new List<RoomInfo>();
    List<RoomListItem> allListItem = new List<RoomListItem>();
    public void CreateRoomButton()
    {
        /*if(!PhotonNetwork.IsConnected)
            return;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(roomName.text, roomOptions, TypedLobby.Default);
        PhotonNetwork.LoadLevel("GameModesScene");*/
    }
    /*public override void OnRoomListUpdate(List<RoomInfo> roomList)
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
    }*/
    public void OnRefreshClick()
    {
        string additional;
        if (hideFullRooms.isOn)
            additional = "getnotfullroomsinfo";
        else
            additional = "getallroomsinfo";
        StartCoroutine(Refresh(additional));

    }
    IEnumerator Refresh(string additional)
    {
        foreach (RoomListItem item in allListItem)
            Destroy(item.gameObject);
        allListItem.Clear();
        UnityWebRequest www = UnityWebRequest.Get($"{URL}/{additional}");
        yield return www.SendWebRequest();
        string roominfo = www.downloadHandler.text;
        roominfo = roominfo.Replace("\"", "");
        string[] rooms = roominfo.Split("/");
        foreach (string room in rooms)
        {
            if(room != "")
            {
                RoomListItem listItem = Instantiate(itemPrefab, content);
                string[] info = room.Split(",");
                listItem.SetInfo(Convert.ToInt32(info[0]), info[1], Convert.ToInt32(info[2]));
                allListItem.Add(listItem);
            }
        }
    }
    public void JoinRandomRoomButton()
    {
        //PhotonNetwork.JoinRandomRoom();
    }
    public void JoinButton()
    {
        //PhotonNetwork.JoinRoom(roomName.text);
    }
}
