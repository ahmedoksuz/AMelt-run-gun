using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class M_Door : IMeltable
{
    #region Garbage

    private float valueTextUpdateDuration = .01f;
    private float dotThree = .3f;
    private float dotfifteen = .15f;

    #endregion

    private float meltedAmount;

    private float MeltedAmount
    {
        get => meltedAmount;
        set => meltedAmount = value;
    }

    [SerializeField] private float startValue;
    [SerializeField] private RectTransform solidBarLineRect;
    [SerializeField] private Collider realDoorCollider;

    private Collider collider;
    private MeshRenderer currentDissolveMesh;
    private MaterialPropertyBlock dissolveBlock;

    private float startSolidBarXValue = -285;
    private string dissolveKey = "_Dissolve";
    [SerializeField] private Image fillBar;
    [SerializeField] private GameObject canvas;
    private Coroutine MeltCO;

    [SerializeField] private TextMeshPro moneyText;
    [SerializeField] private Animator moneyTextAnimator;
    private static readonly int Play = Animator.StringToHash("Play");

    private void Start()
    {
        collider = GetComponent<Collider>();
        currentDissolveMesh = GetComponent<MeshRenderer>();
        dissolveBlock = new MaterialPropertyBlock();

        SetStartValue();
        SetState();

        if (startValue <= 0)
        {
            StartCoroutine(Die());
            canvas.SetActive(false);
        }
    }

    private void SetState()
    {
        var _targetBarX = startSolidBarXValue + 570f / health * MeltedAmount;
        var _dissolve = 1f / health * MeltedAmount;

        dissolveBlock.SetFloat(dissolveKey, _dissolve);
        currentDissolveMesh.SetPropertyBlock(dissolveBlock);


        fillBar.fillAmount = _dissolve;
        solidBarLineRect.anchoredPosition = new Vector3(_targetBarX, 100f, 0);
    }

    private void SetStartValue()
    {
        MeltedAmount = health - startValue;
    }


    public override void StartMelt(string state)
    {
        if (!melting)
        {
            melting = true;

            StopMeltCO();
            MeltCO = StartCoroutine(CO_Melt());
        }
    }

    public override void StopMelt()
    {
        StopMeltCO();
    }

    public override void ForceStopAction()
    {
        StopMeltCO();
        if (startValue <= 0) StartCoroutine(Die());
    }

    public override void Invisiable()
    {
        moneyText.gameObject.SetActive(true);
        moneyTextAnimator.enabled = true;
        moneyTextAnimator.SetTrigger(Play);
    }

    public override void TakeDamage(float damage)
    {
        MeltedAmount += damage;
    }

    private void StopMeltCO()
    {
        if (MeltCO != null)
        {
            StopCoroutine(MeltCO);
            MeltCO = null;
        }
    }

    private int exText;
    private int extTextCheck = -1;

    private float t = .1f;

    private IEnumerator CO_Melt()
    {
        while (solidBarLineRect.anchoredPosition.x < 285f)
        {
            var _targetBarX = startSolidBarXValue + 570f / health * MeltedAmount;
            var _dissolve = 1f / health * MeltedAmount;

            var ex_Dissolve = dissolveBlock.GetFloat(dissolveKey);
            var ex_TargetBarX = solidBarLineRect.anchoredPosition.x;

            var new_Dissolve = Mathf.Lerp(ex_Dissolve, _dissolve, t);
            var new_TargetBarX = Mathf.Lerp(ex_TargetBarX, _targetBarX, t);

            dissolveBlock.SetFloat(dissolveKey, new_Dissolve);
            fillBar.fillAmount = new_Dissolve;
            currentDissolveMesh.SetPropertyBlock(dissolveBlock);

            solidBarLineRect.anchoredPosition = new Vector3(new_TargetBarX, 100f, 0);


            yield return new WaitForEndOfFrame();
        }

        dissolveBlock.SetFloat(dissolveKey, 1f);
        currentDissolveMesh.SetPropertyBlock(dissolveBlock);

        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        collider.enabled = false;
        yield return new WaitForEndOfFrame();
        realDoorCollider.enabled = true;
    }
}