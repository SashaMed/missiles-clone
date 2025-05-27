using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UIScreensLibrary", menuName = "ScriptableObject/UI/ScreensLibrary")]
public class UIScreensLibrary : ScriptableObject
{
    public List<GameObject> screens;

    public void RegisterInNavigation()
    {
        var navigation = SimpleNavigation.Instance ?? (FindAnyObjectByType<SimpleNavigation>());
        navigation.RegisterScreens(screens);
    }
}
