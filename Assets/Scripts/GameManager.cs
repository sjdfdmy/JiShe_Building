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

    // ����Ƿ�ȫ�����
    public void CheckComplete()
    {
        foreach (var block in allBlocks)
        {
            if (!block.isPlaced)
                return;
        }

    }
}