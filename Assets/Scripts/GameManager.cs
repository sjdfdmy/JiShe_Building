using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("游戏对象")]
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

    }
}