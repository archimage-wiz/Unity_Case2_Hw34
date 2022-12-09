using System.Collections.Generic;
using UnityEngine;
//using System.Diagnostics;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class CellComponent : BaseClickComponent
    {
        [SerializeField]  int x, y;

        private GameObject _game_engine_obj;
        private TheGameEngine _game_engine_script;

        void Start() {
            _game_engine_obj = GameObject.FindGameObjectWithTag("Exgine");
            _game_engine_script = _game_engine_obj.GetComponent<TheGameEngine>();
            _game_engine_script.GameData[y][x]._cell_obj = gameObject;

            _mesh = GetComponent<MeshRenderer>();
            _meshMaterials[0] = _mesh.material;
            OnFocusEventHandler += CellComponent_OnFocusEventHandler;
            OnClickEventHandler += CellComponent_OnClickEventHandler;
        }
        private void CellComponent_OnFocusEventHandler(CellComponent component, bool isSelect) {
            if (isSelect) {
                AddAdditionalMaterial(_game_engine_script.GetHighlihtMaterial, 1);
            } else {
                RemoveAdditionalMaterial(1);
            }
        }
        private void CellComponent_OnClickEventHandler(BaseClickComponent component) {
            _game_engine_script.MakeTurn(x, y);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent(this, true);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(this, false);
        }
	}
}