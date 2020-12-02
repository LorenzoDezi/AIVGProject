using GOAP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentDebug : MonoBehaviour
{
    [SerializeField]
    private Agent agent;
    [SerializeField]
    private GameObject textDebugPrefab;
    private List<TMP_Text> textComponents;

    void Start()
    {
        textComponents = new List<TMP_Text>();
        foreach(var goal in agent.Goals) {
            var textDebugObj = GameObject.Instantiate(textDebugPrefab, transform);
            textComponents.Add(textDebugObj.GetComponent<TMP_Text>());
        }
        var textDebugObj2 = GameObject.Instantiate(textDebugPrefab, transform);
        textComponents.Add(textDebugObj2.GetComponent<TMP_Text>());
    }

    private void UpdateTexts() {
        int i = 0;
        for(i = 0; i < textComponents.Count - 1; i++) {
            if(agent.Goals[i] != null) {
                textComponents[i].text = "Goal: " + agent.Goals[i].name + "\n Priority: " + agent.Goals[i].Priority;
            }
        }
        if(agent.CurrAction != null)
            textComponents[i].text = "CurrAction: " + agent.CurrAction.name;
    }

    void Update()
    {
        transform.position = agent.transform.position;
        UpdateTexts();
    }
}
