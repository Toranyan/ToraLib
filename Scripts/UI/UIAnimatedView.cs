using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tora.ui {

    public class UIAnimatedView : UIViewBase {

        protected Action m_openCallback;
        protected Action m_closeCallback;

        public virtual void Open(Action openCallback) {
            m_openCallback = openCallback;
            Open();
		}

        public virtual void Close(Action closeCallback) {
            m_closeCallback = closeCallback;
            Close();
		}
 
    }

}