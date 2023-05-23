using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tora.ui {

    public class UIViewBase : MonoBehaviour, IUIView {

        [SerializeField]
        private GameObject _root;

        public bool IsOpen {
            get;
            protected set;
        }

        public virtual void Open() {
            _root.SetActive(true);
            IsOpen = true;
        }

        public virtual void Close() {
            _root.SetActive(false);
            IsOpen = false;
        }

    }

}