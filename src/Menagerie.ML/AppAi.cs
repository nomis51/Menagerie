using System;
using System.Collections.Generic;
using Menagerie.ML.Models;

namespace Menagerie.ML
{
    public class AppAi
    {
        #region Members

        private readonly IntPtr _poeWindowHandle;
        private readonly Dictionary<ModelType, CnnModelV1> _cnnModelV1s;

        #endregion

        #region Constructors

        public AppAi(IntPtr poeWindowHandle)
        {
            _poeWindowHandle = poeWindowHandle;
            _cnnModelV1s = new Dictionary<ModelType, CnnModelV1>()
            {
                { ModelType.CurrencyType, new CurrencyTypeModel() }
            };
        }

        #endregion

        #region Public methods

        public string Predict(ModelType type)
        {
            var image = ScreenCapture.CaptureWindow(_poeWindowHandle);
            return _cnnModelV1s[type].Predict(image);
        }

        #endregion
    }

    public enum ModelType
    {
        CurrencyType
    }
}