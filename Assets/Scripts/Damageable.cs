using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;
    public UnityEvent<int, int> healthChanged;
    Animator animator;

    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth {
        get {
            return _maxHealth;
        }
        set {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;
    public int Health {
        get {
            return _health;
        }
        set {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);

            if (_health <= 0)
                IsAlive = false;
        }
    }

    [SerializeField]
    private bool _isAlive = true;
    public bool IsAlive {
        get {
            return _isAlive;
        }
        set {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            
            if (value == false)
                damageableDeath.Invoke();
        }
    }

    public bool IsHit {
        get {
            return animator.GetBool(AnimationStrings.isHit);
        }
        set {
            animator.SetBool(AnimationStrings.isHit, value);
        }
    }

    public float invincibilityTime = 1f;
    private float timeSinceHit;
    [SerializeField]
    private bool _isInvincible = false;
    public bool IsInvincible {
        get {
            return _isInvincible;
        }
        set {
            _isInvincible = value;
        }
    }

    void Awake()
    {
       animator = GetComponent<Animator>(); 
    }

    private void Update() {
        if (IsInvincible) {
            if (timeSinceHit > invincibilityTime) {
                IsInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage, Vector2 knockback) {
        if (IsAlive && !IsInvincible) {
            Health -= damage;
            IsInvincible = true;

            IsHit = true;
            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            return true;
        }

        return false;
    }

    public bool Heal(int healthRestore) {
        if (IsAlive) {
            if (Health + healthRestore > MaxHealth) {
                healthRestore = MaxHealth - Health;
                Health = MaxHealth;
            }
            else
                Health += healthRestore;
            
            if (healthRestore > 0) {
                CharacterEvents.characterHealed.Invoke(gameObject, healthRestore);
                return true;
            }
        }

        return false;
    }
}
