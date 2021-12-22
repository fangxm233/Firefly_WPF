﻿using ShaderLib;
using ShaderLib.Attributes;
using System.Numerics;
using System;

[Shader]
public class LightingShader
{
    public float Gloss, Diffuse;
    public Vector4 Spscular;

    [VertexShader]
    public V2F VS(A2V a)
    {
        Vector4 pos = ShaderMath.Mul(Matrixs.Entity2World, new Vector4(a.Position, 1));
        return new V2F
        {
            Position = ShaderMath.Mul(Matrixs.MVP, new Vector4(a.Position, 1)),
            WorldPosition = new Vector3(pos.X, pos.Y, pos.Z),
            Normal = Vector3.Normalize(ShaderMath.Mul(Matrixs.Entity2World, new Vector4(a.Normal, 0)).XYZ()),
        };
    }

    [FragmentShader]
    public F FS(V2F v)
    {
        F o = new F();
        v.Normal = Vector3.Normalize(v.Normal);
        foreach (PointLight item in Lighting.PointLights)
        {
            Vector3 i = item.Position - v.WorldPosition;
            Vector3 re = Vector3.Normalize(ShaderMath.GetReflection(i, v.Normal));
            Vector3 view = Vector3.Normalize(Matrixs.CameraPosition - v.WorldPosition);
            Vector3 h = Vector3.Normalize(view + i);

            //Phong
            //o.Color += ShaderMath.ColorMul(item.Color, Spscular) * MathF.Pow(ShaderMath.Max(0, Vector3.Dot(re, view)), Gloss);

            //Blinn-Phong
            o.Color += ShaderMath.ColorMul(item.Color, Spscular) * MathF.Pow(ShaderMath.Max(0, Vector3.Dot(v.Normal, h)), Gloss);

            //Diffuse
            o.Color += item.Color * Diffuse * ShaderMath.Max(0, Vector3.Dot(v.Normal, i));
        }
        o.Color += Lighting.AmbientColor;

        return o;
    }
}

[VertexInput]
public struct A2V
{
    public Vector3 Position;
    public Vector3 Normal;
}

[VertexOutput]
public struct V2F
{
    public Vector4 Position;
    public Vector3 WorldPosition;
    public Vector3 Normal;
}

[FragmentOutput]
public struct F
{
    public Vector4 Color;
}
