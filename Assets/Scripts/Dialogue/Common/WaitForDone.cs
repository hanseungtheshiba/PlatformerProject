using System;
using UnityEngine;

public sealed class WaitForDone : CustomYieldInstruction
{
    private Func<bool> m_Predicate;
    private float m_timeout;
    private bool WaitForDoneProcess()
    {
        m_timeout -= Time.unscaledDeltaTime;
        return m_timeout <= 0f || m_Predicate();
    }
    public override bool keepWaiting => !WaitForDoneProcess();

    public WaitForDone(float timeout, Func<bool> predicate)
    {
        m_Predicate = predicate;
        m_timeout = timeout;
    }
}
