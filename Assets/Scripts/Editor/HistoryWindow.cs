using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HistoryWindow : EditorWindow
{
    List<Object> history = new List<Object>();

    int maxItems = 30;
    bool showTypes = false;

    [MenuItem("Window/History")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(HistoryWindow));
    }

    void Awake()
    {
        titleContent = new GUIContent("History", "History of recent selections");
    }

    void OnGUI()
    {
        showTypes = GUILayout.Toggle(showTypes, "Show Object Types");

        // remove any entries from the list that have been deleted
        history.RemoveAll(obj => { return obj == null; });

        foreach (Object obj in history)
        {
            // get a trimmed typename
            string typeName = "";
            if (showTypes)
            {
                typeName = obj.GetType().ToString();
                typeName = typeName.Replace("UnityEngine.", "");
                typeName = typeName.Replace("UnityEditor.", "");
                typeName = "(" + typeName + ")";
            }

            // button for selecting top level character
            if (GUILayout.Button(obj.name + typeName))
                Selection.activeObject = obj; 
        }
    }

    void OnSelectionChange()
    {
        // when we select something new, add it to our list if it isn't already in there
        Object obj = UnityEditor.Selection.activeObject;
        if (obj && obj.GetType() != typeof(DefaultAsset))
        {
            if (history.Contains(obj))
            {
                history.Remove(obj);
            }

            // keep the list to max number of items items
            if (history.Count >= maxItems)
                history.RemoveAt(0);

            history.Add(obj);

            // redraw the buttons
            Repaint();
        }
    }
}

