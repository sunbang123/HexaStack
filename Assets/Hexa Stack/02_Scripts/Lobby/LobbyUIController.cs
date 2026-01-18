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

        public void OnClickStartBtn()
        {
            Logger.Log($"{GetType()}::OnClickStartBtn");

            if (!System.Object.ReferenceEquals(SceneLoader.Instance, null))
            {
                var startData = new InGameSceneData(1, false);
                SceneLoader.Instance.LoadScene(SceneType.InGame, startData);
            }
        }
    }
}
