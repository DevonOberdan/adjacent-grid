using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UniqueSolutionUIHandler : MonoBehaviour
{
    [SerializeField] private Slider delaySlider;
    [SerializeField] private Button solveButton;
    [SerializeField] private TMP_Text uniqueSolutionText, delayTimeText;

    private UniqueSolutionBot solutionBot;

    private TMP_Text ButtonTextField => solveButton.GetComponentInChildren<TMP_Text>();

    private void Awake()
    {
        if(TryGetComponent(out solutionBot))
        {
            solutionBot = GetComponentInParent<UniqueSolutionBot>();

            if(solutionBot == null)
            {
                Debug.LogError("UniqueSolutionUIHandler: No UniqueSolutionBot found", gameObject);
                return;
            }
        }

        solutionBot.OnSetDelay += SetTimeDelay;
        solutionBot.OnSetSolutionCount += OnSetCount;
        solutionBot.OnSetSolving += SetSolving;

        solveButton.onClick.AddListener(solutionBot.SolveCurrentGrid);
        delaySlider.onValueChanged.AddListener(SetTimeDelay);
    }

    private void SetSolving(bool solving)
    {
         ButtonTextField.text = solving ? "Reset" : "Find Solutions";
    }

    private void OnSetCount(int count)
    {
        uniqueSolutionText.text = "" + count;
    }

    private void SetTimeDelay(float value)
    {
        solutionBot.MoveDelayTime = value;
        delayTimeText.text = value.ToString("F2");
    }
}