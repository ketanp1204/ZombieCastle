﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Cached References
    public CanvasGroup healthBarCG;
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void HideHealthBarAfterDelay(float delay)
    {
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(healthBarCG, 1f, 0f, delay));
    }

    public void ShowHealthBarAfterDelay(float delay)
    {
        new Task(UIAnimation.FadeCanvasGroupAfterDelay(healthBarCG, 0f, 1f, delay, 0.1f));
    }

    public void UseHealthPotion()
    {
        // Play health potion sound
        AudioManager.PlaySoundOnceOnNonPersistentObject(AudioManager.Sound.HealthPotionCollect);

        slider.value = 100f;

        fill.color = gradient.Evaluate(slider.normalizedValue);

        string text = "Health restored";
        InventoryManager.instance.ShowTextOnHighlightText(text, 0f, 2f);

        InventoryManager.instance.DeleteInventoryItem(GameAssets.instance.healthPotion);

        ShowHealthBarAfterDelay(0f);

        HideHealthBarAfterDelay(3f);
    }
}
