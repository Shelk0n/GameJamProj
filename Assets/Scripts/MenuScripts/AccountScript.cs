using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountScript : MonoBehaviour
{
    [SerializeField] GameObject accountPage;
    [SerializeField] InputField usernameField;
    public void OnAccountClick()
    {
        accountPage.SetActive(true);
        if (PlayerPrefs.HasKey("username"))
            usernameField.text = PlayerPrefs.GetString("username");
    }
    private void Start()
    {
        if (!PlayerPrefs.HasKey("username"))
        {
            PlayerPrefs.SetString("username", "Anonym");
            PlayerPrefs.Save();
        }

    }
    public void OnBackClick()
    {
        accountPage.SetActive(false);
    }
    public void OnSaveClick()
    {
        PlayerPrefs.SetString("username", usernameField.text);
        PlayerPrefs.Save();
    }
}
