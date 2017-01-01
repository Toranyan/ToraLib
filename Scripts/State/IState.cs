using UnityEngine;
using System.Collections;

public interface IState {

	void OnEnter (IState prevState);
	void OnExit (IState nextState);
	void Update ();

}
