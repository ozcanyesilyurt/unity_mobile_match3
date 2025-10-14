using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Character Config")]
    public string Name;
    public Sprite Sprite;
    public float MoveSpeed;
    public float AttackSpeed;
    public int Health;
    public int AttackPower;
    public int SpecialAttackPower;
    public float SpecialAttackCooldown;//for enemies
    public int RewardPoints;//for enemies

    [Header("Animation Set")]
    public CharacterAnimationSet Animations; // clips for Idle/Walk/Run/Attack/etc.


    [Header("Sounds")]
    public AudioClip AttackSound;
    public AudioClip DeathSound;
    public AudioClip MoveSound;
    public AudioClip IdleSound;
    public AudioClip HurtSound;
    public AudioClip SpecialAttackSound;
}
