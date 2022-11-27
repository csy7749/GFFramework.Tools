using UnityEditor;

namespace FuguFirecracker.TakeNote
{
    public static class Menu
    {
        [MenuItem("Tools/Fugu Firecracker/Take Note")]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<Window>("Take Note");
        }
    }
}