using _Project.Develop.StunGames.GameJam29.Runtime.Services;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime.UI
{
    public abstract class UIPanel : MonoBehaviour
    {
        protected UIService uiService;
        public virtual bool IsVisible() => gameObject.activeSelf;
        public virtual bool IsInitialized { get; protected set; }
        
        public void Construct(UIService uiService)
        {
            this.uiService = uiService;
        }

        protected virtual void BindButtons() {} 
        public virtual void Release() { RemoveAllListeners(); IsInitialized = false; } 
        public virtual void Hide()
        {
            RemoveAllListeners();
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            if (IsVisible()) return;
            gameObject.SetActive(true);
            BindButtons();
        }

        protected virtual void RemoveAllListeners() { }
    }
}