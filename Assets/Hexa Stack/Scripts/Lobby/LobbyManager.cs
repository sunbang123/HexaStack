using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : SingletonBehaviour<LobbyManager>
{
    public LobbyUIController LobbyUIController { get; private set; }

    protected override void Init()
    {
        m_IsDestroyOnLoad = true;
        
        base.Init();
    }

    private void Start()
    {
        LobbyUIController = FindObjectOfType<LobbyUIController>();
        if(!LobbyUIController)
        {
            Logger.Log("LobbyUIController does not exist.");
            return;
        }

        LobbyUIController.Init();
    }

    public void StartInGame()
    {
        SceneLoader.Instance.LoadScene(SceneType.InGame);
    }
}
