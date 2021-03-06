using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    public class GroundState
    {
        private GameObject player;
        private Collider2D collider_;
        private float width;
        private float height;
        private float length;

        void Start()
        {
            collider_ = player.GetComponent<Collider2D>();
        }
        //GroundState constructor.  Sets offsets for raycasting.
        public GroundState(GameObject playerRef)
        {
            player = playerRef;
            width = collider_.bounds.extents.x + 0.1f;
            height = collider_.bounds.extents.y + 0.2f;
            //width = player.collider2D.bounds.extents.x + 0.1f;
            //height = player.collider2D.bounds.extents.y + 0.2f;
            length = 0.05f;
        }

        //Returns whether or not player is touching wall.
        public bool isWall()
        {
            bool left = Physics2D.Raycast(new Vector2(player.transform.position.x - width, player.transform.position.y), -Vector2.right, length);
            bool right = Physics2D.Raycast(new Vector2(player.transform.position.x + width, player.transform.position.y), Vector2.right, length);

            if (left || right)
                return true;
            else
                return false;
        }

        //Returns whether or not player is touching ground.
        public bool isGround()
        {
            bool bottom1 = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y - height), -Vector2.up, length);
            bool bottom2 = Physics2D.Raycast(new Vector2(player.transform.position.x + (width - 0.2f), player.transform.position.y - height), -Vector2.up, length);
            bool bottom3 = Physics2D.Raycast(new Vector2(player.transform.position.x - (width - 0.2f), player.transform.position.y - height), -Vector2.up, length);
            if (bottom1 || bottom2 || bottom3)
                return true;
            else
                return false;
        }

        //Returns whether or not player is touching wall or ground.
        public bool isTouching()
        {
            if (isGround() || isWall())
                return true;
            else
                return false;
        }

        //Returns direction of wall.
        public int wallDirection()
        {
            bool left = Physics2D.Raycast(new Vector2(player.transform.position.x - width, player.transform.position.y), -Vector2.right, length);
            bool right = Physics2D.Raycast(new Vector2(player.transform.position.x + width, player.transform.position.y), Vector2.right, length);

            if (left)
                return -1;
            else if (right)
                return 1;
            else
                return 0;
        }
    }

    //Feel free to tweak these values in the inspector to perfection.  I prefer them private.
    private Rigidbody2D rigid;
    public float speed = 14f;
    public float accel = 6f;
    public float airAccel = 3f;
    public float jump = 14f;  //I could use the "speed" variable, but this is only coincidental in my case.  Replace line 89 if you think otherwise.

    private GroundState groundState;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        //Create an object to check if player is grounded or touching wall
        groundState = new GroundState(transform.gameObject);
    }

    private Vector2 input;

    void Update()
    {
        //Handle input
        if (Input.GetKey(KeyCode.LeftArrow))
            input.x = -1;
        else if (Input.GetKey(KeyCode.RightArrow))
            input.x = 1;
        else
            input.x = 0;

        if (Input.GetKeyDown(KeyCode.Space))
            input.y = 1;

        //Reverse player if going different direction
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, (input.x == 0) ? transform.localEulerAngles.y : (input.x + 1) * 90, transform.localEulerAngles.z);
    }

    void FixedUpdate()
    {
        rigid.AddForce(new Vector2(((input.x * speed) - rigid.velocity.x) * (groundState.isGround() ? accel : airAccel), 0)); //Move player.
        rigid.velocity = new Vector2((input.x == 0 && groundState.isGround()) ? 0 : rigid.velocity.x, (input.y == 1 && groundState.isTouching()) ? jump : rigid.velocity.y); //Stop player if input.x is 0 (and grounded) and jump if input.y is 1

        if (groundState.isWall() && !groundState.isGround() && input.y == 1)
            //rigidbody2D.velocity = new Vector2(-groundState.wallDirection() * speed * 0.75f, rigidbody2D.velocity.y); //Add force negative to wall direction (with speed reduction)

        input.y = 0;
    }
}