﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {

    public PlayerID PlayerNumber {
        get { return _PlayerNumber; }
    }
    public Animator UIAnimator {
        get { return _UIAnimator; }
    }
    public TextMeshProUGUI TimeRemaining {
        get { return _TimeRemaining; }
    }
    public TextMeshProUGUI MicrogameTitle {
        get { return _MicrogameTitle; }
    }

    [Header("Gameplay Attributes")]
    [SerializeField]
    private PlayerID _PlayerNumber;
    [SerializeField]
    private float _MoveSpeed = 1f;
    [SerializeField]
    private Camera _Camera;

    [Header("UI Objects")]
    [SerializeField]
    private RawImage _MicrogameImage;
    [SerializeField]
    private Animator _UIAnimator;
    [SerializeField]
    private TextMeshProUGUI _TimeRemaining;
    [SerializeField]
    private TextMeshProUGUI _MicrogameTitle;
    [SerializeField]
    private RectTransform _MicrogameSizer;

    private Rigidbody2D _Body;
    private RaycastHit2D[] _Hits;

    private Vector2 _MoveDir = Vector2.zero;

    public RenderTexture MicrogameTexture {
        get {
            return _MicrogameTexture;
        }
        private set {
            if(_MicrogameTexture != null)
                _MicrogameTexture.Release();
            
            _MicrogameTexture = value;
            _MicrogameImage.texture = value;

            var mg = GameManager.Instance.GetMicrogameForPlayer(PlayerNumber);
            if(mg != null)
                mg.RefetchMicrogameTexture();
        }
    }
    private RenderTexture _MicrogameTexture;

	private void Start ()
    {
        GameManager.Instance.RegisterPlayer(this);
        _Body = GetComponent<Rigidbody2D>();
        _Hits = new RaycastHit2D[1];

        if (_PlayerNumber == PlayerID.One)
            _Camera.rect = new Rect(0, 0, 0.5f, 1);
        else
            _Camera.rect = new Rect(0.5f, 0, 0.5f, 1);

        int width = (int)_MicrogameSizer.sizeDelta.x + Screen.width / 2;
        MicrogameTexture = new RenderTexture(width, width, 24);
	}

    private void Update()
    {
        _MoveDir = new Vector2(GetAxis(PlayerAxis.Horizontal),
                               GetAxis(PlayerAxis.Vertical));

        if (GameManager.Instance.PlayingMicrogame(PlayerNumber)) {
            int width = (int)_MicrogameSizer.sizeDelta.x + Screen.width / 2;

            if (width != MicrogameTexture.width || width != MicrogameTexture.height)
                MicrogameTexture = new RenderTexture(width, width, 24);
        }
    }

    private void FixedUpdate()
    {
        // You can't move if we are playing a microgame
        if (GameManager.Instance.PlayingMicrogame(PlayerNumber))
            return;
        
        float d = _MoveSpeed * Time.fixedDeltaTime;
        Move(_MoveDir, d);
    }

    private void Move(Vector2 dir, float d) {
        if(d < 0) {
            d *= -1;
            dir *= -1;
        }
        // See if we hit anything
        if (_Body.Cast(dir, _Hits, d) != 0)
        {
            Debug.Assert(_Hits[0].distance <= d);

            // If we are moving against a wall...
            if (Vector2.Dot(_Hits[0].normal, dir) < 0)
            {
                // Slide along the wall
                Vector2 tan = Quaternion.Euler(0, 0, 90) * _Hits[0].normal;
                Vector2 pen = Vector3.Project(dir, tan);
                var slide = pen * (d - _Hits[0].distance); // slide by remaining penetration

                d = Mathf.Max(_Hits[0].distance - 0.01f, 0); // truncate "unblocked" movement

                _Body.MovePosition(_Body.position + dir * d + slide);
                return;
            }
        }

        _Body.MovePosition(_Body.position + dir * d);
    }

    public float GetAxis(PlayerAxis axis) {
        if(PlayerNumber == PlayerID.One) {
            if(axis == PlayerAxis.Horizontal)
                return Input.GetAxisRaw("Horizontal");
            if (axis == PlayerAxis.Vertical)
                return Input.GetAxisRaw("Vertical");
        } else {
            if (axis == PlayerAxis.Horizontal)
                return Input.GetAxisRaw("Horizontal 2");
            if (axis == PlayerAxis.Vertical)
                return Input.GetAxisRaw("Vertical 2");
        }
        return 0;
    }
}

public enum PlayerID
{
    One = 0, Two = 1
}

public enum PlayerAxis
{
    Horizontal, Vertical
}