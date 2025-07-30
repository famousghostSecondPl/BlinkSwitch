namespace BlinkSwitch
{
    using UnityEngine;

    public interface IPostProcessEffect 
    {
        RenderTexture GeneratePostProcess(RenderTexture source);
        void Setup();
        void Refresh();
    }
}
