import os
import numpy as np
import tensorflow as tf
from tensorflow.keras.preprocessing import image as Image

os.environ['CUDA_VISIBLE_DEVICES'] = '-1'


def load_classes(classes_file_path):
    if not os.path.exists(classes_file_path):
        return None

    f = open(classes_file_path, "r")
    data = f.read()
    return data.split(",")[:-1]


def predict(model, classes, image_file_path, image_size):
    if not os.path.exists(image_file_path):
        return None

    img = Image.load_img(image_file_path, target_size=image_size)
    x = Image.img_to_array(img)
    x = np.expand_dims(x, axis=0)
    images = np.vstack([x])

    predictions = model.predict(images)
    score = tf.nn.softmax(predictions[0])
    confidence = np.max(score)
    result = classes[np.argmax(score)]

    return result
