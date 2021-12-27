
using System.Collections;
using UnityEngine;
using TMPro;


public class Movement : MonoBehaviour
{
    public static Movement Char;
    public Animator anim;

    public Animator turtleAnim;

    public Rigidbody2D rb;

    public CountdownTimer timer;

    public GameObject CoinPrefab;
    public GameObject headButton;
    public GameObject knockbackBox;

    public Transform EnemPos;
    public Transform Player;

    [SerializeField] public AudioSource footstepsSand;

    public float moveSpeed = 250;

    public bool isWalking;
    public bool isCharging;
    public bool isStunned;
    private static Vector2 moveDir = Vector2.zero;
    private static Vector2 chargeDir;
    public float boostTimer;
    public float chargeTimer;
    public bool boosting;


    public float range = 8;

    public static float knockbackPower = 15;
    public static float knockbackDuration = 3;
    [SerializeField] public MoveJoystick joystick;

    [SerializeField] public TextMeshProUGUI coinCounter, trashCounter, scoreCounter;

    [HideInInspector]
    public int trashAmount;
    public int coinAmount;
    public static int scoreAmount;

    public TextMeshProUGUI myScore;
    public TextMeshProUGUI myName;
    public int currentScore;
    public int targetScore = 0;
    public int coinPrefabCount = 0;
    public float chargeTime;
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsStunned = Animator.StringToHash("isStunned");
    private static readonly int IsCharging = Animator.StringToHash("isCharging");
    private static readonly int NoNet = Animator.StringToHash("noNet");

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();//player RB
        EnemPos = GameObject.FindGameObjectWithTag("Enemy").transform; //enemy position
        chargeDir = EnemPos.transform.position;

        isStunned = false;
        isCharging = false;

        trashAmount = 0;
        coinAmount = 0;
        scoreAmount = 0;

        moveSpeed = 250;
        boostTimer = 0;
        chargeTimer = 0;
        boosting = false;

        range = 8;
    }
    void Awake()
    {
        Char = this;
    }

    void Update()
    {
        ActivateHeadbuttOn();
        SpawnCoin();

        if (!isCharging && !isStunned) // disable player velocity input while charging or stunned.
        {
            if (joystick.InputDir != Vector2.zero)
            {
                moveDir = joystick.InputDir;

            }

            if (moveDir.x != 0 || moveDir.y != 0)
            {
                anim.SetFloat(X, moveDir.x);
                anim.SetFloat(Y, moveDir.y);
                if (!isWalking)
                {
                    isWalking = true;
                    anim.SetBool(IsWalking, isWalking);
                    footstepsSand.Play();
                }
            }
            else
            {
                if (isWalking)
                {
                    isWalking = false;
                    anim.SetBool(IsWalking, isWalking);
                    footstepsSand.Pause();


                }
            }


            StopMoving();

            moveDir = new Vector2(moveDir.x, moveDir.y).normalized;

            coinCounter.text = " Tokens : " + coinAmount.ToString();  //scoreboard stuff
            trashCounter.text = " Trash : " + trashAmount.ToString();
            scoreCounter.text = " Score : " + scoreAmount.ToString();

            if (boosting) //move speed increase
            {
                boostTimer += Time.deltaTime;
                if (boostTimer >= 5)
                {
                    moveSpeed = 250;
                    boostTimer = 0;
                    boosting = false;
                }
            }
        }
        
        
    }
    public void SpawnCoin()
    {
        if (coinPrefabCount == 1)  //coin prefab instantiates
        {
            return;
        }
        else if (targetScore >= 100)
        {
            var randomPos = new Vector3(Random.Range(-35, 35f), Random.Range(-26, 4f));
            randomPos.z = 0;

            Instantiate(CoinPrefab, randomPos, Quaternion.identity);
            targetScore -= 100;
        }

    }
    void FixedUpdate()
    {
        rb.velocity = moveSpeed * Time.deltaTime * moveDir;
    }
    public void StopMoving()
    {
       

        rb.velocity = Vector2.zero;
        
    }
    public void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.GetComponent<NetTurtle>()) //turtle free, net destroyed
        {
            
            turtleAnim.SetBool(NoNet, true);
            scoreAmount += 10;
            trashAmount += 5;
            targetScore += 10;
            CountdownTimer.currentTime += 10f;
            Destroy(trigger.gameObject);
        }
        if (trigger.gameObject.GetComponent<Trash>())  //trash score, relocate trash
        {
            
            scoreAmount += 5;
            trashAmount += 1;
            targetScore += 5;
            CountdownTimer.currentTime += 1f;
            trigger.gameObject.transform.position = new Vector2(Random.Range(-35, 35f), Random.Range(-26, 4f));
        }
        else if (trigger.gameObject.GetComponent<CoinPrefab>()) //coin score destroy, move speed increase
        {
            
            if (trigger.gameObject.CompareTag("Coin"))
            {
                boosting = true;
                moveSpeed = 500;
                boostTimer -= 5;
                
            }
            scoreAmount += 10;
            coinAmount += 1;
            targetScore += 10;
            CountdownTimer.currentTime += 5f;
            Destroy(trigger.gameObject);
            coinPrefabCount = 0;
        }

    }
    public void ActivateHeadbuttOn() //if enemy in range activate head-butt button
    {
       
        if (Vector2.Distance(rb.transform.position, EnemPos.transform.position) <= range)
        {
            headButton.SetActive(true);
        }
        else
        {
            headButton.SetActive(false);
            
        }
    }
    public void ChargeEnemy()   //OnClick-->headButton. Charge to enemy position(not working correctly), disable enemy script. 
    {
        //Play charge animation for the direction charged (also not working).
        if (isStunned || !(chargeTimer >= 5)) return;
        knockbackBox.SetActive(true);
        isCharging = true;
        anim.SetBool(IsCharging, true);
        chargeTimer += Time.deltaTime;
        moveSpeed = 800;
        chargeTimer = 0;
        headButton.SetActive(false);
    }
        public IEnumerator Stunned() //enemy stun player
        {
            isCharging = false;
            isWalking = false;
            isStunned = true;
            moveDir.x = 0;
            moveDir.y = 0;
            moveSpeed = 0f;
            anim.SetBool(IsStunned, true);
            footstepsSand.Pause();
            CountdownTimer.currentTime -= 10f;

            yield return new WaitForSeconds(3f);

            isStunned = false;
            anim.SetBool(IsStunned, false);
            footstepsSand.Play();
        }
        public IEnumerator Slowed() //enemy slow player
        {
            moveSpeed = 185f;

            CountdownTimer.currentTime -= 10f;

            yield return new WaitForSeconds(5f);
        }
    }
