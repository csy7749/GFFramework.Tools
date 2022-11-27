using System;
using UnityEditor;
using UnityEngine;

namespace FuguFirecracker.TakeNote
{
    public class EditTaskUtilityWindow : EditorWindow
    {
        private float _colorHeight;
        private float _detailsHeight;

        private bool _doSaveEdit;

        private Task _tempTask;
        private Task _task;

        private ColoringView _coloring;
        private Color _cursorColorCache;
        private bool _showCursor;
     

        internal void Init(Task task)
        {
            _task = task;
            _tempTask = TaskMaster.Clone(task);
            _coloring = new ColoringView(_tempTask.DrawColor, _tempTask.TextColor);
            _cursorColorCache = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }

        private void OnGUI()
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

                _tempTask.Details = EditorGUILayout.TextArea(_tempTask.Details, Style.WordWrap, GUILayout.Height(180));

                if (Event.current.type == EventType.Used)
                {
                    GUI.skin.settings.cursorColor = _cursorColorCache;
                    _showCursor = true;
                }
            }


            EditorGUILayout.BeginHorizontal("Button");
            _tempTask.IsColored =
                GUILayout.Toggle(_tempTask.IsColored, "Colorize", Style.AlignCenter, GUILayout.Height(24));
            _tempTask.IsColored = GUILayout.Toggle(_tempTask.IsColored, string.Empty, Style.OnOffSwitch);
            EditorGUILayout.EndHorizontal();

            if (_tempTask.IsColored)
            {
                _coloring.Draw();
                _tempTask.DrawColor = _coloring.BackgroundColor;
                _tempTask.TextColor = _coloring.ContentColor;
            }

            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Apply", GUILayout.Height(22)))
            {
                _doSaveEdit = true;
                Close();
            }

            if (GUILayout.Button("Cancel", GUILayout.Height(22)))
            {
                _doSaveEdit = false;
                Close();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(6);
            EditorGUILayout.EndVertical();
        }

        private void OnDisable()
        {
            UtilityMinder.UtilityClosed(titleContent.text);
            
            if (!_doSaveEdit) return;
            TaskMaster.Assimilate(_tempTask, _task);
            Ledger.Manifest.Save();

        }
    }
}