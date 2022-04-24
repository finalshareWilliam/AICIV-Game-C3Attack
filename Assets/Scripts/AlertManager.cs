using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance = null;
    private GameObject canvasComponent;
    private GameObject alertComponent;

    private List<string> phrases = new List<string> {
        "BOAAA!",
        "MUITO BOM!",
        "CONTINUE ASSIM",
        "UM BELO MATADOR",
        "INCRÍVEL",
        "IMPECÁVEL",
        "COISA BOA DEMAISSS!",
        "SERIAL KILLER",
        "MONSTER KILL"
    };

    private Stack<int> lastPhrasesIndex = new Stack<int>();

    private void Start()
    {

        if (Instance == null)
        {
            Instance = this;
        }

        ImportDependencies();

        DontDestroyOnLoad(this);
    }

    public void ShowText(string text, Color color, float timeToDestroy = 1f, float xPos = 0, float yPos = 280)
    {
        ImportDependencies();

        GameObject newAlertComponent = Instantiate(alertComponent);

        newAlertComponent.transform.SetParent(canvasComponent.transform);

        RectTransform newAlertRectTransform = newAlertComponent.GetComponent<RectTransform>();
        if (newAlertRectTransform != null)
            newAlertRectTransform.localPosition = new Vector3(xPos, yPos, 0);        

        TMP_Text newAlertText = newAlertComponent.GetComponent<TMP_Text>();
        if (newAlertText == null)
            return;

        newAlertText.text = text;
        newAlertText.color = color;
        //Destroy(newAlertComponent, timeToDestroy);
        StartCoroutine(HideText(newAlertText, timeToDestroy));
    }

    public void DangerText(string text)
    {
        ShowText(text, Color.red, 1f, 0, 200);
    }

    public void RandomPhrases()
    {
        int randomIndex = Random.Range(0, phrases.Count);
        if(lastPhrasesIndex.Count == 0)
            lastPhrasesIndex.Push(randomIndex);

        while (lastPhrasesIndex.Peek() == randomIndex) //enquanto for igual ao ultimo index da frase mostrada
        {
            randomIndex = Random.Range(0, phrases.Count); //gera outro index
        }
        lastPhrasesIndex.Push(randomIndex);
        ShowText(phrases[randomIndex], Color.cyan, 1.5f);
    }

    IEnumerator HideText(TMP_Text text, float time)
    {
        Color startColor = text.color;
        //Define o alpha como zero para deixar transparente
        startColor.a = 0;

        while(text.color.a > 0)
        {
            text.color = Color.Lerp(text.color, startColor, time * Time.deltaTime);
            yield return null;
        }
    }

    private void Update()
    {
        
    }

    private void ImportDependencies()
    {
        if (canvasComponent == null)
            canvasComponent = GameObject.Find("Canvas");

        if (alertComponent == null)
        {
            alertComponent = Resources.Load<GameObject>("Prefabs/AlertText");
            
            if (alertComponent == null)
                print("Alert text not found!");
        }
    }


}
