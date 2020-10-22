using UnityEngine;
using System.Collections;

namespace tora.fsm {

	public abstract class State : IState {

		public virtual void OnEnter(IState prevState) {

		}

		public virtual void OnExit(IState nextState) {

		}

		public virtual void Run() {

		}

	}

}