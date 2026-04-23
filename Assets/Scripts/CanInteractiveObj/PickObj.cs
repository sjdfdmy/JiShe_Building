using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickObj : MonoBehaviour,IInteractable
{
    public enum Type
    {
        Middle=0,
        High=1,
    }
    public Type type;
    public int minnum;
    public int maxnum;
    public List<MaterialData> datas;

    private void Start()
    {
        int num=Random.Range(minnum, maxnum + 1);
        for(int i = 0; i < num; i++)
        {
            MaterialData tmp = GameDataManager.Instance.GetMaterialData((int)type);
            datas.Add(tmp);
        }
    }

    private void Update()
    {
        if(datas.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    public string GetInteractPrompt()
    {
        return "°´ F ËÑË÷Îï×Ê";
    }

    public void OnInteract(PlayerMoveManager player)
    {
        if (player == null)
        {
            return;
        }
        Cursor.lockState = CursorLockMode.None;
        GameDataManager.Instance.cursor.SetActive(true);
        GameDataManager.Instance.player.GetComponent<PlayerMoveManager>().enabled = false;
        for(int i = PickManager.Instance.GetObjs.childCount - 1; i >= 0; i--)
        {
            Destroy(PickManager.Instance.GetObjs.GetChild(i).gameObject);
        }
        for(int i=0;i< datas.Count; i++)
        {
            GameObject tmp=Instantiate(datas[i].objUI, PickManager.Instance.GetObjs);
                tmp.GetComponent<ObjUI>().Init();
        }

        PickManager.Instance.picks.SetActive(true);
    }
}
