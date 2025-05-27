using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


[CustomPropertyDrawer(typeof(Hidden.UIStatesBase), true)]
public class StatesCustomDrawer : PropertyDrawer
{
    private const string FIELD_entries = "entries";

    private const string FIELD_STATE_Key = "Key";
    private const string FIELD_STATE_ShowObjects = "ShowObjects";
    private const string FIELD_STATE_InAnimation = "InAnimation";
    private const string FIELD_STATE_OutAnimation = "OutAnimation";

    private const float ROW_HEIGHT = 16f;
    private const float SUB_FIELDS_OFFSET = 14f;

    private const float ENTRY_HEIGHT_COEF = 3.5f;
    private const float ENTRIES_OFFSET_COEF = 1.5f;
    private const float ENTRIES_END_COEF = 0.3f;

    private enum Status
    {
        NOT_CONFIGURATED,
        SUCCESS,
        WARNINGS
    }

    private Color[] statusColors = new[]{
        Color.white,
        Color.gray,
        new Color(1f, 1f, 0.5f)
    };


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var entriesProperty = property.FindPropertyRelative(FIELD_entries);
        entriesProperty.isExpanded = true; // HACK

        EditorGUI.PropertyField(position.SetHeight(ROW_HEIGHT), entriesProperty, label, false);
        var status = DrawStatus(position, property);


        //entriesProperty.isExpanded = EditorGUILayout.Foldout(entriesProperty.isExpanded, "xxx", true);

        if (entriesProperty.isExpanded)
        {
            position.y += ROW_HEIGHT * ENTRIES_OFFSET_COEF;
            entriesProperty.arraySize.Repeat(i => position = DrawEntry(entriesProperty.GetArrayElementAtIndex(i), position));

            if (status != Status.SUCCESS)
            {
                if (GUI.Button(position.SplitHorizontal(new[] { 1, 4f, 1 })[1].SetHeight(ROW_HEIGHT), "Set Configuration"))
                {
                    DoResetConfiguration(property);
                }
            }
        }


    }

    private Status DrawStatus(Rect position, SerializedProperty property)
    {
        string statusText;
        var status = GetStatus(property, out statusText);

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleRight;

        var saved = GUI.contentColor;
        GUI.contentColor = statusColors[(int)status];

        EditorGUI.LabelField(position.SetHeight(ROW_HEIGHT), statusText, style);

        GUI.contentColor = saved;

        return status;
    }

    public Rect DrawEntry(SerializedProperty property, Rect position)
    {

        var objectsProp = property.FindPropertyRelative(FIELD_STATE_ShowObjects);
        //var animInProp = property.FindPropertyRelative(FIELD_STATE_InAnimation);
        //var animOutProp = property.FindPropertyRelative(FIELD_STATE_OutAnimation);

        var height = EntryHeight(property);// - ROW_HEIGHT; // * 2.8f;
        position = position.SetHeight(ROW_HEIGHT);

        EditorGUI.indentLevel++;

        // Header + Box
        {
            EditorGUI.LabelField(position, EntryValueString(property), EditorStyles.miniButtonMid);
            position.y += ROW_HEIGHT;

            EditorGUI.LabelField(position.SetHeight(height - ROW_HEIGHT), "", EditorStyles.helpBox);
        }

        // Inner fields
        EditorGUI.indentLevel++;
        {
            position.y += ROW_HEIGHT * 0.5f;

            // Animation fields
            /*
            {
                EditorGUI.LabelField(position, "Animations:");
                position.y += ROW_HEIGHT;

                
                var animDatas = new[] { new {name = "in", property = animInProp },
                                    new {name = "out", property = animOutProp }};
                
                EditorGUI.indentLevel++;
                {
                    
                    animDatas.ForEach(anim =>
                    {
                        EditorGUI.PropertyField(position, anim.property, new GUIContent(anim.name));
                        position.y += ROW_HEIGHT;
                    });
                    

                    position.y += ROW_HEIGHT * 0.3f;

                }
                EditorGUI.indentLevel--;
            }
            */

            // Objects field
            {
                int size = EditorGUI.DelayedIntField(position.RightOffset(80f), new GUIContent("Objects:"), objectsProp.arraySize, EditorStyles.toolbarTextField);

                if (size != objectsProp.arraySize)
                {
                    objectsProp.arraySize = size;
                    objectsProp.serializedObject.ApplyModifiedProperties();
                }

                position.y += ROW_HEIGHT;

                EditorGUI.indentLevel++;
                objectsProp.arraySize.Repeat(i =>
                    {
                        EditorGUI.PropertyField(position, objectsProp.GetArrayElementAtIndex(i), GUIContent.none);
                        position.y += ROW_HEIGHT;
                    });
                EditorGUI.indentLevel--;

                position.y += ROW_HEIGHT;
            }

        }
        EditorGUI.indentLevel--;

        EditorGUI.indentLevel--;


        return position;
    }

    private string EntryValueString(SerializedProperty property)
    {
        var prop = property.FindPropertyRelative(FIELD_STATE_Key);

        if (prop.propertyType == SerializedPropertyType.Boolean)
        {
            return prop.boolValue.ToString();
        }

        if (prop.propertyType == SerializedPropertyType.Enum)
        {
            return prop.enumDisplayNames[prop.enumValueIndex];
        }

        return prop.displayName;

    }

    private float EntryHeight(SerializedProperty property)
    {
        var objectsProp = property.FindPropertyRelative(FIELD_STATE_ShowObjects);

        return (ENTRY_HEIGHT_COEF + objectsProp.arraySize) * ROW_HEIGHT;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = ROW_HEIGHT;
        var entriesProperty = property.FindPropertyRelative(FIELD_entries);

        if (entriesProperty.isExpanded)
        {
            entriesProperty.arraySize.Repeat(i => height += EntryHeight(entriesProperty.GetArrayElementAtIndex(i)));

            height += ROW_HEIGHT * (ENTRIES_OFFSET_COEF + ENTRIES_END_COEF);
        }

        return height;

    }

    private Status GetStatus(SerializedProperty property, out string text)
    {
        var entriesProperty = property.FindPropertyRelative(FIELD_entries);

        if (entriesProperty.arraySize == 0)
        {
            text = "no configurated";

            return Status.NOT_CONFIGURATED;
        }

        var entryKeyProp = entriesProperty.GetArrayElementAtIndex(0).FindPropertyRelative(FIELD_STATE_Key);
        var keysCount = 0;

        if (entryKeyProp.propertyType == SerializedPropertyType.Boolean) keysCount = 2;
        if (entryKeyProp.propertyType == SerializedPropertyType.Enum) keysCount = entryKeyProp.enumNames.Length;

        if (entriesProperty.arraySize == keysCount)
        {
            List<int> valuesIndexes = new List<int>();

            entriesProperty.arraySize.Repeat(i =>
            {
                var keyProp = entriesProperty.GetArrayElementAtIndex(i).FindPropertyRelative(FIELD_STATE_Key);

                if (entryKeyProp.propertyType == SerializedPropertyType.Boolean)
                    valuesIndexes.Add(keyProp.boolValue ? 1 : 0);

                if (entryKeyProp.propertyType == SerializedPropertyType.Enum)
                    valuesIndexes.Add(keyProp.enumValueIndex);
            });

            if (valuesIndexes.Union(new int[] { }).Count() == keysCount)
            {
                text = "success config";

                return Status.SUCCESS;
            }

        }

        text = $"setup warnings ({entriesProperty.arraySize}/{keysCount} keys)";

        return Status.WARNINGS;

    }

    private void DoResetConfiguration(SerializedProperty property)
    {
        var entriesProperty = property.FindPropertyRelative(FIELD_entries);

        entriesProperty.arraySize = Mathf.Max(1, entriesProperty.arraySize);
        var entryKeyProp = entriesProperty.GetArrayElementAtIndex(0).FindPropertyRelative(FIELD_STATE_Key);
        var keysCount = 0;

        if (entryKeyProp.propertyType == SerializedPropertyType.Boolean) keysCount = 2;
        if (entryKeyProp.propertyType == SerializedPropertyType.Enum) keysCount = entryKeyProp.enumNames.Length;

        if (keysCount == 0)
        {
            Debug.Log("Configurated fail. Type of element not supported");
            return;
        }

        entriesProperty.arraySize = keysCount;


        entriesProperty.arraySize.Repeat(i =>
        {
            var keyProp = entriesProperty.GetArrayElementAtIndex(i).FindPropertyRelative(FIELD_STATE_Key);

            if (entryKeyProp.propertyType == SerializedPropertyType.Boolean)
                keyProp.boolValue = i > 0;

            if (entryKeyProp.propertyType == SerializedPropertyType.Enum)
                keyProp.enumValueIndex = i;
        });


        property.serializedObject.ApplyModifiedProperties();
        Debug.Log("Configuration finished success");

    }

    // TODO: lern this field and add cache
    public override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        return false;
    }


}
