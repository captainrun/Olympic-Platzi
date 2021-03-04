﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PlayerRunnerController : MonoBehaviour, IActiveInputObserver
{
    [SerializeField] private GlobalFloat _gameVelocity;
    private Animator _animator;
    private Rigidbody2D _rb;
    private float _forceToJump;
    [SerializeField] private float _forceForDamage;
    [SerializeField] private float _forceNormalizer;
    [SerializeField] private GameObject _enemy;

    [SerializeField] private UnityEvent HurtPlayer;
    [SerializeField] private UnityEvent OnPlayerDied;
    private Difficulty _currentDifficulty;

    private int _counterOfDodgeClear;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _currentDifficulty = GameObject.Find("CurrentDifficulty").GetComponent<CurrentDifficulty>().currentDifficulty;

    }

    private void Start()
    {
        _counterOfDodgeClear = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetFloat("GlobalVelocity", _gameVelocity.vFloat + .1f);
        _animator.SetFloat("MoveY", transform.position.y);


        if(_counterOfDodgeClear >= _currentDifficulty.counterOfDodgeClear)
        {
            _counterOfDodgeClear = 0;
            _enemy.GetComponent<Rigidbody2D>()
            .AddRelativeForce(_enemy.transform.right * -1 * _forceForDamage, ForceMode2D.Force);

            _currentDifficulty.targetGameVelocity += 0.1f;
            //Debug.Log("ForceToEnemy");
        }
    }

    public void Notify(Obstacle obstacle)
    {
        StartCoroutine(ActionPlayer(obstacle));
        _counterOfDodgeClear++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            if(HurtPlayer != null)
                HurtPlayer.Invoke();
        }
        else if(collision.CompareTag("Enemy"))
        {
            if(OnPlayerDied != null)
                OnPlayerDied.Invoke();

            Debug.Log("player died");
        }
    }

    public void Hurt()
    {
        _rb.AddRelativeForce(transform.right * -1 * _forceForDamage, ForceMode2D.Force);
        Debug.Log("HURT");

        _counterOfDodgeClear = 0;
        _currentDifficulty.targetGameVelocity -= 0.06f;

    }

    private IEnumerator ActionPlayer(Obstacle obstacle)
    {
        while (true)
        {
            if (obstacle && Vector3.Distance(transform.position, obstacle.transform.position) < 2f)
            {
                switch (obstacle.GetObstacleType())
                {
                    case var tmp when (tmp == ObstacleType.JumpBox || tmp == ObstacleType.JumpGhost):

                        _animator.SetTrigger("Jump");
                        _forceToJump = obstacle.GetComponent<BoxCollider2D>().bounds.size.y 
                                        + Mathf.Abs(obstacle.GetMinYPosition()) * _forceNormalizer;
                        _rb.AddRelativeForce(transform.up * _forceToJump, ForceMode2D.Impulse);
                        break;
                    case ObstacleType.Duck:

                        _animator.SetTrigger("Duck");

                        break;
                    default:
                        break;
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

    }
}
