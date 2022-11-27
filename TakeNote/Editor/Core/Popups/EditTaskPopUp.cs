using UnityEditor;
using UnityEngine;

namespace FuguFirecracker.TakeNote
{
    internal class EditTaskPopUp : PopupWindowContent
    {
        private const float MAX_HEIGHT = 162;
        private const float COLOR_HEIGHT = 80;
        private const float DETAILS_HEIGHT = 60;

        private float _colorHeight;
        private float _detailsHeight;

        private bool _doSaveEdit;

        private readonly Task _tempTask;
        private readonly Task _task;

        private readonly ColoringView _coloring;
        private readonly Color _cursorColorCache;
        private bool _showCursor;


        public EditTaskPopUp(Task task)
        {
            _task = task;
            _tempTask = TaskMaster.Clone(task);
            _coloring = new ColoringView(_tempTask.DrawColor, _tempTask.TextColor);
            _cursorColorCache = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            _doSaveEdit = true;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(Main.Window.position.width - 42, MAX_HEIGHT + _colorHeight + _detailsHeight);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Space(6);
            EditorGUILayout.BeginVertical(Style.PopUp);
            GUI.backgroundColor = Color.white;

            EditorGUILayout.LabelField("Edit Task", EditorStyles.boldLabel);

            GUI.SetNextControlName("TaskString");
            _tempTask.Title = EditorGUILayout.TextField(string.Empty, _tempTask.Title);

            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal("Button");
            _tempTask.HasDetails = GUILayout.Toggle(_tempTask.HasDetails, "Add Details", Style.AlignCenter,
                GUILayout.Height(24));
            _tempTask.HasDetails = GUILayout.Toggle(_tempTask.HasDetails, string.Empty, Style.OnOffSwitch);
            EditorGUILayout.EndHorizontal();
            if (_tempTask.HasDetails)
            {
                if (!_showCursor) GUI.skin.settings.cursorColor = Color.clear;
                
                _detailsHeight = DETAILS_HEIGHT;
                _tempTask.Details = EditorGUILayout.TextArea(_tempTask.Details, Style.WordWrap, GUILayout.Height(DETAILS_HEIGHT));
             
                if (Event.current.type == EventType.Used)
                {
                    GUI.skin.settings.cursorColor = _cursorColorCache;
                    _showCursor = true;
                }
            }
            else _detailsHeight = 0;


            EditorGUILayout.BeginHorizontal("Button");
            _tempTask.IsColored =
                GUILayout.Toggle(_tempTask.IsColored, "Colorize", Style.AlignCenter, GUILayout.Height(24));
            _tempTask.IsColored = GUILayout.Toggle(_tempTask.IsColored, string.Empty, Style.OnOffSwitch);
            EditorGUILayout.EndHorizontal();

            if (_tempTask.IsColored)
            {
                _colorHeight = COLOR_HEIGHT;
                _coloring.Draw();
                _tempTask.DrawColor = _coloring.BackgroundColor;
                _tempTask.TextColor = _coloring.ContentColor;
            }
            else _colorHeight = 0;

            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Apply", GUILayout.Height(22)))
            {
                _doSaveEdit = true;
                editorWindow.Close();
            }

            if (GUILayout.Button("Cancel", GUILayout.Height(22)))
            {
                _doSaveEdit = false;
                editorWindow.Close();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(6);
            EditorGUILayout.EndVertical();
        }

        public override void OnClose()
        {
            if (!Ledger.Manifest.MuteEditAlert && _tempTask.HasDetails &&
                Stringway.IsNullOrWhiteSpace(_tempTask.Details))
            {
                var decision = EditorUtility.DisplayDialogComplex(English.ALERT_TITLE, English.ALERT_CONTENT,
                    English.ALERT_AGREE, English.ALERT_MUTE, English.ALERT_DISAGREE);

                switch (decision)
                {
                    case 0:
                        _doSaveEdit = true;
                        _tempTask.HasDetails = false;
                        break;
                    case 1:
                        _doSaveEdit = true;
                        _tempTask.HasDetails = false;
                        Ledger.Manifest.MuteEditAlert = true;
                        break;
                    case 2:
                        _doSaveEdit = false;
                        break;
                }
            }

            if (!_doSaveEdit) return;
            TaskMaster.Assimilate(_tempTask, _task);
            Ledger.Manifest.Save();
        }
    }
}