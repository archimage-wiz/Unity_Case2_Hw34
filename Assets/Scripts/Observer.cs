using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers {
    internal class Observer : MonoBehaviour {
        enum ActionId { select, turn }
        enum ObserverMode { none, record, play }
        private struct ActionZ {
            public ActionId _action;
            public Position _pos;
        }
        private const string _file_name = "movements_db.txt";
        [SerializeField] private EventSystem _event_system;
        [SerializeField] private TheGameEngine _engine;
        [SerializeField] private ObserverMode _mode;
        private List<ActionZ> ActionsList = new List<ActionZ>();

        private void Start() {
            _event_system.enabled = true;

            if (_mode == ObserverMode.record) {
                if(File.Exists(_file_name)) { File.Delete(_file_name); }
                File.Create(_file_name).Close();
            }
            if (_mode == ObserverMode.play) {
                if (File.Exists(_file_name)) {
                    var _buff = File.ReadAllLines(_file_name, Encoding.UTF8);
                    foreach (var _line in _buff) {
                        var p = _line.Split(":")[1].Split("->");
                        Position xy = new Position { x = Convert.ToInt32(p[1].Trim().Split(",")[0]), y = Convert.ToInt32(p[1].Trim().Split(",")[1]) };
                        ActionId act = (ActionId)Convert.ToInt32(p[0].Trim());
                        ActionsList.Add(new ActionZ { _action = act, _pos = xy } );
                    }
                    if(ActionsList.Count > 0) {
                        StartCoroutine(PlayRecord());
                        _event_system.enabled = false;                     
                    }
                }
            }
            _engine.OnSelectCell += OnSelectCellAction;
            _engine.OnMakeTurn += OnMakeTurnAction;
            
        }
        private IEnumerator PlayRecord() {
            int current_action = 0;
            while (true) {
                if (ActionsList[current_action]._action == ActionId.select) {
                    _engine.SelectCell(ActionsList[current_action]._pos.x, ActionsList[current_action]._pos.y);
                }
                if (ActionsList[current_action]._action == ActionId.turn) {
                    _engine.MakeTurn(ActionsList[current_action]._pos.x, ActionsList[current_action]._pos.y);
                }
                current_action++;
                if(current_action > ActionsList.Count-1) { break; }
                yield return new WaitForSeconds(1.0f);
            }
        }

        private void MakeRecord(string file_name, string text) {
            File.AppendAllText(file_name, text + Environment.NewLine, Encoding.UTF8);
        }

        void OnSelectCellAction(int x, int y) {
            if(_mode == ObserverMode.record) {
                MakeRecord(_file_name, $"Select cell: {(int)ActionId.select} -> {x}, {y}");
            }
        }
        void OnMakeTurnAction(int x, int y) {
            if (_mode == ObserverMode.record) {
                MakeRecord(_file_name, $"Make Turn: {(int)ActionId.turn} -> {x}, {y}");
            }
        }


    }

}