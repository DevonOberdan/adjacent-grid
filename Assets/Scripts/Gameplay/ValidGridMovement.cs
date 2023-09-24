using UnityEngine;
using UnityEngine.Events;

public abstract class ValidGridMovement : MonoBehaviour
{
    [SerializeField] private bool flagNoValidMovement;

    [SerializeField] private UnityEvent<bool> OnDoomed;

    private GridManager gridManager;


    private bool wasDoomed;


    protected void Awake()
    {

    }

    protected void Start()
    {
        gridManager = GetComponent<GridManager>();
    }

    protected void Update()
    {

    }



    protected void CheckNoValidMovement()
    {
        bool isDoomed = !AnyValidMovement();

        if (isDoomed != wasDoomed)
            OnDoomed.Invoke(isDoomed);

        wasDoomed = isDoomed;
    }

    protected abstract bool AnyValidMovement();
}
