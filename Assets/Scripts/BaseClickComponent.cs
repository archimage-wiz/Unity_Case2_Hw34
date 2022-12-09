using System.ComponentModel;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public enum ColorType {
        White,
        Black
    }
    public abstract class BaseClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

        //Меш игрового объекта
        protected MeshRenderer _mesh;
        //Список материалов на меше объекта
        protected Material[] _meshMaterials = new Material[3];

        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        private ColorType _color;
        public ColorType GetColor => _color;
        public void AddAdditionalMaterial(Material material, int index = 1) {
            if (index < 1 || index > 2) {
                Debug.LogError("Попытка добавить лишний материал. Индекс может быть равен только 1 или 2");
                return;
            }
            _meshMaterials[index] = material;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }
        public void RemoveAdditionalMaterial(int index = 1) {
            if (index < 1 || index > 2) {
                Debug.LogError("Попытка удалить несуществующий материал. Индекс может быть равен только 1 или 2");
                return;
            }
            _meshMaterials[index] = null;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }
        public event ClickEventHandler OnClickEventHandler;
        public event FocusEventHandler OnFocusEventHandler;
        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);

        public void OnPointerClick(PointerEventData eventData) {
            OnClickEventHandler?.Invoke(this);
        }
        protected void CallBackEvent(CellComponent target, bool isSelect) {
            OnFocusEventHandler?.Invoke(target, isSelect);
        }
    }
    public delegate void ClickEventHandler(BaseClickComponent component);
    public delegate void FocusEventHandler(CellComponent component, bool isSelect);
}