using UnityEngine;
using System.Collections;
using System;

public class WaitAction : ZAction {

    private float _time;

	public WaitAction(float time)
    {
        _time = time;
    }    

    public override void Update(float dt)
    {
        _time -= dt;
        if (_time <= 0)
        {
            base._isComplete = true;
        }
    }
}

public class ExecuteAction : ZAction
{

    private float _delay;
    private Action _action;

    public ExecuteAction(Action action, float delay = 0.0f)
    {
        _delay = delay;
        _action = action;
    }

    public override void Update(float dt)
    {
        _delay -= dt;
        if (_delay <= 0)
        {
            base._isComplete = true;
            _action();
        }
    }
}
