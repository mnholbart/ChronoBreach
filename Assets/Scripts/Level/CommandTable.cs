using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The CommandTable script is used to turn the menus command table into an interactive level selector
/// </summary>
/// <remarks>
/// todo: lots of stuff.. not yet implemented other than loading the default mission
/// </remarks>
public class CommandTable : MonoBehaviour
{
    public static CommandTable instance;

    private List<int> missionIndices = new List<int>();
    private int selectedMissionIndex = 0;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string s = SceneUtility.GetScenePathByBuildIndex(i);
            if (s.Contains("Game/"))
                missionIndices.Add(i);
        }

        selectedMissionIndex = missionIndices[0];
    }

    /// <summary>
    /// Callback for command table button press to load the selected mission
    /// </summary>
    public void OnLevelSelectButtonPush()
    {
        GameManager.instance.LoadMission(selectedMissionIndex);
    }
}
