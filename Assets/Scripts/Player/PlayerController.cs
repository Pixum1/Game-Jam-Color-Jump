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
    [SerializeField] private float colorSwitchCooldown;

    [SerializeField] private GameObject colorWheel;

    private TrailRenderer trailRend;
    private SpriteRenderer sprRend;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator anim;

    private float colorSwitchTimer;
    private float switchTime { get { return colorSwitchCooldown / Time.timeScale; } } // Scales down with increasing time scale

    private float borderCoords { get { return Camera.main.orthographicSize + (Camera.main.orthographicSize / 2); } }

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
        colorSwitchTimer = switchTime;
        rb.gravityScale = 0;

        // Set player gravity when game starts
        GameManager.Instance.e_GameStarted += Init;
    }

    private void Init()
    {
        rb.gravityScale = 2;
        SetColor(ColorID.Red);
        e_ColorChanged?.Invoke();
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != EGameState.Running) return;

        HandleColorChange();

        // Reset horizontal velocity
        rb.velocity = new Vector2(0, rb.velocity.y);

        #region Falling & Jumping
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
        #endregion

        Move();

        #region Flip Sprite
        if (rb.velocity.x > 0)
            sprRend.flipX = false;
        else if (rb.velocity.x < 0)
            sprRend.flipX = true;
        #endregion

        // Kepp in bounds
        this.transform.position = new Vector3(Mathf.Clamp(transform.position.x, -borderCoords, borderCoords), this.transform.position.y);

        // Destroy when out of bounds
        if (transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize - 4)
        {
            DestroyColorObject();
            GameManager.Instance.GameOver();
        }

        colorSwitchTimer -= Time.deltaTime;
    }

    private void HandleColorChange()
    {
        if (colorSwitchTimer >= 0) return;

        if (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(0))
            SetToPreviousColorInCycle();

        else if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(1))
            SetToNextColorInCycle();

        else
            return;

        e_ColorChanged?.Invoke();

        colorSwitchTimer = switchTime;
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

    private IEnumerator RotateMe(Vector3 _byAngles, float _inTime)
    {
        Quaternion fromAngle = colorWheel.transform.rotation;
        Quaternion toAngle = Quaternion.Euler(colorWheel.transform.eulerAngles + _byAngles);

        for (float t = 0f; t < 1; t += Time.deltaTime / _inTime)
        {
            colorWheel.transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }

        colorWheel.transform.rotation = toAngle;

        switch (ActiveColor.ColorID)
        {
            case ColorID.Red:
                colorWheel.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case ColorID.Green:
                colorWheel.transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case ColorID.Blue:
                colorWheel.transform.eulerAngles = new Vector3(0, 0, -180);
                break;
            case ColorID.Yellow:
                colorWheel.transform.eulerAngles = new Vector3(0, 0, -90);
                break;
        }
    }

    public void SetToNextColorInCycle()
    {
        SetColor(ColorManager.Instance.GetNextColorInCycle(ActiveColor.ColorID));
        StartCoroutine(RotateMe(new Vector3(0, 0, 90), switchTime));
    }
    public void SetToPreviousColorInCycle()
    {
        SetColor(ColorManager.Instance.GetPreviousColorInCycle(ActiveColor.ColorID));
        StartCoroutine(RotateMe(new Vector3(0, 0, -90), switchTime));
    }

    private void OnCollisionEnter2D(Collision2D _other)
    {
        if (GameManager.Instance.GameState != EGameState.Running) return;

        IHasColor otherObject = _other.gameObject.GetComponent<IHasColor>();

        if (otherObject != null)
        {
            Jump();
            otherObject.DestroyColorObject();
        }
    }

    private void Jump()
    {
        anim.SetTrigger("Jump");
        rb.velocity = new Vector2(rb.velocity.x, maxJumpSpeed);
        GameManager.Instance.PlayJumpSound(EAudioClip.Jump);
    }

    public void DestroyColorObject()
    {
        GameManager.Instance.PlayJumpSound(EAudioClip.Death);
        Destroy(this.gameObject);
    }
}
