using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLaser : MonoBehaviour
{
    protected AudioSource sound;
    public float Tick;
    protected float lifeSpan;
    private NPC advancedRobot;
    private Rigidbody _rb;
    private float _speed;

    void Awake()
    {
        _speed = 40f;
        sound = GetComponent<AudioSource>();
        advancedRobot = FindObjectOfType<NPC>();
        _rb = GetComponent<Rigidbody>();
        lifeSpan = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        OnStage();
    }

    public void Move()
    {
        _rb.AddForce(-transform.forward * _speed);
    }

    public BulletLaser InitializePosition(Vector2 spawnerPos)
    {
        transform.localPosition = spawnerPos;
        return this;
    }

    public BulletLaser PlayBulletSound()
    {
        sound.PlayOneShot(sound.clip);
        return this;
    }

    protected virtual void BackToPool()
    {
        advancedRobot.GetLaserBulletPool().ReturnObject(this);
    }

    protected virtual void OnStage()
    {
        Tick += Time.deltaTime;
        if (Tick >= lifeSpan)
        {

            Tick = 0f;
            BackToPool();

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<DonePlayerHealth>() != null) {
            collision.gameObject.GetComponent<DonePlayerHealth>().TakeDamage(150f);
            BackToPool();
        }

        if (collision.gameObject.layer == Layers.PLAY_AREA) {
            BackToPool();
        }
    }
}
