import time
from flask \
    import Flask, \
    request, \
    jsonify
from shared \
    import load_classes, \
    predict
from currency_type \
    import load_model as currency_type_load_model, \
    classes_file_path as currency_type_file_path, \
    image_size as currency_type_image_size
from stack_size \
    import load_model as stack_size_load_model, \
    classes_file_path as stack_size_classes_file_path, \
    image_size as stack_size_image_size

TEMP_FOLDER = "./.temp"
DEFAULT_EXTENSION = "png"
models = {}


def load_models():
    currency_type_classes = load_classes(currency_type_file_path)
    stack_size_classes = load_classes(stack_size_classes_file_path)

    models["currency_type"] = {
        "model": currency_type_load_model(len(currency_type_classes)),
        "classes": currency_type_classes,
        "image_size": currency_type_image_size
    }
    models["stack_size"] = {
        "model": stack_size_load_model(len(stack_size_classes)),
        "classes": stack_size_classes,
        "image_size": stack_size_image_size
    }


def do_predict(training_name, image_file_path, image_size):
    return predict(models[training_name]["model"], models[training_name]["classes"], image_file_path, image_size)


def do_run(training_name):
    if training_name not in models:
        return jsonify({
            "error": True,
            "message": "{} not available".format(training_name)
        })

    t0 = time.time()

    if "file_id" not in request.args:
        return jsonify({
            "error": True,
            "message": "No file_id provided."
        })

    file_id = request.args["file_id"]
    file_path = "{}/{}.{}".format(TEMP_FOLDER, file_id, DEFAULT_EXTENSION)

    result = do_predict(training_name, file_path, models[training_name]["image_size"])

    t1 = time.time()

    return jsonify({
        "error": False,
        "file_id": file_id,
        "prediction": result,
        "elapsed_time": (t1 - t0) * 1000
    })


load_models()
app = Flask(__name__)


@app.route("/api/currency_type", methods=["GET"])
def api_currency_type():
    return do_run("currency_type")


@app.route("/api/stack_size", methods=["GET"])
def api_stack_size():
    return do_run("stack_size")


@app.route("/api/item_links", methods=["GET"])
def api_item_links():
    return do_run("item_links")


@app.route("/api/item_sockets", methods=["GET"])
def api_item_sockets():
    return do_run("item_sockets")


@app.route("/api/socket_color", methods=["GET"])
def api_socket_color():
    return do_run("socket_color")


app.run(debug=True)
