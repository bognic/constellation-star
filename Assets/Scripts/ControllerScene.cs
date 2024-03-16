using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ControllerSceneUI : MonoBehaviour
{
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject ButtonGridElementPrefab;
    [SerializeField] private GameObject SteeringWheelGridElementPrefab;

    private ControllerLayout _layout;

    private Vector3 _firstTouchEventPos = Vector3.zero;
    private Vector3 _lastTouchEventPos = Vector3.zero;
    public Action<Vector3, Vector3> TouchTriangleEvent { get; set; }

    void Start()
    {
        NetworkClientController.Instance.OnDisconnected += OnExitButtonPressed;

        print("Controller JSON data is: " + NetworkClientController.Instance._rawControllerLayoutData);
        _layout = new()
        {
            GridSize = new Vector2Int(10, 5),
            GridElements = new List<GridElement>()
            {
                new ButtonGridElement(0, new Vector2Int(8, 1), "fram"),
                new ButtonGridElement(1, new Vector2Int(8, 3), "bak"),
                new SteeringWheelGridElement(2, new Vector2Int(3, 2), File.ReadAllBytes(Path.Combine(Application.dataPath, "Textures", "steering.png")))
            }
        };
        float gridXSize = Screen.width / _layout.GridSize.x;
        float gridYSize = Screen.height / _layout.GridSize.y;

        foreach (GridElement element in _layout.GridElements)
        {
            element.NetworkMessageRequest += OnNetworkMessageRequest;

            if (element is ButtonGridElement)
            {
                GameObject button = Instantiate(ButtonGridElementPrefab);
                button.transform.SetParent(Panel.transform, false);
                button.transform.localPosition = new Vector3(element.Position.x * gridXSize, -element.Position.y * gridYSize) - new Vector3(Screen.width / 2, -Screen.height / 2);
                button.GetComponent<UnityEngine.UI.Button>().GetComponent<RectTransform>().sizeDelta = new Vector2(element.Size.x * gridXSize, element.Size.y * gridYSize);
                button.GetComponent<UnityEngine.UI.Button>().transform.GetChild(0).GetComponent<TMP_Text>().text = ((ButtonGridElement)element).Legend;
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ((ButtonGridElement)element).OnUserInteraction());
            }

            if (element is SteeringWheelGridElement)
            {
                GameObject steeringWheel = Instantiate(SteeringWheelGridElementPrefab);
                steeringWheel.transform.SetParent(Panel.transform, false);
                steeringWheel.transform.localPosition = new Vector3(element.Position.x * gridXSize, -element.Position.y * gridYSize) - new Vector3(Screen.width / 2, -Screen.height / 2);
                //steeringWheel.transform.localPosition += new Vector3(gridXSize / 2, -gridYSize / 2) * element.Size.magnitude / 2;
                steeringWheel.GetComponent<RawImage>().texture = ((SteeringWheelGridElement)element).Texture;
                steeringWheel.GetComponent<RawImage>().GetComponent<RectTransform>().sizeDelta = new Vector2(element.Size.x * gridXSize, element.Size.y * gridYSize);
            }
        }
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _firstTouchEventPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Stationary)
            {
                _lastTouchEventPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _firstTouchEventPos = Vector3.zero;
                _lastTouchEventPos = Vector3.zero;
            }
            TouchTriangleEvent.Invoke(_firstTouchEventPos, _lastTouchEventPos);
        }
    }

    private void OnNetworkMessageRequest(string message)
    {
        NetworkClientController.Instance.SendInputToServer(message);
    }

    private void OnExitButtonPressed()
    {
        Destroy(NetworkClientController.Instance.gameObject);
        SceneManager.LoadScene("Scenes/ClientScene");
    }
}
