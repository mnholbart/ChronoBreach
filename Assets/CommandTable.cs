using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommandTable : MonoBehaviour {

    [SerializeField]
    private List<int> missionIndices = new List<int>();
    public static CommandTable instance;

    private int selectedMissionIndex = 0;

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

    private void Start()
    {

    }

    public void OnLevelSelectButtonPush()
    {
        GameManager.instance.LoadMission(selectedMissionIndex);
    }
}
