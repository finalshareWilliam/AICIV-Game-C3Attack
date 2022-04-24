using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance = null;
    [SerializeField] private GameObject panel;

    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        panel.SetActive(false);    
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public void BackToTheLobby()
    {
        StartCoroutine(LeaveRoom());
    }

    IEnumerator LeaveRoom()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(0);
    }
}
