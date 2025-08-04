namespace BlinkSwitch
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Experimental.GlobalIllumination;

    public class PostProcessGenerator
    {
        #region Public Methods
        public PostProcessGenerator(ComicBookSettings comicBookSettings,
                                    SketchEffectSettings sketchEffectSettings,
                                    OldTvSettings oldTvSettings,
                                    in Camera camera,
                                    Transform directionalLight)
        {
            _PostProcessEffects = new List<IPostProcessEffect>();
            _PostProcessEffects.Add(new ComicBookEffect(comicBookSettings, camera));
            _PostProcessEffects.Add(new SketchDrawingEffect(sketchEffectSettings, camera, directionalLight));
            _PostProcessEffects.Add(new OldTvEffect(oldTvSettings, camera));
            _PostProcessEffects.Add(new InvertScreenEffect(camera));
        }

        public IPostProcessEffect GetPostProcessEffectFromId(int postProcessEffectIndex)
        {
            if(postProcessEffectIndex == 0)
            {
                return _PostProcessEffects[0];
            }
            if(postProcessEffectIndex == 1)
            {
                return _PostProcessEffects[1];
            }
            if (postProcessEffectIndex == 2)
            {
                return _PostProcessEffects[2];
            }
            if(postProcessEffectIndex == 3)
            {
                return _PostProcessEffects[3];
            }
            return _PostProcessEffects[0];
        }

        public int GetPostProcessEffectsCounter()
        {
            return _PostProcessEffects.Count;
        }

        #endregion Public Methods

        #region Private Variables
        private List<IPostProcessEffect> _PostProcessEffects;
        #endregion Private Variables
    }
}
