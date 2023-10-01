using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static bool gameStarted = false;
    public Text firstPrompt;
    public Object Tower;
    public Transform Camera;
    public Transform Thresholds;
    public float delayAnimation;
    public Text scoreText;

    public float timeRemaining;
    public Text timerText;
    public int score, varPlusScore = 50, varMinusScore = 25;
    public Vector3 initialHeight;
    public Vector3 finalHeight;
    Vector3 positionCamera;
    Vector3 positionThresholds;
    public float plusHeight;  //Valor que será aumentado na altura a cada acerto de alinhamento

    //Adiministração das torres criadas em cena
    public List<TowerMovement> towersMov = new List<TowerMovement>();
    bool anyBlockIsActive;

    private void Awake()
    {
        //Pseudo-Singleton, facilita o acesso ao código
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        finalHeight = initialHeight;
        positionCamera = Camera.position;
        positionThresholds = Thresholds.position;
    }

    // Update is called once per frame
    void Update()
    {
        StartGame();
        if( gameStarted ) ClockTime();

        //Administração de torres
        //Caso não houver uma torre ativa, cria-se uma
        foreach(TowerMovement block in towersMov)
        {
            if (block.isActive)
            {
                //Caso houver algum bloco ativo, não há necessidade de criar mais um
                anyBlockIsActive = true;
                Debug.Log("Há torres ativas");
                break;
            }
            anyBlockIsActive = false;
        }

        //Caso nenhum block estiver ativo, está na hora de criar mais um
        if (!anyBlockIsActive)
        {
            Debug.Log("Não há torres ativas");
        }
    }

    public void HeightUpdate(float height)
    {
        finalHeight.y += height;
        fixHeightCamera();
    }
    void StartGame()
    {
        if( !gameStarted && Input.GetKeyDown(KeyCode.Space) )
        {
            gameStarted = true;
            firstPrompt.gameObject.SetActive(false);

            //primeira chamada de uma torre do jogo
            StartCoroutine(NewTower());
        }
    }

    void fixHeightCamera()
    {
        //Adicionar uma transição mais suave na adição de altura
        positionCamera.y += plusHeight;
        Camera.position = positionCamera;

        positionThresholds.y += plusHeight;
        Thresholds.position = positionThresholds;
    }

    public IEnumerator NewTower()
    {
        //Há uma lista feita com os scripts de cada torre lançada na plataforma
        yield return new WaitForSeconds(delayAnimation);
        GameObject towerCreated = Instantiate(Tower, finalHeight, Quaternion.identity) as GameObject;
        TowerMovement towerMov = towerCreated.GetComponent<TowerMovement>();
        towersMov.Add(towerMov);
    }

    void EndGame()
    {
        SceneManager.LoadScene("Menu");
        gameStarted = false;
        towersMov.Clear();
    }
    public void ScoreUp()
    {
        score += varPlusScore;
        scoreText.text = score.ToString();
    }

    public void ScoreDown()
    {
        score -= varMinusScore;
        scoreText.text = score.ToString();
        if (score <= 0)
        {
            score = 0;
            scoreText.text = "000";
        }
    }

    void ClockTime()
    {
        if ( timeRemaining > 0 )
        {
            timeRemaining -= Time.deltaTime;
            if (timerText)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
        else
        {
            EndGame();
        }
    }
}
