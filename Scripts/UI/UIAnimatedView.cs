using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tora.ui {

    public class UIAnimatedView : UIViewBase {

        [SerializeField]
        protected Animator _animator;

        [SerializeField]
        protected string _openStateName = "Open";

        [SerializeField]
        protected string _closeStateName = "Close";

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

        public override async void Open() {
            Init();

            OnPreOpen();
            Show();
            IsOpen = true;

            _animator.Play(_openHash);
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {
                await UniTask.Yield();
			}

            OnPostOpen();
            RaiseOpened();
        }

        public override async void Close() {
            Init();

            OnPreClose();

            _animator.Play(_closeHash);
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                await UniTask.Yield();
            }

            Hide();
            IsOpen = false;
            OnPostClose();
            RaiseClosed();
        }

    }

}