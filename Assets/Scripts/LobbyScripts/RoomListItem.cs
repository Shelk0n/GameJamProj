using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    string URL = "https://daun.yatopiacraft.ru:44384";

    public Text textRoomName;
    public Text textPlayerCount;
    [SerializeField] private int roomId;
    public void SetInfo(int roomid, string username, int userscount)
    {
        textRoomName.text = username;
        textPlayerCount.text = userscount + "/2";
        roomId = roomid;
    }
    public void OnJoinRoomClick()
    {
        StartCoroutine(JoinRoom());
    }
    IEnumerator JoinRoom()
    {
        UnityWebRequest www = UnityWebRequest.Post($"{URL}/joinroom/{roomId}", PlayerPrefs.GetString("username"), "string");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            PlayerPrefs.SetInt("roomid", roomId);
            SceneManager.LoadScene("GameModesScene");
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
