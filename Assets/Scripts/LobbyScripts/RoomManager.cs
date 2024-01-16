using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
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
        StartCoroutine(CreateRoom());
    }
    IEnumerator CreateRoom()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{URL}/createroom/{PlayerPrefs.GetInt("id")}");
        yield return www.SendWebRequest();
        string strinfo = www.downloadHandler.text;
        PlayerPrefs.SetInt("roomid", Convert.ToInt32(strinfo));
        SceneManager.LoadScene("GameModesScene");
    }
    IEnumerator JoinRoom()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{URL}/getroomid/{roomName.text}");
        yield return www.SendWebRequest();
        string strinfo = www.downloadHandler.text;
        PlayerPrefs.SetInt("roomid", Convert.ToInt32(strinfo));
        UnityWebRequest www1 = UnityWebRequest.Post($"{URL}/joinroom/{PlayerPrefs.GetInt("roomid")}", PlayerPrefs.GetString("username"), "string");
        yield return www1.SendWebRequest();
        if (www1.result == UnityWebRequest.Result.Success)
            SceneManager.LoadScene("GameModesScene");
        else
        {
            throw new NotImplementedException();
        }
    }
    public void OnJoinButtonClick()
    {
        StartCoroutine(JoinRoom());
    }
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
    private void Start()
    {
        StartCoroutine(Refresh("getnotfullroomsinfo"));
    }
}
