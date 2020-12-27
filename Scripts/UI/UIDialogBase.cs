using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;

namespace tora.ui {

    public class UIDialogBase : UIAnimatedView {

        [SerializeField]
        protected Button m_buttonOK;

        [SerializeField]
        protected Button m_buttonClose;

        protected DialogCallback m_callback;
        protected int m_response;

        public delegate void DialogCallback(int response);

        public virtual void Init(string title, string message, string[] buttonLabel, DialogCallback callback) {
            m_callback = callback;
		}

        public virtual void OnClick(int index) {
            m_response = index;
            Close();
		}

    }

}