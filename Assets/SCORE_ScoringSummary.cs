using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SCORE_ScoringSummary : MonoBehaviour
{
    GameController_DDOL _gc;
    [SerializeField] TextMeshProUGUI roundPoints;
    [SerializeField] TextMeshProUGUI roundBonus;
    [SerializeField] TextMeshProUGUI movesBonus;
    [SerializeField] TextMeshProUGUI movesCount;
    [SerializeField] TextMeshProUGUI roundScore;
    [SerializeField] TextMeshProUGUI totalScore;

    private void Start()
    {
        roundPoints.gameObject.SetActive(false);
        roundBonus.gameObject.SetActive(false);
        movesBonus.gameObject.SetActive(false);
        movesCount.gameObject.SetActive(false);
        roundScore.gameObject.SetActive(false);
        totalScore.gameObject.SetActive(false);

        _gc = GameObject.FindObjectOfType<GameController_DDOL>();
        StartCoroutine("DoScoreDisplay");
    }

    void SetValues()
    {
        roundPoints.text = "" + _gc.PreviousRoundScore;
        int roundBo = _gc.round * 1000;
        roundBonus.text = roundBo + "";

        float moveBonus = 1f;

        if (_gc.PreviousRoundMoves <= 20)
        {
            moveBonus = 3f;
        } else if (_gc.PreviousRoundMoves <= 25)
        {
            moveBonus = 2f;
        } else if (_gc.PreviousRoundMoves <= 30)
        {
            moveBonus = 1.5f;
        } else if (_gc.PreviousRoundMoves <= 35)
        {
            moveBonus = 1.25f;
        }

        movesBonus.text = "x" + moveBonus;
        movesCount.text = _gc.PreviousRoundMoves + "";

        int roundSco = (int) ((_gc.PreviousRoundScore + roundBo) * moveBonus);
        roundScore.text = roundSco + "";

        int extra = roundSco - _gc.PreviousRoundScore;

        int totalSco = _gc.score + extra;
        totalScore.text = totalSco + "";

        _gc.score += extra;
    }

    IEnumerator DoScoreDisplay()
    {
        SetValues();
        yield return new WaitForSeconds(0.3f);
        roundPoints.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        roundBonus.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        movesBonus.gameObject.SetActive(true);
        movesCount.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        roundScore.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        totalScore.gameObject.SetActive(true);
    }
}
