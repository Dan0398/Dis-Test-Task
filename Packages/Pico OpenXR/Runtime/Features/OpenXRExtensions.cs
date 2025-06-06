﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
#if AR_FOUNDATION_5||AR_FOUNDATION_6
using UnityEngine.XR.ARSubsystems;
#endif
using UnityEngine.XR.OpenXR.Features;
#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif


namespace Unity.XR.OpenXR.Features.PICOSupport
{
#if UNITY_EDITOR
    public class FeatureConfig
    {
        public const string OpenXrExtensionList = "XR_EXT_local_floor " +
                                                  "XR_FB_triangle_mesh " +
                                                  "XR_FB_composition_layer_alpha_blend " +
                                                  "XR_KHR_composition_layer_color_scale_bias " +
                                                  "XR_KHR_composition_layer_cylinder " +
                                                  "XR_KHR_composition_layer_equirect " +
                                                  "XR_KHR_composition_layer_cube";
    }

    [OpenXRFeature(UiName = "PICO OpenXR Features",
        Desc = "PICO XR Features for OpenXR.",
        Company = "PICO",
        Version = "1.0.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Android },
        OpenxrExtensionStrings = FeatureConfig.OpenXrExtensionList,
        FeatureId = featureId
    )]
#endif
    public class OpenXRExtensions : OpenXRFeature
    {
        public const string featureId = "com.unity.openxr.pico.features";
        private static ulong xrInstance = 0ul;
        private static ulong xrSession = 0ul;
        public static event Action<ulong> SenseDataUpdated;
        public static event Action SpatialAnchorDataUpdated;
        public static event Action SceneAnchorDataUpdated;
        
        public static event Action<PxrEventSenseDataProviderStateChanged> SenseDataProviderStateChanged;
        public static event Action<List<PxrSpatialMeshInfo>> SpatialMeshDataUpdated;
        
        static bool isCoroutineRunning = false;
        protected override bool OnInstanceCreate(ulong instance)
        {
            xrInstance = instance;
            xrSession = 0ul;
            return true;
        }

        /// <inheritdoc/>
        protected override void OnSessionCreate(ulong xrSessionId)
        {
            xrSession = xrSessionId;
            Initialize(xrGetInstanceProcAddr, xrInstance, xrSession);
            Pxr_SetEventDataBufferCallBack(XrEventDataBufferFunction);
            setColorSpace((int)QualitySettings.activeColorSpace);
            base.OnSessionCreate(xrSessionId);
        
        }

        /// <inheritdoc/>
        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            xrInstance = 0ul;
        }

        // HookGetInstanceProcAddr
        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            // return base.HookGetInstanceProcAddr(func);
            return HookCreateInstance(func);
        }

        /// <inheritdoc/>
        protected override void OnSessionDestroy(ulong xrSessionId)
        {
            base.OnSessionDestroy(xrSessionId);
            xrSession = 0ul;
        }

        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            SpaceChange(xrSpace);
            base.OnAppSpaceChange(xrSpace);
        }

        public static int GetReferenceSpaceBoundsRect(XrReferenceSpaceType referenceSpace, ref XrExtent2Df extent2D)
        {
            return xrGetReferenceSpaceBoundsRect(
                xrSession, referenceSpace, ref extent2D);
        }

        public static XrReferenceSpaceType[] EnumerateReferenceSpaces()
        {
            UInt32 Output = 0;
            XrReferenceSpaceType[] outSpaces = null;
            xrEnumerateReferenceSpaces(xrSession, 0, ref Output, outSpaces);
            if (Output <= 0)
            {
                return null;
            }

            outSpaces = new XrReferenceSpaceType[Output];
            xrEnumerateReferenceSpaces(xrSession, Output, ref Output, outSpaces);
            return outSpaces;
        }

        public static void CreateLayerParam(PxrLayerParam layerParam)
        {
            PLog.i("POXR_CreateLayerParam() ");
#if UNITY_ANDROID && !UNITY_EDITOR
                xrCreateLayerParam(layerParam);
#endif
        }

        public static int GetLayerImageCount(int layerId, EyeType eye, ref UInt32 imageCount)
        {
            int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = xrGetLayerImageCount(layerId, eye, ref imageCount);
#endif
            PLog.i("GetLayerImageCount() layerId:" + layerId + " eye:" + eye + " imageCount:" + imageCount +
                " result:" + result);
            return result;
        }

        public static void GetLayerImagePtr(int layerId, EyeType eye, int imageIndex, ref IntPtr image)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                xrGetLayerImagePtr(layerId, eye, imageIndex, ref image);
#endif
            PLog.i("GetLayerImagePtr() layerId:" + layerId + " eye:" + eye + " imageIndex:" + imageIndex + " image:" +
                image);
        }

        public static void DestroyLayerByRender(int layerId)
        {
            PLog.i("DestroyLayerByRender() layerId:" + layerId);
#if UNITY_ANDROID && !UNITY_EDITOR
                xrDestroyLayerByRender(layerId);
#endif
        }

        public static bool SubmitLayerQuad(IntPtr ptr)
        {
            int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = xrSubmitLayerQuad(ptr);
#endif
            PLog.d("SubmitLayerQuad() ptr:" + ptr + " result:" + result);
            return result == -8;
        }

        public static bool SubmitLayerCylinder(IntPtr ptr)
        {
            int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = xrSubmitLayerCylinder(ptr);
#endif
            PLog.d("SubmitLayerCylinder() ptr:" + ptr + " result:" + result);
            return result == -8;
        }

        public static bool SubmitLayerEquirect(IntPtr ptr)
        {
            int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = xrSubmitLayerEquirect(ptr);
#endif
            PLog.d("SubmitLayerEquirect() ptr:" + ptr + " result:" + result);
            return result == -8;
        }

        public static bool SubmitLayerCube(IntPtr ptr)
        {
            int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = xrSubmitLayerCube(ptr);
#endif
            PLog.d("xrSubmitLayerCube() ptr:" + ptr + " result:" + result);
            return result == -8;
        }

        public static int GetLayerNextImageIndex(int layerId, ref int imageIndex)
        {
            int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                result = xrGetLayerNextImageIndex(layerId, ref imageIndex);
#endif
            PLog.d("GetLayerNextImageIndex() layerId:" + layerId + " imageIndex:" + imageIndex + " result:" + result);
            return result;
        }

        protected override void OnSystemChange(ulong xrSystem)
        {
            base.OnSystemChange(xrSystem);
            SystemChange(xrSystem);
        }

        public static float GetLocationHeight()
        {
            float height = 0;
            getLocationHeight( ref height);
            return height;
        }
        
        public static int GetControllerType()
        {
            int type = 0;
            Pxr_GetControllerType(ref type);
            return type;
        }
        
        [MonoPInvokeCallback(typeof(XrEventDataBufferCallBack))]
        static void XrEventDataBufferFunction(ref XrEventDataBuffer eventDB)
        {
            int status, action;
            PLog.i($"XrEventDataBufferFunction eventType={eventDB.type}");
            switch (eventDB.type)
            {
                case XrStructureType.XR_TYPE_EVENT_DATA_SENSE_DATA_PROVIDER_STATE_CHANGED:
                {
                    if (SenseDataProviderStateChanged != null)
                    {
                        PxrEventSenseDataProviderStateChanged data = new PxrEventSenseDataProviderStateChanged()
                        {
                            providerHandle = BitConverter.ToUInt64(eventDB.data, 0),
                            newState = (PxrSenseDataProviderState)BitConverter.ToInt32(eventDB.data, 8),
                        };
                        SenseDataProviderStateChanged(data);
                    }
                    break;
                }
                case XrStructureType.XR_TYPE_EVENT_DATA_SENSE_DATA_UPDATED:
                {
                    ulong providerHandle = BitConverter.ToUInt64(eventDB.data, 0);
                    PLog.i($"providerHandle ={providerHandle}");
                    if (SenseDataUpdated != null)
                    {
                        SenseDataUpdated(providerHandle);
                    }

                    if (providerHandle == PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SpatialAnchor))
                    {
                        if (SpatialAnchorDataUpdated != null)
                        {
                            SpatialAnchorDataUpdated();
                        }
                    }

                    if (providerHandle == PXR_Plugin.MixedReality.UPxr_GetSenseDataProviderHandle(PxrSenseDataProviderType.SceneCapture))
                    {
                        if (SceneAnchorDataUpdated != null)
                        {
                            SceneAnchorDataUpdated();
                        }
                    }

                    if (providerHandle == PXR_Plugin.MixedReality.UPxr_GetSpatialMeshProviderHandle())
                    {
                        if (!isCoroutineRunning)
                        {
                            QuerySpatialMeshAnchor();
                        }
                    }

                    break;
                }
            }
        }


        static async void QuerySpatialMeshAnchor()
        {
            isCoroutineRunning = true;
            var task = await PXR_MixedReality.QueryMeshAnchorAsync();
            isCoroutineRunning = false;
            var (result, meshInfos) = task;
            for (int i = 0; i < meshInfos.Count; i++)
            {
                switch (meshInfos[i].state)
                {
                    case MeshChangeState.Added:
                    case MeshChangeState.Updated:
                    {
                        PXR_Plugin.MixedReality.UPxr_AddOrUpdateMesh(meshInfos[i]);
                    }
                        break;
                    case MeshChangeState.Removed:
                    {
                        PXR_Plugin.MixedReality.UPxr_RemoveMesh(meshInfos[i].uuid);
                    }
                        break;
                    case MeshChangeState.Unchanged:
                    {
                        break;
                    }
                }
            }

            if (result == PxrResult.SUCCESS)
            {
                SpatialMeshDataUpdated?.Invoke(meshInfos);
            }
        }
#if AR_FOUNDATION_5||AR_FOUNDATION_6
        public  bool isSessionSubsystem=false;
        private static List<XRSessionSubsystemDescriptor> sessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            if (isSessionSubsystem)
            {
                CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(sessionSubsystemDescriptors, PICOSessionSubsystem.k_SubsystemId);
            }
        }
        protected override void OnSubsystemStart()
        {
            if (isSessionSubsystem)
            {
                StartSubsystem<XRHumanBodySubsystem>();
            }
        }
        protected override void OnSubsystemStop()
        {
            if (isSessionSubsystem)
            {
                StopSubsystem<XRHumanBodySubsystem>();
            }
        }
        protected override void OnSubsystemDestroy()
        {
            if (isSessionSubsystem)
            {
                DestroySubsystem<XRHumanBodySubsystem>();
            }
        }
#endif
        
        const string extLib = "openxr_pico";

        [DllImport(extLib, EntryPoint = "PICO_Initialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Initialize(IntPtr xrGetInstanceProcAddr, ulong xrInstance, ulong xrSession);

        [DllImport(extLib, EntryPoint = "PICO_HookCreateInstance", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr HookCreateInstance(IntPtr func);

        [DllImport(extLib, EntryPoint = "PICO_OnAppSpaceChange", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceChange(ulong xrSession);
        [DllImport(extLib, EntryPoint = "PICO_OnSystemChange", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SystemChange(ulong xrSystem);

        [DllImport(extLib, EntryPoint = "PICO_xrEnumerateReferenceSpaces", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xrEnumerateReferenceSpaces(ulong xrSession, UInt32 CountInput, ref UInt32 CountOutput,
            XrReferenceSpaceType[] Spaces);

        [DllImport(extLib, EntryPoint = "PICO_xrGetReferenceSpaceBoundsRect", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xrGetReferenceSpaceBoundsRect(ulong xrSession, XrReferenceSpaceType referenceSpace,
            ref XrExtent2Df extent2D);

        [DllImport(extLib, EntryPoint = "PICO_CreateLayerParam", CallingConvention = CallingConvention.Cdecl)]
        private static extern void xrCreateLayerParam(PxrLayerParam layerParam);

        [DllImport(extLib, EntryPoint = "PICO_GetLayerImageCount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xrGetLayerImageCount(int layerId, EyeType eye, ref UInt32 imageCount);

        [DllImport(extLib, EntryPoint = "PICO_GetLayerImagePtr", CallingConvention = CallingConvention.Cdecl)]
        public static extern void xrGetLayerImagePtr(int layerId, EyeType eye, int imageIndex, ref IntPtr image);

        [DllImport(extLib, EntryPoint = "PICO_DestroyLayerByRender", CallingConvention = CallingConvention.Cdecl)]
        private static extern void xrDestroyLayerByRender(int layerId);

        [DllImport(extLib, EntryPoint = "PICO_SubmitLayerQuad", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xrSubmitLayerQuad(IntPtr ptr);

        [DllImport(extLib, EntryPoint = "PICO_SubmitLayerCylinder", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xrSubmitLayerCylinder(IntPtr ptr);

        [DllImport(extLib, EntryPoint = "PICO_SubmitLayerEquirect", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xrSubmitLayerEquirect(IntPtr ptr);

        [DllImport(extLib, EntryPoint = "PICO_SubmitLayerCube", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xrSubmitLayerCube(IntPtr ptr);

        [DllImport(extLib, EntryPoint = "PICO_GetLayerNextImageIndex", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xrGetLayerNextImageIndex(int layerId, ref int imageIndex);

        [DllImport(extLib, EntryPoint = "PICO_SetColorSpace", CallingConvention = CallingConvention.Cdecl)]
        private static extern int setColorSpace(int colorSpace);
        [DllImport(extLib, EntryPoint = "PICO_GetLocationHeight", CallingConvention = CallingConvention.Cdecl)]
        private static extern XrResult getLocationHeight(ref float delaY);
        [DllImport(extLib, EntryPoint = "PICO_SetMarkMode", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMarkMode();
        [DllImport(extLib,  CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_GetControllerType(ref int type);
        [DllImport(extLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_SetEventDataBufferCallBack(XrEventDataBufferCallBack callback);
    }
}