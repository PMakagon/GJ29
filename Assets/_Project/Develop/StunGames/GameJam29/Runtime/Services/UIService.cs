using System;
using System.Collections.Generic;
using _Project.Develop.StunGames.GameJam29.Runtime.Gameplay;
using _Project.Develop.StunGames.GameJam29.Runtime.MainMenu;
using _Project.Develop.StunGames.GameJam29.Runtime.UI;
using UnityEditor;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Services
{
    public class UIService
    {
        private Dictionary<Type, UIPanel> _panels = new Dictionary<Type, UIPanel>();
        public bool IsInitialized { get; private set; }
        
        
        public void Initialize()
        {
            if (IsInitialized) _panels.Clear();
            //find all panels in scene, including inactive
            foreach (UIPanel panel in UnityEngine.Resources.FindObjectsOfTypeAll<UIPanel>())
            {
                GameObject panelGameObject = panel.gameObject;
                if (panelGameObject.hideFlags is HideFlags.NotEditable or HideFlags.HideAndDontSave)
                    continue;
#if UNITY_EDITOR
                if (EditorUtility.IsPersistent(panelGameObject)) //skip prefabs on disk
                    continue;
#endif
                Type type = panel.GetType();
                if (_panels.TryGetValue(type, out var uiPanel))
                {
                    Debug.LogWarningFormat("UIManager duplicate panel '{0}' on gameObjects '{1}' and '{2}'.", type.Name, uiPanel.name, panel.name);
                    continue;
                }
                _panels.Add(type, panel);
                panel.Construct(this);
                panel.Hide();
            }
            IsInitialized = true;
        }
        
        public void ShowUIPanel<T>() where T : UIPanel
        {
            Type type = typeof(T);
            if (!_panels.ContainsKey(type))
            {
                Debug.LogWarningFormat("UIPanel '{0}' doesn't exist.", type.Name);
                return;
            }
            UIPanel panel = _panels[type];
            // if (!panel.IsInitialized) Debug.LogError($"Panel {panel} not Initialized");
            if (panel.IsVisible()) Debug.LogWarningFormat("UIPanel '{0}' is already shown.", panel.gameObject.name);
            panel.Show();
        }
        public void HideUIPanel<T>() where T : UIPanel
        {
            Type type = typeof(T);
            if (!_panels.ContainsKey(type))
            {
                Debug.LogWarningFormat("UIPanel '{0}' doesn't exist.", type.Name);
                return;
            }
            UIPanel panel = _panels[type];
            if (!panel.IsVisible()) Debug.LogWarningFormat("UIPanel '{0}' is already hidden.", panel.gameObject.name);
            panel.Hide();
        }

        public void HideAllPanels()
        {
            foreach (var uiPanel in _panels)
            {
                uiPanel.Value.Hide();
            }
        }
        
        public T GetUIPanel<T>() where T : UIPanel
        {
            Type type = typeof(T);
            if (!_panels.ContainsKey(type))
            {
                Debug.LogWarningFormat("UIPanel '{0}' doesn't exist.", type.Name);
                return null;
            }
            UIPanel panel = _panels[type];
            return panel as T;
        }
        public void ReleaseUIPanel<T>() where T : UIPanel
        {
            Type type = typeof(T);
            if (!_panels.ContainsKey(type))
            {
                Debug.LogWarningFormat("UIPanel '{0}' doesn't exist.", type.Name);
                return;
            }
            UIPanel panel = _panels[type];
            panel.Release();
        }
        
        public void InitializeMainMenuScreen(MainMenuFlow mainMenuFlow)
        {
            GetUIPanel<MainMenuScreen>().Init(mainMenuFlow);
        }
        
        public void InitializeHUDScreen(GameplayFlow gameplayFlow)
        {
            GetUIPanel<HUDScreen>().Init(gameplayFlow);
        }
    }
}