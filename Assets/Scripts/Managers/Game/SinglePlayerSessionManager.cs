using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerSessionManager : SessionManagerBase
{
    public override void EndSession()
    {
        throw new System.NotImplementedException();
    }

    public override void EndSessionToMenu()
    {
        throw new System.NotImplementedException();
    }

    public override void RestartSession()
    {
        Refresh();
    }

    public override void StartSession()
    {
        throw new System.NotImplementedException();
    }


}
