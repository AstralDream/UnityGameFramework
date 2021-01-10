using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Resource
{
    public enum ResourceType
    {
        ASSET,
        PREFAB,
        LEVELASSET,
        LEVEL,
    }
    // IDisposable用于释放托管资源，当不再使用托管对象时
    // 垃圾回收会自动释放分配给该对象的内存
    public class ResourceUnit : IDisposable
    {
        private string mPath;
        private Object mAsset;
        private ResourceType mResourceType;
        private List<ResourceUnit> mNextLevelAssets;
        private AssetBundle mAssetBundle;
        private int mAssetBundleSize;
        private int mReferenceCount; // 引用技术

        /// <summary>
        /// 内部构造函数
        /// </summary>
        /// <param name="assetBundle">资源</param>
        /// <param name="assetBundleSize">资源大小</param>
        /// <param name="asset">对象</param>
        /// <param name="path">加载路径</param>
        /// <param name="resourceType">资源类型</param>
        internal ResourceUnit(AssetBundle assetBundle, int assetBundleSize, Object asset, string path, ResourceType resourceType/*, int allDependencesAssetSize*/)
        {
            mPath = path;
            mAsset = asset;
            mResourceType = resourceType;
            mNextLevelAssets = new List<ResourceUnit>();
            mAssetBundle = assetBundle;
            mAssetBundleSize = assetBundleSize;
            mReferenceCount = 0;
            //private int mAllDependencesAssetSize;
        }

        #region 所有私有变量进行set get设置
        public Object Asset
        {
            get { return mAsset;}
            set { mAsset = value; }
        }

        public ResourceType resourceType
        {
            get { return mResourceType; }
        }

        public List<ResourceUnit> NextLevelAssets
        {
            get{ return mNextLevelAssets;}
            set
            {
                foreach (ResourceUnit asset in value)
                {
                    mNextLevelAssets.Add(asset);
                }
            }
        }

        public AssetBundle Assetbundle
        {
            get{ return mAssetBundle; }
            set{  mAssetBundle = value; }
        }

        public int AssetBundleSize
        {
            get { return mAssetBundleSize; }
        }

        public int ReferenceCount
        {
            get { return mReferenceCount; }
        }
        #endregion

        public void dumpNextLevel()
        {
            string info = mPath + " the mReferenceCount : " + mReferenceCount + "\n";
            foreach (ResourceUnit ru in mNextLevelAssets)
            {
                ru.dumpNextLevel();
                info += ru.mPath + "\n";
            }
            DebugEx.Log(info, ResourceCommon.DEBUGTYPENAME);
        }

        #region 引用计数自增与自减
        public void addReferenceCount()
        {
            ++mReferenceCount;
            foreach (ResourceUnit asset in mNextLevelAssets)
            {
                asset.addReferenceCount();
            }
        }

        public bool isCanDestory() { return (0 == mReferenceCount); }

        public void reduceReferenceCount()
        {
            --mReferenceCount;

            foreach (ResourceUnit asset in mNextLevelAssets)
            {
                asset.reduceReferenceCount();
            }
            if (isCanDestory())
            {
                //ResourcesManager.Instance.mLoadedResourceUnit.Remove(ResourceCommon.getFileName(mPath, true));
                Dispose();
            }
        }

        public void Dispose()
        {
            ResourceCommon.Log("Destory " + mPath);

            if (null != mAssetBundle)
            {
                //mAssetBundle.Unload(true);
                mAssetBundle = null;
            }
            mNextLevelAssets.Clear();
            mAsset = null;
        }
        #endregion


    }
}
