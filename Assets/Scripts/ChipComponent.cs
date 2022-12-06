using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        [SerializeField] public int x;
        [SerializeField] public int y;

        private GameObject _game_engine_obj;
        private TheGameEngine _game_engine_script;

        private void Start() {
            _game_engine_obj = GameObject.FindGameObjectWithTag("Exgine");
            _game_engine_script = _game_engine_obj.GetComponent<TheGameEngine>();
            _game_engine_script.GameData[y][x]._chip_obj = gameObject;
            _game_engine_script.GameData[y][x]._color = GetColor;
            OnClickEventHandler += ChipComponent_OnClickEventHandler;
        }

        private void ChipComponent_OnClickEventHandler(BaseClickComponent component) {
            _game_engine_script.SelectCell(x, y);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            _game_engine_script.GameData[y][x]._cell_obj?.GetComponent<CellComponent>()?.OnPointerEnter(eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            _game_engine_script.GameData[y][x]._cell_obj?.GetComponent<CellComponent>()?.OnPointerExit(eventData);
        }

    }
}
