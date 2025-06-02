using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThreat 
{
    void StartThreat(CoreGameplayManagerBase coreManager);

    void EarlyThreatDisable(CoreGameplayManagerBase coreManager);

    event Action<IThreat> OnThreatEnded;
}
