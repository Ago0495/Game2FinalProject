using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using UnityEditor;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using TMPro;

public class GameMaster : NetworkComponent
{
    //sync vars
    [SerializeField] private bool gameStarted;
    [SerializeField] private bool gameFinished;
    private int score;

    //non-sync vars

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "GAMESTART")
        {
            GameObject lobbyPlayerCanvas = GameObject.Find("LobbyPlayerCanvas");
            if (lobbyPlayerCanvas == null)
            {
                throw new System.Exception("ERROR: Could not find LobbyPlayerCanvas on the scene.");
            }
            else
            {
                lobbyPlayerCanvas.SetActive(false);
            }

            if (IsServer)
            {
            }
        }

        if (flag == "ENDGAME")
        {
            gameFinished = true;

            GameObject lobbyPlayerCanvas = GameObject.Find("ScoreCanvas");
            if (lobbyPlayerCanvas == null)
            {
                throw new System.Exception("ERROR: Could not find ScoreCanvas on the scene.");
            }
            else
            {
                lobbyPlayerCanvas.GetComponent<Canvas>().enabled = true;
                lobbyPlayerCanvas.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Score: " + score;
            }
        }

        if (flag == "SCORE")
        {
            Debug.Log(value);
            score = int.Parse(value);
        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        if (IsServer)
        {
            PlayerPanelOptions[] players;

            do
            {
                gameStarted = true;
                players = FindObjectsByType<PlayerPanelOptions>(FindObjectsSortMode.None);

                foreach (PlayerPanelOptions player in players)
                {
                    if (!player.GetIsReady())
                    {
                        gameStarted = false;
                    }
                }

                yield return new WaitForSeconds(MyCore.MasterTimer);

            } while (!gameStarted || players.Length < 2);

            Debug.Log("ALL PLAYERS READY");

            foreach (PlayerPanelOptions player in players)
            {
                //spawn player's chosen character
                GameObject tempPlayer = MyCore.NetCreateObject(player.GetSkillSelection(), player.Owner, Vector3.zero, Quaternion.identity);
            }

            SendUpdate("GAMESTART", "1");

            score = Random.Range(10, 1000);
            SendUpdate("SCORE", score.ToString());

            MyCore.NotifyGameStart();

            while (!gameFinished)
            {
                yield return new WaitForSeconds(60);
                SendUpdate("ENDGAME", "1");

                yield return new WaitForSeconds(5);
                StartCoroutine(MyCore.DisconnectServer());
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartGame()
    {

    }
}
