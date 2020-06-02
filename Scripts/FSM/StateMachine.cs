using UnityEngine;
using System.Collections;

namespace tora.fsm {

	public class StateMachine {

		//Properties
		public IState CurrentState {
			get { return m_currentState; }
			protected set { m_currentState = value; }
		}

		public IState PrevState {
			get { return m_prevState; }
			protected set { m_prevState = value; }
		}

		protected IState m_currentState;
		protected IState m_prevState;

		public void Run() {
			if (CurrentState != null) {
				CurrentState.Run();
			}
		}

		public void SetState(IState state) {
			//exit state
			if (CurrentState != null) {
				CurrentState.OnExit(state);
			}

			//change
			PrevState = CurrentState;
			CurrentState = state;

			//enter new state
			if (CurrentState != null) {
				CurrentState.OnEnter(PrevState);
			}
		}

	}

}
