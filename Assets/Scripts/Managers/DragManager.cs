using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public static DragManager Instance;

    [Header("��Ϸ����")]
    public Drag[] allBlocks;
    public Target[] allTargets;

    public Button exit;

    [Header("UI")]
    public Button resetButton;
    public GameObject completePanel;

    void Awake()
    {
        Instance = this;
        exit.onClick.AddListener(() => { InteractableManager.Instance.CloseInteractable(InteractableManager.InteractableType.Drag); });
    }

    void Start()
    {

    }
    
    public void CheckComplete()
    {
        foreach (var block in allBlocks)
        {
            if (!block.isPlaced)
                return;
        }
        
        if (completePanel != null)
            completePanel.SetActive(true);
    }
    
    public void LoadScene(Scenemanager.Scenes sceneName)
    {
        if (Scenemanager.Instance != null)
            Scenemanager.Instance.LoadScene(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene((int)sceneName);
    }
}