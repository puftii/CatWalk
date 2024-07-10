using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float currentHealth;
    public int level;
    public int exp;
    public int strength;
    public int speed;
    public int endurance;
    public int skill;
    public int skillPoints;
    public float[] position;
    public PlayerData (PlayerCombat Player)
    {
        exp = Player.Exp;
        level = Player.Level;
        currentHealth = Player._currentHealth;
        exp = Player.Exp;
        strength = Player.Strength;
        speed = Player.Speed;
        endurance = Player.Endurance;
        skill = Player.Skill;
        skillPoints = Player.SkillPoints;
        position = new float[3];
        position[0] = Player.transform.position.x;
        position[1] = Player.transform.position.y;
        position[2] = Player.transform.position.z;
    }
}
