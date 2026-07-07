using System.Collections;
using System.Collections.Generic;
using tora.fsm;
using UnityEngine;

namespace tora.ui
{

    /// <summary>
    /// Back-navigable UI state stack. Wraps a tora.fsm.StateMachine for OnEnter/OnExit
    /// dispatch and adds history so Pop() can return to whatever state was active
    /// before the last Push() — a plain StateMachine.SetState() has no memory of that.
    /// </summary>
    public class UIStateManager : MonoBehaviour
    {
        private readonly StateMachine _fsm = new StateMachine();
        private readonly Stack<IUIState> _history = new();

        public IUIState CurrentState => _fsm.CurrentState as IUIState;

        /// <summary>Transitions to a new state, remembering the current one so Pop() can return to it.</summary>
        public void Push(IUIState state)
		{
            if (CurrentState != null)
                _history.Push(CurrentState);

            state.Init(_fsm);
            _fsm.SetState(state);
		}

        /// <summary>Returns to the state that was active before the last Push(). No-op if there is nothing to return to.</summary>
        public void Pop()
		{
            if (_history.Count == 0)
            {
                Debug.LogWarning("[UIStateManager] Pop() called with no previous state to return to.");
                return;
            }

            _fsm.SetState(_history.Pop());
		}

        private void Update()
		{
            _fsm.Update();
		}
    }
}