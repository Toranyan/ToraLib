using UnityEngine;
using System.Collections;

namespace tora.fsm {

	public interface IState {
		void OnEnter(IState prevState);
		void OnExit(IState nextState);
		void Run();
	}

}
