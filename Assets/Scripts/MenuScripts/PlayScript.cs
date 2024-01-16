using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayScript : MonoBehaviour
{
    [SerializeField] GameObject nonRegisterError;
    public void OnPlayClick()
    {
        if(PlayerPrefs.HasKey("username"))
            SceneManager.LoadScene("Menu");
        else
            nonRegisterError.SetActive(true);
    }
    public void OnOkClick() => nonRegisterError.SetActive(false);
}
