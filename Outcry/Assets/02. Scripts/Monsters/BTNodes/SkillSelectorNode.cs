using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillSelectorNode : SelectorNode
{
    public override void Reset()
    {
        base.Reset();
        ShuffleChildren();
    }
}
