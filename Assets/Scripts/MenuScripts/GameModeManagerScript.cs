using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameModeManagerScript : MonoBehaviour
{
    string URL = "https://daun.yatopiacraft.ru:44384";

    [SerializeField] Text masterUsername;
    [SerializeField] Text secondUsername;
    [SerializeField] GameObject waitingForAnotherPalyer;

    private void Start()
    {
        PlayerPrefs.SetInt("isMaster", 2);
        StartCoroutine(SetInfo());
    }
    IEnumerator WaitingForAnotherP()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get($"{URL}/getroominfo/{PlayerPrefs.GetInt("roomid")}");
            yield return www.SendWebRequest();
            string strinfo = www.downloadHandler.text;
            string[] info = strinfo.Split('/');
            if (info[1] != "1")
            {
                secondUsername.text = info[5];
                waitingForAnotherPalyer.SetActive(false);
                SetInfo();
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator SetInfo()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{URL}/getroominfo/{PlayerPrefs.GetInt("roomid")}");
        yield return www.SendWebRequest();
        string strinfo = www.downloadHandler.text;
        strinfo = strinfo.Replace("\"", "");
        string[] info = strinfo.Split('/');

        masterUsername.text = info[4];
        secondUsername.text = info[5];

        if (string.IsNullOrWhiteSpace(secondUsername.text))
            StartCoroutine(WaitingForAnotherP());

        StartCoroutine(GetFieldType());
    }
    IEnumerator GetFieldType()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get($"{URL}/getfieldtype/{PlayerPrefs.GetInt("roomid")}");
            yield return www.SendWebRequest();
            int roomtype = Convert.ToInt32(www.downloadHandler.text);
            if (roomtype == -1)
            {
                StartCoroutine(SetFieldType(0));
                StartCoroutine(SetInfo());
            }
            else if (roomtype > 0)
            {
                SceneManager.LoadScene("BasicField");
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator SetFieldType(int fieldType)
    {
        UnityWebRequest www = UnityWebRequest.Post($"{URL}/setfieldtype/{PlayerPrefs.GetInt("roomid")}", fieldType.ToString(), "string");
        yield return www.SendWebRequest();
    }
    public void OnLeaveLobbyClick()
    {
        StartCoroutine(LeaveLobby());
    }
    IEnumerator LeaveLobby()
    {
        UnityWebRequest www = UnityWebRequest.Post($"{URL}/leaveroom/{PlayerPrefs.GetInt("roomid")}", PlayerPrefs.GetString("username"), "string");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            StopAllCoroutines();
            StartCoroutine(SetFieldType(-1));
            SceneManager.LoadScene("Menu");
            StopAllCoroutines();
        }
    }
    public void OnBasicClick()
    {
        if (masterUsername.text.Trim() == PlayerPrefs.GetString("username") && secondUsername.text != string.Empty)
        {
            PlayerPrefs.SetInt("isMaster", 1);
            StartCoroutine(SetFieldType(1));
        }
    }
    private void LoadBasic()
    {
        //PhotonNetwork.LoadLevel("BasicField");
    }
}
