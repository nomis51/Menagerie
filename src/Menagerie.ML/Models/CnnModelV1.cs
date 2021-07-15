using System;
using System.Drawing;
using NumSharp;
using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace Menagerie.ML.Models
{
    public class CnnModelV1 : BaseModel
    {
        #region Members

        private string ModelFilePath;
        private Functional Model;
        private Tensor InputShape;
        private Size ImageSize;

        #endregion

        #region Constructors

        public CnnModelV1(string modelFilePath, string classesFilePath, TensorShape inputShape) : base(classesFilePath)
        {
            ModelFilePath = modelFilePath;
            ImageSize = new Size(inputShape[0], inputShape[1]);
            InputShape = new Tensor(inputShape);
            BuildsCnn();
            LoadModel();
        }

        #endregion

        #region Public methods

        public string Predict(string filePath)
        {
            var imageArray = ReadTensorFromImageFile(filePath, ImageSize.Height, ImageSize.Width);
            var predictions = Model.predict(imageArray);
            return ReadFirstPrediction(predictions);
        }

        public string Predict(Bitmap image)
        {
            var imageArray = ReadTensorFromImage(image, ImageSize.Height, ImageSize.Width);
            var predictions = Model.predict(imageArray);
            return ReadFirstPrediction(predictions);
        }

        #endregion

        #region Private methods

        private string ReadFirstPrediction(Tensors predictions)
        {
            var score = tf.nn.softmax(predictions[0]).ToArray<int>();
            return Classes[np.argmax(score)];
        }

        private void BuildsCnn()
        {
            var (inputs, outputs) = GetInputsOutputs();
            Model = keras.Model(inputs, outputs);
            Model.compile(optimizer: keras.optimizers.RMSprop(), loss: keras.losses.CategoricalCrossentropy(), metrics: new[] {"acc"});
        }

        private void LoadModel()
        {
            // https://github.com/SciSharp/TensorFlow.NET/issues/814#issuecomment-880765473
            tf.enable_eager_execution();
            Model.load_weights(ModelFilePath);
        }

        private Tuple<Tensor, Tensors> GetInputsOutputs()
        {
            var layers = new LayersApi();

            var inputs = keras.Input(shape: (46, 46, 3));

            var x = layers.Conv2D(16, new TensorShape(3, 3), activation: "relu").Apply(inputs);
            x = layers.MaxPooling2D(new TensorShape(2, 2)).Apply(x);

            x = layers.Conv2D(32, activation: "relu").Apply(x);
            x = layers.MaxPooling2D(new TensorShape(2, 2)).Apply(x);

            x = layers.Conv2D(64, activation: "relu").Apply(x);
            x = layers.MaxPooling2D(new TensorShape(2, 2)).Apply(x);

            x = layers.Flatten().Apply(x);

            x = layers.Dense(512, activation: "relu").Apply(x);
            x = layers.Dense(Classes.Count, activation: "softmax").Apply(x);

            return new Tuple<Tensor, Tensors>(inputs, x);
        }

        #endregion
    }
}