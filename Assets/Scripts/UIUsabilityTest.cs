using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BezierCurveExtrusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRQuestionnaireToolkit;
using VRSketchingGeometry.BezierSurfaceTool;
using VRSketchingGeometry.SketchObjectManagement;
using Debug = UnityEngine.Debug;

public class UIUsabilityTest : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<Sprite> OnImageChanged;
    [SerializeField]
    private TextMeshProUGUI mainTitle;
    [SerializeField]
    private TextMeshProUGUI mainText;
    [SerializeField]
    private Image mainImage;
    [SerializeField]
    private Sprite tulip;
    [SerializeField]
    private Sprite vase;
    [SerializeField]
    private Sprite sculpture;
    [SerializeField]
    private Sprite sombrero;
    [SerializeField]
    private GameObject vrQuestionnaireToolkit;
    [SerializeField]
    private BezierSurfaceToolControllerActions controllerScript;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Camera vrCamera;

    private int _step;
    private int _part;
    private int _task;
    private int _taskStep;
    private Stopwatch _stopWatch;
    private string outputPath;
    private int _drawSurfaceClickCounter;
    private SketchWorld sketchWorld;
    private TextMeshProUGUI nextButtonText;

    // Start is called before the first frame update
    void Start()
    {
        mainTitle.text = "Willkommen!";
        mainText.text = "Danke für dein Interesse an diesem Usability Test. Der Test umfässt drei Aufgaben, die mit " +
                        "drei verschiedenen Interaktionstechniken zu lösen sind. Bitte nehme dir vor Beginn jeder " +
                        "Aufgabe bis zu 10 Minuten Zeit, um dich mit der jeweiligen Interaktionstechnik vertraut zu " +
                        "machen. Nach Abschluss jeder Aufgabe mit der jeweiligen Interaktionstechnik werden sechs " +
                        "Fragen gestellt. \nInsgesamt dauert der Usability Test in etwa " +
                        "1 Stunde und 30 Minuten. \n\nViel Spass und viel Erfolg!";
        
        outputPath = System.IO.Path.Combine(Application.dataPath, "UsabilityTestData\\usability-test.log");
        using (StreamWriter sw = File.AppendText(outputPath))
        {
            sw.WriteLine("Unity start");
        }

        _drawSurfaceClickCounter = 0;

        nextButtonText = nextButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNextPage()
    {
        string variation = "";
        Sprite sprite = null;
        string imageName = "";
        
        if (_part == 3)
        { 
            _step = 4;
        }
        else
        {
            if (_task == 0)
            {
                _step = 1;
                _task++;
                _taskStep = 0;
            }
            else
            {
                if ((_task == 1 || _task == 2 || _task == 3)  && _taskStep == 0)
                {
                    _step = 2;
                    _taskStep++;
                }
                else
                {
                    if ((_task == 1 || _task == 2 || _task == 3) && _taskStep == 1)
                    {
                        _step = 3;
                    }
                }
            }
        }
        
        switch (_part)
        {
            case 0:
                variation = "A";
                controllerScript.SetStartInteractionMethod(BezierCurveExtruder.InteractionMethod.Simple);
                break;
            case 1:
                variation = "B";
                controllerScript.SetStartInteractionMethod(BezierCurveExtruder.InteractionMethod.VectorAngle);
                break;
            case 2:
                variation = "C";
                controllerScript.SetStartInteractionMethod(BezierCurveExtruder.InteractionMethod.Distance);
                break;
        }
        
        switch (_task)
        {
            case 1:
                imageName = "Skulptur";
                sprite = sculpture;
                break;
            case 2:
                imageName = "Sombrero";
                sprite = sombrero;
                break;
            case 3:
                imageName = "Vase";
                sprite = vase;
                break;
        }


        switch (_step)
        {
            case 1:
                // enable this canvas
                GetComponent<Canvas>().enabled = true;
                
                mainTitle.text = "Interaktionstechnik " + variation + " - Vorbereitung";
                mainText.text = "Mache dich mit der Interaktionstechnik in bis zu 10 Minuten vertraut. Versuche wenn " + 
                                "möglich die Tulpe zu zeichnen, die rechts eingeblendet ist. Wenn du dich bereit fühlst, " + 
                                "kannst du mit der nächsten Aufgabe fortfahren.\n\n Info: Die Tulpe ist sehr schwer. Die nächsten " + 
                                "Aufgaben werden leichter.";
                mainImage.sprite = tulip;
                OnImageChanged.Invoke(tulip);
                sketchWorld = controllerScript.CreateNewSketchWorld();
                nextButtonText.text = "Vorbereitung beenden";
                nextButtonText.fontSize = 16f;
                _stopWatch = new Stopwatch();
                _stopWatch.Start();
                break;
            case 2:
                // enable this canvas
                GetComponent<Canvas>().enabled = true;
                
                mainTitle.text = "Interaktionstechnik " + variation + " - Aufgabe " + _task;
                mainText.text = "Zeichne in den nächsten 5 Minuten die rechts dargestellte " + imageName + ". Wenn du fertig " + 
                                "bist, drücke auf 'Aufgabe beenden', um die Fragen zu beantworten.\n\n Info: Vergiss nicht, dass" + 
                                "du auch die Kurvenstärke justieren kannst.";
                mainImage.sprite = sprite;
                OnImageChanged.Invoke(sprite);
                nextButtonText.fontSize = 16f;
                nextButtonText.text = "Aufgabe beenden";
                using (StreamWriter sw = File.AppendText(outputPath))
                {
                    sw.WriteLine("Interaktionstechnik " + variation + " - Aufgabe " + _task);
                }
                if (_task == 1)
                {
                    _stopWatch.Stop();
                    TimeSpan ts = _stopWatch.Elapsed;
                    using (StreamWriter sw = File.AppendText(outputPath))
                    {
                        sw.WriteLine("Vorbereitungszeit: " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10));
                    }
                }
                _stopWatch = new Stopwatch();
                _stopWatch.Start();
                _drawSurfaceClickCounter = 0;
                sketchWorld = controllerScript.CreateNewSketchWorld();
                break;
            case 3:
                // abspeichern von Daten
                _stopWatch.Stop();
                TimeSpan ts2 = _stopWatch.Elapsed;
                using (StreamWriter sw = File.AppendText(outputPath))
                {
                    sw.WriteLine("Bearbeitungszeit: " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts2.Hours, ts2.Minutes, ts2.Seconds,
                        ts2.Milliseconds / 10));
                    sw.WriteLine("Draw surface clicks: " + _drawSurfaceClickCounter);
                }
                
                // abspeichern von sketchworld
                string savePath = System.IO.Path.Combine(Application.dataPath, "UsabilityTestData\\Interaktionstechnik_" + variation + "_-_Aufgabe_" + _task + ".xml");
                sketchWorld.SaveSketchWorld(savePath);
                sketchWorld = controllerScript.CreateNewSketchWorld();

                // questionaire starten
                GameObject qToolkit = Instantiate(vrQuestionnaireToolkit);
                qToolkit.GetComponent<RectTransform>().position = new Vector3(-6.2f, -0.04f, 5.86f);
                qToolkit.GetComponent<Canvas>().worldCamera = vrCamera;
                ExportToCSV exportToCsv = qToolkit.transform.GetChild(0).gameObject.GetComponent<ExportToCSV>();
                exportToCsv.QuestionnaireFinishedEvent.AddListener(() => OnQuestionnaireEnds(qToolkit));
                exportToCsv.UseGlobalPath = true;
                exportToCsv.StorePath = System.IO.Path.Combine(Application.dataPath, "UsabilityTestData\\");
                exportToCsv.FileName = "Interaktionstechnik_" + variation + "_-_Aufgabe_" + _task;
                
                // disable this canvas
                GetComponent<Canvas>().enabled = false;
                break;
            case 4:
                // enable this canvas
                GetComponent<Canvas>().enabled = true;
                
                mainTitle.text = "Ende!";
                mainText.text = "Der Usability Test ist nun vorbei. Vielen Dank für deine Teilnahme!";
                mainImage.sprite = null;
                nextButton.gameObject.SetActive(false);
                break;
        }
    }

    private void OnQuestionnaireEnds(GameObject questionnaire)
    {
        // adjust control logic
        if (_task == 3)
        {
            _part++;
            _task = 0;
        }
        else
        {
            _task++;
            _taskStep = 0;
        }
        Destroy(questionnaire);
        OnNextPage();
    }

    public void OnDrawSurfaceClick()
    {
        _drawSurfaceClickCounter++;
    }
}
