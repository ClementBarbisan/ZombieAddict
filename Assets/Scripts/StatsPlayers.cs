using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class StatsPlayers : MonoBehaviour
{
    public WebsocketManager.InfosPlayer stats;
    [FormerlySerializedAs("textMesh")] public TextMeshProUGUI nameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetStats()
    {
        nameText.text = stats.name + Environment.NewLine + Environment.NewLine + "Accuracy : " + stats.accuracy +
                        Environment.NewLine + "Damages taken : " + stats.damages + Environment.NewLine
                        + "Damages to enemy : " + stats.damagesEnemy + Environment.NewLine + "Enemy killed : " +
                        stats.enemyKilled + Environment.NewLine + "Shoot fired : " + stats.shootFired +
                        Environment.NewLine + "Walk distance : " + stats.walkDistance;
    }
}
