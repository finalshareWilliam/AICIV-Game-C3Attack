using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private PhotonView PV;
    [SerializeField] private TMP_Text playerNameText;
    void Start()
    {
        playerNameText.text = PV.Owner.NickName;
    }
}
