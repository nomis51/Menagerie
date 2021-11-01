import os
import numpy as np
import tensorflow as tf
import cv2
from tensorflow.keras.preprocessing import image as Image
from currency_type import load_model as currency_type_load_model, \
    classes_file_path as currency_type_file_path, \
    image_size as currency_type_image_size
from stack_size import load_model as stack_size_load_model, \
    classes_file_path as stack_size_classes_file_path, \
    image_size as stack_size_image_size
from flask import make_response, \
    jsonify

os.environ['CUDA_VISIBLE_DEVICES'] = '-1'


def load_models():
    models = dict()
    currency_type_classes = load_classes(currency_type_file_path)
    stack_size_classes = load_classes(stack_size_classes_file_path)

    if currency_type_classes is not None:
        models["currency_type"] = {
            "model": currency_type_load_model(len(currency_type_classes)),
            "classes": currency_type_classes,
            "image_size": currency_type_image_size
        }

    if stack_size_classes is not None:
        models["stack_size"] = {
            "model": stack_size_load_model(len(stack_size_classes)),
            "classes": stack_size_classes,
            "image_size": stack_size_image_size
        }

    return models


def create_error(message):
    return make_response(
        jsonify({
            "error": True,
            "message": message
        }),
        400
    )


def create_image_field_error():
    return {
        "error": True,
        "message": "filed_id, file_path or models field is missing"
    }


def load_classes(classes_file_path):
    if not os.path.exists(classes_file_path):
        return None

    f = open(classes_file_path, "r")
    data = f.read()
    return data.split(",")[:-1]


def is_empty_image(empty_image, test_image_path):
    EMPTY_IMAGE_THRESHOLD = 0.15
    EMPTY_IMAGE_SIZE = (46, 46)

    test_image = cv2.imread(test_image_path, cv2.IMREAD_UNCHANGED)

    if test_image.shape[0] > empty_image.shape[0] or test_image.shape[1] > empty_image.shape[1]:
        test_image = cv2.resize(test_image, EMPTY_IMAGE_SIZE, interpolation=cv2.INTER_AREA)

    match = cv2.matchTemplate(empty_image, test_image, cv2.TM_CCOEFF_NORMED)
    min_val, max_val, min_loc, max_loc = cv2.minMaxLoc(match)

    return True if max_val >= EMPTY_IMAGE_THRESHOLD else False


def load_empty_image():
    empty_file_path = "./images/empty.jpeg"
    empty_image = cv2.imread(empty_file_path, cv2.IMREAD_UNCHANGED)
    return empty_image


def predict(model, classes, image_file_paths, image_size):
    if image_file_paths is None or len(image_file_paths) == 0:
        return []

    images = []
    results = []
    empty_image = load_empty_image()
    prediction_indices = []
    result_index = 0
    prediction_index = 0

    for path in image_file_paths:
        if not os.path.exists(path):
            results.append({
                "file_path": path,
                "error": True,
                "message": "File doesn't exists"
            })
            prediction_indices.append([result_index, -1])
            result_index = result_index + 1
            continue

        if is_empty_image(empty_image, path):
            results.append({
                "file_path": path,
                "error": False,
                "message": "Processed by OpenCV",
                "prediction": "empty"
            })
            prediction_indices.append([result_index, -1])
            result_index = result_index + 1
            continue

        results.append({
            "error": False,
            "file_path": path
        })

        prediction_indices.append([result_index, prediction_index])
        result_index = result_index + 1
        prediction_index = prediction_index + 1

        img = Image.load_img(path, target_size=image_size)
        x = Image.img_to_array(img)
        x = np.expand_dims(x, axis=0)

        images.append(x)

    if len(images) == 0:
        return results

    images = np.vstack(images)
    predictions = model.predict(images)

    for i in range(len(prediction_indices)):
        if prediction_indices[i][1] == -1:
            continue

        score = tf.nn.softmax(predictions[prediction_indices[i][1]])
        # confidence = np.max(score)
        result = classes[np.argmax(score)]
        results[prediction_indices[i][0]]["prediction"] = result

    return results


def group_images(images):
    image_groups = dict()

    for image_field in images:
        if "file_id" not in image_field or "file_path" not in image_field or "models" not in image_field or len(
                image_field["models"]) == 0:
            continue

        for model_name in image_field["models"]:
            if model_name not in image_groups:
                image_groups[model_name] = []

            image_groups[model_name].append({
                "file_id": image_field["file_id"],
                "file_path": image_field["file_path"]
            })

    return image_groups


def read_predictions(output, predictions, images, model_name):
    for i in range(len(images)):
        if not predictions[i]["error"]:
            if images[i]["file_id"] not in output:
                output[images[i]["file_id"]] = {
                    "file_id": images[i]["file_id"],
                    "file_path": images[i]["file_path"],
                    "predictions": [
                        {
                            "model": model_name,
                            "value": predictions[i]["prediction"],
                        }
                    ]
                }
            else:
                output[images[i]["file_id"]]["predictions"].append({
                    "model": model_name,
                    "value": predictions[i]["prediction"],
                })
        else:
            if images[i]["file_id"] in output:
                output[images[i]["file_id"]]["error"] = True
            else:
                output[images[i]["file_id"]] = {"error": True}

        if "message" in predictions[i]:
            if images[i]["file_id"] in output:
                output[images[i]["file_id"]]["message"] = predictions[i]["message"]
            else:
                output[images[i]["file_id"]] = {"message": predictions[i]["message"]}

    return output


def bulk_predict(models, image_groups):
    result = dict()

    for model_name, images in image_groups.items():
        if model_name not in models:
            continue

        model = models[model_name]

        if model is None:
            continue

        predictions = predict(
            model=model["model"],
            classes=model["classes"],
            image_size=model["image_size"],
            image_file_paths=list(map(lambda x: x["file_path"], images))
        )

        result = read_predictions(
            output=result,
            predictions=predictions,
            images=images,
            model_name=model_name
        )

    return result
