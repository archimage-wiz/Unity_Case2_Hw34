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

        private Dictionary<NeighborType, CellComponent> _neighbors;

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


        /// <summary>
        /// Возвращает соседа клетки по указанному направлению
        /// </summary>
        /// <param name="type">Перечисление направления</param>
        /// <returns>Клетка-сосед или null</returns>
        public CellComponent GetNeighbors(NeighborType type) => _neighbors[type];

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(this, false);
        }

        /// <summary>
        /// Конфигурирование связей клеток
        /// </summary>
		public void Configuration(Dictionary<NeighborType, CellComponent> neighbors)
		{
            if (_neighbors != null) return;
            _neighbors = neighbors;
		}

	}

    /// <summary>
    /// Тип соседа клетки
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// Клетка сверху и слева от данной
        /// </summary>
        TopLeft,
        /// <summary>
        /// Клетка сверху и справа от данной
        /// </summary>
        TopRight,
        /// <summary>
        /// Клетка снизу и слева от данной
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Клетка снизу и справа от данной
        /// </summary>
        BottomRight
    }
}