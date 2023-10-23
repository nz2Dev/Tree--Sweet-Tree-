using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialLerp : MonoBehaviour
{

    public Material material1;
    public Material material2;
    Renderer rend;



float timeElapsed;
float lerpDuration = 3;
float startValue=0;
float endValue=1;
float valueToLerp;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer> ();
        rend.material = material1;

}

    void Update()

  {
    if (timeElapsed < lerpDuration)
    {
      valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
      timeElapsed += Time.deltaTime;
    rend.material.Lerp(material1, material2, valueToLerp);
    }
    else 
    {
      valueToLerp = endValue;
    }
  }
        }