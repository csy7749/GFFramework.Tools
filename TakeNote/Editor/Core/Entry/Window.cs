using UnityEditor;
using UnityEngine;

namespace FuguFirecracker.TakeNote
{
    internal partial class Window : EditorWindow
    {
        #region UIFields

        internal string TaskString;
        internal string DetailsString;
        internal bool DoAdd;
        internal bool DoDetails;
        internal bool DoColor;
        internal Color ColorizeColor = Color.white;
        internal Color TextColor = Color.black;
        internal Color TaskAlertColor = Color.white;
        internal Color ResetColor;
        internal ColoringView ColoringView;
        private Vector2 _scrollVector;

        #endregion
    }
}