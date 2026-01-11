using System.Collections.Generic;
using UnityEngine;
using HexaStack.Controllers.Managers;
using HexaStack.Views;
using HexaStack.Controllers;
using HexaStack.Models;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Core
{
    /// <summary>
    /// 모든 Manager와 Controller를 중앙에서 관리하는 레지스트리
    /// MVC 패턴의 중앙 관리 시스템
    /// </summary>
    public class ManagerRegistry : SingletonBehaviour<ManagerRegistry>
    {
        // Managers
        public InGameManager InGameManager { get; private set; }
        public LobbyManager LobbyManager { get; private set; }

        // Views
        public InGameUIController InGameUIController { get; private set; }
        public LobbyUIController LobbyUIController { get; private set; }

        // Controllers
        public MergeController MergeController { get; private set; }
        public StackSpawnerController StackSpawnerController { get; private set; }
        public StackController StackController { get; private set; }
        public GridRotatorController GridRotatorController { get; private set; }

        // Models
        public ScoreManager ScoreManager { get; private set; }

        protected override void Init()
        {
            m_IsDestroyOnLoad = false; // 씬 전환 시 유지
            base.Init();
        }

        private void Start()
        {
            RegisterAllManagers();
        }

        /// <summary>
        /// 모든 Manager와 Controller를 찾아서 등록
        /// </summary>
        private void RegisterAllManagers()
        {
            // Managers 등록
            InGameManager = FindObjectOfType<InGameManager>();
            LobbyManager = FindObjectOfType<LobbyManager>();

            // Views 등록
            InGameUIController = FindObjectOfType<InGameUIController>();
            LobbyUIController = FindObjectOfType<LobbyUIController>();

            // Controllers 등록
            MergeController = FindObjectOfType<MergeController>();
            StackSpawnerController = FindObjectOfType<StackSpawnerController>();
            StackController = FindObjectOfType<StackController>();
            GridRotatorController = FindObjectOfType<GridRotatorController>();

            // Models 등록
            ScoreManager = FindObjectOfType<ScoreManager>();

            LogRegistrationStatus();
        }

        /// <summary>
        /// 등록 상태를 로그로 출력
        /// </summary>
        private void LogRegistrationStatus()
        {
            Logger.Log("=== ManagerRegistry Registration Status ===");
            Logger.Log($"InGameManager: {(InGameManager != null ? "✓" : "✗")}");
            Logger.Log($"LobbyManager: {(LobbyManager != null ? "✓" : "✗")}");
            Logger.Log($"InGameUIController: {(InGameUIController != null ? "✓" : "✗")}");
            Logger.Log($"LobbyUIController: {(LobbyUIController != null ? "✓" : "✗")}");
            Logger.Log($"MergeController: {(MergeController != null ? "✓" : "✗")}");
            Logger.Log($"StackSpawnerController: {(StackSpawnerController != null ? "✓" : "✗")}");
            Logger.Log($"StackController: {(StackController != null ? "✓" : "✗")}");
            Logger.Log($"GridRotatorController: {(GridRotatorController != null ? "✓" : "✗")}");
            Logger.Log($"ScoreManager: {(ScoreManager != null ? "✓" : "✗")}");
            Logger.Log("===========================================");
        }

        /// <summary>
        /// 특정 Manager를 동적으로 등록 (런타임에 생성된 경우)
        /// </summary>
        public void RegisterManager<T>(T manager) where T : MonoBehaviour
        {
            switch (manager)
            {
                case InGameManager mgr:
                    InGameManager = mgr;
                    Logger.Log("InGameManager registered dynamically");
                    break;
                case LobbyManager mgr:
                    LobbyManager = mgr;
                    Logger.Log("LobbyManager registered dynamically");
                    break;
                case MergeController mgr:
                    MergeController = mgr;
                    Logger.Log("MergeController registered dynamically");
                    break;
                case StackSpawnerController mgr:
                    StackSpawnerController = mgr;
                    Logger.Log("StackSpawnerController registered dynamically");
                    break;
                case StackController mgr:
                    StackController = mgr;
                    Logger.Log("StackController registered dynamically");
                    break;
                case GridRotatorController mgr:
                    GridRotatorController = mgr;
                    Logger.Log("GridRotatorController registered dynamically");
                    break;
                case ScoreManager mgr:
                    ScoreManager = mgr;
                    Logger.Log("ScoreManager registered dynamically");
                    break;
            }
        }

        /// <summary>
        /// 모든 Manager 초기화
        /// </summary>
        public void InitializeAllManagers()
        {
            if (InGameManager != null)
            {
                // InGameManager는 이미 초기화됨
            }

            if (LobbyManager != null)
            {
                // LobbyManager는 이미 초기화됨
            }
        }
    }
}
