using UnityEditor;
using UnityEngine;

namespace FuguFirecracker.TakeNote
{
    internal partial class Window
    {
      
        internal void OnEnable()
        {
            Main.Window = this;
            Ledger.Manifest = SoBuilder.FindOrCreateInRelativePath<Ledger>("TakeNote/Editor/Persistence/Ledger.asset");

            SetEnumeratedLabel(TaskCollection.Outstanding);
            SetEnumeratedLabel(TaskCollection.Completed);
            SetEnumeratedLabel(TaskCollection.Deferred);

            ResetColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            ColoringView = new ColoringView(ColorizeColor, TextColor);

            if (Ledger.Manifest.Version == 2) return;
            Upgrade.FixColors();
            Ledger.Manifest.Version = 2;
            Ledger.Manifest.Save();
        }
    }
}

