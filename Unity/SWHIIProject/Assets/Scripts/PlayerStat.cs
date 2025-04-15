using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    //각 최대스텟
    private int maxhungryStat = 100;
    private int maxthirstyStat = 100;
    private int maxtemperature = 100;
    private int maxhp = 100;
    private int maxstamina = 100;
    private int maxweight = 100;

    //각 현재스텟
    private int hungryStat;
    private int thirstyStat;
    private int temperature;
    private int hp;
    private int stamina;
    private int weight;


    //상태이상 여러개 될수있으니
    [Flags]
    public enum situation
    {
        hunger,
        exhaustion,
        haviness,
        thirst,
        dead
    }
    public situation Sit;
    public int Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            if (hp <= 0)
            {
                //죽음구현
                Sit |= situation.dead;
            }
            if (hp > maxhp)
            {
                hp = maxhp;
            }
        }
    }

    public int HungryStat
    {
        get
        {
            return hungryStat;
        }
        set
        {
            hungryStat = value;
            if (hungryStat <= 0)
            {
                hungryStat = 0;
                //안되면 float로 바꾸지 뭐
                Sit |= situation.hunger;
            }
            if (Sit.HasFlag(situation.hunger) && hungryStat > 0)
            {
                Sit &= ~situation.hunger;
            }
            if (hungryStat > maxhungryStat)
            {
                hungryStat = maxhungryStat;
            }
        }
    }
    public int ThirstyStat
    {
        get
        {
            return thirstyStat;
        }
        set
        {
            thirstyStat = value;
            if (thirstyStat <= 0)
            {
                thirstyStat = 0;
                Sit |= situation.exhaustion;
            }
            if (Sit.HasFlag(situation.exhaustion) && thirstyStat > 0)
            {
                Sit &= ~situation.exhaustion;
            }
            if (thirstyStat > maxthirstyStat)
            {
                thirstyStat = maxthirstyStat;
            }
        }
    }
    public int Weight
    {
        get
        {
            return weight;
        }
        set
        {
            Weight = value;
            if (Weight <= maxweight)
            {
                Sit |= situation.haviness;
            }
            if (Sit.HasFlag(situation.haviness) && weight < maxweight)
            {
                Sit &= ~situation.exhaustion;
            }
        }
    }

    public int Stamina
    {
        get
        {
            return stamina;
        }
        set
        {
            stamina = value;
            if (stamina <= 0)
            {
                Sit |= situation.thirst;
            }
            if (Sit.HasFlag(situation.thirst) && stamina >= maxstamina)
            {
                Sit &= ~situation.thirst;
            }
            if (stamina > maxstamina)
            {
               maxstamina = stamina;
            }
        }
    }

    //캐릭터생성뿐만아니라 장비장착에도 쓸수있을거같아서 =0 붙임 맨처음 스텟을 0으로 맞춰두고 하면되지않을까
    public PlayerStat(int hungryStat = 0, int thirstyStat = 0, int temperature = 0, int hp = 0, int stamina = 0, int weight = 0)
    {
        this.maxhungryStat += hungryStat;
        this.maxthirstyStat += thirstyStat;
        this.temperature += temperature;
        this.maxhp += hp;
        this.maxstamina = stamina;
        this.maxweight = weight;
    }
}