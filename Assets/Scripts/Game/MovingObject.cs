using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {

    [Header("Mvmt")]
    [SerializeField] float speed;

    [SerializeField] float walkTime;
    [SerializeField] float pauseTime;

    [Header("Sprites")]
    [SerializeField] Sprite upSprite;
    [SerializeField] Sprite downSprite;
    [SerializeField] Sprite rightSprite;
    [SerializeField] Sprite leftSprite;

    private SpriteRenderer spriteRenderer;

    private Vector2Int chunkIndex = Vector2Int.zero;
    private Vector2 direction = Vector2.zero;
    private bool isPaused = true;
    private float destTime;

    // These objects should start after the world has been initialized...
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = upSprite;

        chunkIndex = Game.Instance.world.GetChunkIndex(transform.position);
    }

    // Update is called once per frame
    void Update () {
        if (isPaused) {
            if(destTime < Time.time) {
                StartMoving();
            }
        } else {
            if (destTime < Time.time) {
                Pause();
            } else {
                Move();
            }
        }
	}

    public void Move() {
        transform.Translate(direction * speed * Time.deltaTime);

        //obj.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(obj.transform.position.y * 100f) * -1;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        World world = Game.Instance.world;
        chunkIndex = world.CheckChunk(this.gameObject, chunkIndex);
    }

    public void StartMoving() {
        isPaused = false;

        destTime = Time.time + Random.value * walkTime;

        int dirValue = Random.Range(0, 4);
        switch (dirValue) {
            case 0:
                direction = Vector2.up;
                spriteRenderer.sprite = upSprite; // Use Animations in the future???
                break;
            case 1:
                direction = Vector2.down;
                spriteRenderer.sprite = downSprite;
                break;
            case 2:
                direction = Vector2.left;
                spriteRenderer.sprite = leftSprite;
                break;
            case 3:
                direction = Vector2.right;
                spriteRenderer.sprite = rightSprite;
                break;
        }
    }

    public void Pause() {
        isPaused = true;

        destTime = Time.time + pauseTime;
    }
}
