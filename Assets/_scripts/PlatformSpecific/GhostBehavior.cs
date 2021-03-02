﻿using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostBehavior : DieOnAnimationFinishComponent
{
    public enum GhostType
    {
        Dark = 1,
        White = 2
    }

    private GameObject _player;

    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private SpriteRenderer ownSprite;
    [SerializeField] private Collider2D ghostCollider;
    private static readonly int AnimatorIsAlive = Animator.StringToHash("isAlive");


    private GhostType _ghostType;
    public GhostType GetGhostType => _ghostType;

    [SerializeField] private float velocity, minWaitTime, maxWaitTime;
    private float _initialVelocity;

    private void Awake()
    {
        if (_player == null)
        {
            _player = GameObject.Find("Player");
        }
    }


    private void ChangeRotation(Vector3 lookAt)
    {
        var rotateY = transform.position.x > lookAt.x;

        float yRotation = rotateY ? 0 : 180;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, yRotation,
            Util.GetAngleFromTwoVectors(transform.position, rotateY ? -lookAt : lookAt));
    }

    [SerializeField] private Vector2 offsetPlayer;

    private IEnumerator DoAttack()
    {
        ChangeRotation(_player.transform.position);

        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        var goRight = _player.transform.position.x > transform.position.x;
        Vector3 offset = goRight ? (Vector3) offsetPlayer : new Vector3(-offsetPlayer.x, offsetPlayer.y);

        transform.SetParent(null);
        var currentCount = 0.0;
        //State 1
        while (Math.Abs(transform.position.x - _player.transform.position.x) > 0.2f &&
               Math.Abs(transform.position.y - _player.transform.position.y) > 0.05f)
        {
            transform.position =
                Vector3.MoveTowards(transform.position,
                    _player.transform.position + offset,
                    velocity * Time.deltaTime);
            ChangeRotation(_player.transform.position);


            if (currentCount > 1.0f)
            {
                velocity += 1;
                currentCount = 0;
            }
            else
                currentCount += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(SimulateMove(goRight));
    }

    private IEnumerator SimulateMove(bool goRight)
    {
        var currentCount = 0.0;
        while (currentCount < 1.0f)
        {
            transform.position += new Vector3(goRight ? velocity * Time.deltaTime : -velocity * Time.deltaTime,
                -1.0f * Time.deltaTime);
            currentCount += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(GoToOtherSide(goRight));
    }

    private IEnumerator GoToOtherSide(bool goRight)
    {
        var newPos = GhostSpawnSystem.Instance.GetRandomPos(goRight);
        while (Math.Abs(transform.position.x - newPos.x) > 0.2f)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, newPos, velocity * Time.deltaTime);

            ChangeRotation(newPos);
            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }


    public void Die()
    {
        ghostCollider.enabled = false;
        StopAllCoroutines();
        StartCoroutine(DieCoroutine());
    }


    private IEnumerator DieCoroutine()
    {
        ghostAnimator.SetBool(AnimatorIsAlive, false);
        while (!canDead)
        {
            transform.position =
                Vector3.MoveTowards(transform.position,
                    new Vector3(
                        GhostSpawnSystem.Instance.GetRandomPos(_player.transform.position.x > transform.position.x).x,
                        -10)
                    ,
                    velocity * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        SetValues();
        StartCoroutine(DoAttack());
        PlatPlayerInteractive.OnDamage += OnDamage;
    }

    private void OnDamage()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        PlatPlayerInteractive.OnDamage -= OnDamage;
    }


    private void SetValues()
    {
        ghostCollider.enabled = true;
        ghostAnimator.SetBool(AnimatorIsAlive, true);
        canDead = false;
        _ghostType = Random.Range(0, 1.0f) > 0.5f ? GhostType.Dark : GhostType.White;
        ownSprite.color = _ghostType == GhostType.Dark ? Color.black : Color.white;
    }
}


public abstract class DieOnAnimationFinishComponent : MonoBehaviour
{
    [HideInInspector] public bool canDead;
}