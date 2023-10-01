using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkComponentController : MonoBehaviour
{
    public int units = 5;
    [SerializeField] private TextMeshPro unitsCounter;
    List<GameObject> connectors = new List<GameObject>();
    List<GameObject> connections = new List<GameObject>();
    public int summonableUnits = 100;
    public int moneyPerMove = 10;

    public int ID;
    public string Ownership = "Neutral";

    public bool isDeffence = false;
    public bool isScout = false;

    public delegate void OnNetworkComponentClick(int currentUnits, List<GameObject> connection, List<GameObject> connector, GameObject thisNetworkComponent, string sideNC);
    public event OnNetworkComponentClick onNetworkComponentClickEvent;
    public delegate void OnUnitsSendingToThis(int currentUnits, GameObject thisObject);
    public event OnUnitsSendingToThis onUnitsSendingToThisEvent;

    private void Start()
    {
        ButtonsControlNC.InitializeNC(this);
        unitsCounter.text = units.ToString();
        OnChangeOwnership();
        foreach (var connector in GameObject.FindGameObjectsWithTag("Connection"))
        {
            float angle, x1About = 0, x2About = 0, y1About = 0 ,y2About = 0;
            if(connector.transform.eulerAngles.z > 180)
            {
                angle = (360-connector.transform.eulerAngles.z) * Mathf.Deg2Rad;
                y1About = Mathf.Sin(angle) * connector.transform.localScale.x / 2 + connector.transform.position.y;
                y2About = -Mathf.Sin(angle) * connector.transform.localScale.x / 2 + connector.transform.position.y;
                x1About = -Mathf.Cos(angle) * connector.transform.localScale.x / 2 + connector.transform.position.x;
                x2About = Mathf.Cos(angle) * connector.transform.localScale.x / 2 + connector.transform.position.x;
            }
            else
            {
                angle = connector.transform.eulerAngles.z * Mathf.Deg2Rad;
                y1About = -Mathf.Sin(angle) * connector.transform.localScale.x / 2 + connector.transform.position.y;
                y2About = Mathf.Sin(angle) * connector.transform.localScale.x / 2 + connector.transform.position.y;
                x1About = -Mathf.Cos(angle) * connector.transform.localScale.x / 2 + connector.transform.position.x;
                x2About = Mathf.Cos(angle) * connector.transform.localScale.x / 2 + connector.transform.position.x;
            }

            if ((new Vector2(transform.position.x, transform.position.y) - new Vector2(x1About, y1About)).magnitude < 1)
            {
                connectors.Add(connector);
                foreach (var connection in GameObject.FindGameObjectsWithTag("NetworkComponent"))
                {
                    if((new Vector2(connection.transform.position.x, connection.transform.position.y) - new Vector2(x2About, y2About)).magnitude < 1 && connection != gameObject)
                    {
                        connections.Add(connection);
                    }
                }
            }   
            else if ((new Vector2(transform.position.x, transform.position.y) - new Vector2(x2About, y2About)).magnitude < 1 )
            {
                connectors.Add(connector);
                foreach (var connection in GameObject.FindGameObjectsWithTag("NetworkComponent"))
                {
                    if ((new Vector2(connection.transform.position.x, connection.transform.position.y) - new Vector2(x1About, y1About)).magnitude < 1 && connection != gameObject)
                    {
                        connections.Add(connection);
                    }
                }
            }
        }
    }
    public void ChangeUnits(int unit, string playerSide)
    {
        units += unit;
        if(playerSide == Ownership || playerSide == "Neutral")
            unitsCounter.text = units.ToString();
        else
            for(int i = 0;i < connections.Count; i++)
            {
                if (connections[i].GetComponent<NetworkComponentController>().isScout)
                    unitsCounter.text = units.ToString();
            }
    }
    public void OnChangeOwnership()
    {
        if(Ownership == "Hacker")
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if(Ownership == "Admin")
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        else if (Ownership == "Neutral")
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    public void ChangeSide(string newSide)
    {
        Ownership = newSide;
        OnChangeOwnership();
    }
    private void OnMouseUp()
    {
        if (gameObject.GetComponent<SpriteRenderer>().color == Color.yellow)
            onUnitsSendingToThisEvent?.Invoke(units, gameObject);
        else
            onNetworkComponentClickEvent?.Invoke(units, connections, connectors, gameObject, Ownership);
    }
    
}
