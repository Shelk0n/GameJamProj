using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonScript : MonoBehaviour
{
    public void OnBackClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
