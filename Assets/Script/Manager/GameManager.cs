using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform gameTransform;

    [SerializeField]
    private Transform piecePrefab;

    [SerializeField]
    private GameComplete gameComplete;

    private List<Transform> pieces;
    private string level;
    private int emptyLocation;
    private int size;

    private bool isShuffling;

    public static GameManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
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

    private void Start()
    {
        SettingManager.Instance.LoadSettings();
        SetSize(SettingManager.Instance.level);
        pieces = new List<Transform>();
        CreateGamePieces(0.02f);

        isShuffling = true;
        Restart();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SoundManager.Instance.PlayClickSound(SettingManager.Instance.sfxSlider.value);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {

                        if (SwapIfValid(i, -size, size))
                        {
                            break;
                        }
                        if (SwapIfValid(i, +size, size))
                        {
                            break;
                        }
                        if (SwapIfValid(i, -1, 0))
                        {
                            break;
                        }
                        if (SwapIfValid(i, +1, size - 1))
                        {
                            break;
                        }
                    }
                }
            }
        }

        if (isShuffling && CheckComplete())
        {
            gameComplete.OpenPopup();
            isShuffling = true;
        }
    }

    private void SetSize(string levelstr)
    {
      switch (levelstr)
        {
            case "Easy":
                size = 3;
                break;
            case "Medium":
                size = 5;
                break;
            case "Hard":
                size = 7;
                break;                                        
        }
    }

    private void CreateGamePieces(float gapThickness)
    {
        float width = 1 / (float)size;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Transform piece = Instantiate(piecePrefab, gameTransform);
                pieces.Add(piece);

                piece.localPosition = new Vector3(-1 + (2 * width * col) + width,
                                                  +1 - (2 * width * row) - width,0);

                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * size) + col}";

                //empty bottom right
                if ((row == size - 1) && (col == size - 1))
                {
                    emptyLocation = (size * size) -1;
                    piece.gameObject.SetActive(false);
                }
                else
                {
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];

                    uv[0] = new Vector2((width * col), 1 - ((width * (row + 1))));
                    uv[1] = new Vector2((width * (col + 1)), 1 - ((width * (row + 1))));
                    uv[2] = new Vector2((width * col), 1 - ((width * row)));
                    uv[3] = new Vector2((width * (col + 1)), 1 - ((width * row)));

                    mesh.uv = uv;
                }
            }
        }
    }

    private void SwapPiece(int idx, int offset)
    {
        Transform temp = pieces[idx];
        pieces[idx] = pieces[idx + offset];
        pieces[idx + offset] = temp;

        Vector3 tmepPos = pieces[idx].localPosition;
        pieces[idx].DOLocalMove(pieces[idx + offset].localPosition, 0.25f);
        pieces[idx + offset].DOLocalMove(tmepPos, 0.25f);
    }

    private bool SwapIfValid(int i, int offset, int colCheck)
    {
        if (((i % size) != colCheck) && ((i + offset) == emptyLocation))
        {
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);

            (pieces[i].localPosition, pieces[i + offset].localPosition) = ((pieces[i + offset].localPosition, pieces[i].localPosition));

            emptyLocation = i;
            return true;
        }
        return false;      
    }

    private bool CheckComplete()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].name != $"{i}")
            {
                return false;
            }
        }
        return true;
    }

    public void Restart()
    {
        gameComplete.gameObject.SetActive(false);
        Shuffle();
        isShuffling = true;
    }

    private void Shuffle()
    {
        int count =0;
        int last = 0;
        
        while (count < (size * size * size))
        {
            int rnd = Random.Range(0, size * size);

            if (rnd == last)
            {
                continue;
            }

            last = emptyLocation;

            if (SwapIfValid(rnd,-size,size))
            {
                count++;
            }
            else if (SwapIfValid(rnd,size,size))
            {
                count++;
            }
            else if (SwapIfValid(rnd,-1,0))
            {
                count++;
            }
            else if (SwapIfValid(rnd,1,size - 1))
            {
                count++;
            }
        }
    }
}
