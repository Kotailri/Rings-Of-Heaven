using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    public static float PlayerIFrameTime = 1.5f;

    public static float RingKnockbackForce = 15.0f;

    public static ControlConfig controlConfig = ControlConfig.WASD;
    public static float ControllerDeadZone = 0.5f;

    public static float soundVolume = 1.25f;
    public static float musicVolume = 1.0f;

    public static Vector2 poolPosition = new(0, -500f);


    // ===== DEBUG =====
    public static bool HitboxDebugMode = false;
}

public enum ControlConfig
{
    Arrows,
    WASD
}