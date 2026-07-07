using System;
using tora.fsm;

namespace tora.ui
{
	/// <summary>
	/// Generic state that opens a view on enter and closes it on exit, and notifies
	/// via callback when the view closes itself (e.g. its own close button) rather
	/// than being closed by a state transition. Lets a manager push/pop a view onto
	/// a UIStateManager without a hand-written IUIState per screen.
	/// </summary>
	public class ViewState : State, IUIState
	{
		private readonly IUIView _view;
		private readonly Action _onClosedByUser;

		public ViewState(IUIView view, Action onClosedByUser)
		{
			_view = view;
			_onClosedByUser = onClosedByUser;
		}

		public override void OnEnter(IState prevState)
		{
			_view.Closed += HandleViewClosedByUser;
			_view.Open();
		}

		public override void OnExit(IState nextState)
		{
			// Unsubscribe before the defensive Close() so a state-driven exit can't
			// loop back through HandleViewClosedByUser and trigger a second pop.
			_view.Closed -= HandleViewClosedByUser;

			if (_view.IsOpen)
				_view.Close();
		}

		private void HandleViewClosedByUser()
		{
			_view.Closed -= HandleViewClosedByUser;
			_onClosedByUser?.Invoke();
		}
	}
}
