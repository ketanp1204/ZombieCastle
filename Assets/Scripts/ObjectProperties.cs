using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties : MonoBehaviour
{

    public string objectName;
    public DefaultItem objectData;

    private SpriteGlow.SpriteGlowEffect spriteGlowEffectComponent;

    // Start is called before the first frame update
    void Start()
    {
        spriteGlowEffectComponent = GetComponent<SpriteGlow.SpriteGlowEffect>();
        if (spriteGlowEffectComponent)
            spriteGlowEffectComponent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
