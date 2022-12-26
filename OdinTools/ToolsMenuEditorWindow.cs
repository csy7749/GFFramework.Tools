/*
 *FileName:      ToolsMenuEditorWindow.cs
 *Author:        95319
 *Date:          2022/12/26 22:44:17
 *UnityVersion:  2020.3.33f1c2
 *Description:
 */
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using Sirenix.Utilities;

public class ToolsMenuEditorWindow: OdinMenuEditorWindow
{
    [MenuItem("Tools/ToolsMenu")]
    private static void OpenWindow()
    {
        var window = GetWindow<ToolsMenuEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
    }

    protected override void OnGUI()
    {
        SirenixEditorGUI.Title("工具箱", "里面包含UI工具，数据查看，PlayerSetting", TextAlignment.Center, true);
        EditorGUILayout.Space();
        base.OnGUI();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                //{ "UI",UICreatEdiotr.Instance,EditorIcons.SpeechBubblesRound},
                //{ "Excel/ExcelCreat",ExcelCreatEditor.Instance,EditorIcons.Bell},
                //{ "Player Settings", Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault() },
            };
        tree.Config.DrawSearchToolbar = true;//开启搜索状态
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;//菜单设置成树形模式
        tree.AddAllAssetsAtPath("ConfigData", "Resources/Config", typeof(ScriptableObject), true)
           .AddThumbnailIcons();

        //tree.AddAssetAtPath("UI/UIConfig", "Three/IFramework/Test/Resources/UIConfig.asset");
        tree.SortMenuItemsByName();
        return tree;
    }
}
