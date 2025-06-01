using System;


public class BlockerScreen : UIScreen<BlockerIntent>, IDisposable
{
    public enum TypeState { Basic, Full }
    [Serializable] public class TypeStateStates : UIStates<TypeState> { }

    public TypeStateStates stateStates;
    public TMPro.TextMeshProUGUI titleText;

    public override void Prepare()
    {
        stateStates.SetState(TypeState.Basic);
    }

    public override void Refresh()
    {
        // Model can be NULL ! It is okay

        stateStates.SetState(Model?.State ?? TypeState.Basic);

        SetTitle(Model?.Title ?? "Loading...");

    }

    public void SetTitle(string title)
    {
        if (titleText != null)
            titleText.text = title;
    }

    public void Dispose()
    {
        Close();
    }
}

public class BlockerIntent
{
    public string Title;
    public BlockerScreen.TypeState State = BlockerScreen.TypeState.Basic;
}
