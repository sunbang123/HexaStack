using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexaStack.Controllers.Managers;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Views
{
    public class InGameUIController : MonoBehaviour
    {
        public void Init()
        {
        }
        public void OnClickSettingsBtn()
        {
            Logger.Log($"{GetType()}::OnClickSettingsBtn");

            if (!object.ReferenceEquals(UIManager.Instance, null))
            {
                UIManager.Instance.OpenUI<OptionPopup>(null);
            }
            else
            {
                Logger.LogError("UIManager Instance를 찾을 수 없습니다!");
            }
        }

        public void OnClickProfileBtn()
        {
            Logger.Log($"{GetType()}::OnClickProfileBtn");
        }

        public void OnClickCurrChapter()
        {
            Logger.Log($"{GetType()}::OnClickCurrChapter");
        }

        public void OnClickLobbyBtn()
        {
            if (!System.Object.ReferenceEquals(SceneLoader.Instance, null))
                SceneLoader.Instance.LoadScene(SceneType.Lobby);
        }
    }
}
