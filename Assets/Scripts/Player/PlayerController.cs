using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHasColor
{
    public ScriptableColor ActiveColor { get; private set; }
    public Action e_ColorChanged;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    private SpriteRenderer sprRend;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator anim;

    private void Awake()
    {
        #region Get Components
        sprRend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        #endregion
    }

    private void Start()
    {
        SetColor(ColorID.Red);
    }

    private void Update()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (rb.velocity.y > 0)
        {
            col.enabled = false;
        }
        else if (rb.velocity.y < 0)
        {
            anim.SetTrigger("Fall");
            col.enabled = true;
        }

        Move();

        HandleColorChange();
    }

    private void HandleColorChange()
    {
        if (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(0))
        {
            SetToPreviousColorInCycle();
            e_ColorChanged?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(1))
        {
            SetToNextColorInCycle();
            e_ColorChanged?.Invoke();
        }
    }

    private void Move()
    {
        float moveVal = Input.GetAxis("Horizontal");

        rb.AddForce(Vector2.right * moveVal * moveSpeed);
        if (Mathf.Abs(rb.velocity.x) > 5)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * 5, rb.velocity.y); //Clamp velocity when max speed is reached!
    }

    public void SetColor(ColorID _color)
    {
        ActiveColor = ColorManager.Instance.GetColorByID(_color);
        sprRend.color = ActiveColor.Color;
    }

    public void SetToNextColorInCycle()
    {
        SetColor(ColorManager.Instance.GetNextColorInCycle(ActiveColor.ColorID));
    }
    public void SetToPreviousColorInCycle()
    {
        SetColor(ColorManager.Instance.GetPreviousColorInCycle(ActiveColor.ColorID));

    }

    private void OnCollisionEnter2D(Collision2D _other)
    {
        if (_other.gameObject.GetComponent<IHasColor>() != null)
            Jump();
    }

    private void Jump()
    {
        anim.SetTrigger("Jump");
        rb.AddForce(Vector2.up * jumpForce);
    }

}
