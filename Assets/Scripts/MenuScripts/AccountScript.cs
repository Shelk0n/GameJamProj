using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Security.Cryptography;
using System;

public class AccountScript : MonoBehaviour
{
    string URL = "https://daun.yatopiacraft.ru:44384";

    [SerializeField] GameObject accountPage;
    [SerializeField] GameObject accountInfoPage;
    [SerializeField] GameObject regPage;
    [SerializeField] InputField usernameField;
    [SerializeField] InputField passField;
    [SerializeField] Text accountButtonText;
    [SerializeField] GameObject incorrect;

    [SerializeField] int accountLevel;
    [SerializeField] int wins;
    [SerializeField] int looses;

    [SerializeField] Text accountLvlText;
    [SerializeField] Text loosesText;
    [SerializeField] Text winsText;
    [SerializeField] Text UsernameText;

    [SerializeField] InputField UsernameTextReg;
    [SerializeField] InputField PasswordTextReg;
    [SerializeField] InputField PasswordRepTextReg;
    [SerializeField] GameObject notEqual;

    [SerializeField] int id;

    public void OnAccountClick()
    {
        if(accountButtonText.text == "Log in")
            accountPage.SetActive(true);
        else
            accountInfoPage.SetActive(true);
    }
    public void OnLogInClick()
    {
        if(usernameField.text == string.Empty)
        {
            usernameField.text = "Please enter your login";
            return;
        }
        if (passField.text == string.Empty)
        {
            passField.text = "Please enter your password";
            return;
        }

        byte[] sourcePassBytes;
        byte[] hashPassBytes;
        sourcePassBytes = ASCIIEncoding.ASCII.GetBytes(passField.text);
        hashPassBytes = new MD5CryptoServiceProvider().ComputeHash(sourcePassBytes);
        string hashPass = ByteArrayToString(hashPassBytes);
        StartCoroutine(CheckAccount(hashPass));

    }
    IEnumerator CheckAccount(string hash)
    {
        UnityWebRequest www = UnityWebRequest.Get($"{URL}/gethashaccount/{usernameField.text}");
        yield return www.SendWebRequest();
        if("\"" + hash + "\"" == www.downloadHandler.text)
        {
            PlayerPrefs.SetString("username", usernameField.text);
            PlayerPrefs.Save();
            StartCoroutine(LoadAccount());
            OnBackClick();
        }
        else
        {
            incorrect.SetActive(true);
        }
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            StartCoroutine(LoadAccount());
        }

    }
    IEnumerator LoadAccount()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{URL}/getinfoaccount/{PlayerPrefs.GetString("username")}");
        yield return www.SendWebRequest();
        string strinfo = www.downloadHandler.text;
        strinfo = strinfo.Replace("\"", "");
        accountButtonText.text = PlayerPrefs.GetString("username");
        string[] info = strinfo.Split("/");
        id = Convert.ToInt32(info[0]);
        UsernameText.text = info[1].Trim();
        accountLvlText.text = info[2];
        winsText.text = info[3];
        loosesText.text = info[4];
        PlayerPrefs.SetInt("id", id);
    }
    public void OnRegisterClick()
    {
        OnBackClick();
        regPage.SetActive(true);
    }
    public void OnRegClick()
    {
        if (UsernameTextReg.text == string.Empty)
        {
            UsernameTextReg.text = "Please enter your login";
            return;
        }
        if (PasswordTextReg.text == string.Empty)
        {
            PasswordTextReg.text = "Please enter your password";
            return;
        }
        if (PasswordRepTextReg.text == string.Empty)
        {
            PasswordRepTextReg.text = "Please enter your password";
            return;
        }
        if(PasswordRepTextReg.text != PasswordTextReg.text)
        {
            notEqual.SetActive(true);
            return;
        }
        byte[] sourcePassBytes;
        byte[] hashPassBytes;
        sourcePassBytes = ASCIIEncoding.ASCII.GetBytes(PasswordTextReg.text);
        hashPassBytes = new MD5CryptoServiceProvider().ComputeHash(sourcePassBytes);
        string hashPass = ByteArrayToString(hashPassBytes);
        StartCoroutine(CreateAccount(UsernameTextReg.text, hashPass));
    }
    IEnumerator CreateAccount(string username, string hashPassword)
    {
        UnityWebRequest www = UnityWebRequest.Post($"{URL}/createaccount/{username}", hashPassword, "string");
        yield return www.SendWebRequest();
        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("success");
        }
        OnBackClick();
    }
    public void OnLogoutClick()
    {
        PlayerPrefs.DeleteKey("username");
        accountButtonText.text = "Log in";
        OnBackClick();
    }
    public void OnBackClick()
    {
        accountPage.SetActive(false);
        accountInfoPage.SetActive(false);
        notEqual.SetActive(false);
        incorrect.SetActive(false);
        regPage.SetActive(false);
        usernameField.text = string.Empty;
        passField.text = string.Empty;
        PasswordRepTextReg.text = string.Empty;
        PasswordTextReg.text = string.Empty;
        UsernameTextReg.text = string.Empty;
    }
    static string ByteArrayToString(byte[] arrInput)
    {
        int i;
        StringBuilder sOutput = new StringBuilder(arrInput.Length);
        for (i = 0; i < arrInput.Length; i++)
        {
            sOutput.Append(arrInput[i].ToString("X2"));
        }
        return sOutput.ToString();
    }
}
