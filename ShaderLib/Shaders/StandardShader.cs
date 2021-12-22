using ShaderLib;
using ShaderLib.Attributes;
using System.Numerics;
using System;

[Shader]
public class StandardShader
{
    public float Gloss, Diffuse;
    public Vector4 Spscular;

    public Texture Tex;

    [VertexShader]
    public SSV2F VS(SSA2V a)
    {
        Vector4 pos = ShaderMath.Mul(Matrixs.Entity2World, a.Position.XYZ1());
        Matrix3x3 rotation = new Matrix3x3(a.Tangent, a.Bitangent, a.Normal);
        return new SSV2F
        {
            Position = ShaderMath.Mul(Matrixs.MVP, a.Position.XYZ1()),
            WorldPosition = new Vector3(pos.X, pos.Y, pos.Z),
            Normal = Vector3.Normalize(ShaderMath.Mul(Matrixs.Entity2World, a.Normal.XYZ0()).XYZ()),
            UV = a.UV,
        };
    }

    [FragmentShader]
    public SSF FS(SSV2F v)
    {
        SSF o = new SSF();
        v.Normal = Vector3.Normalize(v.Normal);
        foreach (PointLight item in Lighting.PointLights)
        {
            Vector3 i = item.Position - v.WorldPosition;
            Vector3 re = Vector3.Normalize(ShaderMath.GetReflection(i, v.Normal));
            Vector3 view = Vector3.Normalize(Matrixs.CameraPosition - v.WorldPosition);
            Vector3 h = Vector3.Normalize(view + i);
            Vector4 Color = Vector3.Distance(v.WorldPosition, item.Position) / item.Range * item.Color;
            //Phong
            //o.Color += ShaderMath.ColorMul(Color, Spscular) * MathF.Pow(ShaderMath.Max(0, Vector3.Dot(re, view)), Gloss);

            //Blinn-Phong
            o.Color += ShaderMath.ColorMul(Color, Spscular) * MathF.Pow(ShaderMath.Max(0, Vector3.Dot(v.Normal, h)), Gloss);

            //Diffuse
            o.Color += ShaderMath.ColorMul(Color * ShaderMath.Max(0, Vector3.Dot(v.Normal, i)), Tex.Value(v.UV));
        }
        o.Color += ShaderMath.ColorMul(Lighting.AmbientColor, Tex.Value(v.UV));
        //Console.WriteLine(o.Color);
        //o.Color = ShaderMath.ColorMul(o.Color, Tex.Value(v.UV));
        return o;
    }
}

[VertexInput]
public struct SSA2V
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector3 Tangent;
    public Vector3 Bitangent;
    public Vector2 UV;
}

[VertexOutput]
public struct SSV2F
{
    public Vector4 Position;
    public Vector3 WorldPosition;
    public Vector3 Normal;
    public Vector2 UV;
}

[FragmentOutput]
public struct SSF
{
    public Vector4 Color;
}
