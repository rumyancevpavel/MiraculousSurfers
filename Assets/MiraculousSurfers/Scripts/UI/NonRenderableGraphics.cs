using UnityEngine;
using UnityEngine.UI;

namespace MiraculousSurfers.UI
{
    [AddComponentMenu("UI/Non Renderable Graphics")]
    public class NonRenderableGraphics : Graphic
    {
        public override void SetMaterialDirty() { }

        public override void SetVerticesDirty() { }

        protected override void OnPopulateMesh(VertexHelper vh) => vh.Clear();
    }
}
