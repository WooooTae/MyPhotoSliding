using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform puzzleBoard;
    [SerializeField] private GamePiece piecePrefab;
    [SerializeField] private GameComplete gameComplete;

    private List<GamePiece> pieces;

    private int emptyLocation; // 현재 빈 칸의 '보드판 인덱스'
    private int size;
    private float pieceSize;    // 계산된 조각 크기 저장용
    private float boardSize;    // 보드 크기 저장용

    private bool isShuffling;
    private Texture2D puzzleTexture;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private async void Start()
    {
        await UniTask.Yield();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(puzzleBoard);

        SettingManager.Instance.LoadSettings();
        SetSize(SettingManager.Instance.level);

        pieces = new List<GamePiece>();

        puzzleTexture = MakeSquare(
            MakeReadable(ImageLoader.Instance.GetTexture())
        );

        CreateGamePieces();
        Restart();
    }

    private async UniTaskVoid CheckCompleteLoop()
    {
        while (!isShuffling)
        {
            if (CheckComplete())
            {
                gameComplete.OpenPopup();
                break;
            }

            await UniTask.Yield();
        }
    }

    // =========================
    // IMAGE UTILS
    // =========================
    private Texture2D MakeReadable(Texture2D tex) { /* 기존과 동일 */ return tex; }
    private Texture2D MakeSquare(Texture2D tex) { /* 기존과 동일 */ return tex; }
    public void ApplyTexture(Texture2D texture) { puzzleTexture = texture; }
    private void SetSize(string level)
    {
        size = level switch { "Easy" => 3, "Medium" => 5, "Hard" => 7, _ => 3 };
    }

    // =========================
    // CREATE PIECES
    // =========================
    private void CreateGamePieces()
    {
        boardSize = Mathf.Min(puzzleBoard.rect.width, puzzleBoard.rect.height);
        pieceSize = boardSize / size;
        float uvSize = 1f / size;

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                GamePiece piece = Instantiate(piecePrefab, puzzleBoard);
                pieces.Add(piece);

                int index = row * size + col;
                piece.Index = index; // 원래 가야 할 정답 인덱스(ID)
                piece.name = index.ToString();

                RectTransform rt = piece.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(pieceSize, pieceSize);

                // 초기 위치 설정
                rt.anchoredPosition = GetPositionFromIndex(index);

                if (row == size - 1 && col == size - 1)
                {
                    emptyLocation = index;
                    piece.gameObject.SetActive(false);
                    continue;
                }

                RawImage img = piece.GetComponent<RawImage>();
                if (img != null)
                {
                    img.texture = puzzleTexture;
                    img.uvRect = new Rect(col * uvSize, 1f - ((row + 1) * uvSize), uvSize, uvSize);
                }
            }
        }
    }

    // 인덱스(0 ~ size^2-1)를 바탕으로 UI 좌표를 구하는 헬퍼 함수
    private Vector2 GetPositionFromIndex(int index)
    {
        int row = index / size;
        int col = index % size;
        float startX = -boardSize * 0.5f + pieceSize * 0.5f;
        float startY = boardSize * 0.5f - pieceSize * 0.5f;
        return new Vector2(startX + col * pieceSize, startY - row * pieceSize);
    }

    // =========================
    // MOVE LOGIC
    // =========================
    public void TryMove(int targetBoardIndex)
    {
        if (isShuffling) return; // 셔플 중 클릭 방지

        if (SwapIfValid(targetBoardIndex, -size, size, true)) return;
        if (SwapIfValid(targetBoardIndex, +size, size, true)) return;
        if (SwapIfValid(targetBoardIndex, -1, 0, true)) return;
        if (SwapIfValid(targetBoardIndex, +1, size - 1, true)) return;
    }

    private bool SwapIfValid(int boardIndex, int offset, int colCheck, bool useTween)
    {
        int targetIndex = boardIndex + offset;
        if (targetIndex < 0 || targetIndex >= pieces.Count) return false;

        if (offset == -1 || offset == 1)
        {
            if ((boardIndex % size) == colCheck) return false;
        }

        if (targetIndex == emptyLocation)
        {
            // 1. 데이터(리스트) 상에서 위치 교환
            GamePiece temp = pieces[boardIndex];
            pieces[boardIndex] = pieces[targetIndex];
            pieces[targetIndex] = temp;

            pieces[boardIndex].transform.SetSiblingIndex(boardIndex);
            pieces[targetIndex].transform.SetSiblingIndex(targetIndex);

            // 2. 실제 화면상의 위치 이동 (Tween 또는 즉시 이동)
            RectTransform a = pieces[boardIndex].GetComponent<RectTransform>();
            RectTransform b = pieces[targetIndex].GetComponent<RectTransform>();

            Vector2 posA = GetPositionFromIndex(boardIndex);
            Vector2 posB = GetPositionFromIndex(targetIndex);

            if (useTween)
            {
                a.DOKill();
                b.DOKill();
                a.DOAnchorPos(posA, 0.15f);
                b.DOAnchorPos(posB, 0.15f);
            }
            else
            {
                a.anchoredPosition = posA;
                b.anchoredPosition = posB;
            }

            // 빈 공간의 위치 업데이트
            emptyLocation = boardIndex;
            return true;
        }

        return false;
    }

    // =========================
    // CHECK COMPLETE
    // =========================
    private bool CheckComplete()
    {
        // 리스트의 i번째 칸에 있는 조각의 원래 ID(Index)가 i와 일치하는지 확인
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].Index != i)
                return false;
        }
        return true;
    }

    // =========================
    // RESTART & SHUFFLE
    // =========================
    public void Restart()
    {
        gameComplete.gameObject.SetActive(false);

        isShuffling = true;
        Shuffle();
        isShuffling = false; // 셔플이 끝나면 false로 변경

        CheckCompleteLoop().Forget();
    }

    private void Shuffle()
    {
        int count = 0;
        int lastEmpty = emptyLocation;

        // 무한루프 방지를 위해 최대 시도 횟수 제한 설정
        int maxAttempts = size * size * size * 5;
        int attempts = 0;

        while (count < size * size * size && attempts < maxAttempts)
        {
            attempts++;
            int rnd = Random.Range(0, size * size);

            // 빈 칸 주변의 인덱스인지 계산해서 스왑 시도 (useTween = false)
            if (rnd == emptyLocation) continue;

            if (SwapIfValid(rnd, -size, size, false)) count++;
            else if (SwapIfValid(rnd, size, size, false)) count++;
            else if (SwapIfValid(rnd, -1, 0, false)) count++;
            else if (SwapIfValid(rnd, 1, size - 1, false)) count++;
        }
    }
}

