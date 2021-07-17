import time
from flask import Flask, \
    request, \
    jsonify
from shared import load_models, \
    group_images, \
    bulk_predict, \
    create_error

TEMP_FOLDER = "./.temp"
DEFAULT_EXTENSION = "jpeg"

models = load_models()
app = Flask(__name__)


@app.route("/api", methods=["POST"])
def api():
    start_time = time.time()
    body = request.json

    if "images" not in body or len(body["images"]) == 0:
        return create_error("images field is missing or empty in the body")

    image_groups = group_images(body["images"])
    result = bulk_predict(models, image_groups)

    return jsonify({
        "elapsed_time": (time.time() - start_time) * 1000,
        "images": list(map(lambda k: result[k], result))
    })


app.run(debug=True, port=8302)
