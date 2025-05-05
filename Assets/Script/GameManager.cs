using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform gameTransform;
    [SerializeField]
    private Transform piecePrefab;

    private List<Transform> pieces;
    private string level;
    private int emptyLocation;
    private int size;

    private bool isShuffling;

    private void SetSize(string levelstr)
    {
      switch (levelstr)
        {
            case "easy":
                size = 3;
                break;
            case "medium":
                size = 5;
                break;
            case "hard":
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
                    float gap = gapThickness / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];

                    uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
                    uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
                    uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
                    uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));

                    mesh.uv = uv;
                }
            }
        }
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

    IEnumerator WaitSuffle(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Shuffle();
        isShuffling = false;
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

    private void Start()
    {
        SetSize("easy");
        pieces = new List<Transform>();
        CreateGamePieces(0.02f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                for (int i = 0; i < pieces.Count; i++)
                { 
                    if (pieces[i] == hit.transform)
                    {

                        if (SwapIfValid(i,-size,size))
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

        if (!isShuffling && CheckComplete())
        {
            isShuffling = true;
            StartCoroutine(WaitSuffle(0.5f));
        }
    }
}
