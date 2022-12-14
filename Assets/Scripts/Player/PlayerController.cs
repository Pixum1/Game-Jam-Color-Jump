using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour, IHasColor
{
    [SerializeField] private float moveSpeed;

    public ScriptableColor ActiveColor { get; private set; }

    private Rigidbody2D rb;
    private SpriteRenderer sprRend;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprRend = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ChangeColor(ColorID.Red);
    }

    private void Update()
    {
        rb.velocity = Vector2.zero;

        Move();

        if(Input.GetKeyDown(KeyCode.Space))
            SetToNextColorInCycle();

    }

    private void Move()
    {
        float moveVal = Input.GetAxis("Horizontal");
        rb.velocity = Vector2.right * moveVal * moveSpeed * Time.deltaTime;
    }

    public void ChangeColor(ColorID _color)
    {
        ActiveColor = ColorManager.Instance.GetColorByID(_color);
        sprRend.color = ActiveColor.Color;
    }

    public void SetToNextColorInCycle()
    {
        ChangeColor(ColorManager.Instance.GetNextColor(ActiveColor.ColorID));
    }
}
