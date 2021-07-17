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


def predict(model, classes, image_file_paths, image_size):
    if image_file_paths is None or len(image_file_paths) == 0:
        return None

    images = []
    results = []

    for path in image_file_paths:
        if not os.path.exists(path):
            results.append({
                "path": path,
                "error": True,
                "message": "File doesn't exists"
            })
            continue

        img = Image.load_img(path, target_size=image_size)
        x = Image.img_to_array(img)
        x = np.expand_dims(x, axis=0)

        images.append(x)

    if len(images) == 0:
        return results

    images = np.vstack(images)
    predictions = model.predict(images)

    for prediction in predictions:
        score = tf.nn.softmax(prediction)
        confidence = np.max(score)
        result = classes[np.argmax(score)]
        results.append({
            "path": path,
            "error": False,
            "prediction": result
        })

    return results
