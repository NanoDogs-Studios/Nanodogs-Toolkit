// © 2025 Nanodogs Studios. All rights reserved.

using UnityEditor;

namespace Nanodogs.Toolkit
{
    public class NDSEditorWindow : EditorWindow
    {
        public virtual void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Made with ❤️ by the Nanodogs Team.", MessageType.Info);
            EditorGUILayout.HelpBox("© 2025 Nanodogs Studios. All rights reserved.", MessageType.Info);
        }
    }
}