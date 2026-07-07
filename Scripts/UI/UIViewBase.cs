using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace tora.ui {

    public class UIViewBase : MonoBehaviour, IUIView {

        [SerializeField]
        private GameObject _root;

        public bool IsOpen {
            get;
            protected set;
        }

        public bool IsShown
		{
            get;
            protected set;
		}

        /// <summary>Raised once a view has finished opening (after OnPostOpen).</summary>
        public event Action Opened;

        /// <summary>Raised once a view has finished closing (after OnPostClose).</summary>
        public event Action Closed;

        public virtual void Show() {
            _root.SetActive(true);
            IsShown = true;
        }

        public virtual void Hide() {
            _root.SetActive(false);
            IsShown = false;
        }

		public virtual void Open()
		{
            OnPreOpen();
            Show();
            IsOpen = true;
            OnPostOpen();
            RaiseOpened();
        }

		public virtual void Close()
		{
            OnPreClose();
            Hide();
            IsOpen = false;
            OnPostClose();
            RaiseClosed();
        }

        /// <summary>Called immediately before the view is shown. Override to prepare state.</summary>
        protected virtual void OnPreOpen() { }

        /// <summary>Called once the view has finished opening (after any open animation).</summary>
        protected virtual void OnPostOpen() { }

        /// <summary>Called immediately before the view starts closing (before any close animation).</summary>
        protected virtual void OnPreClose() { }

        /// <summary>Called once the view has finished closing (after being hidden).</summary>
        protected virtual void OnPostClose() { }

        /// <summary>Derived views that override Open() must call this once opening is fully finished.</summary>
        protected void RaiseOpened() => Opened?.Invoke();

        /// <summary>Derived views that override Close() must call this once closing is fully finished.</summary>
        protected void RaiseClosed() => Closed?.Invoke();
	}

}