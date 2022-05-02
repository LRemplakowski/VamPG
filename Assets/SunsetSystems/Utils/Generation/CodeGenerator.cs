using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

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
                valueName = valueName.Replace(' ', '_');
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
                valueName = valueName.Replace(" ", "_");
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

        public async static Task GenerateScriptAsync(ClassData data)
        {
            ;
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
            if (data.classAttributes.Length > 0)
            {
                contentBuilder.Append(nameSpaceTab + data.classAttributes + "\n");
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
            foreach (FieldData fieldData in data.privateFieldsData)
            {
                if (fieldData.fieldAttributes.Count > 0)
                {
                    contentBuilder.Append(nameSpaceTab + "\t[");
                    foreach (string attribute in fieldData.fieldAttributes)
                    {
                        if (attribute.Trim().Length > 0)
                            contentBuilder.Append(attribute + ", ");
                    }
                    contentBuilder.Remove(contentBuilder.Length - 2, 2);
                    contentBuilder.Append("]\n");
                }
                contentBuilder.Append(nameSpaceTab + "\t");
                string fieldName = fieldData.fieldName;
                string fieldType = fieldData.fieldType;
                fieldName = fieldName.ToCamelCase();
                fieldType = fieldType.RemoveSpecialCharacters();
                contentBuilder.Append("private " + fieldType + " " + fieldName + ";\n");
                await Task.Yield();
            }
            foreach (FieldData fieldData in data.publicFieldsData)
            {
                if (fieldData.fieldAttributes.Count > 0)
                {
                    contentBuilder.Append(nameSpaceTab + "\t[");
                    foreach (string attribute in fieldData.fieldAttributes)
                    {
                        if (attribute.Trim().Length > 0)
                            contentBuilder.Append(attribute + ", ");
                    }
                    contentBuilder.Remove(contentBuilder.Length - 2, 2);
                    contentBuilder.Append("]\n");
                }
                contentBuilder.Append(nameSpaceTab + "\t");
                string fieldName = fieldData.fieldName;
                string fieldType = fieldData.fieldType;
                fieldName = fieldName.ToCamelCase();
                fieldType = fieldType.RemoveSpecialCharacters();
                contentBuilder.Append("private " + fieldType + " " + fieldName + ";\n");
                await Task.Yield();
            }
            contentBuilder.Append(nameSpaceTab + "}\n");
            if (nameSpace.Length > 0)
                contentBuilder.Append("}\n");
            string contents = contentBuilder.ToString();
            string finalPath = data.dataPath + data.className + ".cs";
            await File.WriteAllTextAsync(finalPath, contents);
        }

        public static void GenerateScript(ClassData data)
        {
            ;
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
            if (data.classAttributes.Length > 0)
            {
                contentBuilder.Append(nameSpaceTab + data.classAttributes + "\n");
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
            foreach (FieldData fieldData in data.privateFieldsData)
            {
                if (fieldData.fieldAttributes.Count > 0)
                {
                    contentBuilder.Append(nameSpaceTab + "\t[");
                    foreach (string attribute in fieldData.fieldAttributes)
                    {
                        if (attribute.Trim().Length > 0)
                            contentBuilder.Append(attribute + ", ");
                    }
                    contentBuilder.Remove(contentBuilder.Length - 2, 2);
                    contentBuilder.Append("]\n");
                }
                contentBuilder.Append(nameSpaceTab + "\t");
                string fieldName = fieldData.fieldName;
                string fieldType = fieldData.fieldType;
                fieldName = fieldName.ToCamelCase();
                fieldType = fieldType.RemoveSpecialCharacters();
                contentBuilder.Append("private " + fieldType + " " + fieldName + ";\n");
            }
            foreach (FieldData fieldData in data.publicFieldsData)
            {
                if (fieldData.fieldAttributes.Count > 0)
                {
                    contentBuilder.Append(nameSpaceTab + "\t[");
                    foreach (string attribute in fieldData.fieldAttributes)
                    {
                        if (attribute.Trim().Length > 0)
                            contentBuilder.Append(attribute + ", ");
                    }
                    contentBuilder.Remove(contentBuilder.Length - 2, 2);
                    contentBuilder.Append("]\n");
                }
                contentBuilder.Append(nameSpaceTab + "\t");
                string fieldName = fieldData.fieldName;
                string fieldType = fieldData.fieldType;
                fieldName = fieldName.ToCamelCase();
                fieldType = fieldType.RemoveSpecialCharacters();
                contentBuilder.Append("private " + fieldType + " " + fieldName + ";\n");
            }
            contentBuilder.Append(nameSpaceTab + "}\n");
            if (nameSpace.Length > 0)
                contentBuilder.Append("}\n");
            string contents = contentBuilder.ToString();
            string finalPath = data.dataPath + data.className + ".cs";
            File.WriteAllText(finalPath, contents);
        }

        public class ClassData
        {
            public readonly string baseClass, className, dataPath, nameSpace, privateFieldAttributes, publicFieldAttributes, classAttributes;
            public readonly IReadOnlyList<FieldData> privateFieldsData, publicFieldsData;
            public readonly IReadOnlyList<string> usingDirectives, baseClassGenerics, generatedClassGenerics;
            public ClassData(string baseClass,
                string className,
                string dataPath,
                string nameSpace,
                List<FieldData> privateFieldsData,
                List<FieldData> publicFieldsData,
                List<string> usingDirectives,
                string classAttributes,
                List<string> baseClassGenerics,
                List<string> generatedClassGenerics)
            {
                this.baseClass = baseClass;
                this.className = className;
                this.dataPath = dataPath;
                this.nameSpace = nameSpace;
                this.privateFieldsData = privateFieldsData;
                this.publicFieldsData = publicFieldsData;
                this.usingDirectives = usingDirectives;
                this.classAttributes = classAttributes;
                this.baseClassGenerics = baseClassGenerics;
                this.generatedClassGenerics = generatedClassGenerics;
            }
        }

        public class FieldData
        {
            public readonly string fieldName, fieldType;
            public readonly IReadOnlyList<string> fieldAttributes;

            public FieldData(string fieldName, string fieldType, List<string> fieldAttributes)
            {
                this.fieldName = fieldName;
                this.fieldType = fieldType;
                this.fieldAttributes = fieldAttributes;
            }
        }

        public class ClassBuilder
        {
            private string _nameSpace;
            private string _baseClass;
            private readonly string _className;
            private readonly string _dataPath;
            private readonly List<FieldData> _privateFields = new();
            private readonly List<FieldData> _publicFields = new();
            private readonly List<string> _usingDirectives = new();
            private string _classAttributes;
            private readonly List<string> _baseClassGenerics = new();
            private readonly List<string> _generatedClassGenerics = new();

            public ClassBuilder(string _className, string _dataPath)
            {
                this._className = _className;
                this._dataPath = _dataPath;
            }

            public ClassBuilder SetNameSpace(string _nameSpace)
            {
                this._nameSpace = _nameSpace;
                return this;
            }

            public ClassBuilder SetBaseClass(string _baseClass)
            {
                this._baseClass = _baseClass;
                return this;
            }

            public ClassBuilder SetClassAttributes(string _classAttributes)
            {
                this._classAttributes = _classAttributes;
                return this;
            }

            public ClassBuilder AddPrivateField(FieldData data)
            {
                if (_publicFields.Find(d => d.fieldName.Equals(data.fieldName)) == null && _privateFields.Find(d => d.fieldName.Equals(data.fieldName)) == null)
                    _privateFields.Add(data);
                else
                    Debug.LogError("There is already a public field with the name " + data.fieldName + "!");
                return this;
            }

            public ClassBuilder AddPrivateField(string name, string type, params string[] attributes)
            {
                Debug.Log("Adding private field " + name + " of type " + type);
                if (_publicFields.Find(data => data.fieldName.Equals(name)) == null && _privateFields.Find(data => data.fieldName.Equals(name)) == null)
                    _privateFields.Add(new(name, type, attributes.ToList()));
                else
                    Debug.LogError("There is already a public field with the name " + name + "!");
                return this;
            }

            public ClassBuilder AddPublicField(FieldData data)
            {
                if (_publicFields.Find(d => d.fieldName.Equals(data.fieldName)) == null && _privateFields.Find(d => d.fieldName.Equals(data.fieldName)) == null)
                    _publicFields.Add(data);
                else
                    Debug.LogError("There is already a public field with the name " + data.fieldName + "!");
                return this;
            }

            public ClassBuilder AddPublicField(string name, string type, params string[] attributes)
            {
                Debug.Log("Adding private field " + name + " of type " + type);
                if (_privateFields.Find(data => data.fieldName.Equals(name)) == null && _publicFields.Find(data => data.fieldName.Equals(name)) == null)
                    _publicFields.Add(new(name, type, attributes.ToList()));
                else
                    Debug.LogError("There is already a public field with the name " + name + "!");
                return this;
            }

            public ClassBuilder AddUsingDirective(string nameSpace)
            {
                if (!_usingDirectives.Contains(nameSpace))
                {
                    _usingDirectives.Add(nameSpace);
                }
                return this;
            }

            public ClassBuilder AddBaseClassGeneric(Type generic)
            {
                _baseClassGenerics.Add(generic.Name);
                return this;
            }

            public ClassBuilder AddGeneratedClassGeneric(Type generic)
            {
                _generatedClassGenerics.Add(generic.Name);
                return this;
            }

            public ClassData Build()
            {
                return new ClassData(_baseClass,
                    _className,
                    _dataPath,
                    _nameSpace,
                    _privateFields,
                    _publicFields,
                    _usingDirectives,
                    _classAttributes,
                    _baseClassGenerics,
                    _generatedClassGenerics);
            }
        }
    }
}
