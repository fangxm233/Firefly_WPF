﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using FireflyUtility.Renderable;
using FireflyUtility.Structure;
using Newtonsoft.Json.Linq;
using System.Linq;
using ShaderGen;
using System;
using ShaderLib;

/*
 * 模型文件格式
 * byte 233 魔数
 * string 名字
 * int 顶点数目
 * //位置
 * float X
 * float Y
 * float Z
 * //法线
 * float X
 * float Y
 * float Z
 * //UV
 * float U
 * float V
 * ...
 * int 三角形个数
 * int 顶点编号
 * ...
 */

namespace Firefly
{
    public class ResourceLoader
    {
        public static List<(string MaterialName, JProperty prop)> MaterialInputs = new List<(string MaterialName, JProperty prop)>();
        private static readonly Dictionary<string, Material> _materials = new Dictionary<string, Material>();

        public static Scene LoadScene(string name)
        {
            JObject json = JObject.Parse(File.ReadAllText($"Scenes/{name}.json"));
            Scene scene = new Scene(json["Name"].ToString(),
                GetCamera(json["Camera"]),
                GetVector4(json["AmbientColor"]),
                null,
                new Dictionary<string, PointLight>(),
                new Dictionary<string, Entity>());
            foreach (JToken item in (JArray)json["Entities"])
            {
                Entity entity = LoadEntity(item.ToString());
                scene.Entities.Add(entity.Name, entity);
            }
            foreach (JToken item in (JArray)json["PointLights"])
            {
                PointLight pointLight = LoadPointLights(item.ToString());
                scene.PointLights.Add(pointLight.Name, pointLight);
            }
            return scene;
        }

        public static PointLight LoadPointLights(string name)
        {
            JObject json = JObject.Parse(File.ReadAllText($"Lights/{name}.json"));
            return new PointLight(json["Name"].ToString(),
                GetVector3(json["Position"]),
                GetVector3(json["Rotation"]) / 360 * MathF.PI,
                GetVector4(json["Color"]),
                json["Intensity"].ToObject<float>(),
                json["Range"].ToObject<float>());
        }

        public static Entity LoadEntity(string name)
        {
            JObject json = JObject.Parse(File.ReadAllText($"Entities/{name}.json"));
            string mName = json["Material"].ToString();
            return new Entity(json["Name"].ToString(),
                GetVector3(json["Position"]),
                ToQuaternion(GetVector3(json["Rotation"])),
                LoadMesh(json["Mesh"].ToString()),
                _materials.ContainsKey(mName) ? _materials[mName] : LoadMaterial(mName));
        }

        private static Camera GetCamera(JToken token) => new Camera(
            GetVector3(token["Position"]),
            ToQuaternion(GetVector3(token["Rotation"]))
        );

        public static Material LoadMaterial(string name)
        {
            JObject json = JObject.Parse(File.ReadAllText($"Materials/{name}.json"));
            foreach (JProperty item in json.Properties())
                if (item.Name != "Name" && item.Name != "Shader")
                    MaterialInputs.Add((name, item));
            Material m = new Material(json["Name"].ToString(), json["Shader"].ToString());
            _materials.Add(m.Name, m);
            return m;
        }

        public static Mesh LoadMesh(string name)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream($"Models/{name}.Model", FileMode.Open)))
            {
                if (reader.ReadByte() != 233) throw new Exception("模型魔数不正确");
                string mName = "";
                //string mName = reader.ReadString();

                Vertex[] vertices = new Vertex[reader.ReadInt32()];
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = ReadBinaryVertex(reader);

                int[] triangles = new int[reader.ReadInt32() * 3];
                for (int i = 0; i < triangles.Length; i++)
                    triangles[i] = reader.ReadInt32();

                return new Mesh(mName, vertices, triangles);
            }
        }

        private static Vertex ReadBinaryVertex(BinaryReader reader)
        {
            Vertex vertex = new Vertex();
            vertex.Point.X = reader.ReadSingle();
            vertex.Point.Y = reader.ReadSingle();
            vertex.Point.Z = reader.ReadSingle();
            vertex.Normal.X = reader.ReadSingle();
            vertex.Normal.Y = reader.ReadSingle();
            vertex.Normal.Z = reader.ReadSingle();
            vertex.UV.X = reader.ReadSingle();
            vertex.UV.Y = reader.ReadSingle();
            return vertex;
        }

        private static int[] GetIntArray(JArray list)
        {
            List<int> vs = new List<int>();
            foreach (JToken item in list)
                vs.Add(item.ToObject<int>());
            return vs.ToArray();
        }

        public static Vector2 GetVector2(JToken token) =>
            new Vector2(token["X"].ToObject<float>(), token["Y"].ToObject<float>());

        public static Vector3 GetVector3(JToken token) =>
            new Vector3(token["X"].ToObject<float>(), token["Y"].ToObject<float>(), token["Z"].ToObject<float>());

        public static Vector4 GetVector4(JToken token) =>
            new Vector4(token["X"].ToObject<float>(), token["Y"].ToObject<float>(),
                token["Z"].ToObject<float>(), token["W"].ToObject<float>());

        public static Quaternion ToQuaternion(Vector3 rotation)
        {
            rotation = rotation / 180 * MathF.PI;
            return Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
        }
    }
}
