import time
from flask import Flask, \
    request, \
    jsonify
from shared import load_models, \
    group_images, \
    bulk_predict, \
    create_error
from gevent import monkey
from gevent.pywsgi import WSGIServer
import gevent
import os
import threading

TEMP_FOLDER = "./.temp"
DEFAULT_EXTENSION = "jpeg"

models = load_models()
app = Flask(__name__)


@app.route("/api", methods=["POST"])
def api():
    global models

    last_ping = time.time()

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


@app.route("/verify", methods=["GET"])
def verify():
    global last_ping

    last_ping = time.time()

    return jsonify({
        "ok": True
    })


@app.route("/reload", methods=["GET"])
def reload():
    global models

    models = load_models()
    return jsonify({
        "ok": True
    })


server = WSGIServer(('', 8302), app.wsgi_app)
g = gevent.spawn(server.serve_forever)


def exit_if_no_connection():
    global last_ping

    while True:
        if time.time() - last_ping > 10:
            os._exit(0)

        time.sleep(5)


def exception_callback(e):
    global g

    try:
        h = e.get()
    except:
        pass

    g.kill(block=False)
    os._exit(0)


last_ping = time.time()

threading.Thread(target=exit_if_no_connection, daemon=True).start()

g.link_exception(exception_callback)
gevent.get_hub().join()

# app.run(debug=False, host="0.0.0.0", port=8302, threaded=True)
