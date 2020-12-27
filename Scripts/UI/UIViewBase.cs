using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tora.ui {

    public class UIViewBase : MonoBehaviour, IUIView {
        public bool IsOpen {
            get;
            protected set;
        }

        public virtual void Open() {

		}

        public virtual void Close() {

		}

    }

}