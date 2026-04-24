using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="CreateGuide",menuName ="CreateData/CreateGuide",order =1)]
public class CreateGuide : ScriptableObject
{
    public UnityAction wt;
    [Header("展示界面文字")]
    public List<string> UItext;
}
