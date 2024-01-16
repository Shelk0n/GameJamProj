using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectingRoleManagerScript : MonoBehaviour
{
    string URL = "https://daun.yatopiacraft.ru:44384";

    [SerializeField] Text HackerNickname;
    [SerializeField] Text SysadminNickname;

    [SerializeField] GameObject SelectingMenu;
    [SerializeField] GameObject Player;

    [SerializeField] GameObject HCheck;
    [SerializeField] GameObject SCheck;

    [SerializeField] GameObject Controller;

    void Start()
    {
        StartCoroutine(WaitingForAnotherPlayerSideDesision());
    }
    IEnumerator WaitingForAnotherPlayerSideDesision()
    {
        int otherSide = 0;
        if (PlayerPrefs.GetInt("isMaster") == 1)
            otherSide = 2;
        else
            otherSide = 1;
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get($"{URL}/getuserside/{PlayerPrefs.GetInt("roomid")}/{otherSide}");
            yield return www.SendWebRequest();

            UnityWebRequest www1 = UnityWebRequest.Get($"{URL}/getroominfo/{PlayerPrefs.GetInt("roomid")}");
            yield return www1.SendWebRequest();
            string strinfo = www1.downloadHandler.text;
            strinfo = strinfo.Replace("\"", "");
            string[] info = strinfo.Split('/');
            if (www.downloadHandler.text == "true")
            {
                if (HackerNickname.text == info[3 + otherSide])
                    HackerNickname.text = string.Empty;
                SysadminNickname.text = string.Empty;
                SysadminNickname.text = info[3 + otherSide];
            }
            else if (www.downloadHandler.text == "false")
            {
                if (SysadminNickname.text == info[3 + otherSide])
                    SysadminNickname.text = string.Empty;
                HackerNickname.text = string.Empty;
                HackerNickname.text = info[3 + otherSide];
            }
            else if (www.downloadHandler.text == "null")
            {
                if(HackerNickname.text == info[3 + otherSide])
                    HackerNickname.text = string.Empty;
                else if (SysadminNickname.text == info[3 + otherSide])
                    SysadminNickname.text = string.Empty;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void OnHackerClick()
    {
        if(HackerNickname.text == string.Empty && SysadminNickname.text != PlayerPrefs.GetString("username"))
        {
            StartCoroutine(SetSide(false));
            HackerNickname.text = PlayerPrefs.GetString("username");
        }
        else if (HackerNickname.text == PlayerPrefs.GetString("username"))
        {
            StartCoroutine(SetSide(null));
            HackerNickname.text = string.Empty;
        }
    }
    IEnumerator SetSide(bool? side)
    {
        string data = "null";
        if (side == true)
            data = "true";
        else if (side == false)
            data = "false";
        UnityWebRequest www = UnityWebRequest.Get($"{URL}/setuserside/{PlayerPrefs.GetInt("roomid")}/{PlayerPrefs.GetInt("isMaster")}/{data}");
        yield return www.SendWebRequest();
    }
    public void OnSysadminClick()
    {
        if (SysadminNickname.text == string.Empty && HackerNickname.text != PlayerPrefs.GetString("username"))
        {
            StartCoroutine(SetSide(true));
            SysadminNickname.text = PlayerPrefs.GetString("username");
        }
        else if (SysadminNickname.text == PlayerPrefs.GetString("username"))
        {
            StartCoroutine(SetSide(null));
            SysadminNickname.text = string.Empty;
        }
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
