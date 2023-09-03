using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 _clickDownPosition;
    private Camera _camera;
    private GameObject _selectedGem;
    public static bool MatchExists;
    public GridOperations gridOperations;
    private  bool _hasPlayed;
    public AudioSource audioSource;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.CanPlay && !_hasPlayed)
        {
            SelectGem();
        }
    }
    
    private void SelectGem()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _clickDownPosition = touch.position;
                var hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
                if (hit.transform != null && hit.transform.tag is "Square" or "Diamond" or "Hexagon" or "Luna" or "Circle" or "Heart")
                {
                    _selectedGem = hit.transform.gameObject;
                }
            }
            else if (touch.phase == TouchPhase.Ended && _selectedGem != null)
            {
                Vector3 currentPos = touch.position;
                var dragDirection = (currentPos - _clickDownPosition).normalized;
                CheckAndMove(dragDirection);
            }
        }
    }
    
    private void CheckAndMove(Vector3 dragDirection)
    {
        _hasPlayed = true;
        var position = _selectedGem.transform.position;
        Vector2Int targetPosition = default;
        var currentPosition = new Vector2Int((int)position.x, (int)position.y);
        if (dragDirection.x > 0.5f)
        {
            targetPosition = new Vector2Int(currentPosition.x + 1, currentPosition.y);
        }
        else if (dragDirection.x < -0.5f)
        {
            targetPosition = new Vector2Int(currentPosition.x - 1, currentPosition.y);
        }
        else if (dragDirection.y > 0.5f)
        {
            targetPosition = new Vector2Int(currentPosition.x, currentPosition.y + 1);
        }
        else if (dragDirection.y < -0.5f)
        {
            targetPosition = new Vector2Int(currentPosition.x, currentPosition.y - 1);
        }
        gridOperations.CheckMatchForSelected(currentPosition, targetPosition, true);
        gridOperations.CheckMatchForSelected(targetPosition, currentPosition, false);
        if (MatchExists)
        {
            MoveGems(currentPosition, targetPosition);
        }
        else
        {
            FailedMove(currentPosition, targetPosition);
        }
    }
    
    private void MoveGems(Vector2Int selectedGemPosition, Vector2Int targetGemPosition)
    {
        var selectedGem = GameManager.Grid[selectedGemPosition.x, selectedGemPosition.y];
        var targetGem = GameManager.Grid[targetGemPosition.x, targetGemPosition.y];
        GameManager.Grid[selectedGemPosition.x, selectedGemPosition.y] = targetGem;
        targetGem.name = "(" + selectedGemPosition.x + "," + selectedGemPosition.y + ")";
        selectedGem.name = "(" + targetGemPosition.x + "," + targetGemPosition.y + ")";
        GameManager.Grid[targetGemPosition.x, targetGemPosition.y] = selectedGem;
        selectedGem.transform.DOMove(new Vector3(targetGemPosition.x, targetGemPosition.y, 0), 0.5f);
        targetGem.transform.DOMove(new Vector3(selectedGemPosition.x, selectedGemPosition.y, 0), 0.5f).OnComplete(() =>
        {
            gridOperations.RemoveGems(GridOperations.SelectedMatchHorizontal);
            gridOperations.RemoveGems(GridOperations.SelectedMatchVertical);
            gridOperations.RemoveGems(GridOperations.TargetMatchHorizontal);
            gridOperations.RemoveGems(GridOperations.TargetMatchVertical);
            gridOperations.MoveGemsAfterRemoval();
            StartCoroutine(CheckIfMatchExists());
            audioSource.Play();
        });
        MatchExists = false;
    }

    private void FailedMove(Vector2Int selectedGemPosition, Vector2Int targetGemPosition)
    {
        var selectedGem = GameManager.Grid[selectedGemPosition.x, selectedGemPosition.y];
        var targetGem = GameManager.Grid[targetGemPosition.x, targetGemPosition.y];
        selectedGem.transform.DOMove(new Vector3(targetGemPosition.x, targetGemPosition.y, 0), 0.5f).OnComplete(() =>
        {
            selectedGem.transform.DOMove(new Vector3(selectedGemPosition.x, selectedGemPosition.y), 0.5f);
        });
        targetGem.transform.DOMove(new Vector3(selectedGemPosition.x, selectedGemPosition.y, 0), 0.5f).OnComplete(() =>
        {
            targetGem.transform.DOMove(new Vector3(targetGemPosition.x, targetGemPosition.y, 0), 0.5f);
        });
        MatchExists = false;
        _hasPlayed = false;
    }

    private IEnumerator CheckIfMatchExists()
    {
        yield return new WaitForSeconds(1.0f);
        for (var i = 0; i < GameManager.Rows; i++)
        {
            for (var j = 0; j < GameManager.Columns; j++)
            {
                CheckForMatches(j, i);
            }
        }

        if (GridOperations.RemovalGems.Count > 0)
        {
            StartCoroutine(CheckForMatchesAfterRemoval());
            yield break;
        }
        _hasPlayed = false;
    }
    
    private void CheckForMatches(int col, int row)
    {
        var currentGem = GameManager.Grid[col, row];
        var horizontalMatches = 1;
        for (var c = col - 1; c >= 0; c--)
        {
            var adjacentGem = GameManager.Grid[c, row].gameObject;
            if (adjacentGem.CompareTag(currentGem.tag))
            {
                horizontalMatches++;
            }
            else
            {
                break;
            }
        }

        for (var c = col + 1; c < GameManager.Columns; c++)
        {
            var adjacentGem = GameManager.Grid[c, row].gameObject;
            if (adjacentGem.CompareTag(currentGem.tag))
            {
                horizontalMatches++;
            }
            else
            {
                break;
            }
        }
        
        var verticalMatches = 1;
        for (var r = row - 1; r >= 0; r--)
        {
            var adjacentGem = GameManager.Grid[col, r].gameObject;
            if (adjacentGem.CompareTag(currentGem.tag))
            {
                verticalMatches++;
            }
            else
            {
                break;
            }
        }

        for (var r = row + 1; r < GameManager.Rows; r++)
        {
            var adjacentGem = GameManager.Grid[col, r].gameObject;
            if (adjacentGem.CompareTag(currentGem.tag))
            {
                verticalMatches++;
            }
            else
            {
                break;
            }
        }

        if (horizontalMatches >= 3 || verticalMatches >= 3)
        {
            if (!GridOperations.RemovalGems.Contains(currentGem))
            {
                GridOperations.RemovalGems.Add(currentGem);
            }
        }
    }
    
    private IEnumerator CheckForMatchesAfterRemoval()
    {
        gridOperations.RemoveGems(GridOperations.RemovalGems);
        audioSource.Play();
        gridOperations.MoveGemsAfterRemoval();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CheckIfMatchExists());
    }
}
