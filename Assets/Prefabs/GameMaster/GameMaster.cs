using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using UnityEditor;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.PlayerSettings;

public class GameMaster : NetworkComponent
{
    //sync vars
    [SerializeField] private bool gameStarted = false;
    [SerializeField] private bool gameFinished = false;
    [SerializeField] private bool allPlayersReady = false;
    private int score;
    public int numcoinchestscollected;

    //non-sync vars

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "GAMESTART")
        {
            gameStarted = true;
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
                allPlayersReady = true;
                players = FindObjectsByType<PlayerPanelOptions>(FindObjectsSortMode.None);

                foreach (PlayerPanelOptions player in players)
                {
                    if (!player.GetIsReady())
                    {
                        allPlayersReady = false;
                    }
                }

                yield return new WaitForSeconds(MyCore.MasterTimer);

            } while (!allPlayersReady || players.Length < 2);

            Debug.Log("ALL PLAYERS READY");

            foreach (PlayerPanelOptions player in players)
            {
                //spawn player's chosen character
                GameObject tempPlayer = MyCore.NetCreateObject(player.GetSkillSelection(), player.Owner, Vector3.up * 20, Quaternion.identity);
            }

            gameStarted = true;
            SendUpdate("GAMESTART", "1");

            numcoinchestscollected = 0;

            Vector3 chestSpawnPos = new Vector3(1f, 1f, 14f);

            Debug.Log("Spawning chest at " + chestSpawnPos);
            GameObject chest = MyCore.NetCreateObject(9, -1, chestSpawnPos, Quaternion.identity);

            score = Random.Range(10, 1000);
            SendUpdate("SCORE", score.ToString());

            MyCore.NotifyGameStart();

            while (!gameFinished)
            {
                float timer = 0;
                while ((numcoinchestscollected < 1) && timer < 10)
                {
                    yield return new WaitForSeconds(1);
                    timer++;
                }
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

    public bool GetGameStarted()
    {
        return gameStarted;
    }
    public void AddScore(int points)
    {
        score += points;
        if (IsServer)
        {
            SendUpdate("SCORE", score.ToString());
            Debug.Log("Score Updated: " + score);
        }
    }
}
