using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FuguFirecracker.TakeNote
{
    public class UtilityMinder : ScriptableSingleton<UtilityMinder>
    {
        private static event Action<string> UtilityClosedEvent;

        private readonly HashSet<EditTaskUtilityWindow> _utilityWindowSet = new HashSet<EditTaskUtilityWindow>();
        private Rect _lastRect;

        private void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            UtilityClosedEvent += OnUtilityClosed;
        }

        private void OnDisable()
        {
            
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            UtilityClosedEvent -= OnUtilityClosed;
        }

        private void OnBeforeAssemblyReload()
        {
            var nixList = new List<EditTaskUtilityWindow>(_utilityWindowSet);
            foreach (var utilityWindow in nixList) utilityWindow.Close();
        }

        internal void LaunchAsUtility(Task task)
        {
            if (FocusIfOpen(task.Title)) return;
            var doNextPosition = DoNextPosition();

            var utilityWindow = CreateInstance<EditTaskUtilityWindow>();
            utilityWindow.titleContent = new GUIContent(task.Title);
            utilityWindow.Init(task);

            _utilityWindowSet.Add(utilityWindow);
            utilityWindow.ShowUtility();

            if (doNextPosition) utilityWindow.position = _lastRect;
        }

        private bool FocusIfOpen(string taskTitle)
        {
            var utilityWindow = _utilityWindowSet
                .FirstOrDefault(x => x.titleContent.text == taskTitle);

            if (!utilityWindow) return false;
            utilityWindow.Focus();
            return true;
        }

        private bool DoNextPosition()
        {
            if (!EditorWindow.HasOpenInstances<EditTaskUtilityWindow>()) return false;
            var r = EditorWindow.GetWindow<EditTaskUtilityWindow>().position;
            _lastRect = new Rect(r.x + r.width / 2, r.y + 42, r.width, r.height);
            return true;
        }

        public static void UtilityClosed(string name)
        {
            UtilityClosedEvent?.Invoke(name);
        }

        private void OnUtilityClosed(string s)
        {
            _utilityWindowSet.Remove(_utilityWindowSet.First(x => x.titleContent.text == s));
        }
    }
}