using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

public class PlayerController : MonoBehaviour
{
    //#region Public

    [Header("Movement")]
    public float maxSpeed = 1f;

    public float velocitySmoothTime = 1f;
    public float angleSmoothTime;

    [Header("VFX")]
    public Transform vfx;

    [Header("Weapons")]
    public Weapon[] Weapons;

    [Header("Pickups")]
    public float xpPickupRange;

    public LayerMask collectiblesLayer;

    [Header("UI")]
    public GameObject levelUpPanel;

    public Button[] itemButtons;
    public Button buttonContinue;
    public TextMeshProUGUI xpTmp;
    public TextMeshProUGUI levelTmp;
    public Slider hpSlider;
    public Slider xpSlider;
    private float _angle;
    private float _angleVelocity;

    private Coroutine[] _coroutines;
    private Vector3 _currentVelocity;

    private readonly float _hp = 100f;

    //#endregion

    //#region Private

    private InputSystem_Actions _input;

    private int _level = 1;

    private Vector2 _moveDirection;
    private Rigidbody2D _rb;
    private int _requiredXp;
    private Quaternion _rotation;
    private float _targetAngle;

    private Vector3 _velocity;
    private int _xp;

    //#endregion

    private void Awake()
    {
        _input = new InputSystem_Actions();
        var playerActions = _input.Player;

        playerActions.Move.started += ctx =>
        {
            var value = ctx.ReadValue<Vector2>();
            SetMove(value);
            SetDirection(value);
        };
        playerActions.Move.performed += ctx =>
        {
            var value = ctx.ReadValue<Vector2>();
            SetMove(value);
            SetDirection(value);
        };
        playerActions.Move.canceled += _ => SetMove(Vector2.zero);

        _rb = GetComponent<Rigidbody2D>();

        levelUpPanel.SetActive(false);
        buttonContinue.onClick.AddListener(MenuContinue);

        UpdateXpRequirement();
    }

    private void Start()
    {
        UpdateUi();
        _coroutines = Weapons.Select(weapon => StartCoroutine(AttackRoutine(weapon))).ToArray();
    }

    private void Update()
    {
        UpdatePosition();
        FindNearbyPickups();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void OnDestroy()
    {
        foreach (var coroutine in _coroutines)
        {
            StopCoroutine(coroutine);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, xpPickupRange);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Gem"))
        {
            return;
        }

        var gem = other.GetComponent<XpGem>();

        _xp += gem.xp;

        if (_xp >= _requiredXp)
        {
            LevelUp();
        }

        UpdateUi();

        gem.Collect();
    }

    private void SetMove(Vector2 direction)
    {
        _moveDirection = direction;
    }

    private void SetDirection(Vector2 direction)
    {
        _targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private IEnumerator AttackRoutine(Weapon weapon)
    {
        while (true)
        {
            weapon.Attack(_rb.position, _rotation);
            yield return new WaitForSeconds(weapon.cooldown);
        }
    }

    private void UpdatePosition()
    {
        var targetVelocity = _moveDirection * maxSpeed;

        _velocity = Vector3.SmoothDamp(_velocity, targetVelocity, ref _currentVelocity, velocitySmoothTime);
        _rb.linearVelocity = _velocity;

        _angle = Mathf.SmoothDampAngle(_angle, _targetAngle, ref _angleVelocity, angleSmoothTime);
        _rotation = Quaternion.AngleAxis(_angle, Vector3.forward);
        vfx.rotation = _rotation;
    }

    private void FindNearbyPickups()
    {
        // Use Physics2D.OverlapCircleAll with a LayerMask for better performance
        var hitColliders = Physics2D.OverlapCircleAll(
            transform.position,
            xpPickupRange,
            collectiblesLayer
        );

        foreach (var c in hitColliders)
        {
            var gem = c.GetComponent<XpGem>();
            gem?.AttractToPlayer(transform);
        }
    }

    private void UpdateUi()
    {
        levelTmp.text = $"Level {_level}";
        xpTmp.text = $"{_xp}/{_requiredXp} XP";

        xpSlider.value = _xp / (float)_requiredXp;
        hpSlider.value = _hp / 100f;
    }

    private void LevelUp()
    {
        Time.timeScale = 0;

        itemButtons[0].gameObject.SetActive(true);
        itemButtons[1].gameObject.SetActive(true);
        itemButtons[2].gameObject.SetActive(false);
        itemButtons[3].gameObject.SetActive(false);

        itemButtons[0].onClick.AddListener(() => { Debug.Log("Clicked item 1!"); });
        itemButtons[1].onClick.AddListener(() => { Debug.Log("Clicked item 2!"); });

        levelUpPanel.SetActive(true);

        _level++;
        _xp -= _requiredXp;

        UpdateXpRequirement();

        UpdateUi();
    }

    private void MenuContinue()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1;
    }

    private void UpdateXpRequirement()
    {
        var nextLevel = _level + 1;
        var value = 5;

        value += 10 * Mathf.Max(0, Mathf.Min(_level - 1, 20));

        if (nextLevel > 20)
        {
            value += 13 * Mathf.Min(nextLevel - 20, 20);
        }

        if (nextLevel > 40)
        {
            value += 16 * (nextLevel - 40);
        }

        _requiredXp += value;
    }
}