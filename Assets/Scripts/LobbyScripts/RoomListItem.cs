using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class RoomListItem : MonoBehaviour
{
    public Text textRoomName;
    public Text textPlayerCount;
    public void SetInfo(RoomInfo info)
    {
        textRoomName.text = info.Name;
        textPlayerCount.text = info.PlayerCount + "/" + info.MaxPlayers;
    }
    public void JoinToRoom()
    {
        PhotonNetwork.JoinRoom(textRoomName.text);
    }
}
