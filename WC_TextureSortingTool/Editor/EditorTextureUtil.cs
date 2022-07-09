
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// <see cref="UnityEditor.TextureUtil"/> Accessor
/// </summary>
/// <author>Seibe TAKAHASHI</author>
/// <remarks>
/// (c) 2017 Seibe TAKAHASHI.
/// This code is released under the MIT License.
/// http://opensource.org/licenses/mit-license.php
/// </remarks>
public static class EditorTextureUtil
{
    private static readonly System.Type cType;
    private static MethodInfo mMethod_GetRuntimeMemorySizeLong;
    private static MethodInfo mMethod_GetStorageMemorySizeLong;

    static EditorTextureUtil()
    {
        cType = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
        Assert.IsNotNull(cType);
    }

    public static long GetRuntimeMemorySize(Texture texture)
    {
        if (mMethod_GetRuntimeMemorySizeLong == null)
            mMethod_GetRuntimeMemorySizeLong = cType.GetMethod("GetRuntimeMemorySizeLong", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(mMethod_GetRuntimeMemorySizeLong);
        return (long)mMethod_GetRuntimeMemorySizeLong.Invoke(null, new[] { texture });
    }

    public static long GetStorageMemorySize(Texture texture)
    {
        if (mMethod_GetStorageMemorySizeLong == null)
            mMethod_GetStorageMemorySizeLong = cType.GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(mMethod_GetStorageMemorySizeLong);
        return (long)mMethod_GetStorageMemorySizeLong.Invoke(null, new object[] {texture});
    }
}