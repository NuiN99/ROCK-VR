using System;
using Animancer;

public static class AnimancerExtensions
{
    public static AnimancerState Force(this AnimancerState state)
    {
        state.Time = 0;
        return state;
    }
    public static AnimancerState SetSpeed(this AnimancerState state, float speed)
    {
        state.Speed = speed;
        return state;
    }
    public static AnimancerState OnComplete(this AnimancerState state, Action onEnd)
    {
        state.Events.OnEnd = onEnd;
        return state;
    }
    public static AnimancerState OnProgress(this AnimancerState state, float targetProgress, Action onProgres)
    {
        state.Events.Add(targetProgress, onProgres);
        return state;
    }
}
