using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCActivation : MonoBehaviour {
    
    private MushroomNPC npc;

    private void Awake() {
        npc = GetComponent<MushroomNPC>();
    }

    public void OnActivate(Player player) {
        player.ActivateNPCInteraction(npc);
    }

}
