namespace BlinkSwitch
{
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class MainCameraPostProcess : MonoBehaviour
    {
        #region Unity Methods
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(null, destination);
        }
        #endregion Unity Methods

        #region Private Variables
        private List<PostProcess> _PostProcesses;
        #endregion Private Variables
    }
}
