//using Checkers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Checkers {
    public struct Position { public int x; public int y; }
    public struct CellData {
        //public string name;
        public float MoveStage { get; set; }
        public ColorType _color;
        public GameObject _chip_obj;
        public GameObject _cell_obj;
    
    }
    public class TheGameEngine : MonoBehaviour
    {
        [SerializeField] GameObject _camera_sphere;
        private float _camera_angle;
        [SerializeField]
        private float _camera_rotation_speed = 299;
        [SerializeField]
        private Material _selected_material;
        [SerializeField]
        private Material _highlight_material;
        public Material GetSelectedMaterial { get => _selected_material; }
        public Material GetHighlihtMaterial { get => _highlight_material; }

        public event Action<int, int> OnSelectCell;
        public event Action<int, int> OnMakeTurn;

        List<Position> possible_turn = new List<Position>();
        List<Position> Pos2Del = new List<Position>();
        private ColorType WhosTurn = ColorType.White;
        private Position CurrentSelected;
        internal CellData[][] GameData = new CellData[8][];

        private void Awake() {
            for (int i = 0; i < 8; i++) {
                GameData[i] = new CellData[8];
            }
        }

        private void Start() {

            StartCoroutine(CheckIfGameOver());
            StartCoroutine(MoveChips());
        }
        private IEnumerator MoveChips() {
            while (true) {
                for (var row = 0; row < GameData.Length; row++) {
                    for (var col = 0; col < GameData[row].Length; col++) { 
                        if (GameData[row][col]._chip_obj != null && GameData[row][col]._cell_obj != null && GameData[row][col].MoveStage < 1) {
                            GameData[row][col].MoveStage += Time.deltaTime;
                            GameData[row][col]._chip_obj.transform.position = Vector3.Lerp(GameData[row][col]._chip_obj.transform.position, GameData[row][col]._cell_obj.transform.position, GameData[row][col].MoveStage);
                            GameData[row][col]._chip_obj.transform.position = new Vector3(GameData[row][col]._chip_obj.transform.position.x, 0.15f, GameData[row][col]._chip_obj.transform.position.z);
                        }
                    }
                }
                if (WhosTurn == ColorType.Black && _camera_angle < 180) { _camera_angle += Time.deltaTime * _camera_rotation_speed; }
                if (WhosTurn == ColorType.White && _camera_angle > 0) { _camera_angle -= Time.deltaTime * _camera_rotation_speed; }
                _camera_sphere.transform.localRotation = Quaternion.Euler(0, _camera_angle, 0);
                yield return new WaitForSeconds(0.01f);
            }
        }

        private void ClearSelection() {
            possible_turn.Clear();
            Pos2Del.Clear();
            CurrentSelected = new Position { x = -1, y = -1 };
            for (int row = 0; row < GameData.Length; row++) {
                for (int col = 0; col < GameData[row].Length; col++) {
                    if (GameData[row][col]._cell_obj) {
                        GameData[row][col]._cell_obj.GetComponent<CellComponent>().RemoveAdditionalMaterial(2);
                    }
                }
            }
        }

        public void SelectCell(int x, int y) {

            ClearSelection();
            OnSelectCell?.Invoke(x, y);

            List<Position> directions = new List<Position>();

            void SetCurrentSelected() {
                CurrentSelected = new Position { x = x, y = y };
                GameData[y][x]._cell_obj.GetComponent<CellComponent>().AddAdditionalMaterial(GetSelectedMaterial, 2);
            }

            if (WhosTurn == ColorType.White && GameData[y][x]._color == ColorType.White) {
                SetCurrentSelected();
                directions.Add(new Position { x = 1, y = 1 });
                directions.Add(new Position { x = -1, y = 1 });
            }
            if (WhosTurn == ColorType.Black && GameData[y][x]._color == ColorType.Black) {
                SetCurrentSelected();
                directions.Add(new Position { x = 1, y = -1 });
                directions.Add(new Position { x = -1, y = -1 });
            }

            foreach (var direction in directions) {
                var checkPos = new Position { x = x + direction.x, y = y + direction.y };
                if (checkPos.y < 8 && checkPos.x < 8 && checkPos.y >= 0 && checkPos.x >= 0) {
                    if (GameData[checkPos.y][checkPos.x]._chip_obj == null) {
                        possible_turn.Add(checkPos);
                        Pos2Del.Add(new Position { x = -1, y = -1});
                    } else {
                        var checkPosNext = new Position { x = checkPos.x + direction.x, y = checkPos.y + direction.y };
                        if (checkPosNext.y < 8 && checkPosNext.x < 8 && checkPosNext.y >= 0 && checkPosNext.x >= 0 && GameData[checkPosNext.y][checkPosNext.x]._chip_obj == null) {
                            if(GameData[CurrentSelected.y][CurrentSelected.x]._color != GameData[checkPos.y][checkPos.x]._color) {
                                possible_turn.Add(checkPosNext);
                                Pos2Del.Add(checkPos);
                            }
                        }
                    }
                }
            }
            foreach (var item in possible_turn) {
                    GameData[item.y][item.x]._cell_obj.GetComponent<CellComponent>().AddAdditionalMaterial(GetSelectedMaterial, 2);
            }

        } // end of select cell

        private IEnumerator CheckIfGameOver() {
            while (true) {
                bool isGameOver = false;
                bool white_is = false;
                bool black_is = false;
                for (int row = 0; row < GameData.Length; row++) {
                    for (int col = 0; col < GameData[row].Length; col++) {
                        if (GameData[row][col]._chip_obj != null) {
                            if (GameData[row][col]._color == ColorType.White) {
                                white_is = true;
                                if (row == 7) {
                                    Debug.Log("White v damkah." + row + " " + col);
                                    isGameOver = true;
                                }
                            }
                            if (GameData[row][col]._color == ColorType.Black) {
                                black_is = true;
                                if (row == 0) {
                                    Debug.Log("Black v damkah." + row + " " + col);
                                    isGameOver = true;
                                }
                            }
                        }
                    }
                }
                if (!white_is || !black_is || isGameOver) {
                    Debug.Log("Game Over!");
                    EditorApplication.isPaused = true;
                }
                //Debug.Log("gameover check");
                yield return new WaitForSeconds(10.51f);
            }
        }

        public void MakeTurn(int x, int y) {

            for(var pt_cnt = 0; pt_cnt < possible_turn.Count; pt_cnt++) {
                if (possible_turn[pt_cnt].x == x && possible_turn[pt_cnt].y == y) {
                    OnMakeTurn?.Invoke(x, y);
                    GameData[y][x]._chip_obj = GameData[CurrentSelected.y][CurrentSelected.x]._chip_obj;
                    GameData[y][x]._color = GameData[CurrentSelected.y][CurrentSelected.x]._color;
                    GameData[y][x]._chip_obj.GetComponent<ChipComponent>().x = x;
                    GameData[y][x]._chip_obj.GetComponent<ChipComponent>().y = y;
                    GameData[y][x].MoveStage = 0;
                    GameData[CurrentSelected.y][CurrentSelected.x]._chip_obj = null;
                    if (Pos2Del[pt_cnt].x > -1 && Pos2Del[pt_cnt].y > -1) {
                        GameData[Pos2Del[pt_cnt].y][Pos2Del[pt_cnt].x]._chip_obj.SetActive(false);
                        GameData[Pos2Del[pt_cnt].y][Pos2Del[pt_cnt].x]._chip_obj = null;
                    } else {
                        WhosTurn = (WhosTurn == ColorType.White) ? ColorType.Black : ColorType.White;
                    }
                    ClearSelection();
                
                    break;
                }
            }
        }

    }

}
