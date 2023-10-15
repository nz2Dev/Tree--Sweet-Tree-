using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tex_change : StateMachineBehaviour
{
    public Material material1;
    public Material material2;
    float speed = 1f;
    Renderer rend;
    Renderer rend1;
    Renderer rend2;
    float startTime;

override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
{

        rend = GameObject.Find("Mush_ref:body").GetComponent<Renderer> ();
        rend1 = GameObject.Find("Mush_ref:eyes").GetComponent<Renderer> ();
        rend2 = GameObject.Find("Mush_ref:must").GetComponent<Renderer> ();

        // At start, use the first material
        rend.material = material1;
        rend1.material = material1;
        rend2.material = material1;
        startTime = Time.time;
}
override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log ("I'm here'");
        // ping-pong between the materials over the duration
 //       float lerp = Mathf.PingPong(Time.time, speed) / speed;
        float t = (Time.time - startTime)*speed;
        rend.material.Lerp(material1, material2, t);
        rend1.material.Lerp(material1, material2, t);
        rend2.material.Lerp(material1, material2, t);
    }
}
   
