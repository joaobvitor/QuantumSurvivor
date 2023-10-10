using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AffectedByGravity : MonoBehaviour
{
    [SerializeField]
    private float gravityCooldown = 0f;

    [SerializeField]
    private bool _onCooldown = false;

    public bool OnCooldown { 
        get {
            return _onCooldown;
        }
        private set {
            _onCooldown = value;
        }
    }

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (OnCooldown) {
            gravityCooldown -= Time.deltaTime;
            if (gravityCooldown <= 0f)
                OnCooldown = false;
        }
    }

    public void OnGravityWasSwitched(float gravityCooldownToSet) {
        rb.gravityScale = -rb.gravityScale;
        OnCooldown = true;
        gravityCooldown = gravityCooldownToSet;
        gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, -gameObject.transform.localScale.y);
    }
}