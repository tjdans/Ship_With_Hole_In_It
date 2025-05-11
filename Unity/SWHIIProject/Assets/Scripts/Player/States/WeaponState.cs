using Unity.XR.OpenVR;
using UnityEngine;

public class WeaponState : PlayerState
{

    public WeaponState(PlayerManager player, PlayerStateMachine machine) : base(player, machine) { }

    public virtual void OnAttack(){}

}
