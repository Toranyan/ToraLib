﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace tora.ui {

    public class UIAnimatedView : UIViewBase {

        [SerializeField]
        protected Animator _animator;

        [SerializeField]
        protected string _openStateName = "Open";

        [SerializeField]
        protected string _closeStateName = "Close";

        protected Action m_openCallback;
        protected Action m_closeCallback;

        private int _openHash;
        private int _closeHash;

        private bool _initialized;

		private void Awake() {
            Init();
		}


		private void Init() {
            if (_initialized) {
                return;
			}

            _openHash = Animator.StringToHash(_openStateName);
            _closeHash = Animator.StringToHash(_closeStateName);

            _initialized = true;
		}

        public async virtual void Open(Action openFinishCallback) {
            Init();

            Open();

            _animator.Play(_openHash);

            while(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {
                await Task.Yield();
			}

            m_openCallback = openFinishCallback;
        }

        public async virtual void Close(Action closeCallback) {
            m_closeCallback = closeCallback;



            Close();
		}
 
    }

}