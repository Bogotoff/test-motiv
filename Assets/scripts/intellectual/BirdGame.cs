using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BirdGame: IntellectualGame
{
    public List<CustomMovableItem> items = new List<CustomMovableItem>();
    public CustomMovableItem targetMovable;

    private GameObject currentItem = null;
    private Vector3 offsetPoint;
    private Vector4 clipRect;
    
    public override void startGame()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("BirdGameItem");

        for (int i = 0; i < objects.Length; i++) {
            items.Add(objects[i].GetComponent<CustomMovableItem>());
        }

        if (items == null || items.Count <= 0 || targetMovable == null) {
            finishGame(false);
        }
    }

    public override bool checkEndOfGame()
    {
        return false;
    }

    public override void saveGameResults()
    {
        if (_countDownTime < 0) {
            _countDownTime = 0;
        }
        
        int totalScore = _countDownTime * 3;
        
        GameData.saveCurrentIntellectualResult(_countDownTime, totalScore);
    }

    public override void step()
    {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = UICamera.currentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                CustomMovableItem item = hit.collider.gameObject.GetComponent<CustomMovableItem>();

                if (item != null && (item.moveX || item.moveY)) {
                    currentItem = item.gameObject;
                    offsetPoint = hit.point - currentItem.transform.position;
                    clipRect = new Vector4(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);

                    Vector3 itemPos = item.transform.position;

                    for (int i = 0; i < items.Count; i++) {
                        if (items[i] != item) {
                            Vector3 pos = items[i].transform.position;

                            if (item.moveX) {
                                if (itemPos.y - item.getHeight() * 0.5f <= pos.y + items[i].getHeight() * 0.5f &&
                                    itemPos.y + item.getHeight() * 0.5f >= pos.y - items[i].getHeight() * 0.5f
                                ) {
                                    if (clipRect.z > pos.x - items[i].getWidth() * 0.5f - item.getWidth() * 0.5f &&
                                        pos.x - items[i].getWidth() * 0.5f > itemPos.x + item.getWidth() * 0.5f
                                    ) {
                                        clipRect.z = pos.x - items[i].getWidth() * 0.5f - item.getWidth() * 0.5f;
                                    } else
                                    if (clipRect.x < pos.x + items[i].getWidth() * 0.5f + item.getWidth() * 0.5f &&
                                        pos.x + items[i].getWidth() * 0.5f < itemPos.x - item.getWidth() * 0.5f
                                    ) {
                                        clipRect.x = pos.x + items[i].getWidth() * 0.5f + item.getWidth() * 0.5f;
                                    }
                                }
                            } else if (item.moveY) {
                                if (itemPos.x - item.getWidth() * 0.5f <= pos.x + items[i].getWidth() * 0.5f &&
                                    itemPos.x + item.getWidth() * 0.5f >= pos.x - items[i].getWidth() * 0.5f
                                ) {
                                    if (clipRect.w > pos.y - items[i].getHeight() * 0.5f - item.getHeight() * 0.5f &&
                                        pos.y - items[i].getHeight() * 0.5f > itemPos.y + item.getHeight() * 0.5f
                                    ) {
                                        clipRect.w = pos.y - items[i].getHeight() * 0.5f - item.getHeight() * 0.5f;
                                    } else 
                                    if (clipRect.y < pos.y + items[i].getHeight() * 0.5f + item.getHeight() * 0.5f &&
                                        pos.y + items[i].getHeight() * 0.5f < itemPos.y - item.getHeight() * 0.5f
                                    ) {
                                        clipRect.y = pos.y + items[i].getHeight() * 0.5f + item.getHeight() * 0.5f;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        } else if (Input.GetMouseButtonUp(0)) {
            currentItem = null;
        }

        if (Input.GetMouseButton(0)) {
            if (currentItem != null) {
                Vector3 mousePos = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 targetPos = mousePos - offsetPoint;
                CustomMovableItem dragItem = currentItem.GetComponent<CustomMovableItem>();

                if (targetPos.x < clipRect.x) {
                    targetPos.x = clipRect.x;
                } else
                if (targetPos.x > clipRect.z) {
                    targetPos.x = clipRect.z;
                }

                if (targetPos.y < clipRect.y) {
                    targetPos.y = clipRect.y;
                } else
                if (targetPos.y > clipRect.w) {
                    targetPos.y = clipRect.w;
                }

                if (!dragItem.moveX) {
                    targetPos.x = currentItem.transform.position.x;
                }

                if (!dragItem.moveY) {
                    targetPos.y = currentItem.transform.position.y;
                }

                currentItem.transform.position = targetPos;
            }
        }
    }

    public void onTrigger(Collider collider)
    {
        if (collider == targetMovable.collider) {
            finishGame(true);
        }
    }
}