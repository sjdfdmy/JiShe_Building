using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTalentBranch", menuName = "Game/Talent Branch")]
public class TalentData : ScriptableObject
{
    [System.Serializable]
    public class TalentNode
    {
        public string id;
        public string talentName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
    }

    public string branchName;
    public List<TalentNode> talents = new List<TalentNode>();
}