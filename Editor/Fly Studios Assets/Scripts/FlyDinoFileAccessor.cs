using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Fly Dino Importer V1.0 : (2022)
// Developed by : Fly Studios Assets (Panfilii Victor)
// Contact or Suport : flystudiosassets@gmail.com

namespace FlyDinoImporter
{
    // Static class that summarizes operations around file access
    public static class FlyDinoFileAccessor
    {
        public static string defaultDirectoryPatch = Application.dataPath; //Default directory path

        // Get the saved or default patch
        public static string GetLocalPackagePath()
        {
            string path = "";
            if (path != null)
            {
                if (FlyDinoImporter.curentDirectoryPatch == null)
                {
                    path = defaultDirectoryPatch;
                }
                else
                {
                    path = FlyDinoImporter.curentDirectoryPatch;
                }
            }
            else
            {
                path = defaultDirectoryPatch;
            }
            return path;
        }

        /// <param name="path">Specified directory path</param>
        // Get the unitypackage file list in the specified directory
        public static List<string> GetPackageList(string path) 
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            if (FlyDinoImporter._searchOptionMod == true)
            {
                FileInfo[] files = dir.GetFiles("*.unitypackage", SearchOption.AllDirectories); // Get files with .unitypackage extension in AllDirectories
                List<string> pathList = new List<string>();

                for (int i = 0; i < files.Length; ++i)
                {
                    pathList.Add(files[i].FullName);
                }
                return pathList;
            }
            else
            {
                FileInfo[] files = dir.GetFiles("*.unitypackage", SearchOption.TopDirectoryOnly); // Get files with .unitypackage extension in TopDirectoryOnly
                List<string> pathList = new List<string>();

                for (int i = 0; i < files.Length; ++i)
                {
                    pathList.Add(files[i].FullName);
                }
                return pathList;
            }
        }


        /// <param name="PackageInfoList">package info list</param>
        /// <param name="packagePath">path of unitypackage</param>
        /// <param name="infoPath">Path of folder containing package information</param>
        public static void PackageInfo(ref List<FlyDinoUnityPackageInfo> PackageInfoList, string packagePath) // Read all of the held unitypackage information
        {
            PackageInfoList.Clear();
            List<string> allList = FlyDinoFileAccessor.GetPackageList(packagePath);
            foreach (var path in allList)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(path);
                FlyDinoUnityPackageInfo info = new FlyDinoUnityPackageInfo();
                info.name = fileNameNoExt;

                info.size = GetPackageSize(path);
                PackageInfoList.Add(info);
            }
        }

        /// <param name="packagePath">File path</param>
        /// Get unitypackage file size
        public static string GetPackageSize(string packagePath)
        {
            string size = "";
            using (var fs = File.OpenRead(packagePath))
            {
                size = ((float)fs.Length / 1000000).ToString("0.0") + " MB";
            }
            return size;
        }
    }
}

