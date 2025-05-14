using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    //�� �ִ뽺��
    [Header("Max Player Stat")]
    private int maxhungryStat = 100;
    private int maxthirstyStat = 100;
    private int maxtemperature = 100;
    private int maxhp = 100;
    private int maxstamina = 100;
    private int maxweight = 100;
    private float maxglidingStat = 100f;

    //�� ���罺��
    [Header("Current Player Stat")]

    private int hungryStat=100;
    private int thirstyStat;
    private int temperature;
    private int hp;
    private int stamina;
    private int weight;
    [SerializeField]
    private float glidingStat;
    private int hpregeneration=1;
    private int staminaregeneration=1;


    //�����̻� ������ �ɼ�������
    [Flags]
    public enum situation
    {
        None,
        smallhunger,
        hunger,
        smallthirst,
        exhaustion,
        haviness,
        thirst,
        dead
    }
    public situation Sit = situation.None;

    public float GlidingStat
    {
        get
        {
            return glidingStat;
        }
        set
        {
            glidingStat = value;
            if (glidingStat <= 0)
            {
                glidingStat = 0;
            }
            if (glidingStat >= maxglidingStat)
            {
                glidingStat = maxglidingStat;
            }
        }
    }
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
                //��������
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
            //����� ��ġ�� �������� ���϶�� �߻��Ǵ� ü����������
            if (hungryStat <= (maxhungryStat * 0.5) && hungryStat > 0)
            {
                Sit |= situation.smallhunger;
            }
            else if (hungryStat <= 0)
            {
                hungryStat = 0;
                //�ȵǸ� float�� �ٲ��� ��
                Sit |= situation.hunger;
            }
            if (Sit.HasFlag(situation.hunger) && hungryStat > 0)
            {
                Sit &= ~situation.hunger;
            }
            if (Sit.HasFlag(situation.smallhunger)&& hungryStat > (maxhungryStat * 0.5f))
            {
                Sit &= ~situation.smallhunger;
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
            //���� ��ġ�� �������� ���϶�� �߻��Ǵ� ü����������
            if (ThirstyStat <= (maxthirstyStat * 0.5) && thirstyStat > 0)
            {
                Sit |= situation.smallthirst;
            }
            //0�̵Ǹ� �߻��ϴ� Ż������
            else if (thirstyStat <= 0)
            {
                thirstyStat = 0;
                Sit |= situation.exhaustion;
            }
            if (Sit.HasFlag(situation.smallthirst) && thirstyStat > (maxthirstyStat * 0.5f))
            {
                Sit &= ~situation.smallthirst;
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
            if (Weight > maxweight)
            {
                Sit |= situation.haviness;
            }
            if (Sit.HasFlag(situation.haviness) && weight <= maxweight)
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
                stamina = 0;
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
   
    //��� �԰����̱��ѵ� Ȥ�� �𸣴ϱ� �� Ǯ����� ü��,���׹̳� ��� ������Ƽ
    public int Hpregeneration
    {
        get
        {
            return hpregeneration;
        }
        set
        {
            hpregeneration = value;
        }
    }
    public int Staminaregeneration
    {
        get
        {
            return staminaregeneration;
        }
        set 
        {
            staminaregeneration = value;
        }
    }
    //1������ ĳ��������1ȸ��
    public PlayerStat()
    {
        this.hungryStat = maxhungryStat;
        this.thirstyStat = maxthirstyStat;
        this.maxhp = hp;
        this.stamina = maxstamina;
        this.weight = 0;
        this.hpregeneration = 1;
        this.staminaregeneration = 1;
        this.glidingStat = maxglidingStat;
    }
    //ĳ���ͻ����Ӹ��ƴ϶� ����������� ���������Ű��Ƽ� =0 ���� ��ó�� ������ 0���� ����ΰ� �ϸ����������
    public PlayerStat(int hungryStat = 0, int thirstyStat = 0, int temperature = 0, int hp = 0, int stamina = 0, int weight = 0, int hpregeneration = 0, int stregeneration = 0, float GullidingStat = 0)
    {
        this.maxhungryStat += hungryStat;
        this.maxthirstyStat += thirstyStat;
        this.temperature += temperature;
        this.maxhp += hp;
        this.maxstamina = stamina;
        this.maxweight = weight;
        this.hpregeneration += hpregeneration;
        this.staminaregeneration += stregeneration;
        this.glidingStat += GullidingStat;
    }
}