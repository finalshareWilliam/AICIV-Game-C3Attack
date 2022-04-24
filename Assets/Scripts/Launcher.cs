using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField playerNameField;
    [SerializeField] private InputField roomNameField;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject connectedUIPanel;
    [SerializeField] private TMP_Text playerList;
    [SerializeField] private GameObject btnStartGame;

    void Start()
    {
        //Faz com que todos os clientes sincronizem a cena com o master client
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        connectedUIPanel.SetActive(false);
        loadingPanel.SetActive(true);
        roomPanel.SetActive(false);

        playerNameField.text = string.Format("Player {0}", Random.Range(0, 9999));
        roomNameField.text = "dev";

        SetPlayerName();
        SetRoomName();
    }

    private void Update()
    {
        //SetPlayerName(playerNameField.text);
        //SetRoomName(roomNameField.text);
    }

    public void SetPlayerName()
    {
        if (string.IsNullOrWhiteSpace(playerNameField.text))
        {
            AlertManager.Instance.DangerText("INSIRA UM NOME");
            return;
        }

        PhotonNetwork.NickName = playerNameField.text;
    }

    public void SetRoomName()
    {
        if (string.IsNullOrWhiteSpace(roomNameField.text))
        {
            AlertManager.Instance.DangerText("INSIRA NOME DE SALA");
            return;
        }
    }

    public void HostGame()
    {
        if (string.IsNullOrWhiteSpace(roomNameField.text) || string.IsNullOrWhiteSpace(playerNameField.text))
        {
            AlertManager.Instance.DangerText("PREENCHA OS CAMPOS");
            return;
        }

        PhotonNetwork.CreateRoom(roomNameField.text, new Photon.Realtime.RoomOptions { IsVisible = true, MaxPlayers = 4, IsOpen = true });

        connectedUIPanel.SetActive(false);
        loadingPanel.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        loadingPanel.SetActive(true);
        roomPanel.SetActive(false);
    }

    public void JoinGame()
    {
        if (string.IsNullOrWhiteSpace(roomNameField.text) || string.IsNullOrWhiteSpace(playerNameField.text))
        {
            AlertManager.Instance.DangerText("PREENCHA OS CAMPOS");
            return;
        }
        
        PhotonNetwork.JoinRoom(roomNameField.text);

        connectedUIPanel.SetActive(false);
        loadingPanel.SetActive(true);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    //Callbacks
 
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        connectedUIPanel.SetActive(true);
        loadingPanel.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);
        loadingPanel.SetActive(false);

        playerList.text = string.Empty; //Reseta a lista para quando o jogador tiver entrado novamente não aparecer os antigos nomes de players

        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerList.text += player.NickName + "\n";
        }

        btnStartGame.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        btnStartGame.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        AlertManager.Instance.ShowText("Erro ao entrar na sala!", Color.yellow, 0.8f, 0, 230);
        connectedUIPanel.SetActive(true);
        loadingPanel.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        AlertManager.Instance.ShowText("Erro ao criar a sala!", Color.yellow, 0.8f, 0, 230);
        connectedUIPanel.SetActive(true);
        loadingPanel.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerList.text += newPlayer.NickName + "\n";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int indexPlayerName = playerList.text.IndexOf(otherPlayer.NickName);
        if(indexPlayerName >= 0)
        {
            playerList.text = playerList.text.Remove(indexPlayerName);
        }
    }

}
