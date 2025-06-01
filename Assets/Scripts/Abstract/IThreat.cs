using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThreat 
{
    void StartThreat(CoreGameplayManagerBase coreManager);
    event Action<IThreat> OnEnded;
}
