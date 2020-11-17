using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject startMenu;
    public InputField usernameField;

    public void StartGame()
    {
        ConnectionManager.instance.MakeConnection();

        ConnectionManager.instance.SendUsername(usernameField.text);
         
        PlayerManager.instance.id = ConnectionManager.instance.ReceiveId();

        DisableMenuUI();

        ConnectionManager.instance.isGameStarted = true;
    }


    private void DisableMenuUI()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
    }
}
