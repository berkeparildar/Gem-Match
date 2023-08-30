using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private Vector3 _clickDownPosition;
    private Camera _camera;
    private GameObject _selectedGem;
    public static bool MatchExists;
    public GridOperations gridOperations;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _clickDownPosition = Input.mousePosition;
            _selectedGem = SelectGem();
        }

        var dragDirection = (Input.mousePosition - _clickDownPosition).normalized;
        if (Input.GetMouseButtonUp(0))
        {
            if (dragDirection.x > 0.5f)
            {
                Debug.Log("right swipe");
                if (_selectedGem != null)
                {
                    var position = _selectedGem.transform.position;
                    var currentPosition = new Vector2Int((int)position.x, (int)position.y);
                    var targetPosition = new Vector2Int(currentPosition.x + 1, currentPosition.y);
                    GridOperations.CheckMatchForSelected(currentPosition, targetPosition, true);
                    GridOperations.CheckMatchForSelected(targetPosition, currentPosition, false);
                    if (MatchExists)
                    {
                        MoveGems(currentPosition, targetPosition);
                    }
                    else
                    {
                        FailedMove(currentPosition, targetPosition);
                    }
                }
            }
            else if (dragDirection.x < -0.5f)
            {
                Debug.Log("left swipe");
                if (_selectedGem != null)
                {
                    var position = _selectedGem.transform.position;
                    var currentPosition = new Vector2Int((int)position.x, (int)position.y);
                    var targetPosition = new Vector2Int(currentPosition.x - 1, currentPosition.y);
                    GridOperations.CheckMatchForSelected(currentPosition, targetPosition, true);
                    GridOperations.CheckMatchForSelected(targetPosition, currentPosition, false);
                    if (MatchExists)
                    {
                       MoveGems(currentPosition, targetPosition);
                    }
                    else
                    {
                        FailedMove(currentPosition, targetPosition);
                    }
                }
            }
            else if (dragDirection.y > 0.5f)
            {
                Debug.Log("up swipe");
                if (_selectedGem != null)
                {
                    var position = _selectedGem.transform.position;
                    var currentPosition = new Vector2Int((int)position.x, (int)position.y);
                    var targetPosition = new Vector2Int(currentPosition.x, currentPosition.y + 1);
                    GridOperations.CheckMatchForSelected(currentPosition, targetPosition, true);
                    GridOperations.CheckMatchForSelected(targetPosition, currentPosition, false);
                    if (MatchExists)
                    {
                        MoveGems(currentPosition, targetPosition);
                    }
                    else
                    {
                        FailedMove(currentPosition, targetPosition);
                    }
                }
            }
            else if (dragDirection.y < -0.5f)
            {
                Debug.Log("down swipe");
                if (_selectedGem != null)
                {
                    var position = _selectedGem.transform.position;
                    var currentPosition = new Vector2Int((int)position.x, (int)position.y);
                    var targetPosition = new Vector2Int(currentPosition.x, currentPosition.y - 1);
                    GridOperations.CheckMatchForSelected(currentPosition, targetPosition, true);
                    GridOperations.CheckMatchForSelected(targetPosition, currentPosition, false);
                    if (MatchExists)
                    {
                        MoveGems(currentPosition, targetPosition);
                    }
                    else
                    {
                        FailedMove(currentPosition, targetPosition);
                    }
                }
            }
        }
    }

    private GameObject SelectGem()
    {
        var hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.transform != null && hit.transform.tag is "Square" or "Diamond" or "Hexagon" or "Star")
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
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
            GridOperations.MoveGemsAfterRemoval();
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
    }
}
