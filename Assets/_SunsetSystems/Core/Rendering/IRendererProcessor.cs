using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Core.Rendering
{
    public interface IRendererProcessor
    {
        void InjectRenderers(IEnumerable<Renderer> renderers);
    }
}
