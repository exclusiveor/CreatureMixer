using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZAction
{
    protected bool _isComplete;
    private List<ZAction> _aAction;

    public ZAction()
    {
        _aAction = new List<ZAction>();
        _isComplete = false;
    }

    public virtual void Start()
    {

    }

    public virtual void End()
    {

    }

    public virtual void Break()
    {

    }

    public virtual void Suspend(bool isSuspended)
    {

    }

    public virtual void Update(float dt)
    {

    }

    public bool IsComplete()
    {
        return _isComplete;
    }

	public void	SetComplete()
    {
        _isComplete = true;
    }

    public ZAction AddAction(ZAction ta)
    {
        _aAction.Add(ta);
        return ta;
    }

    public List<ZAction> GetAAction()
    {
        return _aAction;
    }
}

public class ZActionWorker
{
    public ZAction _lastAction;
    private List<ZAction> _actionsList;

	public ZActionWorker()
    {
        _actionsList = new List<ZAction>();
        _lastAction = null;
    }

    ~ZActionWorker()
    {
        ResetActions();
    }

    public ZAction AddAction(ZAction ta)
    {
        ZAction last = GetLastAction();
        if (last != null)
        {
            last.AddAction(ta);
        }
        else
        {
            AddParalelAction(ta);
        }
        _lastAction = ta;
        return ta;
    }

    public ZAction AddParalelAction(ZAction ta)
    {
        ta.Start();
        _actionsList.Add(ta);
        return ta;
    }

    public ZAction GetLastAction()
    {
        if (_lastAction != null && _lastAction.IsComplete())
        {
            _lastAction = null;
        }
        return _lastAction;
    }

    public void ResetActions()
    {
        foreach (var action in _actionsList)
        {
            action.End();
        }
        _actionsList.Clear();
        _lastAction = null;
    }
    public void Suspend(bool isSuspended)
    {
        foreach (var action in _actionsList)
        {
            action.Suspend(isSuspended);
        }
    }
    public List<ZAction> GetAAction()
    {
        return _actionsList;
    }

    public void UpdateActions(float dt)
    {        
        int index = 0;
        while (index < _actionsList.Count)
        {            
            ZAction action = _actionsList[index];
		    if (action.IsComplete()) {
			    action.End();
                foreach (var actionChild in action.GetAAction())
                {
                    AddParalelAction(actionChild);
                }
                _actionsList.Remove(action);
		    } else {
                index++;
			    action.Update(dt);
		    }
        }        
    }
}
