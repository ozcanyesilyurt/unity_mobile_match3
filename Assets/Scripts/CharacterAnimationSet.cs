using UnityEngine;

[CreateAssetMenu(menuName = "Character Data/Animation Set")]
public class CharacterAnimationSet : ScriptableObject
{
    public AnimationClip Idle;
    public AnimationClip Walk;
    public AnimationClip Run;

    public AnimationClip Attack1;
    public AnimationClip Attack2;
    public AnimationClip Attack3;   // optional
    public AnimationClip Defend;   // optional

    public AnimationClip Hurt;
    public AnimationClip Dead;

    public AnimationClip SpecialAttack1;
    public AnimationClip SpecialAttack2;   // optional
}