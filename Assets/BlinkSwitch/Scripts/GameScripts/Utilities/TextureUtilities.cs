namespace BlinkSwitch
{
    using UnityEngine;

    public sealed class TextureUtilities
    {
        public static void ReleaseTexture(RenderTexture texture)
        {
            if(texture == null)
            {
                return;
            }
            texture.Release();
            texture = null;
        }

        public static RenderTexture CreateTextureClampPoint(int width, int height, float depth, bool supportMipMaps = true)
        {
            RenderTexture result = new RenderTexture(width, height, (int)depth)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
                enableRandomWrite = true,
                autoGenerateMips = supportMipMaps,
                useMipMap = supportMipMaps
            };
            result.Create();
            return result;
        }

        public static RenderTexture CreateTextureBilinearClamp(int width, int height, float depth, bool supportMipMaps = false)
        {
            RenderTexture result = new RenderTexture(width, height, (int)depth)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                enableRandomWrite = true,
                autoGenerateMips = supportMipMaps,
                useMipMap = supportMipMaps
            };
            result.Create();
            return result;
        }
    }
}
