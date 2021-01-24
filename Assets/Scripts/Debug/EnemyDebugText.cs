using GOAP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyDebugText : MonoBehaviour
{
    [SerializeField]
    private Agent agent;
    private TMP_Text text;
    private HealthComponent healthComponent;

    void Awake() {
        if(agent == null) {
            this.enabled = false;
            return;
        }
        healthComponent = agent.GetComponent<HealthComponent>();
        text = GetComponent<TMP_Text>();
        healthComponent.Death.AddListener(OnDeath);
        text.text = "";
    }

    private void OnDeath() {
        this.enabled = false;
    }

    void Update() {

        string textDisplayed = "";

        if (agent.Goals.Count > 0) {
            var goal = agent.Goals[0];
            textDisplayed += goal.name;
        }

        textDisplayed += "\n";

        Action currAction = agent.CurrAction;
        if(currAction != null) {
            textDisplayed += currAction.name;
        }

        text.text = textDisplayed;
    }
}
