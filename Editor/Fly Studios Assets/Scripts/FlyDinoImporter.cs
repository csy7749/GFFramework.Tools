using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Fly Dino Importer V1.0 : (2022)
// Developed by : Fly Studios Assets (Panfilii Victor)
// Contact or Suport : flystudiosassets@gmail.com

namespace FlyDinoImporter
{
    // EditorPrefs is used to save data
    // for (searchOptionMod (Bool)) the hash key is - "SearchOptionMod_key"
    // for (curentDirectoryPatch (String)) the hash key is - "CurentDirectoryPatch_key"
    // This keys will be used to reset the value or save a new value!

    // An editor extension that allows you to list and import unitypackages held in your locale
    public class FlyDinoImporter : EditorWindow 
    {
        private bool _settingsStatus; // Settings curent status
        private bool _boxAlert_Text; // Open message (No Pakages Fond!)
        private bool _multiplyInstall; // Open multypli import button
        public static bool _searchOptionMod; // Search Option Mod curent status

        public string curentFolderPatch; // Displays the current path in the box
        public static string curentDirectoryPatch; // Curent unitypackage storage directory path
        private string showSettingsStatus; // Displays the current Settings Status
        private string searchPackage = ""; // Search package by name
        private string settingsDefaultDirectory = ""; // Shows the Default Directory patch in settings
        private const string menuName = "Tools/Fly Package Importer"; // Menu name

        private List<string> packagePathList; // Full path list of unitypackage files
        private List<FlyDinoUnityPackageInfo> displayList; // Unitypackage list to display
        private List<FlyDinoUnityPackageInfo> PackageInfoList; // List of retained unitypackage information
        public List<string> ceckedList = new List<string>(); // List of currently pressed boxes

        private Texture copy_icon; // Copy icon_sprite
        private Texture logo_icon; // Logo icon_sprite
        private Texture folder_icon; // Foldern icon_sprite
        private Texture settings_icon; // Settings icon_sprite
        private Texture checkBox_on_icon; // CheckBox On_sprite
        private Texture checkBox_off_icon;  // CheckBox Off_sprite
        private Texture magnifyingGlass_icon; // Magnifying Glass icon_sprite

        private GUIStyle button_Style; // Button style
        private GUIStyle checkBox_Style; // CheckBox toggle style

        public int presedCount; // The number of boxes currently pressed
        private int allPackageCount; // Mumber of unitypackages stored locally
        private float rootHeight; // Height of the scroll area

        private Vector2 scrollPos; // Scroll position

        void OnLostFocus()
        {
            EditorPrefs.SetString("CurentDirectoryPatch_key", curentDirectoryPatch);
            EditorPrefs.SetBool("SearchOptionMod_key", _searchOptionMod);
        }

        void OnDestroy()
        {
            EditorPrefs.SetString("CurentDirectoryPatch_key", curentDirectoryPatch);
            EditorPrefs.SetBool("SearchOptionMod_key", _searchOptionMod);
            presedCount = 0;
        }

        //Show window
        [MenuItem(menuName)]
        public static void ShowWindow()
        {
            //Exit if not executable
            if (!IsExecutable())
            {
                return;
            }

            var window = EditorWindow.GetWindow(typeof(FlyDinoImporter)); //If WindowSample already exists, get its instance, else create it
            window.titleContent = new GUIContent("Fly Dino Importer"); //Set window title
            window.minSize = new Vector2(380f, 600f); // We set the minimum size of the window
        }

        // Returns whether the menu is executable
        // Enable menu if executable, Disable menu if not executable
        [MenuItem(menuName, true)]
        private static bool IsExecutable()
        {
            return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
        }

        // Do initialization
        private void OnEnable()
        {
            if (EditorPrefs.HasKey("SearchOptionMod_key")) // We check if the HasKey exists
            {
                _searchOptionMod = EditorPrefs.GetBool("SearchOptionMod_key");
            }

            if (EditorPrefs.HasKey("CurentDirectoryPatch_key")) // We check if the HasKey exists
            {
                curentDirectoryPatch = EditorPrefs.GetString("CurentDirectoryPatch_key");

                if (Directory.Exists(curentDirectoryPatch))
                {
                    curentDirectoryPatch = EditorPrefs.GetString("CurentDirectoryPatch_key");
                }
                else
                {
                    curentDirectoryPatch = FlyDinoFileAccessor.defaultDirectoryPatch;
                    curentFolderPatch = "Default Folder";
                }

                CeckCurentFolderPatch();
            }
            else
            {
                curentDirectoryPatch = FlyDinoFileAccessor.defaultDirectoryPatch;
                curentFolderPatch = "Default Folder";
            }


            displayList = new List<FlyDinoUnityPackageInfo>();
            curentDirectoryPatch = FlyDinoFileAccessor.GetLocalPackagePath();
            var path = "Assets/GFFramework.Tools/Editor/Fly Studios Assets/Icons/";
            // Get sprite from selected folder
            checkBox_on_icon = (Texture)AssetDatabase.LoadAssetAtPath(path+"checkbox_on_icon.png", typeof(Texture2D));
            checkBox_off_icon = (Texture)AssetDatabase.LoadAssetAtPath(path+"checkbox_off_icon.png", typeof(Texture2D));
            folder_icon = (Texture)AssetDatabase.LoadAssetAtPath(path + "folder_icon.png", typeof(Texture2D));
            magnifyingGlass_icon = (Texture)AssetDatabase.LoadAssetAtPath(path + "magnifying_glass_icon.png", typeof(Texture2D));
            settings_icon = (Texture)AssetDatabase.LoadAssetAtPath(path + "settings_icon.png", typeof(Texture2D));
            copy_icon = (Texture)AssetDatabase.LoadAssetAtPath(path + "copy_icon.png", typeof(Texture2D));
            logo_icon = (Texture)AssetDatabase.LoadAssetAtPath(path + "flydinoimporter_logo.png", typeof(Texture2D));
            // Get sprite from selected folder


            //Get a list of unitypackage files
            packagePathList = FlyDinoFileAccessor.GetPackageList(curentDirectoryPatch);
            if (packagePathList == null)
            {
                //Exit if invalid directory
                DestroyImmediate(this);
            }

            //Total number of unitypackages you have locally
            allPackageCount = packagePathList.Count;

            // If there are no packages in the list, we will display the message "No Pakages Fond!" otherwise, we will hide the message
            if (allPackageCount >= 1)
            {
                closeBoxAlert_Text(); // Close message (No Pakages Fond!)
            }
            else
            {
                openBoxAlert_Text(); // Open message (No Pakages Fond!)
            }

            //Load the unitypackage information held in the infoPath folder in advance
            PackageInfoList = new List<FlyDinoUnityPackageInfo>();
            FlyDinoFileAccessor.PackageInfo(ref PackageInfoList, curentDirectoryPatch);

            SetDisplayPackageInfo();
            AssetDatabase.Refresh();
        }

        private void openMultiplyInstall_Button() // Open multypli import button
        {
            _multiplyInstall = true;
        }

        private void closeMultiplyInstall_Buttonx() // Close multypli import button
        {
            _multiplyInstall = false;
        }

        private void openBoxAlert_Text() // Open message (No Pakages Fond!)
        {
            _boxAlert_Text = true;
        }

        private void closeBoxAlert_Text() // Close message (No Pakages Fond!)
        {
            _boxAlert_Text = false;
        }

        private void OnGUI()
        {
            //Total number of unitypackages you have locally
            allPackageCount = packagePathList.Count;

            if (allPackageCount >= 1) // If the number of packets is less than 1, then we will display message (No Pakages Fond!)
            {
                closeBoxAlert_Text();
            }
            else
            {
                openBoxAlert_Text();
            }

            if (presedCount > 1) // If the number of checked checkboxes is greater than 1, then we will display multypli import button
            {
                openMultiplyInstall_Button();
            }
            else
            {
                closeMultiplyInstall_Buttonx();
            }

            GUILayout.BeginHorizontal("box");
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(logo_icon); // The tool logo is displayed
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            // Styling the checkBox
            if (checkBox_Style == null)
            {
                checkBox_Style = new GUIStyle(GUI.skin.button);
                checkBox_Style.margin = new RectOffset(6, 0, 0, 0);
                checkBox_Style.padding = new RectOffset(0, 0, 0, 0);
            }

            // EndChangeCheck returns true when any changes are made to the GUI
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginHorizontal("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(folder_icon, checkBox_Style, GUILayout.Width(20f), GUILayout.Height(20f)))
                        {
                            if (Application.platform == RuntimePlatform.OSXEditor)
                            {
                                System.Diagnostics.Process.Start(curentDirectoryPatch); // We open the current patch in the file browser for (OSXEditor)
                            }
                            else if (Application.platform == RuntimePlatform.WindowsEditor)
                            {
                                EditorUtility.RevealInFinder(curentDirectoryPatch); // We open the current patch in the file browser for (WindowsEditor)
                            }
                            else
                            {
                                Debug.LogWarning("This operating system is not supported.");
                            }
                        }

                        EditorGUILayout.SelectableLabel(curentFolderPatch, EditorStyles.textField, GUILayout.Width(150f), GUILayout.Height(20f));

                        if (GUILayout.Button(copy_icon, checkBox_Style, GUILayout.Width(20f), GUILayout.Height(20f)))
                        {
                            Debug.Log("Patch Copyed To Clipboard: " + curentDirectoryPatch);
                            GUIUtility.systemCopyBuffer = curentDirectoryPatch; // Copy to clipboard curentDirectoryPatch
                        }
                        GUILayout.Label("Copy Patch ", EditorStyles.boldLabel);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button("Folder", GUILayout.Width(80f)))
                    {
                        string selectedDirectory = EditorUtility.OpenFolderPanel("Select Directory", "", ""); // Open the file browser
                        SelectFolder(selectedDirectory); // We take the path of the chosen folder
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box");
                {
                    if (GUILayout.Button(magnifyingGlass_icon, checkBox_Style, GUILayout.Width(20f), GUILayout.Height(20f)))
                    {
                        Debug.Log("Enter in the box a symbol or the name of the package you want to find in the list.");
                    }

                    // Search by name
                    searchPackage = GUILayout.TextField(searchPackage, GUILayout.Width(150f));

                    if (GUILayout.Button(checkBox_off_icon, checkBox_Style, GUILayout.Width(20f), GUILayout.Height(20f)))
                    {
                        openMultiplyInstall_Button();
                        presedCount = 0;
                    }

                    GUILayout.Label("Uncheck All", EditorStyles.boldLabel);

                    if (GUILayout.Button("Update", GUILayout.Width(80f)))
                    {
                        UpdateMetadata();
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.BeginVertical();
                {
                    if (_multiplyInstall)
                    {
                        if (GUILayout.Button("Multiple Install"))
                        {
                            for (int i = 0; i < ceckedList.Count; ++i)
                            {
                                AssetDatabase.ImportPackage(ceckedList[i], false); // Imports all selected packages
                            }

                            presedCount = 0; // Reset the presedCount, and close the multi import button
                            ceckedList.Clear(); // Clear List
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (searchPackage != "")
                {
                    packagePathList = SearchPackageByName(searchPackage); // Search
                }
                else
                {
                    packagePathList = FlyDinoFileAccessor.GetPackageList(curentDirectoryPatch); // Show all packages if blank
                }

                SetDisplayPackageInfo();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.Space();

            var scrollArea = EditorGUILayout.BeginHorizontal();
            {
                // Scroll view
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUI.skin.box);
                {
                    if (_boxAlert_Text)
                    {
                        GUILayout.BeginHorizontal("box");
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("No Pakages Found!"); // If there are no packages in the list, this message will be displayed in the window
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        closeBoxAlert_Text();
                    }

                    int lineHeight = 54; // Height of one item
                    var startIndex = (int)(scrollPos.y / lineHeight); // Fill the areas where you don't want top drawing with spaces
                    GUILayout.Space(startIndex * lineHeight);

                    var listCount = packagePathList.Count;
                    var endIndex = listCount;
                    if (rootHeight > 0f)
                    {
                        // Draw this count extra to prevent whitespace from appearing at the bottom
                        int marginCount = 5;
                        endIndex = startIndex + (int)(rootHeight / lineHeight) + marginCount;
                        if (endIndex > listCount)
                        {
                            endIndex = listCount;
                        }
                    }

                    for (int i = startIndex; i < endIndex; ++i)
                    {
                        string path = packagePathList[i];
                        string fileNameNoExt = Path.GetFileNameWithoutExtension(path);

                        EditorGUILayout.BeginHorizontal("box");
                        {
                            EditorGUILayout.BeginVertical(GUILayout.Width(80f));
                            {
                                if (GUILayout.Button("Import"))
                                {
                                    CeckIfPakageExist(path); // We check if it exists and install the selected package
                                }

                                // Styling the button
                                if (button_Style == null)
                                {
                                    button_Style = new GUIStyle(GUI.skin.label);
                                    button_Style.margin = new RectOffset(32, 0, 0, 0);
                                }

                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        GUILayout.FlexibleSpace();
                                        if (GUILayout.Button(displayList[i].ceckBox ? checkBox_on_icon : checkBox_off_icon, button_Style))
                                        {
                                            List<string> allList = FlyDinoFileAccessor.GetPackageList(curentDirectoryPatch);

                                            if (allList.Contains(path))
                                            {
                                                PressedCeckBox(i);

                                                if (displayList[i].ceckBox != true)
                                                {
                                                    presedCount--;
                                                }
                                                else
                                                {
                                                    presedCount++;
                                                }
                                            }
                                            else
                                            {
                                                    Debug.LogError("Package not found! " + path + ":" +
                                                        " It's probably been deleted from the folder, or it's not valid."); // If the file path is not found, we will display ERROR + not found location
                                            }
                                        }
                                        GUILayout.FlexibleSpace();
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical();
                            {
                                GUILayout.Label(fileNameNoExt);
                                GUILayout.Label("Size: " + displayList[i].size);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    // Fill the area where drawing at the bottom is unnecessary with spaces
                    GUILayout.Space((listCount - endIndex) * lineHeight);
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();

            // Update when the drawing of the scroll area is completed
            if (scrollArea.height > 0f)
            {
                rootHeight = scrollArea.height;
            }

            GUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginHorizontal("box");
                {
                    if (GUILayout.Button(settings_icon, checkBox_Style, GUILayout.Width(20f), GUILayout.Height(20f)))
                    {
                        if (_settingsStatus != true)
                        {
                            _settingsStatus = true;
                        }
                        else
                        {
                            _settingsStatus = false;
                        }
                    }

                    GUILayout.Label(showSettingsStatus, EditorStyles.boldLabel);
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (_searchOptionMod)
                        {
                            GUILayout.Label("SO: " + "AllDirectories", EditorStyles.boldLabel);
                        }
                        else
                        {
                            GUILayout.Label("SO: " + "TopDirectoryOnly", EditorStyles.boldLabel);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

            if (_settingsStatus)
            {
                showSettingsStatus = "Close Settings";

                GUILayout.Label("Search Option");
                EditorGUILayout.BeginHorizontal("box");
                {
                    _searchOptionMod = GUILayout.Toggle(_searchOptionMod, _searchOptionMod ? checkBox_on_icon : checkBox_off_icon, checkBox_Style, GUILayout.Width(20f), GUILayout.Height(20f));
                    {
                        EditorPrefs.SetBool("SearchOptionMod_key", _searchOptionMod);
                    }
                    GUILayout.Label("AllDirectories - true / TopDirectoryOnly - false");
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Default Folder: Will be the current path to the current project!");

                EditorGUILayout.BeginHorizontal("box");
                {
                    if (GUILayout.Button(copy_icon, checkBox_Style, GUILayout.Width(20f), GUILayout.Height(20f)))
                    {
                        Debug.Log("Patch Copyed To Clipboard: " + FlyDinoFileAccessor.defaultDirectoryPatch);
                        GUIUtility.systemCopyBuffer = FlyDinoFileAccessor.defaultDirectoryPatch; // Copy to clipboard defaultDirectoryPatch
                    }

                    // We display in the box the current path of our project
                    settingsDefaultDirectory = Application.dataPath;
                    settingsDefaultDirectory = GUILayout.TextField(settingsDefaultDirectory, GUILayout.Width(332f));
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                showSettingsStatus = "Settings";
            }

            GUILayout.EndVertical();
        }

        /// <param name="path">Specified directory path</param>
        // We check if the current selected path exists
        public void CeckIfPakageExist(string path)
        {
            List<string> allList = FlyDinoFileAccessor.GetPackageList(curentDirectoryPatch);

            if (allList.Contains(path)) 
            {
                AssetDatabase.ImportPackage(path, true); // Install selected pakage
            }
            else
            {
                Debug.LogError("Package not found! " + path + ":" +
                    " It's probably been deleted from the folder, or it's not valid."); // If the file path is not found, we will display ERROR + not found location
            }
        }

        /// <param name="selectedDirectory">Specified directory path</param>
        public void SelectFolder(string selectedDirectory)
        {
            if (selectedDirectory == "")
            {
                if (EditorPrefs.HasKey("CurentDirectoryPatch_key"))
                {
                    curentDirectoryPatch = EditorPrefs.GetString("CurentDirectoryPatch_key");
                    CeckCurentFolderPatch();
                }
                else
                {
                    curentDirectoryPatch = FlyDinoFileAccessor.defaultDirectoryPatch;
                    curentFolderPatch = "Default Folder";
                }
            }
            else
            {
                curentDirectoryPatch = selectedDirectory;
                EditorPrefs.SetString("CurentDirectoryPatch_key", curentDirectoryPatch);
                CeckCurentFolderPatch();
                curentDirectoryPatch = FlyDinoFileAccessor.GetLocalPackagePath();
            }

            UpdateMetadata();
        }


        // Get/Update unitypackage metadata
        private void UpdateMetadata()
        {
            if (Directory.Exists(curentDirectoryPatch)) // We check if the current folder exists
            {
                FlyDinoFileAccessor.PackageInfo(ref PackageInfoList, curentDirectoryPatch);
                SetDisplayPackageInfo();
                AssetDatabase.Refresh();
                CeckCurentFolderPatch();
            }
            else
            {
                // If the folder was not found, we set default data
                curentDirectoryPatch = FlyDinoFileAccessor.defaultDirectoryPatch;
                curentFolderPatch = "Default Folder";
            }
        }

        /// <param name="keyword">Search Keyword</param>
        // Search package by name
        private List<string> SearchPackageByName(string keyword)
        {
            List<string> allList = FlyDinoFileAccessor.GetPackageList(curentDirectoryPatch);
            List<string> pathList = new List<string>();

            for (int i = 0; i < allList.Count; ++i)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(allList[i]);
                if (fileNameNoExt.ToLower().Contains(keyword.ToLower()))
                {
                    pathList.Add(allList[i]);
                }
            }
            return pathList;
        }

        // Set the unitypackage information to display (must be called when packagePathList changes)
        // Add matching names from the holding unitypackage to the list for display
        private void SetDisplayPackageInfo()
        {
            presedCount = 0;
            displayList.Clear();

            for (int i = 0; i < PackageInfoList.Count; ++i)
            {
                for (int j = 0; j < packagePathList.Count; ++j)
                {
                    string filenameNoExt = Path.GetFileNameWithoutExtension(packagePathList[j]);
                    if (PackageInfoList[i].name == filenameNoExt)
                    {
                        displayList.Add(PackageInfoList[i]);
                    }
                }
            }
        }

        /// <param name="index">Index of pressed CeckBox</param>
        private void PressedCeckBox(int index)
        {
            List<string> allList = FlyDinoFileAccessor.GetPackageList(curentDirectoryPatch);
            FlyDinoUnityPackageInfo info = displayList[index];
            info.ceckBox = !info.ceckBox;
            displayList[index] = info;
            ceckedList.Add(allList[index]);
        }

        // Check if the current patch folder exists and set curentFolderPatch
        private void CeckCurentFolderPatch()
        {
            if (curentDirectoryPatch == FlyDinoFileAccessor.defaultDirectoryPatch)
            {
                curentFolderPatch = "Default Folder"; // Ee display this text in the box if the patch is default
                EditorPrefs.DeleteKey("CurentDirectoryPatch_key");
            }
            else
            {
                curentFolderPatch = curentDirectoryPatch; // We display the current path in the box
            }
        }        
    }
}
