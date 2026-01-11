using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HexaStack.Controllers.Managers;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Views
{
    public class LobbyUIController : MonoBehaviour
    {
        public void Init()
        {
        }

        public void OnClickSettingsBtn()
        {
            Logger.Log($"{GetType()}::OnClickSettingsBtn");
        }

        public void OnClickProfileBtn()
        {
            Logger.Log($"{GetType()}::OnClickProfileBtn");
        }

        public void OnClickCurrChapter()
        {
            Logger.Log($"{GetType()}::OnClickCurrChapter");
        }

        public void OnClickStartBtn()
        {
            Logger.Log($"{GetType()}::OnClickStartBtn");

            LobbyManager.Instance.StartInGame();
        }
    }
}
