using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using HexaStack.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = HexaStack.Core.Logger;

public class FirebaseManager : SingletonBehaviour<FirebaseManager>
{
    // Firebase 모든 접근 타입
    FirebaseApp m_App;
    // Remote Config
    private FirebaseRemoteConfig m_RemoteConfig;
    private bool m_IsRemoteConfigInit = false;
    private Dictionary<string, object> m_RemoteConfigDic = new Dictionary<string, object>();
    protected override void Init()
    {
        base.Init();
        StartCoroutine(InitFirebaseServiceCo());
    }

    public bool IsInit()
    {
        return m_IsRemoteConfigInit;
    }

    private IEnumerator InitFirebaseServiceCo()
    {
        // Firebase 비동기 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => // task로 초기화 확인
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                m_App = FirebaseApp.DefaultInstance;
                Debug.Log("FirebaseApp initialization success");

                InitRemoteConfig();
            }
            else
            {
                Debug.LogError($"failed. Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });

        var elapsedTime = 0f; // 경과시간 트래킹
        while(elapsedTime < GlobalDefine.THIRD_PARTY_SERVICE_INIT_TIME)
        {
            if(IsInit())
            {
                Logger.Log($"{GetType()} initialized success");
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Logger.LogError($"FirebaseApp initialization failed");
    }

    #region REMOTE_CONFIG
    private void InitRemoteConfig()
    {
        m_RemoteConfig = FirebaseRemoteConfig.DefaultInstance;
        if(m_RemoteConfig == null)
        {
            Logger.LogError("Initialization failed. Firebase Remote Config is null");
            return;
        }

        m_RemoteConfigDic.Add("dev_app_version", string.Empty);
        m_RemoteConfigDic.Add("real_app_version", string.Empty);
        
        m_RemoteConfig.SetDefaultsAsync(m_RemoteConfigDic).ContinueWithOnMainThread(task =>
        {
            m_RemoteConfig.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(fetchTask =>
            {
                if(fetchTask.IsCompleted)
                {
                    m_RemoteConfig.ActivateAsync().ContinueWithOnMainThread(activateTask =>
                    {
                        if (activateTask.IsCompleted)
                        {
                            m_RemoteConfigDic["dev_app_version"] = m_RemoteConfig.GetValue("dev_app_version").StringValue;
                            m_RemoteConfigDic["real_app_version"] = m_RemoteConfig.GetValue("real_app_version").StringValue;
                            m_IsRemoteConfigInit = true;
                        }
                    });
                }
            });
        });
    }

    public string GetAppVersion()
    {
#if DEV_VER
        if(m_RemoteConfigDic.ContainsKey("dev_app_version"))
        {
            return m_RemoteConfigDic["dev_app_version"].ToString();
        }
#else
        if (m_RemoteConfigDic.ContainsKey("real_app_version"))
        {
            return m_RemoteConfigDic["real_app_version"].ToString();
        }
#endif
        return string.Empty;
    }
#endregion
}
