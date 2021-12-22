using System;
using FireflyUtility.Renderable;

namespace ShaderGen
{
    public delegate void DrawDelegate(Entity entity);
    public delegate object GetShaderDelegate();
    public delegate void SetShaderDelegate(object shader);
    public delegate void SetFieldDelegate(string name, object value);
    public delegate void SetModeDelegate(int renderType);

    public class DelegateCollection
    {
        public string Name;
        public DrawDelegate Draw;
        public GetShaderDelegate GetShader;
        public SetShaderDelegate SetShader;
        public SetFieldDelegate SetField;
        public SetModeDelegate SetMode;
        public Type Type;
    }
}
