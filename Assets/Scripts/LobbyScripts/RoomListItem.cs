using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public Text textRoomName;
    public Text textPlayerCount;
    [SerializeField] private int roomId;
    public void SetInfo(int roomid, string username, int userscount)
    {
        textRoomName.text = username;
        textPlayerCount.text = userscount + "/2";
        roomId = roomid;
    }
    public void JoinRoom()
    {
    }
}
