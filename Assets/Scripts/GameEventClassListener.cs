using FinishOne.GeneralUtilities;
using System;

public class GameEventClassListener : IEventListener
{
    private Action Event;

    public GameEventClassListener(Action action) => Event += action;
    public void RaiseEvent() => Event?.Invoke();
}

public class GameEventClassListener<TParameter> : IEventListener<TParameter>
{
    private Action<TParameter> Event;

    public GameEventClassListener(Action<TParameter> action) => Event += action;
    public void RaiseEvent(TParameter parameter) => Event?.Invoke(parameter);
}