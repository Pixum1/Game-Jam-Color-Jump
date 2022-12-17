using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHasColor
{
    public ScriptableColor ActiveColor { get; private set; }
    public Action e_ColorChanged;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxJumpSpeed;

    private TrailRenderer trailRend;
    private SpriteRenderer sprRend;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator anim;
    private bool started;

    private float borderCoords { get { return Camera.main.orthographicSize * 1.333f; } }

    private void Awake()
    {
        #region Get Components
        trailRend = GetComponentInChildren<TrailRenderer>();
        sprRend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        #endregion
    }

    private void Start()
    {
        SetColor(ColorID.Red);

        e_ColorChanged?.Invoke();

        Time.timeScale = 0;
    }

    private void Update()
    {
        HandleColorChange();

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

        if (rb.velocity.y > maxJumpSpeed)
            rb.velocity = new Vector2(rb.velocity.x, maxJumpSpeed);

        Move();

        if (this.transform.position.x < -borderCoords)
            this.transform.position = new Vector2(-borderCoords, this.transform.position.y);
        else if (this.transform.position.x > borderCoords)
            this.transform.position = new Vector2(borderCoords, this.transform.position.y);
    }

    private void HandleColorChange()
    {
        if (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(0))
        {
            if (!started)
            {
                GameManager.Instance.Started = true;
                started = true;
                Time.timeScale = 1;
                return;
            }

            SetToPreviousColorInCycle();
            e_ColorChanged?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(1))
        {
            if (!started)
            {
                GameManager.Instance.Started = true;
                started = true;
                Time.timeScale = 1;
                return;
            }

            SetToNextColorInCycle();
            e_ColorChanged?.Invoke();
        }
    }

    private void Move()
    {
        float moveVal = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(moveVal * moveSpeed, rb.velocity.y);
    }

    public void SetColor(ColorID _color)
    {
        ActiveColor = ColorManager.Instance.GetColorByID(_color);
        sprRend.color = ActiveColor.Color;


        Color.RGBToHSV(ActiveColor.Color, out float H, out float S, out float V);
        V *= .8f;
        Color newColor = Color.HSVToRGB(H, S, V);

        trailRend.endColor = newColor;
        trailRend.startColor = newColor;
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
        if (!started) return;

        IHasColor otherObject = _other.gameObject.GetComponent<IHasColor>();

        if (otherObject != null)
        {
            Jump();
            otherObject.DestroyColorObject();
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!started) return;

        if (_other.CompareTag("Player"))
            DestroyColorObject();
    }

    private void Jump()
    {
        anim.SetTrigger("Jump");
        rb.velocity = new Vector2(rb.velocity.x, maxJumpSpeed);
    }

    public void DestroyColorObject()
    {
        Destroy(this.gameObject);
    }
}
