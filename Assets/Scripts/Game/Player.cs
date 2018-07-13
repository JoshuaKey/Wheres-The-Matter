using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] float speed = 1f;

    [Header("Other")]
    [SerializeField] AudioClip collectSound;
    [SerializeField] AudioSource collectSource;

    [Header("Cursor")]
    [SerializeField] private Cursor cursor;

    private new Camera camera;

    private int AtomCollectorLayer;
    private int AtomCollectorLayerMask;

    private bool canCollect = true;
    private bool canEscape = true;

    // Use this for initialization
    void Start () {
        camera = GetComponent<Camera>();

        AtomCollectorLayer = LayerMask.NameToLayer("AtomCollector");
        AtomCollectorLayerMask = 1 << AtomCollectorLayer;

        print("Radius : " + Game.Instance.playerData.GetValue(PlayerData.UpgradeType.Collect_Radius));

        cursor.SetSize(Game.Instance.playerData.GetValue(PlayerData.UpgradeType.Collect_Radius));
        Game.Instance.playerData.OnCollectRadiusChange += OnRadiusChange;
    }

    private void Update() {
        if (canCollect) {
            CheckForCollect();     
        }

        cursor.UpdatePosition();

        if (Input.GetKeyDown(KeyCode.Escape) && canEscape) {
            Game.Instance.ToggleMenu();
        }


        //if (Input.GetKey(KeyCode.H)) {
        //    Game.Instance.Absorb(Game.Instance.gameData.FindAtom(1), 1000000);
        //    Game.Instance.Absorb(Game.Instance.gameData.FindAtom(6), 1000000);
        //    Game.Instance.Absorb(Game.Instance.gameData.FindAtom(7), 1000000);
        //    Game.Instance.Absorb(Game.Instance.gameData.FindAtom(13), 1000000);
        //}
        //if (Input.GetKey(KeyCode.J)) {
        //    Game.Instance.Absorb(Game.Instance.gameData.FindAtom(118), 1);
        //}
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    Game.Instance.QueueDialogue("Hello");
        //    Game.Instance.QueueDialogue("How are you?");
        //    Game.Instance.QueueDialogue("I am Doctor Scientist");
        //    Game.Instance.QueueDialogue("You may refer to me as DocSci.\nI am a famous Scientist Doctor who cures Science from all Disease.");
        //    Game.Instance.QueueDialogue("Fascinating isn't it?");
        //    Game.Instance.QueueDialogue("... Are you listening?", true);
        //}

    }

    public void CheckForCollect() {
        if (Input.GetMouseButton(0)) {
            if (!collectSource.isPlaying) {
                collectSource.Play();
            }

            var playerData = Game.Instance.playerData;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hit = Physics.SphereCastAll(ray, playerData.GetValue
                (PlayerData.UpgradeType.Collect_Radius), 20.0f, AtomCollectorLayerMask, QueryTriggerInteraction.Collide);

            Vector3 pos = Input.mousePosition;

            float weightLimit = Game.Instance.playerData.GetValue(PlayerData.UpgradeType.Collect_Weight);
            for (int i = 0; i < hit.Length; i++) {
                AtomCollector collector = hit[i].collider.GetComponent<AtomCollector>();

                var atoms = collector.Absorb();

                for (int y = 0; y < atoms.Count; y++) {
                    var info = Game.Instance.gameData.FindAtomInfo(atoms[y].atom.GetAtomicNumber());
                    if(info.GetProtons() + info.GetNeutrons() <= weightLimit ) {
                        Game.Instance.PlayEffect(atoms[y].atom, atoms[y].amo, pos);
                    }         
                }

            }
            if (hit.Length == 0) {
                print("WE aren't collecting anything...");
            }
        }

        Vector2 mvmt = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) {
            mvmt.y += 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            mvmt.y -= 1;
        }
        if (Input.GetKey(KeyCode.D)) {
            mvmt.x += 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            mvmt.x -= 1;
        }

        if (mvmt != Vector2.zero) {
            float speed = this.speed;
            if (Input.GetKey(KeyCode.LeftShift)) { speed *= speed; }
            Game.Instance.Move(mvmt * speed * Time.deltaTime);
        }
    }

    public void CanEscape(bool value) { canEscape = value; }
    public void CanCollect(bool value) { canCollect = value; }

    private void OnRadiusChange(float radius) {
        cursor.SetSize(radius);
    }
}
