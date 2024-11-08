using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : EnemyBehavior {
    [Header("Red Slime Settings")]
    public int slimeHealth = 1;
    public float slimeSpeed = 2f;
    public int slimeXPValue = 1;

    // Override the Start method to apply specific settings for Red Slime
    protected override void Start() {
        base.Start(); // Call the base class Start method
        health = slimeHealth; // Set specific health for Red Slime
        speed = slimeSpeed; // Set specific speed for Red Slime
        xpValue = slimeXPValue; // Set specific XP value for Red Slime
    }
}
