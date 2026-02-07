using Firebase;
using Firebase.Extensions;
using HexaStack.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : SingletonBehaviour<FirebaseManager>
{
    // Firebase 모든 접근 타입
    FirebaseApp m_App;

    protected override void Init()
    {
        base.Init();
        StartCoroutine(InitFirebaseServiceCo());
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
            }
            else
            {
                Debug.LogError($"failed. Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
        yield break;
    }
}
