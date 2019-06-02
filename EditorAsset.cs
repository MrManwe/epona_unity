using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class EditorAsset<AssetType> where AssetType: UnityEngine.Object
{
    string m_assetPath;
    FileSystemWatcher m_watcher;
    bool m_upToDate;
    public EditorAsset(string i_assetPath, string i_hiddenExtension = "")
    {
        m_assetPath = i_assetPath;
        FileInfo fileInfo = new FileInfo(Application.dataPath + "/Resources/" + i_assetPath + i_hiddenExtension);
        m_watcher = new FileSystemWatcher();
        string pathToWatch = fileInfo.Directory.FullName;
        m_watcher.Path = pathToWatch;
        m_watcher.NotifyFilter = NotifyFilters.LastWrite;
        m_watcher.Changed += OnChanged;
        m_watcher.Filter = fileInfo.Name;
        m_upToDate = false;
        m_watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        m_upToDate = false;
    }

    public AssetType LoadAsset()
    {
        m_upToDate = true;
        return Resources.Load<AssetType>(m_assetPath);
    }

    public bool IsUpToDate()
    {
        return m_upToDate;
    }
}
