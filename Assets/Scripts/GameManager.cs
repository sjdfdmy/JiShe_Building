using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("��Ϸ����")]
    public Drag[] allBlocks;
    public Target[] allTargets;

    [Header("UI")]
    public Button resetButton;
    public GameObject completePanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    // 检查是否全部完成
    public void CheckComplete()
    {
        foreach (var block in allBlocks)
        {
            if (!block.isPlaced)
                return;
        }

        // 所有块体已放置：显示完成面板
        if (completePanel != null)
            completePanel.SetActive(true);
    }

    // 切换到指定场景（带过渡动画）
    public void LoadScene(string sceneName)
    {
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadScene(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}