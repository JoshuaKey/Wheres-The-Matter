using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Player : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] float speed = 1f;
    private KeyCode upMvmtKey;
    private KeyCode leftMvmtKey;
    private KeyCode rightMvmtKey;
    private KeyCode downMvmtKey;
    private KeyCode boostKey;

    [Header("Other")]
    [SerializeField] AudioClip collectSound;
    [SerializeField] AudioClip errorSound;
    [SerializeField] AudioSource collectSource;
    [SerializeField] ParticleSystem discoverEffect;

    [Header("Cursor")]
    [SerializeField] private Cursor cursor;

    private new Camera camera;

    private int AtomCollectorLayer;
    private int AtomCollectorLayerMask;

    [SerializeField] private bool canCollect = true;
    [SerializeField] private bool canEscape = true;
    [SerializeField] private bool canMove = true;
    [SerializeField] private float lastCollectTime;
    [SerializeField] private bool isShaking;

    static public bool isRightHanded = true; // Where the flip do I store this???

    // Use this for initialization
    void Start () {
        camera = GetComponent<Camera>();

        AtomCollectorLayer = LayerMask.NameToLayer("AtomCollector");
        AtomCollectorLayerMask = 1 << AtomCollectorLayer;

        print("Radius : " + Game.Instance.playerData.GetValue(PlayerData.UpgradeType.Collect_Radius));

        cursor.SetSize(Game.Instance.playerData.GetValue(PlayerData.UpgradeType.Collect_Radius));
        Game.Instance.playerData.OnCollectRadiusChange += OnRadiusChange;

        IsRightHanded(true);
    }

    private void Update() {
        if (canCollect) {
            CheckForCollect();     
        }
        if (canMove) {
            CheckForMvmt();
        }

        cursor.UpdatePosition();

        if (Input.GetKeyDown(KeyCode.Escape) && canEscape) {
            Game.Instance.ToggleMenu();
        }


        if (Input.GetKey(KeyCode.H)) {
            Game.Instance.Absorb(Game.Instance.gameData.FindAtom(1), 1000000);
            Game.Instance.Absorb(Game.Instance.gameData.FindAtom(6), 1000000);
            Game.Instance.Absorb(Game.Instance.gameData.FindAtom(7), 1000000);
            Game.Instance.Absorb(Game.Instance.gameData.FindAtom(13), 1000000);
        }
        if (Input.GetKey(KeyCode.J)) {
            Game.Instance.Absorb(Game.Instance.gameData.FindAtom(118), 1);
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            Craftable c = Game.Instance.story.beachUnlockable;
            Game.Instance.playerData.AddCraftable(c);
            //for (int i = 0; i < c.GetAtomsForProduction().Length; i++) {
            //    Game.Instance.Absorb(c.GetAtomsForProduction()[i].atom, c.GetAtomsForProduction()[i].amo);
            //}
            //Game.Instance.playerData.Craft(c, 1);

            c = Game.Instance.story.mineUnlockable;
            Game.Instance.playerData.AddCraftable(c);
            //for (int i = 0; i < c.GetAtomsForProduction().Length; i++) {
            //    Game.Instance.Absorb(c.GetAtomsForProduction()[i].atom, c.GetAtomsForProduction()[i].amo);
            //}
            //Game.Instance.playerData.Craft(c, 1);

            c = Game.Instance.story.oceanUnlockable;
            Game.Instance.playerData.AddCraftable(c);
            //for (int i = 0; i < c.GetAtomsForProduction().Length; i++) {
            //    Game.Instance.Absorb(c.GetAtomsForProduction()[i].atom, c.GetAtomsForProduction()[i].amo);
            //}
            //Game.Instance.playerData.Craft(c, 1);

            c = Game.Instance.story.townUnlockable;
            Game.Instance.playerData.AddCraftable(c);
            //for (int i = 0; i < c.GetAtomsForProduction().Length; i++) {
            //    Game.Instance.Absorb(c.GetAtomsForProduction()[i].atom, c.GetAtomsForProduction()[i].amo);
            //}
            //Game.Instance.playerData.Craft(c, 1);

            c = Game.Instance.story.desertUnlockable;
            Game.Instance.playerData.AddCraftable(c);
            //for (int i = 0; i < c.GetAtomsForProduction().Length; i++) {
            //    Game.Instance.Absorb(c.GetAtomsForProduction()[i].atom, c.GetAtomsForProduction()[i].amo);
            //}
            //Game.Instance.playerData.Craft(c, 1);
        }
    }

    public void CheckForCollect() {
        // Hovering over UI Object
        if (Input.GetMouseButton(0) && !cursor.IsOverUIElement()) {
            var playerData = Game.Instance.playerData;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hit = Physics.SphereCastAll(ray, playerData.GetValue
                (PlayerData.UpgradeType.Collect_Radius), 20.0f, AtomCollectorLayerMask, QueryTriggerInteraction.Collide);

            Vector3 pos = Input.mousePosition;
            bool collected = false;
            for (int i = 0; i < hit.Length; i++) {
                AtomCollector collector = hit[i].collider.GetComponent<AtomCollector>();

                var atoms = collector.Absorb();
                //if (!collector.CanBeCollected()) {
                //    collected = true;
                //}

                for (int y = 0; y < atoms.Count; y++) {
                    Game.Instance.PlayEffect(atoms[y].atom, atoms[y].amo, pos);
                    collected = true;
                    lastCollectTime = Time.time;
                }             
            }
            
            
            if (collected || Time.time - lastCollectTime < .1f) { // Check if we are or have collected atoms previously.  
                if (!collectSource.isPlaying) {
                    collectSource.clip = collectSound;
                    collectSource.Play();
                }
            } else {
                //if (!collectSource.isPlaying) {
                //    collectSource.clip = errorSound;
                //    collectSource.Play();
                //}
            }
        }

    }
    public void CheckForMvmt() {
        Vector2 cursorMvmt = Vector2.zero; // Hovering over UI Object
        if (!cursor.IsOverUIElement()) { 
            if (cursor.transform.position.x > Screen.width * .8f) {
                // 1000 screen
                // At 800,  0 mvmt
                // At 900, 1 mvmt or 5 speed
                // At 1000, 2 mvmt or 25 speeed
                float screenSize = Screen.width / 10f;
                float cursorSpeed = (cursor.transform.position.x - Screen.width * .8f) / screenSize;
                if (cursorSpeed < 1) {
                    cursorSpeed = Mathf.Lerp(0, speed, cursorSpeed);
                } else {
                    cursorSpeed = Mathf.Lerp(speed, speed * speed, cursorSpeed - 1);
                }
                cursorMvmt.x += cursorSpeed;
            }
            if (cursor.transform.position.x < Screen.width * .2f) {
                float screenSize = Screen.width / 10f;
                float cursorSpeed = (Screen.width * .2f - cursor.transform.position.x) / screenSize;
                if (cursorSpeed < 1) {
                    cursorSpeed = Mathf.Lerp(0, speed, cursorSpeed);
                } else {
                    cursorSpeed = Mathf.Lerp(speed, speed * speed, cursorSpeed - 1);
                }
                cursorMvmt.x -= cursorSpeed;
            }
            if (cursor.transform.position.y > Screen.height * .8f) {
                float screenSize = Screen.width / 10f;
                float cursorSpeed = (cursor.transform.position.y - Screen.height * .8f) / screenSize;
                if (cursorSpeed < 1) {
                    cursorSpeed = Mathf.Lerp(0, speed, cursorSpeed);
                } else {
                    cursorSpeed = Mathf.Lerp(speed, speed * speed, cursorSpeed - 1);
                }
                cursorMvmt.y += cursorSpeed;
            }
            if (cursor.transform.position.y < Screen.height * .2f) {
                float screenSize = Screen.width / 10f;
                float cursorSpeed = (Screen.height * .2f - cursor.transform.position.y) / screenSize;
                if (cursorSpeed < 1) {
                    cursorSpeed = Mathf.Lerp(0, speed, cursorSpeed);
                } else {
                    cursorSpeed = Mathf.Lerp(speed, speed * speed, cursorSpeed - 1);
                }
                cursorMvmt.y -= cursorSpeed;
            }
        } 

        Vector2 buttonMvmt = Vector2.zero;
        if (Input.GetKey(upMvmtKey)) {
            buttonMvmt.y += 1;
        }
        if (Input.GetKey(downMvmtKey)) {
            buttonMvmt.y -= 1;
        }
        if (Input.GetKey(rightMvmtKey)) {
            buttonMvmt.x += 1;
        }
        if (Input.GetKey(leftMvmtKey)) {
            buttonMvmt.x -= 1;
        }

        if (buttonMvmt != Vector2.zero) {
            float speed = this.speed;
            if (Input.GetKey(boostKey)) { speed *= speed; }
            Game.Instance.Move(buttonMvmt * speed * Time.deltaTime);
        } else if(cursorMvmt != Vector2.zero) {
            Game.Instance.Move(cursorMvmt * Time.deltaTime);
        }
    }

    public void CanEscape(bool value) { canEscape = value; }
    public void CanCollect(bool value) { canCollect = value; }
    public void CanMove(bool value) { canMove = value; }

    public void IsRightHanded(bool value) {
        if (value) {
            upMvmtKey = KeyCode.W;
            downMvmtKey = KeyCode.S;
            leftMvmtKey = KeyCode.A;
            rightMvmtKey = KeyCode.D;
            boostKey = KeyCode.LeftShift;
        } else {
            upMvmtKey = KeyCode.UpArrow;
            downMvmtKey = KeyCode.DownArrow;
            leftMvmtKey = KeyCode.LeftArrow;
            rightMvmtKey = KeyCode.RightArrow;
            boostKey = KeyCode.RightShift;
        }
    }
    public void SetCustomHand(KeyCode up, KeyCode down, KeyCode left, KeyCode right, KeyCode boost) {
        upMvmtKey = up;
        downMvmtKey = down;
        leftMvmtKey = left;
        rightMvmtKey = right;
        boostKey = boost;
    }

    private void OnRadiusChange(float radius) {
        cursor.SetSize(radius);
    }

    public void PlayDiscoverEffect() {
        if (!discoverEffect.isPlaying) {
            Vector3 pos = cursor.GetWorldPosition();
            pos.z = -1;
            discoverEffect.transform.position = pos;
            discoverEffect.Play();
        }
        //Debug.Break();
    }

    public void ScreenShake(float amo, float duration, bool smooth = false, float smoothAmo = 0f) {
        if (!isShaking) {
            StartCoroutine(CameraShake(amo, duration, smooth, smoothAmo));
        }   
    }
    private IEnumerator CameraShake(float amount, float duration, bool smooth = false, float smoothAmo = 0f) {
        isShaking = true;

        float startAmount = amount;
        float endTime = Time.time + duration;

        while(Time.time < endTime) {
            Vector3 rotationAmount = Random.insideUnitSphere * amount;//A Vector3 to add to the Local Rotation
            rotationAmount.z = 0;//Don't change the Z; it looks funny.

            float t = (endTime - Time.time) / duration; // From 1 -> 0

            amount = t * startAmount; // Dull the shake over time

            Quaternion rotation = Quaternion.Euler(rotationAmount);
            if (smooth) {
                rotation = Quaternion.Lerp(transform.localRotation, rotation, Time.deltaTime * smoothAmo);
            }
            transform.localRotation = rotation;

            yield return null;
        }
        transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
        isShaking = false;
    }
}
