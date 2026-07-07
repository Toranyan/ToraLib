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

        [SerializeField]
        protected Button[] m_buttonResponse;

        protected DialogCallback m_callback;

        public delegate void DialogCallback(int response);


		private void Start()
		{
            for (int i = 0; i < m_buttonResponse.Length; i++)
			{
                m_buttonResponse[i].onClick.AddListener(() => OnClick(i));
			}

            // m_buttonOK and m_buttonClose are optional per-dialog — not every dialog has both.
            if (m_buttonOK != null)
                m_buttonOK.onClick.AddListener(() => OnClick(0));

            if (m_buttonClose != null)
                m_buttonClose.onClick.AddListener(Close);
		}

		public virtual void Init(string title, string message, string[] buttonLabel, DialogCallback callback) {
            m_callback = callback;
		}

        public virtual void OnClick(int index) {
            m_callback?.Invoke(index);
            Close();
		}


    }

}