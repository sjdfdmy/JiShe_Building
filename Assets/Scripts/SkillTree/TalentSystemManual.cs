using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TalentSystemManual : MonoBehaviour
{
    [Header("三条线，每行5个节点，按顺序拖入")]
    public TalentNodeUI[] craftingNodes = new TalentNodeUI[5];
    public TalentNodeUI[] procurementNodes = new TalentNodeUI[5];
    public TalentNodeUI[] salesNodes = new TalentNodeUI[5];

    [Header("三条线数据")]
    public TalentData craftingData;
    public TalentData procurementData;
    public TalentData salesData;

    [Header("右侧详情面板")]
    public GameObject detailPanel;
    public Image detailIcon;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailDescText;
    public TextMeshProUGUI statusText;
    public Button unlockButton;
    public TextMeshProUGUI costText;      // 显示消耗
    public TextMeshProUGUI currencyText;  // 显示货币数量

    [Header("货币")]
    public int currency = 100;  // 初始货币

    // 存储每条线解锁到第几个（-1 = 初始状态，都没解锁，0 = 解锁了第一个）
    private Dictionary<string, int> unlockedCount = new Dictionary<string, int>();

    private string selectedBranch;
    private int selectedIndex;
    private TalentData.TalentNode selectedTalent;

    void Start()
    {
        // 初始都是-1，表示都没解锁，第一个也无法解锁
        unlockedCount["crafting"] = -1;
        unlockedCount["procurement"] = -1;
        unlockedCount["sales"] = -1;

        if (detailPanel != null)
            detailPanel.SetActive(false);

        if (unlockButton != null)
        {
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(OnUnlockClick);
        }

        RefreshLine("crafting", craftingNodes, craftingData);
        RefreshLine("procurement", procurementNodes, procurementData);
        RefreshLine("sales", salesNodes, salesData);

        UpdateCurrencyUI();
    }

    void RefreshLine(string branch, TalentNodeUI[] nodes, TalentData data)
    {
        if (nodes == null || data == null || data.talents == null) return;

        int unlocked = unlockedCount[branch]; // -1表示都没解锁

        for (int i = 0; i < 5 && i < nodes.Length && i < data.talents.Count; i++)
        {
            if (nodes[i] == null) continue;

            int index = i;
            TalentData.TalentNode talent = data.talents[i];

            // 判断状态
            bool isUnlocked = i <= unlocked;  // i <= unlocked 表示已解锁
            bool isAvailable = (i == unlocked + 1) && (currency >= 1); // 下一个且货币够

            nodes[i].Setup(talent.icon, isUnlocked, isAvailable, () => {
                OnNodeClick(branch, index, talent, isUnlocked, isAvailable);
            });
        }
    }

    void OnNodeClick(string branch, int index, TalentData.TalentNode talent, bool isUnlocked, bool isAvailable)
    {
        selectedBranch = branch;
        selectedIndex = index;
        selectedTalent = talent;

        ShowDetailPanel(isUnlocked, isAvailable);
    }

    void ShowDetailPanel(bool isUnlocked, bool isAvailable)
    {
        if (selectedTalent == null) return;

        if (detailPanel != null)
            detailPanel.SetActive(true);

        if (detailIcon != null)
            detailIcon.sprite = selectedTalent.icon;

        if (detailNameText != null)
            detailNameText.text = selectedTalent.talentName;

        if (detailDescText != null)
            detailDescText.text = selectedTalent.description;

        if (isUnlocked)
        {
            // 已解锁
            if (statusText != null)
            {
                statusText.text = "已解锁";
                statusText.color = new Color(0.2f, 0.8f, 0.2f);
            }
            if (unlockButton != null)
                unlockButton.gameObject.SetActive(false);
            if (costText != null)
                costText.gameObject.SetActive(false);
        }
        else if (isAvailable)
        {
            // 可解锁（货币够）
            if (statusText != null)
            {
                statusText.text = "未解锁";
                statusText.color = Color.gray;
            }
            if (unlockButton != null)
            {
                unlockButton.gameObject.SetActive(true);
                unlockButton.interactable = true;
            }
            if (costText != null)
            {
                costText.gameObject.SetActive(true);
                costText.text = "1"; // 消耗1货币
            }
        }
        else
        {
            // 无法解锁（前面没解锁或货币不够）
            if (statusText != null)
            {
                statusText.text = "未解锁（无法解锁）";
                statusText.color = Color.red;
            }
            if (unlockButton != null)
                unlockButton.gameObject.SetActive(false);
            if (costText != null)
                costText.gameObject.SetActive(false);
        }
    }

    void OnUnlockClick()
    {
        if (currency < 1)
        {
            Debug.Log("货币不足！");
            return;
        }

        if (string.IsNullOrEmpty(selectedBranch)) return;

        // 消耗货币
        currency--;

        // 增加该线的解锁计数
        unlockedCount[selectedBranch]++;

        Debug.Log("解锁成功！剩余货币: " + currency);

        // 刷新显示
        RefreshLine("crafting", craftingNodes, craftingData);
        RefreshLine("procurement", procurementNodes, procurementData);
        RefreshLine("sales", salesNodes, salesData);

        UpdateCurrencyUI();

        // 刷新详情面板
        ShowDetailPanel(true, false);
    }

    void UpdateCurrencyUI()
    {
        if (currencyText != null)
            currencyText.text = currency.ToString();
    }

    // 加货币（测试用）
    public void AddCurrency(int amount)
    {
        currency += amount;
        UpdateCurrencyUI();
        RefreshLine("crafting", craftingNodes, craftingData);
        RefreshLine("procurement", procurementNodes, procurementData);
        RefreshLine("sales", salesNodes, salesData);
    }

    [ContextMenu("Reset")]
    public void ResetTalents()
    {
        currency = 100;
        unlockedCount["crafting"] = -1;
        unlockedCount["procurement"] = -1;
        unlockedCount["sales"] = -1;

        if (detailPanel != null)
            detailPanel.SetActive(false);

        RefreshLine("crafting", craftingNodes, craftingData);
        RefreshLine("procurement", procurementNodes, procurementData);
        RefreshLine("sales", salesNodes, salesData);

        UpdateCurrencyUI();
    }
}