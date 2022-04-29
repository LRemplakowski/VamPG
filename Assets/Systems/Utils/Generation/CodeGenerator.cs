using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Utils.Generation
{
    public static class CodeGenerator
    {
        public async static Task GenerateEnumAsync(List<string> names, string dataPath, string enumName, string nameSpace)
        {
            StringBuilder contentBuilder = new();
            nameSpace = nameSpace.Trim();
            string nameSpaceTab = "";
            if (nameSpace.Length > 0)
            {
                contentBuilder.Append("namespace " + nameSpace + "\n");
                contentBuilder.Append("{\n");
                nameSpaceTab = "\t";
            }
            contentBuilder.Append(nameSpaceTab + "public enum " + enumName + "\n");
            contentBuilder.Append(nameSpaceTab + "{\n");
            for (int i = 0; i < names.Count; i++)
            {
                contentBuilder.Append(nameSpaceTab + "\t");
                string valueName = names[i];
                valueName = valueName.RemoveSpecialCharacters();
                valueName = valueName.ToUpper();
                contentBuilder.Append(valueName);
                contentBuilder.Append(",");
                contentBuilder.Append("\n");
                await Task.Yield();
            }
            contentBuilder.Remove(contentBuilder.Length - 2, 1);
            contentBuilder.Append(nameSpaceTab + "}\n");
            if (nameSpace.Length > 0)
                contentBuilder.Append("}\n");
            string contents = contentBuilder.ToString();
            string finalPath = dataPath + enumName + ".cs";
            await File.WriteAllTextAsync(finalPath, contents);
        }

        public static void GenerateEnum(List<string> names, string dataPath, string enumName, string nameSpace)
        {
            StringBuilder contentBuilder = new();
            nameSpace = nameSpace.Trim();
            string nameSpaceTab = "";
            if (nameSpace.Length > 0)
            {
                contentBuilder.Append("namespace " + nameSpace + "\n");
                contentBuilder.Append("{\n");
                nameSpaceTab = "\t";
            }
            contentBuilder.Append(nameSpaceTab + "public enum " + enumName + "\n");
            contentBuilder.Append(nameSpaceTab + "{\n");
            for (int i = 0; i < names.Count; i++)
            {
                contentBuilder.Append(nameSpaceTab + "\t");
                string valueName = names[i];
                valueName = valueName.RemoveSpecialCharacters();
                valueName = valueName.ToUpper();
                contentBuilder.Append(valueName);
                contentBuilder.Append(",");
                contentBuilder.Append("\n");
            }
            contentBuilder.Remove(contentBuilder.Length - 2, 1);
            contentBuilder.Append(nameSpaceTab + "}\n");
            if (nameSpace.Length > 0)
                contentBuilder.Append("}\n");
            string contents = contentBuilder.ToString();
            string finalPath = dataPath + enumName + ".cs";
            File.WriteAllText(finalPath, contents);
        }

        public async static Task GenerateScriptAsync(ClassBuilder builder)
        {
            ClassData data = builder.Build();
            StringBuilder contentBuilder = new();
            foreach (string usingDirective in data.usingDirectives)
            {
                contentBuilder.Append("using " + usingDirective.RemoveSpecialCharacters() + ";\n");
                await Task.Yield();
            }
            contentBuilder.Append("\n");
            string nameSpace = data.nameSpace.Trim();
            string nameSpaceTab = "";
            if (nameSpace.Length > 0)
            {
                contentBuilder.Append("namespace " + nameSpace + "\n");
                contentBuilder.Append("{\n");
                nameSpaceTab = "\t";
            }
            contentBuilder.Append(nameSpaceTab + "public class " + data.className);
            if (data.generatedClassGenerics.Count > 0)
            {
                contentBuilder.Append("<");
                foreach (string generic in data.generatedClassGenerics)
                {
                    contentBuilder.Append(generic + ", ");
                    await Task.Yield();
                }
                contentBuilder.Remove(contentBuilder.Length, 2);
                contentBuilder.Append(">");
            }
            string baseClass = data.baseClass.Trim();
            if (baseClass.Length > 0)
            {
                contentBuilder.Append(" : " + baseClass);
                if (data.baseClassGenerics.Count > 0)
                {
                    contentBuilder.Append("<");
                    foreach (string generic in data.baseClassGenerics)
                    {
                        contentBuilder.Append(generic + ", ");
                        await Task.Yield();
                    }
                    contentBuilder.Remove(contentBuilder.Length, 2);
                    contentBuilder.Append(">");
                }
                contentBuilder.Append("\n");
            }
            else
            {
                contentBuilder.Append("\n");
            }
            contentBuilder.Append(nameSpaceTab + "{\n");
            foreach (string key in data.privateFields.Keys)
            {
                if (data.privateFields.TryGetValue(key, out string fieldType))
                {
                    contentBuilder.Append(nameSpaceTab + "\t");
                    contentBuilder.Append(data.privateFieldAttribute + "\n");
                    contentBuilder.Append(nameSpaceTab + "\t");
                    string fieldName = key;
                    fieldName = fieldName.ToCamelCase();
                    fieldType = fieldType.RemoveSpecialCharacters();
                    contentBuilder.Append("private " + fieldType + " " + fieldName + ";\n");
                    await Task.Yield();
                }
            }
            foreach (string key in data.publicFields.Keys)
            {
                if (data.publicFields.TryGetValue(key, out string fieldType))
                {
                    contentBuilder.Append(nameSpaceTab + "\t");
                    contentBuilder.Append(data.publicFieldAttribute + "\n");
                    contentBuilder.Append(nameSpaceTab + "\t");
                    string fieldName = key;
                    fieldName = fieldName.ToCamelCase();
                    fieldType = fieldType.RemoveSpecialCharacters();
                    contentBuilder.Append("public " + fieldType + " " + fieldName + ";\n");
                    await Task.Yield();
                }
            }
            contentBuilder.Remove(contentBuilder.Length - 2, 1);
            contentBuilder.Append(nameSpaceTab + "}\n");
            if (nameSpace.Length > 0)
                contentBuilder.Append("}\n");
            string contents = contentBuilder.ToString();
            string finalPath = data.dataPath + data.className + ".cs";
            await File.WriteAllTextAsync(finalPath, contents);
        }

        public static void GenerateScript(ClassBuilder builder)
        {
            ClassData data = builder.Build();
            StringBuilder contentBuilder = new();
            foreach (string usingDirective in data.usingDirectives)
            {
                contentBuilder.Append("using " + usingDirective.RemoveSpecialCharacters() + ";\n");
            }
            contentBuilder.Append("\n");
            string nameSpace = data.nameSpace.Trim();
            string nameSpaceTab = "";
            if (nameSpace.Length > 0)
            {
                contentBuilder.Append("namespace " + nameSpace + "\n");
                contentBuilder.Append("{\n");
                nameSpaceTab = "\t";
            }
            contentBuilder.Append(nameSpaceTab + "public class " + data.className);
            if (data.generatedClassGenerics.Count > 0)
            {
                contentBuilder.Append("<");
                foreach (string generic in data.generatedClassGenerics)
                {
                    contentBuilder.Append(generic + ", ");
                }
                contentBuilder.Remove(contentBuilder.Length, 2);
                contentBuilder.Append(">");
            }
            string baseClass = data.baseClass.Trim();
            if (baseClass.Length > 0)
            {
                contentBuilder.Append(" : " + baseClass);
                if (data.baseClassGenerics.Count > 0)
                {
                    contentBuilder.Append("<");
                    foreach (string generic in data.baseClassGenerics)
                    {
                        contentBuilder.Append(generic + ", ");
                    }
                    contentBuilder.Remove(contentBuilder.Length, 2);
                    contentBuilder.Append(">");
                }
                contentBuilder.Append("\n");
            }
            else
            {
                contentBuilder.Append("\n");
            }
            contentBuilder.Append(nameSpaceTab + "{\n");
            foreach (string key in data.privateFields.Keys)
            {
                if (data.privateFields.TryGetValue(key, out string fieldType))
                {
                    contentBuilder.Append(nameSpaceTab + "\t");
                    contentBuilder.Append(data.privateFieldAttribute + "\n");
                    contentBuilder.Append(nameSpaceTab + "\t");
                    string fieldName = key;
                    fieldName = fieldName.ToCamelCase();
                    fieldType = fieldType.RemoveSpecialCharacters();
                    contentBuilder.Append("private " + fieldType + " " + fieldName + ";\n");
                }
            }
            foreach (string key in data.publicFields.Keys)
            {
                if (data.publicFields.TryGetValue(key, out string fieldType))
                {
                    contentBuilder.Append(nameSpaceTab + "\t");
                    contentBuilder.Append(data.publicFieldAttribute + "\n");
                    contentBuilder.Append(nameSpaceTab + "\t");
                    string fieldName = key;
                    fieldName = fieldName.ToCamelCase();
                    fieldType = fieldType.RemoveSpecialCharacters();
                    contentBuilder.Append("public " + fieldType + " " + fieldName + ";\n");
                }
            }
            contentBuilder.Remove(contentBuilder.Length - 2, 1);
            contentBuilder.Append(nameSpaceTab + "}\n");
            if (nameSpace.Length > 0)
                contentBuilder.Append("}\n");
            string contents = contentBuilder.ToString();
            string finalPath = data.dataPath + data.className + ".cs";
            File.WriteAllText(finalPath, contents);
        }

        private static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static string ToCamelCase(this string str)
        {
            string[] words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            string leadWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)",
                m =>
                {
                    return m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value;
                });
            string[] tailWords = words.Skip(1)
                .Select(word => char.ToUpper(word[0]) + word.Substring(1))
                .ToArray();
            return $"{leadWord}{string.Join(string.Empty, tailWords)}";
        }

        internal class ClassData
        {
            public readonly string baseClass, className, dataPath, nameSpace, privateFieldAttribute, publicFieldAttribute;
            public IReadOnlyDictionary<string, string> privateFields, publicFields;
            public IReadOnlyList<string> usingDirectives, baseClassGenerics, generatedClassGenerics;
            public ClassData(string baseClass,
                string className,
                string dataPath,
                string nameSpace,
                Dictionary<string, string> privateFields,
                Dictionary<string, string> publicFields,
                List<string> usingDirectives,
                string privateFieldAttribute,
                string publicFieldAttribute,
                List<string> baseClassGenerics,
                List<string> generatedClassGenerics)
            {
                this.baseClass = baseClass;
                this.className = className;
                this.dataPath = dataPath;
                this.nameSpace = nameSpace;
                this.privateFields = privateFields;
                this.publicFields = publicFields;
                this.usingDirectives = usingDirectives;
                this.privateFieldAttribute = privateFieldAttribute;
                this.publicFieldAttribute = publicFieldAttribute;
                this.baseClassGenerics = baseClassGenerics;
                this.generatedClassGenerics = generatedClassGenerics;
            }
        }

        public class ClassBuilder
        {
            public string BaseClass { get; set; }
            private readonly string _className;
            private readonly string _dataPath;
            private readonly Dictionary<string, string> _privateFields = new();
            private readonly Dictionary<string, string> _publicFields = new();
            private readonly List<string> _usingDirectives = new();
            public string PrivateFieldAttribute { get; set; }
            public string PublicFieldAttributes { get; set; }
            private readonly List<string> _baseClassGenerics = new();
            private readonly List<string> _generatedClassGenerics = new();

            public string NameSpace { get; set; }

            public ClassBuilder(string _className, string _dataPath)
            {
                this._className = _className;
                this._dataPath = _dataPath;
            }

            public bool TryAddPrivateField(string name, Type type)
            {
                bool successful = false;
                if (!_publicFields.ContainsKey(name))
                    successful = _privateFields.TryAdd(name, type.Name);
                else
                    Debug.LogError("There is already a public field with the name " + name + "!");
                return successful;
            }

            public bool TryAddPublicField(string name, Type type)
            {
                bool successful = false;
                if (!_privateFields.ContainsKey(name))
                    successful = _publicFields.TryAdd(name, type.Name);
                else
                    Debug.LogError("There is already a private field with the name " + name + "!");
                return successful;
            }

            public bool TryAddUsingDirective(string nameSpace)
            {
                bool successful = false;
                if (!_usingDirectives.Contains(nameSpace))
                {
                    _usingDirectives.Add(nameSpace);
                    successful = true;
                }
                return successful;
            }

            public bool TryRemovePrivateField(string name)
            {
                return _privateFields.Remove(name);
            }

            public bool TryRemovePublicField(string name)
            {
                return _publicFields.Remove(name);
            }

            public bool TryRemoveUsingDirective(string nameSpace)
            {
                return _usingDirectives.Remove(nameSpace);
            }

            public void AddBaseClassGeneric(Type generic)
            {
                _baseClassGenerics.Add(generic.Name);
            }

            public void AddGeneratedClassGeneric(Type generic)
            {
                _generatedClassGenerics.Add(generic.Name);
            }

            public bool TryRemoveBaseClassGeneric(Type generic)
            {
                return _baseClassGenerics.Remove(generic.Name);
            }

            public bool TryRemoveGeneratedClassGeneric(Type generic)
            {
                return _generatedClassGenerics.Remove(generic.Name);
            }

            internal ClassData Build()
            {
                return new ClassData(BaseClass,
                    _className,
                    _dataPath,
                    NameSpace,
                    _privateFields,
                    _publicFields,
                    _usingDirectives,
                    PrivateFieldAttribute,
                    PublicFieldAttributes,
                    _baseClassGenerics,
                    _generatedClassGenerics);
            }
        }
    }
}
