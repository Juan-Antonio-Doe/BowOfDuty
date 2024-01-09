using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : NPC {

	[field: Header("Ally Settings")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] public AlliesManager allies { get; private set; }


}
