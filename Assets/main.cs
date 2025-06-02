using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class main : MonoBehaviour
{

    public Camera cam;
    public Vector3 mp;
    public GameObject square,mineCount,timer,bg,gameOverText,gameCount;
    int[,] mines = new int[18, 18], board=new int[18,18],squareNum = new int[18, 18];
    public int gameProcedure = 0,squareLeft=256,mineLeft=25,gameNum; //0: just reset 1: begun 2: stopped
    //board 0: close, 1: flag, 2: open
    
    public Image img;
    public Sprite[] spr,bgs;

    void InitializeGame(int real)
    {
        if(real==1)
        {
            GetComponent<timer>().timeCheck = 0f;
            gameNum = 0;
        }
        gameOverText.SetActive(false);
        bg.GetComponent<Image>().sprite = bgs[0];
        gameProcedure = 0;
        squareLeft = 256;
        mineLeft=25;
        int i, j;

        if (transform.childCount != 0) 
        {
            for (i = 0; i <= 255; i++)
            {
                img = transform.GetChild(i).GetComponent<Image>();
                img.sprite = spr[11];
                img = transform.GetChild(i).GetChild(0).GetComponent<Image>();
                img.sprite = spr[0];
                img = transform.GetChild(i).GetChild(1).GetComponent<Image>();
                img.sprite = spr[0];
            }
        }
        else
        {
            for (j = 1; j <= 16; j++)
            {
                for (i = 1; i <= 16; i++)
                {
                    Instantiate(square, new Vector3(-450 + 60 * (i - 1), 390 + -60 * (j - 1), 0), Quaternion.identity, transform);
                }
            }
        }
        for (j = 0; j <= 17; j++)
        {
            for (i = 0; i <= 17; i++)
            {
                mines[i, j] = 0;
                board[i, j] = 0;
                squareNum[i, j] = 0;
            }
        }
        
    }

    void StartGame(int index)
    {
        int num = 1, i, j;
        while (num <= 25)
        {
            i = Random.Range(1, 17);
            j = Random.Range(1, 17);
            if (mines[i, j] != 1 && i - 1 + (j - 1) * 16 != index)
            {
                mines[i, j] = 1;
                for (int a=-1; a<=1; a++)
                {
                    for (int b=-1; b<=1; b++)
                    {
                        if (a != 0 || b != 0) squareNum[i+a,j+b]++;
                    }
                }
                num++;
            }
        }
        gameProcedure = 1;
    }

    int CheckAround(int index)
    {
        Debug.Log(index);
        int mineAround = 0;
        for (int i=-1; i<=1; i++)
        {
            for (int j=-1; j<=1; j++)
            {
                if (mines[index % 16 + 1+j, index / 16 + 1+i] == 1) 
                {
                    mineAround++;
                }
            }
        }
        return mineAround;
    }

    void GameOver(int comp)
    {
        gameOverText.SetActive(true);
        gameProcedure = 2;
        if (comp==0)
        {
            gameOverText.transform.GetChild(0).GetComponent<TMP_Text>().text = "지뢰를 찾았습니다.\nR키를 눌러 재시작";
            bg.GetComponent<Image>().sprite = bgs[1];
            for (int i = 0; i <= 255; i++)
            {
                if (mines[i % 16 + 1, i / 16 + 1] == 1)
                {
                    img = transform.GetChild(i).GetChild(0).GetComponent<Image>();
                    img.sprite = spr[10];
                }
            }
        }
        else
        {
            gameNum += 1;
            if (gameNum > 3) gameNum = 3;
            if (gameNum < 3) 
            {
                gameOverText.transform.GetChild(0).GetComponent<TMP_Text>().text = gameNum.ToString() + "번째 게임 완료.\nR키를 눌러 다음 게임 시작";
            }
            else
            {
                gameOverText.transform.GetChild(0).GetComponent<TMP_Text>().text = gameNum.ToString() + "번째 게임 완료.\n최종 기록: "+ GetComponent<timer>().timeCheck.ToString("0.00")+ "초";
            }
            
        }
    }    

    void EditImage(int index)
    {
        img = transform.GetChild(index).GetComponent<Image>();
        if (board[index % 16 + 1, index / 16 + 1] != 2) img.sprite = spr[11];
        else img.sprite = spr[12];
        img = transform.GetChild(index).GetChild(1).GetComponent<Image>();//num or flag
        if (board[index % 16 + 1, index / 16 + 1] == 1) img.sprite = spr[9];
        else if (board[index % 16 + 1, index / 16 + 1] == 2) img.sprite = spr[squareNum[index % 16 + 1, index / 16 + 1]];
        else img.sprite = spr[0];
    }

    void SquareInteraction(int index,int mod)
    {
        if (board[index % 16 + 1, index / 16 + 1] !=2&&gameProcedure==1)
        {
            switch (mod)//0: left, 1: right, 2: auto
            {
                case 0:
                    {
                        if(board[index % 16 + 1, index / 16 + 1] != 1)
                        {
                            if (mines[index % 16 + 1, index / 16 + 1] == 1)
                            {
                                GameOver(0);
                            }
                            else
                            {
                                board[index % 16 + 1, index / 16 + 1] = 2;
                                squareLeft -= 1;
                                int ma = CheckAround(index);
                                if (ma == 0)
                                {
                                    if (index % 16 != 0 && index / 16 != 0) SquareInteraction(index - 17, 0);
                                    if (index / 16 != 0) SquareInteraction(index - 16, 0);
                                    if (index % 16 != 15 && index / 16 != 0) SquareInteraction(index - 15, 0);
                                    if (index % 16 != 0) SquareInteraction(index - 1, 0);
                                    if (index % 16 != 15) SquareInteraction(index + 1, 0);
                                    if (index % 16 != 0 && index / 16 != 15) SquareInteraction(index + 15, 0);
                                    if (index / 16 != 15) SquareInteraction(index + 16, 0);
                                    if (index % 16 != 15 && index / 16 != 15) SquareInteraction(index + 17, 0);
                                }
                                EditImage(index);
                                if (squareLeft == 25) GameOver(1);
                            }
                        }
                        
                        break;
                    }
                case 1:
                    {
                        if (board[index % 16 + 1, index / 16 + 1] != 2)
                        {
                            if(board[index % 16 + 1, index / 16 + 1] == 0)
                            {
                                board[index % 16 + 1, index / 16 + 1] = 1;
                                mineLeft -= 1;
                            }
                            else
                            {
                                board[index % 16 + 1, index / 16 + 1] = 0;
                                mineLeft += 1;
                            }
                        }
                        EditImage(index);
                        break;
                    }
                default:
                    break;
            }
            
        }
        
    }

    void Start()
    {
        //cam = GetComponent<Camera>();
        InitializeGame(1);   
    }

    // Update is called once per frame
    void Update()
    {
        mineCount.GetComponent<TMP_Text>().text = mineLeft.ToString();
        gameCount.GetComponent<TMP_Text>().text = "게임 "+gameNum.ToString()+"/3";
        if (Input.GetMouseButtonDown(0)|| Input.GetMouseButtonDown(1))
        {
            mp = Input.mousePosition;
            mp = cam.ScreenToWorldPoint(mp);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mp, transform.forward, Mathf.Infinity);
            if (hit)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (gameProcedure == 0) StartGame(hit.transform.GetSiblingIndex());
                    SquareInteraction(hit.transform.GetSiblingIndex(), 0);
                }
                else
                {
                    SquareInteraction(hit.transform.GetSiblingIndex(), 1);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) InitializeGame(0);
        if (Input.GetKeyDown(KeyCode.T)) InitializeGame(1);
    }
}
