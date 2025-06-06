﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Unity.XR.PXR;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.OpenXR.Features;
#if UNITY_XR_HAND
using UnityEngine.XR.Hands.OpenXR;
#endif
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;


namespace Unity.XR.OpenXR.Features.PICOSupport
{
    internal class PICOModifyAndroidManifest : OpenXRFeatureBuildHooks
    {
        public override int callbackOrder => 1;
        public override Type featureType => typeof(PICOFeature);
        protected override void OnPreprocessBuildExt(BuildReport report) { }
        protected override void OnPostGenerateGradleAndroidProjectExt(string path)
        {
            var androidManifest = new AndroidManifest(GetManifestPath(path));
            androidManifest.AddPICOMetaData(path);
            androidManifest.Save();
        }
        protected override void OnPostprocessBuildExt(BuildReport report) { }
        private string _manifestFilePath;
        private string GetManifestPath(string basePath)
        {
            if (!string.IsNullOrEmpty(_manifestFilePath)) return _manifestFilePath;
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            _manifestFilePath = pathBuilder.ToString();

            return _manifestFilePath;
        }
        private class AndroidXmlDocument : XmlDocument
        {
            private string m_Path;
            protected XmlNamespaceManager nsMgr;
            public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

            public AndroidXmlDocument(string path)
            {
                m_Path = path;
                using (var reader = new XmlTextReader(m_Path))
                {
                    reader.Read();
                    Load(reader);
                }
                nsMgr = new XmlNamespaceManager(NameTable);
                nsMgr.AddNamespace("android", AndroidXmlNamespace);
            }
            public string Save()
            {
                return SaveAs(m_Path);
            }
            public string SaveAs(string path)
            {
                using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
                {
                    writer.Formatting = Formatting.Indented;
                    Save(writer);
                }
                return path;
            }
        }
        private class AndroidManifest : AndroidXmlDocument
        {
            private readonly XmlElement ApplicationElement;
            private readonly XmlElement ManifestElement;
            public AndroidManifest(string path) : base(path)
            {
                ManifestElement = SelectSingleNode("/manifest") as XmlElement;
                ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
            }
            private XmlAttribute CreateOrUpdateAndroidAttribute(string key, string value)
            {
                XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
                attr.Value = value;
                return attr;
            }
            private void CreateOrUpdateAndroidPermissionData(string name)
            {
                XmlNodeList nodeList = ManifestElement.SelectNodes("uses-permission");
                foreach (XmlNode node in nodeList)
                {
                    if (node != null)
                    {
                        // Update existing nodes
                        if (node.Attributes != null && name.Equals(node.Attributes[0].Value))
                        {
                            return;
                        }
                    }
                }

                // Create new node
                var md = ManifestElement.AppendChild(CreateElement("uses-permission"));
                md.Attributes.Append(CreateOrUpdateAndroidAttribute("name", name.ToString()));
            }

            private void DeleteAndroidPermissionData(string name)
            {
                XmlNodeList nodeList = ManifestElement.SelectNodes("uses-permission");
                foreach (XmlNode node in nodeList)
                {
                    if (node != null)
                    {
                        // Delete existing nodes
                        if (node.Attributes != null && name.Equals(node.Attributes[0].Value))
                        {
                            node.ParentNode?.RemoveChild(node);
                            return;
                        }
                    }
                }
            }

            private void CreateOrUpdateAndroidMetaData(string name, string value)
            {
                XmlNodeList nodeList = ApplicationElement.SelectNodes("meta-data");
                foreach (XmlNode node in nodeList)
                {
                    if (node != null)
                    {
                        // Update existing nodes
                        if (node.Attributes != null && name.Equals(node.Attributes[0].Value))
                        {
                            node.Attributes[0].Value = name;
                            node.Attributes[1].Value = value;
                            return;
                        }
                    }
                }

                // Create new node
                var md = ApplicationElement.AppendChild(CreateElement("meta-data"));
                md.Attributes.Append(CreateOrUpdateAndroidAttribute("name", name.ToString()));
                md.Attributes.Append(CreateOrUpdateAndroidAttribute("value", value.ToString()));
            }

            private void DeleteAndroidMetaData(string name)
            {
                XmlNodeList nodeList = ApplicationElement.SelectNodes("meta-data");
                foreach (XmlNode node in nodeList)
                {
                    if (node != null)
                    {
                        // Delete existing nodes
                        if (node.Attributes != null && name.Equals(node.Attributes[0].Value))
                        {
                            node.ParentNode?.RemoveChild(node);
                            return;
                        }
                    }
                }
            }

            internal void AddPICOMetaData(string path)
            {
                CreateOrUpdateAndroidMetaData("pvr.app.type", "vr");
                CreateOrUpdateAndroidMetaData("pvr.sdk.version", "Unity OpenXR "+PICOFeature.SDKVersion);
                CreateOrUpdateAndroidMetaData("pxr.sdk.version_code", "5110");
                var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
                bool mrPermission = false;

                foreach (var feature in settings.GetFeatures<OpenXRFeature>())
                {
                    if (feature is BodyTrackingFeature)
                    {
                        if (feature.enabled)
                        {
                            CreateOrUpdateAndroidMetaData("PICO.swift.feature", "1");

                            mrPermission = true;
                        }
                        else
                        {
                            DeleteAndroidMetaData("enable_scene_anchor");
                        }
                    }
                    if (feature is PICOSceneCapture)
                    {
                        if (feature.enabled)
                        {
                            CreateOrUpdateAndroidMetaData("enable_scene_anchor", "1");

                            mrPermission = true;
                        }
                        else
                        {
                            DeleteAndroidMetaData("enable_scene_anchor");
                        }
                    }

                    if (feature is PICOSpatialAnchor)
                    {
                        if (feature.enabled)
                        {
                            CreateOrUpdateAndroidMetaData("enable_spatial_anchor", "1");
                            mrPermission = true;
                        }
                        else
                        {
                            DeleteAndroidMetaData("enable_spatial_anchor");
                        }
                    }

                    if (feature is PICOSpatialMesh)
                    {
                        if (feature.enabled)
                        {
                            CreateOrUpdateAndroidMetaData("enable_mesh_anchor", "1");
                            mrPermission = true;
                        }
                        else
                        {
                            DeleteAndroidMetaData("enable_mesh_anchor");
                        }
                    }
#if UNITY_XR_HAND
                    if (feature is HandTracking)
                    {
                        if (feature.enabled)
                        {
                            if (PICOProjectSetting.GetProjectConfig().isHandTracking)
                            {
                                CreateOrUpdateAndroidPermissionData("com.picovr.permission.HAND_TRACKING");
                                CreateOrUpdateAndroidMetaData("handtracking", "1");
                                if (PICOProjectSetting.GetProjectConfig().handTrackingSupportType == HandTrackingSupport.HandsOnly)
                                {
                                    CreateOrUpdateAndroidMetaData("handtracking", "1");
                                    DeleteAndroidMetaData("controller");

                                }
                                else
                                {
                                    CreateOrUpdateAndroidMetaData("handtracking", "1");
                                    CreateOrUpdateAndroidMetaData("controller", "1");
                                }
                                CreateOrUpdateAndroidMetaData("Hand_Tracking_HighFrequency", PICOProjectSetting.GetProjectConfig().highFrequencyHand ? "1" : "0");
                            }
                            else
                            {
                                DeleteAndroidPermissionData("com.picovr.permission.HAND_TRACKING");
                                DeleteAndroidMetaData("handtracking");
                                DeleteAndroidMetaData("Hand_Tracking_HighFrequency");
                                CreateOrUpdateAndroidMetaData("controller", "1");
                            }
                        }
                        else
                        {
                            DeleteAndroidPermissionData("com.picovr.permission.HAND_TRACKING");
                            DeleteAndroidMetaData("handtracking");
                            DeleteAndroidMetaData("Hand_Tracking_HighFrequency");
                            CreateOrUpdateAndroidMetaData("controller", "1");
                        }
                    }
                }
#else
                }

                DeleteAndroidPermissionData("com.picovr.permission.HAND_TRACKING");
                DeleteAndroidMetaData("handtracking");
                CreateOrUpdateAndroidMetaData("controller", "1");
#endif
                if (PICOProjectSetting.GetProjectConfig().isEyeTracking)
                {
                    CreateOrUpdateAndroidPermissionData("com.picovr.permission.EYE_TRACKING");
                    CreateOrUpdateAndroidMetaData("picovr.software.eye_tracking", "1");
                    CreateOrUpdateAndroidMetaData("eyetracking_calibration", PICOProjectSetting.GetProjectConfig().isEyeTrackingCalibration ? "true" : "false");
                }
                else
                {
                    DeleteAndroidPermissionData("com.picovr.permission.EYE_TRACKING");
                    DeleteAndroidMetaData("picovr.software.eye_tracking");
                    DeleteAndroidMetaData("eyetracking_calibration");
                }
                
            
                if (PICOProjectSetting.GetProjectConfig().MRSafeguard)
                {
                    CreateOrUpdateAndroidMetaData("enable_mr_safeguard", PICOProjectSetting.GetProjectConfig().MRSafeguard ? "1" : "0");
                }
                else
                {
                    DeleteAndroidMetaData("enable_mr_safeguard");
                }

                if (mrPermission)
                {
                    CreateOrUpdateAndroidPermissionData("com.picovr.permission.SPATIAL_DATA");
                }
                else
                {
                    DeleteAndroidPermissionData("com.picovr.permission.SPATIAL_DATA");
                }

                CreateOrUpdateAndroidMetaData("pvr.app.splash", PICOProjectSetting.GetProjectConfig().GetSystemSplashScreen(path));
            }
        }
    }
}