namespace BlinkSwitch
{
    using UnityEngine;

    public interface IPostProcessEffect 
    {
        RenderTexture GeneratePostProcess(RenderTexture source);

        //TODO: remove this update method
        void Update();
        void Setup();
        void Refresh();
    }
}
