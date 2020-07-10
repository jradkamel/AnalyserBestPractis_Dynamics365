using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AnalyserBestPractis_Dynamics365
{
    class AnalyserRules
    {
        
        public static string getObjectNameRules(string _settingValue, ProjectItem _Obejct,string _spd, ErrorListProvider _errorListProvider, DTE _dte, string _projectName)
        {
            string modelName = GetModelName(_Obejct.FileNames[0], 4);
            string name = GetModelName(_Obejct.FileNames[0], 1).Replace(".xml", "");
            string objectType = GetModelName(_Obejct.FileNames[0], 2);
            string objectName = GetModelName(_Obejct.FileNames[0], 1).Replace(".xml", "");
            string _ret = "";
            if (objectType == "AxEnum" || objectType == "AxEnumExtension")
            {
                if (objectType == "AxEnum")
                {
                    if (getNode(_Obejct.FileNames[0], "/AxEnum/Label").Count == 0)
                    {
                        _ret = "[" + objectType + "] : [" + objectName.Replace(".xml", "") + "] must be associated with a label";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                    else
                    {
                        if (getNode(_Obejct.FileNames[0], "/AxEnum/Label")[0].InnerText == "")
                        {
                            _ret = "[" + objectType + "] : [" + objectName.Replace(".xml", "") + "] must be associated with a label";
                            AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                        }
                    }

                    if (getNode(_Obejct.FileNames[0], "/AxEnum/EnumValues/AxEnumValue").Count != 0)
                    {
                        for (int i = 0; i < getNode(_Obejct.FileNames[0], "/AxEnum/EnumValues/AxEnumValue").Count; i++)
                        {
                            if (getNode(_Obejct.FileNames[0], "/AxEnum/EnumValues/AxEnumValue")[i].SelectNodes("Label").Count == 0)
                            {
                                _ret = "[" + objectType + "] : Each value of [" + objectName.Replace(".xml", "") + "] must be associated with a label";
                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                            }
                        }
                    }
                }
                if ((objectType == "AxEnumExtension" && !objectName.Replace(".xml", "").EndsWith("." + modelName)) || (objectType == "AxEnum" && !objectName.Replace(".xml", "").StartsWith(_spd)) )
                {
                    _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format";
                    AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                }
                if (objectType == "AxEnumExtension")
                {
                    if (getNode(_Obejct.FileNames[0], "/AxEnumExtension/EnumValues/AxEnumValue").Count != 0)
                    {
                        for (int i = 0; i < getNode(_Obejct.FileNames[0], "/AxEnumExtension/EnumValues/AxEnumValue").Count; i++)
                        {
                            if (getNode(_Obejct.FileNames[0], "/AxEnumExtension/EnumValues/AxEnumValue")[i].SelectNodes("Label").Count == 0)
                            {
                                _ret = "[" + objectType + "] : Each value of [" + objectName.Replace(".xml", "") + "] must be associated with a label";
                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                            }
                        }
                    }
                    if (getNode(_Obejct.FileNames[0], "/AxEnumExtension/EnumValues/AxEnumValue").Count != 0)
                    {
                        for (int i = 0; i < getNode(_Obejct.FileNames[0], "/AxEnumExtension/EnumValues/AxEnumValue").Count; i++)
                        {
                            if (!getNode(_Obejct.FileNames[0], "/AxEnumExtension/EnumValues/AxEnumValue")[i].SelectNodes("Name")[0].InnerText.StartsWith(_spd))
                            {
                                _ret = "[" + objectType + "] : Each value of [" + objectName.Replace(".xml", "") + "] must follow standard naming with prefix";
                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                            }
                        }
                    }
                }
            }
            if (objectType == "AxEdt" || objectType == "AxEdtExtension")
            {
                if (objectType == "AxEdtExtension")
                {
                    if (getNode(_Obejct.FileNames[0], "/AxEdtExtension/Label").Count == 0)
                    {
                        _ret = "[" + objectType + "] : [" + objectName + "] must be associated with a label";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                }
                if (objectType == "AxEdt" )
                {
                    if (getNode(_Obejct.FileNames[0], "/AxEdt/Label").Count == 0)
                    {
                        _ret = "[" + objectType + "] : [" + objectName + "] must be associated with a label";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                }
                if ((objectType == "AxEdtExtension" && !objectName.EndsWith("." + modelName)) || (objectType == "AxEdt" && !objectName.StartsWith(_spd)))
                {
                    _ret = "[" + objectType + "] : Object name [" + objectName + "] not in correct format";
                    AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                }
            }
            if (objectType == "AxDataEntityView" || objectType == "AxMap" || objectType == "AxQuery" || objectType == "AxTable" || objectType == "AxView")
            {
                if (!objectName.StartsWith(_spd))
                {
                    _ret = "[" + objectType + "] : Object name [" + objectName + "] not in correct format";
                    AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                }
                if (objectType == "AxTable")
                {
                    
                    if (getNode(_Obejct.FileNames[0], "/AxTable/PrimaryIndex").Count == 0)
                    {
                        _ret = "[" + objectType + "] : Object [" + objectName + "] must have a primary key defined";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                    else
                    {
                        if (getNode(_Obejct.FileNames[0], "/AxTable/Indexes/AxTableIndex").Count != 0)
                        {
                            for (int i = 0; i < getNode(_Obejct.FileNames[0], "/AxTable/Indexes/AxTableIndex").Count; i++)
                            {
                                if(getNode(_Obejct.FileNames[0], "/AxTable/Indexes/AxTableIndex")[i].SelectNodes("Name")[0].InnerText == getNode(_Obejct.FileNames[0], "/AxTable/PrimaryIndex")[0].InnerText)
                                {
                                    if(getNode(_Obejct.FileNames[0], "/AxTable/Indexes/AxTableIndex")[i].SelectNodes("AlternateKey")[0].InnerText != "Yes")
                                    {
                                        _ret = "[" + objectType + "] : Object [" + objectName + "] : Alternate key of its primary key must be defined to yes";
                                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                                    }
                                }
                            }
                        }
                    }
                    if( !getPropertiesbool(_Obejct.FileNames[0], "/AxTable/FormRef"))
                    {
                        _ret = "[" + objectType + "] : Object [" + objectName + "] must have a Form Ref defined";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                    if (!getPropertiesbool(_Obejct.FileNames[0], "/AxTable/Label"))
                    {
                        _ret = "[" + objectType + "] : Object [" + objectName + "] must have a Label defined";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                    
                    if (!getPropertiesbool(_Obejct.FileNames[0], "/AxTable/ReplacementKey"))
                    {
                        _ret = "[" + objectType + "] : Object [" + objectName + "] must have a Replacement Key defined";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                    if (!getPropertiesbool(_Obejct.FileNames[0], "/AxTable/SaveDataPerCompany"))
                    {
                        _ret = "[" + objectType + "] : Object [" + objectName + "] must have a Save Data Per Company defined";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                    if (!getPropertiesbool(_Obejct.FileNames[0], "/AxTable/TableGroup"))
                    {
                        _ret = "[" + objectType + "] : Object [" + objectName + "] must have a Table Group defined";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                    if (!getPropertiesbool(_Obejct.FileNames[0], "/AxTable/TitleField1"))
                    {
                        _ret = "[" + objectType + "] : Object [" + objectName + "] must have a Title Field 1 defined";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }
                    if (!getPropertiesbool(_Obejct.FileNames[0], "/AxTable/TitleField2"))
                    {
                        _ret = "[" + objectType + "] : Object [" + objectName + "] must have a Title Field 2 defined";
                        AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                    }

                    if (getNode(_Obejct.FileNames[0], "/AxTable/Relations/AxTableRelation").Count != 0)
                    {
                        for (int i = 0; i < getNode(_Obejct.FileNames[0], "/AxTable/Relations/AxTableRelation").Count; i++)
                        {
                            if (getNode(_Obejct.FileNames[0], "/AxTable/Relations/AxTableRelation")[i].SelectNodes("RelatedTable").Count == 0)
                            {
                                _ret = "[" + objectType + "] : Object [" + objectName + "] : Properties Related Table of relation " + getNode(_Obejct.FileNames[0], "/AxTable/Relations/AxTableRelation")[i].SelectNodes("Name")[0].InnerText+ " must be completed";
                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                            }
                            if (getNode(_Obejct.FileNames[0], "/AxTable/Relations/AxTableRelation")[i].SelectNodes("OnDelete").Count == 0)
                            {
                                _ret = "[" + objectType + "] : Object [" + objectName + "] : Properties OnDelete of relation " + getNode(_Obejct.FileNames[0], "/AxTable/Relations/AxTableRelation")[i].SelectNodes("Name")[0].InnerText + " must be completed";
                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                            }
                        }
                    }
                }
            }
           
            if (objectType == "AxClass")
            {
                string folder = getPathProprieties(_Obejct.FileNames[0], objectName, 4, objectType);
                if (getNode(_Obejct.FileNames[0], "/z:anyType/Extends").Count != 0)
                {
                    if (getNode(_Obejct.FileNames[0], "/z:anyType/Extends")[0].InnerText.EndsWith("Controller"))
                    {
                        if (!objectName.Replace(".xml", "").EndsWith("Controller"))
                        {
                            _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's a controller class";
                            AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                        }
                    }
                    if (getNode(_Obejct.FileNames[0], "/z:anyType/Extends")[0].InnerText.Contains("DataProvider"))
                    {
                        if (!objectName.Replace(".xml", "").EndsWith("Controller"))
                        {
                            _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's a DP class";
                            AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                        }
                    }
                }
                if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute").Count != 0)
                {
                    for (int i = 0; i < getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute").Count; i++)
                    {
                        if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Name")[0].InnerText.EndsWith("DataContractAttribute"))
                        {
                            if (!objectName.Replace(".xml", "").EndsWith("Contract"))
                            {
                                _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's a Data Contaract class";
                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                            }
                        }
                    }
                }
               
                if (!objectName.Replace(".xml", "").StartsWith(_spd))
                {
                    _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format ";
                    AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                }
               
                if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute").Count != 0)
                {
                    for (int i = 0; i < getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute").Count; i++)
                    {
                        if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Name")[0].InnerText == "ExtensionOf")
                        {
                            if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters").Count != 0)
                            {
                                if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters")[0].SelectNodes("AxAttributeParameter").Count != 0)
                                {
                                    for (int j = 0; j < getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters")[0].SelectNodes("AxAttributeParameter").Count; j++)
                                    {
                                        if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters")[0].SelectNodes("AxAttributeParameter")[j].SelectNodes("TypeValue")[0].InnerText.ToUpper() == ("tableStr").ToUpper())
                                        {
                                            if (!objectName.Replace(".xml", "").EndsWith("_T_Extension"))
                                            {
                                                _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's an extension of Table class";
                                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                                            }
                                        }
                                        if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters")[0].SelectNodes("AxAttributeParameter")[j].SelectNodes("TypeValue")[0].InnerText.ToUpper() == ("classStr").ToUpper())
                                        {
                                            if (!objectName.Replace(".xml", "").EndsWith("_C_Extension"))
                                            {
                                                _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's an Class extension";
                                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                                            }
                                        }
                                        if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters")[0].SelectNodes("AxAttributeParameter")[j].SelectNodes("TypeValue")[0].InnerText.ToUpper() == ("viewStr").ToUpper())
                                        {
                                            if (!objectName.Replace(".xml", "").EndsWith("_V_Extension"))
                                            {
                                                _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's an extension of View class";
                                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                                            }
                                        }
                                        if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters")[0].SelectNodes("AxAttributeParameter")[j].SelectNodes("TypeValue")[0].InnerText.ToUpper() == ("formStr").ToUpper())
                                        {
                                            if (!objectName.Replace(".xml", "").EndsWith("_F_Extension"))
                                            {
                                                _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's an extension of Form class";
                                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                                            }
                                        }
                                        if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters")[0].SelectNodes("AxAttributeParameter")[j].SelectNodes("TypeValue")[0].InnerText.ToUpper() == ("mapstr").ToUpper())
                                        {
                                            if (!objectName.Replace(".xml", "").EndsWith("_M_Extension"))
                                            {
                                                _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's an extension of Map class";
                                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                                            }
                                        }
                                        if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters")[0].SelectNodes("AxAttributeParameter")[j].SelectNodes("TypeValue")[0].InnerText.ToUpper() == ("dataentityviewstr").ToUpper())
                                        {
                                            if (!objectName.Replace(".xml", "").EndsWith("_DE_Extension"))
                                            {
                                                _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's an extension of Data Entity class";
                                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                                            }
                                        }
                                        if (getNode(_Obejct.FileNames[0], "/z:anyType/AxAttribute")[i].SelectNodes("Parameters")[0].SelectNodes("AxAttributeParameter")[j].SelectNodes("TypeValue")[0].InnerText.ToUpper() == ("dataEntityDataSourceStr").ToUpper())
                                        {
                                            if (!objectName.Replace(".xml", "").EndsWith("_DS_Extension"))
                                            {
                                                _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format : It's an extension of Data Source class";
                                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                                            }
                                        }
                                    }
                                }
                            }
                            if (!objectName.Replace(".xml", "").Contains("_" + GetModelName(_Obejct.FileNames[0], 4) + "_"))
                            {
                                _ret = "[" + objectType + "] : Object name [" + objectName.Replace(".xml", "") + "] not in correct format ";
                                AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                            }
                        }
                    }
                }
                List<String> list = new List<String>();
                list.Add("Public");
                list.Add("Protected");
                list.Add("Privete ");
                if (getNode(_Obejct.FileNames[0], "/z:anyType/Methods/Method").Count != 0)
                {
                    for (int i = 0; i < getNode(_Obejct.FileNames[0], "/z:anyType/Methods/Method").Count; i++)
                    {
                        if (!list.Contains(getNode(_Obejct.FileNames[0], "/z:anyType/Methods/Method")[i].SelectNodes("Visibility")[0].InnerText))
                        {
                            _ret = "[" + objectType + "] : Method [" + getNode(_Obejct.FileNames[0], "/z:anyType/Methods/Method")[i].SelectNodes("Name")[0].InnerText + "] in [" + objectName.Replace(".xml", "") + "] must be of Private, protected or public type";
                            AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                        }
                    }
                }
            }
            if (objectType == "AxTableExtension" || objectType == "AxMapExtension" || objectType == "AxDataEntityViewExtension" || objectType == "AxViewExtension" )
            {
                if (!objectName.EndsWith("."+modelName))
                {
                    _ret = "[" + objectType + "] : Object name [" + objectName + "] not in correct format : It's an extension Model type";
                    AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                }
            }
            if (objectType == "AxForm" || objectType == "AxMenu" || objectType == "AxMenuItemDisplay" || objectType == "AxMenuItemOutput" || objectType == "AxMenuItemAction")
            {
                if (!objectName.StartsWith(_spd))
                {
                    _ret = "[" + objectType + "] : Object name [" + objectName + "] not in correct format";
                    AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                }
                if( objectType == "AxMenuItemDisplay" || objectType == "AxMenuItemOutput" || objectType == "AxMenuItemAction")
                {
                    
                }
            }
            if (objectType == "AxFormExtension" || objectType == "AxMenuExtension" || objectType == "AxMenuItemDisplayExtension" || objectType == "AxMenuItemOutputExtension" || objectType == "AxMenuItemActionExtension")
            {
                if (!objectName.EndsWith("."+modelName))
                {
                    _ret = "[" + objectType + "] : Object name [" + objectName + "] not in correct format";
                    AnalyserRules.addMessageErrorList(_dte, _errorListProvider, _ret, _Obejct.FileNames[0], _projectName, 1, 1);
                }
            }
            return _ret;
        }
        public static bool getPropertiesbool(string _FileNames,string _maps)
        {
            if (getNode(_FileNames, _maps).Count == 0) return false;

            return true;
        }
        public static Boolean TryByProjectItem(ProjectItems _projectItems,ErrorListProvider _errorListProvider,string _spd,DTE _dte,string _projectName)
        {
            Boolean ret = false;
            string errorText = "";
            foreach (ProjectItem _object in _projectItems)
            {
                if (_object.ProjectItems.Count > 0)
                {
                     TryByProjectItem(_object.ProjectItems, _errorListProvider,_spd,_dte, _projectName);
                }
                else
                { 
                    errorText = AnalyserRules.getObjectNameRules("", _object, _spd, _errorListProvider,_dte,_projectName);
                    if(errorText != "") { ret = true; }
                }
            }
            return ret;
        }
        public static XmlNodeList getNodeByName(string _filePuth,string _node)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_filePuth);
            XmlElement root = doc.DocumentElement;
            if (!_node.Contains("z:anyType"))
            {
                return root.GetElementsByTagName(_node);
            }
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("z", "http://www.w3.org/2001/XMLSchema-instance");
            
            return root.GetElementsByTagName(_node, nsmgr.LookupNamespace("z"));
        }
        public static XmlNodeList getNode(string _filePuth, string _node)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_filePuth);
            XmlNodeList nodeList;
            XmlElement root = doc.DocumentElement;
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("z", "http://www.w3.org/2001/XMLSchema-instance");
            if (!_node.Contains("z:anyType"))
            {
                nodeList = root.SelectNodes(_node);
            }
            else
            {
                nodeList = root.SelectNodes(_node, nsmgr);
            }            
            return nodeList;
        }
       
        public static string GetModelName(string _filePuth, int _pos)
        {
            string _ret = "";
            try
            {
            _ret = _filePuth.Split('\\')[_filePuth.Split('\\').Length - _pos].ToString();
            }
            catch(Exception ex) { }
            return _ret;
        }

        static public ErrorListProvider createErrorList(Package _package)
        {
            Package vsixPackage = _package; 
            ErrorListProvider errorListProvider = new ErrorListProvider(vsixPackage);
            return errorListProvider;
        }
        public static void addMessageErrorList(DTE _dte, ErrorListProvider _errorListProvider, string _errorText, string _FileProject, string _projecName, int _lineNumber,int _ColumnNumber)
        {
            var errorCategory = TaskErrorCategory.Warning;
            IVsSolution ivsSolution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
            IVsHierarchy hierarchyItem;
            ivsSolution.GetProjectOfUniqueName(_projecName, out hierarchyItem);
            var newError = new ErrorTask()
            {
                ErrorCategory = errorCategory,
                Category = TaskCategory.BuildCompile,
                Text = _errorText,
                Document = _FileProject,
                Line = _lineNumber,
                Column = _ColumnNumber,
                HierarchyItem = hierarchyItem                 
            };
            _errorListProvider.Tasks.Add(newError); 
            _errorListProvider.Show();    
        }
        public static string getPathProprieties(string _inPath,string _objectName,int _pos,string _objecttype)
        {
            string path = _inPath;
            for(int i=1;i<_pos;i++)
            {
                path = Path.GetDirectoryName(path).TrimEnd(System.IO.Path.DirectorySeparatorChar);
            }
            path = path + System.IO.Path.DirectorySeparatorChar + "XppMetadata" + System.IO.Path.DirectorySeparatorChar + GetModelName( _inPath, 4) + System.IO.Path.DirectorySeparatorChar + _objecttype + System.IO.Path.DirectorySeparatorChar + _objectName;
            return path;
        }


    }
}
