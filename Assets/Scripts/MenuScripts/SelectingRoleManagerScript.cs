using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectingRoleManagerScript : MonoBehaviour
{
    [SerializeField] Text HackerNickname;
    [SerializeField] Text SysadminNickname;

    [SerializeField] GameObject SelectingMenu;
    [SerializeField] GameObject Player;

    [SerializeField] GameObject HCheck;
    [SerializeField] GameObject SCheck;

    [SerializeField] GameObject Controller;

    void Start()
    {
        //view = GetComponent<PhotonView>();
    }
    public void OnHackerClick()
    {
        /*if(HackerNickname.text == "" && SysadminNickname.text != PlayerPrefs.GetString("username"))
            view.RPC("SendHacker", RpcTarget.AllBuffered, PlayerPrefs.GetString("username"));
        else if (HackerNickname.text == PlayerPrefs.GetString("username"))
            view.RPC("SendHacker", RpcTarget.AllBuffered, "");*/
    }
    public void OnSysadminClick()
    {
        /*if (SysadminNickname.text == "" && HackerNickname.text != PlayerPrefs.GetString("username"))
            view.RPC("SendSysadmin", RpcTarget.AllBuffered, PlayerPrefs.GetString("username"));
        else if (SysadminNickname.text == PlayerPrefs.GetString("username"))
            view.RPC("SendSysadmin", RpcTarget.AllBuffered, "");*/
    }
    private void SendHacker(string nickname)
    {
        HackerNickname.text = nickname;
    }
    private void SendSysadmin(string nickname)
    {
        SysadminNickname.text = nickname;
    }
    public void OnPlayClick()
    {
        if (SysadminNickname.text != HackerNickname.text && SysadminNickname.text != "" && HackerNickname.text != "")
        {
            /*view.RPC("Check", RpcTarget.AllBuffered, PlayerPrefs.GetString("username"));
            if(HCheck.activeSelf && SCheck.activeSelf)
            {
                view.RPC("HideSelectMenu", RpcTarget.AllBuffered);
            }*/
        }
    }
    private void HideSelectMenu()
    {
        if (HackerNickname.text == PlayerPrefs.GetString("username"))
            Controller.GetComponent<ButtonsControlNC>().side = "Hacker";
        else
            Controller.GetComponent<ButtonsControlNC>().side = "Admin";
        Camera.main.transform.position = new Vector3(0,0,-10);
        SelectingMenu.SetActive(false);
    }
    public void Check(string nickname)
    {
        if(HackerNickname.text == nickname)
        {
            if(HCheck.activeSelf == false)
                HCheck.SetActive(true);
            else 
                HCheck.SetActive(false);
        }
        if (SysadminNickname.text == nickname)
        {
            if (SCheck.activeSelf == false)
                SCheck.SetActive(true);
            else
                SCheck.SetActive(false);
        }
    }
}
